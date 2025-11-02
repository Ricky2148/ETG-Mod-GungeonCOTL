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
        private static float fireRateStat = 0.8f;
        private static int spreadAngle = 5;

        private float DivineAscentExpTracker = 0f;
        private float DivineAscentThreshold = 1000f;

        private Gun NextFormWeapon;

        public GameObject prefabToAttachToPlayer;
        private GameObject instanceWings;
        private tk2dSprite instanceWingsSprite;

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

            Projectile wave = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0]);
            gun.DefaultModule.projectiles.Add(wave);

            wave.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(wave.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(wave);

            wave.baseData.damage = projectileDamageStat;
            wave.baseData.speed = projectileSpeedStat;
            wave.baseData.range = projectileRangeStat;
            wave.baseData.force = projectileForceStat;
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

        public override void OnInitializedWithOwner(GameActor actor)
        {
            currentOwner = actor as PlayerController;

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
            currentOwner.SetIsFlying(value: true, "DivineAscent");
            //instanceWings = player.RegisterAttachedObject(prefabToAttachToPlayer, "DivineAscentWings");
            //instanceWingsSprite = instanceWings.GetComponent<tk2dSprite>();
        }

        private void StopFlight()
        {
            currentOwner.SetIsFlying(value: false, "DivineAscent");
            //player.DeregisterAttachedObject(instanceWings);
            //instanceWingsSprite = null;
        }
    }
}
