using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using Alexandria.BreakableAPI;
using BepInEx;
using System.Collections.Generic;
using LOLItems.custom_class_data;
using Dungeonator;

namespace LOLItems.weapons
{
    internal class Crossblade : AdvancedGunBehavior
    {
        public static string internalName;
        public static int ID; 
        public static string realName = "Crossblade"; 

        private static int ammoStat = 750;
        private static float reloadDuration = 1.6f;
        private static float fireRateStat = 0.3f;
        private static int spreadAngle = 5;

        private static int ricochetRange = 5;
        private static int ricochetCount = 8;
        private static float ricochetDamageScale = 0.4f;

        private static float projectileDamageStat = 10f;
        private static float projectileSpeedStat = 20f;
        private static float projectileRangeStat = 100f;
        private static float projectileForceStat = 8f;

        private static List<string> CrossbladeFiringSFXList = new List<string>
        {
            "FishbonesFireSFX1",
            "FishbonesFireSFX2",
            "FishbonesFireSFX3"
        };

        public static void Add()
        {
            string FULLNAME = realName;
            string SPRITENAME = "tempgun";
            internalName = $"LOLItems:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<Crossblade>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, "tempgun_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 20);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun, true, false);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).muzzleFlashEffects;

            gun.gunSwitchGroup = $"LOLItems_{FULLNAME.ToID()}";
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", "Play_WPN_minigun_shot_01"); 
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", null); 

            gun.DefaultModule.angleVariance = spreadAngle;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.gunClass = GunClass.FULLAUTO; 
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random; 
            gun.DefaultModule.ammoCost = 1;
            gun.reloadTime = reloadDuration;
            gun.DefaultModule.cooldownTime = fireRateStat; 
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.SetBaseMaxAmmo(ammoStat);

            gun.gunHandedness = GunHandedness.OneHanded;

            gun.carryPixelOffset += new IntVector2(0, 0);

            gun.barrelOffset.transform.localPosition += new Vector3(0 / 16f, 0 / 16f);

            gun.gunScreenShake.magnitude = 0f;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0]);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            projectile.baseData.damage = projectileDamageStat;
            projectile.baseData.speed = projectileSpeedStat;
            projectile.baseData.range = projectileRangeStat;
            projectile.baseData.force = projectileForceStat;
            projectile.transform.parent = gun.barrelOffset;
            projectile.shouldRotate = true;

            RicochetProjectileModule ricochetModule = projectile.gameObject.AddComponent<RicochetProjectileModule>();
            ricochetModule.ricochetDamageScale = ricochetDamageScale;

            PierceProjModifier pierce = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 7;
            pierce.penetratesBreakables = false;

            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile != null && projectile.Owner != null)
            {
                projectile.OnHitEnemy += HandleHitEnemy;
            }

            base.PostProcessProjectile(projectile);
        }

        private void HandleHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null && enemy.aiActor != null)
            {
                var ricochetModule = proj.GetComponent<RicochetProjectileModule>();
                if (proj.GetComponent<PierceProjModifier>() != null && proj.GetComponent<PierceProjModifier>().penetration > 0 && enemy != null && enemy.aiActor != null)
                {
                    //var dir = UnityEngine.Random.insideUnitCircle;
                    if (enemy.aiActor.ParentRoom != null && enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
                        /*var t = enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != enemy.aiActor && x.HasBeenEngaged && x.healthHaver != null && x.healthHaver.IsVulnerable);
                        if (t.Count > 0)
                        {
                            dir = BraveUtility.RandomElement(t.ToArray()).CenterPosition - proj.specRigidbody.UnitCenter;
                        }*/

                        AIActor closest = null;
                        float closestDistSq = ricochetRange * ricochetRange;
                        var t = enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != enemy.aiActor && x.HasBeenEngaged && x.healthHaver != null && x.healthHaver.IsVulnerable);

                        foreach (AIActor target in t)
                        {
                            if (proj.GetComponent<RicochetProjectileModule>().visited.Contains(target) || !target.IsNormalEnemy) 
                                continue;

                            float distSq = (target.CenterPosition - enemy.aiActor.CenterPosition).sqrMagnitude;
                            if (distSq < closestDistSq)
                            {
                                closest = target;
                                closestDistSq = distSq;
                            }
                        }

                        if (closest != null)
                        {
                            var dir = closest.CenterPosition - proj.specRigidbody.UnitCenter;
                            proj.SendInDirection(dir, false);
                            ricochetModule.visited.Add(enemy.aiActor);
                            //Plugin.Log($"Crossblade ricochet to {closest.GetActorName()}");
                        }
                        else
                        {
                            Plugin.Log($"Crossblade found no ricochet targets");
                            proj.ForceDestruction();
                        }
                    }
                    //proj.SendInDirection(dir, false);
                }
            }
        }
    }
}
