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
    internal class VirtueForm1 : AdvancedGunBehavior
    {
        public static string internalName;
        public static int ID;
        public static string realName = "VirtueForm1";

        private PlayerController currentOwner;

        private static int ammoStat = 750;
        private static float reloadDuration = 1.0f;
        private static float fireRateStat = 0.8f;
        private static int spreadAngle = 5;

        private Gun NextFormWeapon;

        private float DivineAscentExpTracker = 0f;
        //private int DivineAscentFormTracker = 0;
        /*private float[] DivineAscentThreshold =
        {
            500f,
            1000f
        };*/
        private float DivineAscentThreshold = 500f;

        private static float projectileDamageStat = 10f;
        private static float projectileSpeedStat = 20f;
        private static float projectileRangeStat = 25f;
        private static float projectileForceStat = 8f;

        private static List<string> VirtueFiringSFXList = new List<string>
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
            gun.gameObject.AddComponent<VirtueForm1>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, "tempgun_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun, true, false);
            gun.muzzleFlashEffects = null; //(PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).muzzleFlashEffects;

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
            gun.DefaultModule.numberOfShotsInClip = ammoStat;
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

            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }

        public override void OnInitializedWithOwner(GameActor actor)
        {
            currentOwner = actor as PlayerController;
            currentOwner.OnAnyEnemyReceivedDamage += KillEnemyCount;

            Plugin.Log($"picked up {realName}");

            base.OnInitializedWithOwner(actor);
        }

        public override void OnDropped()
        {
            currentOwner.OnAnyEnemyReceivedDamage -= KillEnemyCount;

            Plugin.Log($"dropped up {realName}");

            base.OnDropped();
        }

        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemyHealth)
        {
            if (enemyHealth && fatal && enemyHealth.aiActor != null)
            {
                DivineAscentExpTracker += enemyHealth.aiActor.healthHaver.GetMaxHealth();
                Plugin.Log($"Gained {enemyHealth.aiActor.healthHaver.GetMaxHealth()} Divine Ascent EXP! Current EXP: {DivineAscentExpTracker}/{DivineAscentThreshold}");
                /*if (DivineAscentExpTracker >= DivineAscentThreshold[DivineAscentFormTracker] && DivineAscentFormTracker < DivineAscentThreshold.Length)
                {
                    TriggerAscent();
                    DivineAscentExpTracker = 0f;
                }*/
                if (DivineAscentExpTracker >= DivineAscentThreshold)
                {
                    TriggerAscent();
                }
            }
        }

        private void TriggerAscent()
        {
            //PlayerController player = this.Owner as PlayerController;
            //DivineAscentFormTracker++;

            currentOwner.OnAnyEnemyReceivedDamage -= KillEnemyCount;
            DivineAscentExpTracker = 0f;

            if (NextFormWeapon == null)
            {
                NextFormWeapon = PickupObjectDatabase.GetById((int)VirtueForm2.ID) as Gun;
            }

            if (NextFormWeapon != null || currentOwner != null)
            {
                Plugin.Log("Upgrading Virtue1 to Virtue2");
                currentOwner.inventory.RemoveGunFromInventory(this.gun);
                //currentOwner.inventory.AddGunToInventory(NextFormWeapon, true);
                currentOwner.GiveItem("LOLItems:virtueform2");
            }

            /*
            switch (DivineAscentFormTracker)
            {
                case 1:
                    Plugin.Log($"Virtue has ascended to its 1st form! {DivineAscentFormTracker}");
                    
                    gun.CurrentStrengthTier = 1;

                    Plugin.Log($"current strength tier: {this.gun.CurrentStrengthTier}");

                    break;
                case 2:
                    Plugin.Log($"Virtue has ascended to its 2nd form! {DivineAscentFormTracker}");

                    currentOwner.OnAnyEnemyReceivedDamage -= KillEnemyCount;

                    break;
                default:
                    Plugin.Log("Shouldn't be here...");
                    break;
            }
            */
        }
    }
}
