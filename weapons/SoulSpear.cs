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

namespace LOLItems.weapons
{
    internal class SoulSpear : AdvancedGunBehavior
    {
        public static string internalName; //Internal name of the gun as used by console commands
        public static int ID; //The Gun ID stored by the game.  Can be used by other functions to call your custom gun.
        public static string realName = "Soul Spear"; //The name that shows up in the Ammonomicon and the mod console.

        private static int ammoStat = 200;
        private static float reloadDuration = 0f;
        private static float fireRateStat = 0.6f;
        private static int spreadAngle = 5;

        private static float projectileDamageStat = 5f;
        private static float projectileSpeedStat = 75f;
        private static float projectileRangeStat = 25f;
        private static float projectileForceStat = 0f;

        private static float dashBaseDuration = 0.3f;
        private static float dashBaseSpeed = 20f;

        private Coroutine dashCoroutine;

        private Dictionary<AIActor, int> enemyRendStacks = new Dictionary<AIActor, int>();
        private static float rendScale = 0.4f;

        //private bool isFiring = false;

        private static List<string> normalFiringSFXList = new List<string>
        {
            "hextech_rifle_atk_sfx_001",
            "hextech_rifle_atk_sfx_002",
            "hextech_rifle_atk_sfx_003",
            "hextech_rifle_atk_sfx_004"
        };

        public static void Add()
        {
            string FULLNAME = "Soul Spear";
            string SPRITENAME = "tempgun";
            internalName = $"LOLItems:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<SoulSpear>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, $"{SPRITENAME}_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 5);

            gun.SetAnimationFPS(gun.reloadAnimation, 13);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById((int)Items._38Special) as Gun, true, false);
            gun.muzzleFlashEffects = null;

            gun.gunSwitchGroup = $"LOLItems_{FULLNAME.ToID()}"; 
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", null);
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", "Play_WPN_m1911_reload_01");
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_gun_finale_01", null);
            gun.DefaultModule.angleVariance = spreadAngle;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;

            gun.gunClass = GunClass.RIFLE;
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

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items._38Special) as Gun).DefaultModule.projectiles[0]);

            gun.DefaultModule.projectiles[0] = projectile;

            projectile.hitEffects.deathAny = null;
            projectile.hitEffects.deathEnemy = null;
            projectile.hitEffects.enemy = null;
            projectile.hitEffects.tileMapHorizontal = (PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal;
            projectile.hitEffects.tileMapVertical = (PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical;

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            projectile.baseData.damage = projectileDamageStat;
            projectile.baseData.speed = projectileSpeedStat;
            projectile.baseData.range = projectileRangeStat;
            projectile.baseData.force = projectileForceStat;
            projectile.transform.parent = gun.barrelOffset;
            projectile.shouldRotate = true;

            //projectile.SetProjectileSpriteRight("hextech_projectile_glow", 11, 5, true, tk2dBaseSprite.Anchor.MiddleCenter, 9, 3);

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("soul_spear_ammo",
                "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/hextech_rifle_ammo_full", "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/hextech_rifle_ammo_empty");

            gun.shellCasing = null;
            gun.clipObject = null;
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;
            gun.reloadClipLaunchFrame = 0;

            gun.quality = PickupObject.ItemQuality.A;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;

            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            dashCoroutine = StartCoroutine(MartialPoiseDash(player));

            // if gun is not actively firing
            /*if (!isFiring)
            {
                isFiring = true;
                ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
                float statToMod = player.stats.GetStatValue(PlayerStats.StatType.MovementSpeed);
                ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, -statToMod, StatModifier.ModifyMethod.ADDITIVE);
                player.stats.RecalculateStats(player, true, false);
            }*/

            base.OnPostFired(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy += (projHit, enemy, fatal) =>
            {
                if (enemy != null && enemy.aiActor != null && enemy.aiActor.healthHaver != null && enemy.healthHaver != null)
                {
                    if (fatal && enemyRendStacks.ContainsKey(enemy.aiActor))
                    {
                        enemyRendStacks.Remove(enemy.aiActor);
                    }

                    if (!enemyRendStacks.ContainsKey(enemy.aiActor))
                    {
                        enemyRendStacks.Add(enemy.aiActor, 1);
                    }
                    else
                    {
                        enemyRendStacks[enemy.aiActor] += 1;
                    }
                }
            };

            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressedSafe(PlayerController player, Gun gun, bool manualReload)
        {
            float rendDamagePerStack = gun.DefaultModule.projectiles[0].baseData.damage * rendScale;
            Plugin.Log($"rend damage per stack: {rendDamagePerStack}");
            foreach (KeyValuePair<AIActor, int> target in enemyRendStacks)
            {
                float damageToDeal = (target.Value + 1) * rendDamagePerStack;
                Plugin.Log($"rend stacks: {target.Value}, damage dealt: {damageToDeal}");

                target.Key.healthHaver.ApplyDamage(
                    damageToDeal,
                    Vector2.zero,
                    "soul_spear_rend_damage",
                    CoreDamageTypes.None,
                    DamageCategory.Normal,
                    false
                );
            }

            enemyRendStacks.Clear();
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            /*if (isFiring)
            {
                isFiring = false;
                ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
                ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                player.stats.RecalculateStats(player, true, false);
            }*/

            base.OnFinishAttack(player, gun);
        }

        public override void OnSwitchedAwayFromThisGun()
        {
            PlayerController player = Owner as PlayerController;
            //isFiring = false;
            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStats(player, true, false);

            base.OnSwitchedAwayFromThisGun();
        }

        public System.Collections.IEnumerator MartialPoiseDash(PlayerController player)
        {
            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
            player.stats.RecalculateStats(player, true, false);
            float statToMod = player.stats.GetStatValue(PlayerStats.StatType.MovementSpeed);
            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, -statToMod, StatModifier.ModifyMethod.ADDITIVE);
            player.stats.RecalculateStats(player, true, false);
            Plugin.Log($"modifier: {-statToMod}");

            float duration = dashBaseDuration / player.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
            //float adjSpeed = dashBaseSpeed * player.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
            float adjSpeed = dashBaseSpeed * (1 + ((player.stats.GetStatValue(PlayerStats.StatType.RateOfFire) - 1) * 0.5f));
            float elapsed = -BraveTime.DeltaTime;

            //Plugin.Log($"\n\n{player.m_playerCommandedDirection}\n{player.LastCommandedDirection}\n{player.m_activeActions.Move.Vector}");

            // need to check for player not inputting a direction, idk how yet tho lmao
            //Vector2 angle = player.m_lastNonzeroCommandedDirection.normalized;
            Vector2 angle = Vector2.zero;
            if (player.CurrentInputState != PlayerInputState.NoMovement)
            {
                angle = player.AdjustInputVector(player.m_activeActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal);
                //Plugin.Log($"m_activeActions: {player.m_activeActions.Move.Vector}, angle: {angle}");
            }
            if (angle.magnitude > 1f)
            {
                angle.Normalize();
                //Plugin.Log($"normalized angle: {angle}");
            }

            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                player.specRigidbody.Velocity = angle * adjSpeed;
                yield return null;
            }

            yield return new WaitForSeconds(duration * 2);

            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStats(player, true, false);
            Plugin.Log($"modifier: {1f}");
        }
    }
}
