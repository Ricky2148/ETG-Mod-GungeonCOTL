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
    internal class VirtueForm3 : AdvancedGunBehavior
    {
        public static string internalName;
        public static int ID;
        public static string realName = "VirtueForm3";

        private PlayerController currentOwner;

        private static int ammoStat = 750;
        private static float reloadDuration = 1.0f;
        private static float fireRateStat = 0.35f;
        private static int spreadAngle = 0;

        public GameObject prefabToAttachToPlayer;
        private GameObject instanceWings;
        private tk2dSprite instanceWingsSprite;

        private bool m_isCurrentlyActive;
        private bool m_hiddenForAll;

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
            string SPRITENAME = "hextech";
            internalName = $"LOLItems:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<VirtueForm3>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, "hextech_idle_001", 8);

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

            gun.Volley.ModulesAreTiers = true;
            ProjectileModule mod1 = gun.DefaultModule;
            ProjectileModule mod2 = ProjectileModule.CreateClone(mod1, false);
            gun.Volley.projectiles.Add(mod2);

            Projectile wave = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0]);
            //gun.DefaultModule.projectiles.Add(wave);
            gun.Volley.projectiles[1].projectiles[0] = wave;

            wave.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(wave.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(wave);

            wave.baseData.damage = projectileDamageStat * 0.5f;
            wave.baseData.speed = projectileSpeedStat;
            wave.baseData.range = projectileRangeStat;
            wave.baseData.force = 0;
            wave.transform.parent = gun.barrelOffset;
            wave.shouldRotate = true;

            wave.sprite.color = Color.cyan;
            wave.AdditionalScaleMultiplier = 5f;

            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;

            //gun.CurrentStrengthTier = 0;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile != null && projectile.Owner != null)
            {
                currentOwner.StartCoroutine(FireWaveDelayed(projectile));
            }

            base.PostProcessProjectile(projectile);
        }

        private System.Collections.IEnumerator FireWaveDelayed(Projectile projectile)
        {
            if (projectile == null || projectile.Owner == null || !projectile)
            {
                Plugin.Log("fail 1");
                yield break;
            }

            yield return new WaitForSeconds(0.001f);

            Vector2 direction = projectile.LastVelocity.normalized;

            //Projectile wave = UnityEngine.Object.Instantiate(projectile.gameObject).GetComponent<Projectile>();

            Projectile wave = UnityEngine.Object.Instantiate(this.gun.Volley.projectiles[1].projectiles[0].gameObject).GetComponent<Projectile>();

            /*wave.baseData.damage = projectile.baseData.damage * 0.5f;
            wave.baseData.force = 0;

            wave.sprite.color = Color.cyan;
            wave.AdditionalScaleMultiplier = 5f;*/

            wave.Owner = projectile.Owner;
            wave.Shooter = projectile.Shooter;

            wave.transform.position = currentOwner.CurrentGun.barrelOffset.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            wave.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            wave.SendInDirection(direction, true, true);
        }

        public override void OnInitializedWithOwner(GameActor actor)
        {
            currentOwner = actor as PlayerController;

            currentOwner.OnIsRolling += OnRollFrame;

            TriggerFlight();

            Plugin.Log($"picked up {realName}");

            base.OnInitializedWithOwner(actor);
        }

        public override void OnDropped()
        {
            StopFlight();

            Plugin.Log($"dropped up {realName}");

            base.OnDropped();
        }

        private void TriggerFlight()
        {
            Plugin.Log("trigger flight");
            if (!Dungeon.IsGenerating && currentOwner && currentOwner.sprite && currentOwner.sprite.GetComponent<tk2dSpriteAttachPoint>())
            {
                Plugin.Log("flight work");

                m_isCurrentlyActive = true;
                currentOwner.AdditionalCanDodgeRollWhileFlying.SetOverride("Feather", true);
                currentOwner.SetIsFlying(value: true, "DivineAscent");

                var waxWings = PickupObjectDatabase.GetById((int)Items.WaxWings).gameObject.GetComponent<WingsItem>();

                instanceWings = currentOwner.RegisterAttachedObject(waxWings.prefabToAttachToPlayer, "jetpack", 0.1f);
                instanceWingsSprite = instanceWings.GetComponent<tk2dSprite>();

                if (!instanceWingsSprite)
                {
                    instanceWingsSprite = instanceWings.GetComponentInChildren<tk2dSprite>();
                }

                //instanceWingsSprite.transform.localPosition = waxWings.GetLocalOffsetForCharacter(currentOwner.characterIdentity).ToVector3ZUp();
            }
        }

        private void StopFlight()
        {
            Plugin.Log("Stop flight");

            m_isCurrentlyActive = false;
            currentOwner.AdditionalCanDodgeRollWhileFlying.SetOverride("Feather", false);

            currentOwner.SetIsFlying(value: false, "DivineAscent");
            currentOwner.DeregisterAttachedObject(instanceWings);
            instanceWingsSprite = null;
        }

        protected override void Update()
        {
            base.Update();
            if (currentOwner == null)
            {
                return;
            }
            if (m_isCurrentlyActive)
            {
                if (currentOwner.IsFalling)
                {
                    m_hiddenForAll = true;
                    instanceWingsSprite.renderer.enabled = false;
                }
                else
                {
                    if (m_hiddenForAll)
                    {
                        m_hiddenForAll = false;
                        instanceWingsSprite.renderer.enabled = true;
                    }
                    /*string text = "white_wing" + currentOwner.GetBaseAnimationSuffix(true);
                    if (!instanceWingsSprite.spriteAnimator.IsPlaying(text) && (!currentOwner.IsDodgeRolling))
                    {
                        instanceWingsSprite.spriteAnimator.Play(text);
                    }*/
                    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                    {
                        StopFlight();
                    }
                }
            }
            else if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
            {
                Plugin.Log("update trigger flight");
                TriggerFlight();
            }
        }

        private void OnRollFrame(PlayerController player)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
            {
                return;
            }
        }
    }
}
