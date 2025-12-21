using Alexandria.BreakableAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.SoundAPI;
using BepInEx;
using Dungeonator;
using Gungeon;
using LOLItems.custom_class_data;
using MonoMod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LOLItems.weapons
{
    internal class VirtueForm2 : AdvancedGunBehavior
    {
        public static string internalName;
        public static int ID;
        public static string realName = "VirtueForm2";

        private PlayerController currentOwner;

        private static int ammoStat = 750;
        private static float reloadDuration = 1.0f;
        private static float fireRateStat = 0.8f;
        private static int spreadAngle = 0;

        private int zealStacks = 0;
        private int zealStackCap = 5;
        //private float zealIncPerStack = 0.06f;
        private float zealIncPerStack = 0.12f;
        private float zealSpeedInc = 1.1f;
        private Coroutine zealDecayCoroutine;

        private bool zealCapActivated = false;

        private float DivineAscentExpTracker = 0f;
        private float DivineAscentThreshold = 1000f;

        private Gun NextFormWeapon;

        private Projectile wave;

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
            string SPRITENAME = "virtue_form2";
            internalName = $"LOLItems:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<VirtueForm2>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, $"{SPRITENAME}_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.alternateShootAnimation, 12);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 13);
            gun.SetAnimationFPS(gun.finalShootAnimation, 13);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun, true, false);
            gun.muzzleFlashEffects = null; //(PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).muzzleFlashEffects;

            gun.gunSwitchGroup = $"LOLItems_{FULLNAME.ToID()}";
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", "Play_WPN_minigun_shot_01");
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", null);

            gun.DefaultModule.angleVariance = spreadAngle;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.gunClass = GunClass.RIFLE;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoCost = 1;
            gun.reloadTime = reloadDuration;
            gun.DefaultModule.cooldownTime = fireRateStat;
            gun.DefaultModule.numberOfShotsInClip = ammoStat;
            gun.SetBaseMaxAmmo(ammoStat);

            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.carryPixelOffset += new IntVector2(12, -5); //offset when holding gun vertically
            gun.carryPixelDownOffset += new IntVector2(-18, -10); //offset when aiming down
            gun.carryPixelUpOffset += new IntVector2(-5, 20); //offset when aiming up

            gun.barrelOffset.transform.localPosition += new Vector3(64 / 16f, 52 / 16f);

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
            projectile.shouldRotate = false;

            projectile.SetProjectileSpriteRight("virtue_yellow_projectile_001", 20, 6, true, tk2dBaseSprite.Anchor.MiddleCenter, 16, 5);

            // A list of filenames in the sprites/ProjectileCollection folder for each frame in the animation, extension not required.
            List<string> projectileSpriteNames = new List<string>
            {
                "virtue_yellow_projectile_001",
                "virtue_yellow_projectile_002",
                "virtue_yellow_projectile_003",
                "virtue_yellow_projectile_004",
            };
            // Animation FPS.
            int projectileFPS = 8;
            // Visual sprite size for each frame.  Sprite images will stretch to match these sizes.
            List<IntVector2> projectileSizes = new List<IntVector2>
            {
                new IntVector2(20, 6), //1
                new IntVector2(20, 6), //2
                new IntVector2(20, 6), //3
                new IntVector2(20, 6), //4
            };
            // Whether each frame should have a bit of glow.
            List<bool> projectileLighteneds = new List<bool>
            {
                true,
                true,
                true,
                true,
            };
            // Sprite anchor list.
            List<tk2dBaseSprite.Anchor> projectileAnchors = new List<tk2dBaseSprite.Anchor>
            {
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
            };
            // Whether or not the anchors should affect the hitboxees.
            List<bool> projectileAnchorsChangeColiders = new List<bool>
            {
                false,
                false,
                false,
                false,
            };
            // Unknown, doesn't appear to matter so leave as false. 
            List<bool> projectilefixesScales = new List<bool>
            {
                false,
                false,
                false,
                false,
            };
            // Manual Offsets for each sprite if needed.
            List<Vector3?> projectileManualOffsets = new List<Vector3?>
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
            };
            // Override the projectile hitboxes on each frame.  Either null (same as visuals) or slightly smaller than the visuals is most common.
            List<IntVector2?> projectileOverrideColliderSizes = new List<IntVector2?>
            {
                new IntVector2(16, 5), //1
                new IntVector2(16, 5), //2
                new IntVector2(16, 5), //3
                new IntVector2(16, 5), //4
            };
            // Manually assign the projectile offsets.
            List<IntVector2?> projectileOverrideColliderOffsets = new List<IntVector2?>
            {
                null,
                null,
                null,
                null,
            };
            // Copy another projectile each frame.
            List<Projectile> projectileOverrideProjectilesToCopyFrom = new List<Projectile>
            {
                null,
                null,
                null,
                null,
            };
            // Your animations wrap mode. If you just want it to do a looping animation, leave it as Loop. Only useful for when adding multiple differing animations.
            tk2dSpriteAnimationClip.WrapMode ProjectileWrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            // Optionally, you can give your animations a clip name. Only useful for when adding multiple differing animations.
            //string projectileClipName = "projectileName"; 
            // Optionally, you can assign an animation as the default one that plays.  Only useful for when adding multiple differing animations.  If left as null then it will use the most recently added animation.
            //string projectileDefaultClipName = "projectileName"; 

            projectile.AddAnimationToProjectile(projectileSpriteNames, projectileFPS, projectileSizes, projectileLighteneds, projectileAnchors, projectileAnchorsChangeColiders, projectilefixesScales,
                                                projectileManualOffsets, projectileOverrideColliderSizes, projectileOverrideColliderOffsets, projectileOverrideProjectilesToCopyFrom, ProjectileWrapMode);

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("virtue_form2_ammo",
                "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/virtue_form2_ammo_full", "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/virtue_form1_ammo_empty");


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
            wave.shouldRotate = false;

            wave.sprite.color = Color.cyan;
            wave.AdditionalScaleMultiplier = 5f;

            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;

            //gun.CurrentStrengthTier = 0;

            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
            //ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire, 1.0f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            //ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1.0f, StatModifier.ModifyMethod.MULTIPLICATIVE);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (zealDecayCoroutine != null)
            {
                StopCoroutine(zealDecayCoroutine);
            }
            if (zealStacks <= zealStackCap)
            {
                ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire);
                ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire, 1.0f + (zealStacks * zealIncPerStack), StatModifier.ModifyMethod.MULTIPLICATIVE);
                //player.stats.RecalculateStats(player, true, false);
                player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

                gun.spriteAnimator.OverrideTimeScale = 1f + zealStacks * zealIncPerStack;

                //Plugin.Log($"zealStacks: {zealStacks}");
                zealStacks++;

                if (zealStacks > zealStackCap)
                {
                    Plugin.Log($"{zealCapActivated}");
                    if (!zealCapActivated)
                    {
                        Plugin.Log($"zeal max stack effects activated");

                        BraveUtility.Swap(ref this.gun.shootAnimation, ref this.gun.criticalFireAnimation);
                        BraveUtility.Swap(ref this.gun.alternateShootAnimation, ref this.gun.finalShootAnimation);
                        BraveUtility.Swap(ref this.gun.idleAnimation, ref this.gun.alternateIdleAnimation);

                        ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
                        ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, zealSpeedInc, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);
                        zealCapActivated = true;
                    }
                }
                
                //BraveUtility.Swap(ref this.gun.shootAnimation, ref this.gun.alternateShootAnimation);

                this.Update();
            }

            BraveUtility.Swap(ref this.gun.shootAnimation, ref this.gun.alternateShootAnimation);

            base.OnPostFired(player, gun);
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            Plugin.Log($"Finished Attack - starting zeal decay coroutine: zealstacks {zealStacks}");

            if (zealDecayCoroutine != null)
            {
                StopCoroutine(zealDecayCoroutine);
            }
            zealDecayCoroutine = StartCoroutine(ZealDecayCoroutine(player, gun));

            base.OnFinishAttack(player, gun);
        }

        public override void OnSwitchedAwayFromThisGun()
        {
            for (int i = 0; i < currentOwner.inventory.AllGuns.Count; i++)
            {
                if (currentOwner.inventory.AllGuns[i] == null) continue;
                if (currentOwner.inventory.AllGuns[i] == currentOwner.CurrentGun) continue;
                if (currentOwner.inventory.AllGuns[i].GetComponent<VirtueForm2>() != null)
                {
                    Gun gun = currentOwner.inventory.AllGuns[i];
                    //gun.DefaultModule.cooldownTime = fireRateStat;
                    //gun.SetAnimationFPS(gun.shootAnimation, 7);
                    //player.stats.RecalculateStats(player, true, false);

                    gun.spriteAnimator.OverrideTimeScale = 1f;
                    zealStacks = 0;
                    Plugin.Log($"Reset {gun}'s fire rate to {currentOwner.stats.GetBaseStatValue(PlayerStats.StatType.RateOfFire)}, zealstacks: {zealStacks}");
                }
            }

            base.OnSwitchedAwayFromThisGun();
        }

        private System.Collections.IEnumerator ZealDecayCoroutine(PlayerController player, Gun gun)
        {
            /*Plugin.Log($"zealStacks: {zealStacks}");
            yield return new WaitForSeconds(0.5f);
            while (zealStacks > 0)
            {
                yield return new WaitForSeconds(0.2f);
                zealStacks--;
                //gun.DefaultModule.cooldownTime = fireRateStat - rampUpStacks * rampUpIncPerStack;
                ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire);
                ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire, 1.0f + (zealStacks * zealIncPerStack), StatModifier.ModifyMethod.MULTIPLICATIVE);
                player.stats.RecalculateStats(player, true, false);
                
                gun.spriteAnimator.OverrideTimeScale = 1.0f + (zealStacks * zealIncPerStack);
                Plugin.Log($"Lowered fire rate to {player.stats.GetBaseStatValue(PlayerStats.StatType.RateOfFire)}, zealstacks: {zealStacks}");
            }
            yield break;*/

            yield return new WaitForSeconds(3f);
            zealStacks = 0;
            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire);
            //ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire, 1.0f + (zealStacks * zealIncPerStack), StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
            //ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, zealSpeedInc, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            gun.spriteAnimator.OverrideTimeScale = 1f + zealStacks * zealIncPerStack;

            BraveUtility.Swap(ref this.gun.shootAnimation, ref this.gun.criticalFireAnimation);
            BraveUtility.Swap(ref this.gun.alternateShootAnimation, ref this.gun.finalShootAnimation);
            BraveUtility.Swap(ref this.gun.idleAnimation, ref this.gun.alternateIdleAnimation);
            zealCapActivated = false;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (zealStacks >= zealStackCap)
            {
                if (projectile != null && projectile.Owner != null)
                {
                    //Plugin.Log("doing something");

                    currentOwner.StartCoroutine(FireWaveDelayed(projectile));
                }
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
            currentOwner.OnAnyEnemyReceivedDamage += KillEnemyCount;

            /*wave = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0]);

            wave.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(wave.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(wave);

            wave.baseData.damage = projectileDamageStat;
            wave.baseData.speed = projectileSpeedStat * 2;
            wave.baseData.range = projectileRangeStat;
            wave.baseData.force = projectileForceStat;
            wave.transform.parent = gun.barrelOffset;
            wave.shouldRotate = true;

            wave.sprite.color = Color.cyan;
            wave.AdditionalScaleMultiplier = 5f;*/

            //TriggerFlight();

            Plugin.Log($"picked up {realName}");

            base.OnInitializedWithOwner(actor);
        }

        public override void OnDropped()
        {
            currentOwner.OnAnyEnemyReceivedDamage -= KillEnemyCount;

            currentOwner = null;

            //StopFlight();

            Plugin.Log($"dropped up {realName}");

            base.OnDropped();
        }

        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemyHealth)
        {
            if (enemyHealth && fatal && enemyHealth.aiActor != null)
            {
                DivineAscentExpTracker += enemyHealth.aiActor.healthHaver.GetMaxHealth();
                Plugin.Log($"Gained {enemyHealth.aiActor.healthHaver.GetMaxHealth()} Divine Ascent EXP! Current EXP: {DivineAscentExpTracker}/{DivineAscentThreshold}");
                if (DivineAscentExpTracker >= DivineAscentThreshold)
                {
                    TriggerAscent();
                }
            }
        }

        private void TriggerAscent()
        {
            currentOwner.OnAnyEnemyReceivedDamage -= KillEnemyCount;
            DivineAscentExpTracker = 0f;

            if (NextFormWeapon == null)
            {
                NextFormWeapon = PickupObjectDatabase.GetById((int)VirtueForm3.ID) as Gun;
            }

            if (NextFormWeapon != null || currentOwner != null)
            {
                Plugin.Log("Upgrading Virtue2 to Virtue3");
                currentOwner.inventory.RemoveGunFromInventory(this.gun);
                //currentOwner.inventory.AddGunToInventory(NextFormWeapon, true);
                currentOwner.GiveItem("LOLItems:virtueform3");
            }
        }
    }
}
