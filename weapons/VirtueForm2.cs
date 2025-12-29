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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// make wave projectiles pierce all walls

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
        private static float projectileSpeedStat = 50f; //50f
        private static float projectileRangeStat = 15f;
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

            gun.carryPixelOffset += new IntVector2(15, -1); //offset when holding gun vertically
            gun.carryPixelDownOffset += new IntVector2(-13, -15); //offset when aiming down
            gun.carryPixelUpOffset += new IntVector2(-11, 15); //offset when aiming up

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
            projectile.shouldRotate = true;

            projectile.SetProjectileSpriteRight("virtue_yellow_projectile_straight_001", 19, 7, true, tk2dBaseSprite.Anchor.MiddleCenter, 17, 6);

            EasyTrailBullet projTrail = projectile.gameObject.AddComponent<EasyTrailBullet>();
            projTrail.TrailPos = projectile.transform.position;
            projTrail.StartWidth = 0.25f;
            projTrail.EndWidth = 0f;
            projTrail.LifeTime = 0.15f; //How long the trail lingers
            // BaseColor sets an overall color for the trail. Start and End Colors are subtractive to it. 
            projTrail.BaseColor = ExtendedColours.orange; //Set to white if you don't want to interfere with Start/End Colors.
            projTrail.StartColor = ExtendedColours.paleYellow;
            projTrail.EndColor = Color.white; //Custom Orange example using r/g/b values.

            /*List<string> projectileSpriteNames = new List<string>
            {
                "virtue_yellow_projectile_001",
                "virtue_yellow_projectile_002",
                "virtue_yellow_projectile_003",
                "virtue_yellow_projectile_004",
            };
            int projectileFPS = 8;
            List<IntVector2> projectileSizes = new List<IntVector2>
            {
                new IntVector2(20, 6), //1
                new IntVector2(20, 6), //2
                new IntVector2(20, 6), //3
                new IntVector2(20, 6), //4
            };
            List<bool> projectileLighteneds = new List<bool>
            {
                true,
                true,
                true,
                true,
            };
            List<tk2dBaseSprite.Anchor> projectileAnchors = new List<tk2dBaseSprite.Anchor>
            {
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
            };
            List<bool> projectileAnchorsChangeColiders = new List<bool>
            {
                false,
                false,
                false,
                false,
            };
            List<bool> projectilefixesScales = new List<bool>
            {
                false,
                false,
                false,
                false,
            };
            List<Vector3?> projectileManualOffsets = new List<Vector3?>
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
            };
            List<IntVector2?> projectileOverrideColliderSizes = new List<IntVector2?>
            {
                new IntVector2(16, 5), //1
                new IntVector2(16, 5), //2
                new IntVector2(16, 5), //3
                new IntVector2(16, 5), //4
            };
            List<IntVector2?> projectileOverrideColliderOffsets = new List<IntVector2?>
            {
                null,
                null,
                null,
                null,
            };
            List<Projectile> projectileOverrideProjectilesToCopyFrom = new List<Projectile>
            {
                null,
                null,
                null,
                null,
            };
            tk2dSpriteAnimationClip.WrapMode ProjectileWrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            projectile.AddAnimationToProjectile(projectileSpriteNames, projectileFPS, projectileSizes, projectileLighteneds, projectileAnchors, projectileAnchorsChangeColiders, projectilefixesScales,
                                                projectileManualOffsets, projectileOverrideColliderSizes, projectileOverrideColliderOffsets, projectileOverrideProjectilesToCopyFrom, ProjectileWrapMode);
            */
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
            wave.shouldRotate = true;

            wave.PenetratesInternalWalls = true;
            wave.pierceMinorBreakables = true;
            wave.damagesWalls = false;

            TrueWallPiercingRounds wallPiercingRounds = wave.gameObject.GetOrAddComponent<TrueWallPiercingRounds>();

            //wave.BulletScriptSettings.surviveRigidbodyCollisions = true;
            //wave.BulletScriptSettings.surviveTileCollisions = true;

            PierceProjModifier pierce = wave.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 999;
            pierce.penetratesBreakables = true;

            //wave.sprite.color = Color.cyan;
            //wave.AdditionalScaleMultiplier = 5f;

            wave.SetProjectileSpriteRight("virtue_yellow_large_projectile_thin_001", 23, 96, true, tk2dBaseSprite.Anchor.MiddleCenter, 20, 76);

            var sprite = wave.sprite.gameObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                Plugin.Log("sprite not null");
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 0.8f);
            }
            else
            {
                Plugin.Log("sprite is null");
            }

            /*var tro_1 = wave.gameObject.AddChild("trail object");
            tro_1.transform.position = projectile.transform.position;
            tro_1.transform.localPosition = projectile.transform.position;
            TrailRenderer tr_1 = tro_1.AddComponent<TrailRenderer>();
            tr_1.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr_1.receiveShadows = false;
            var mat_1 = new Material(Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit")); //replace the "Shader.Find("Sprites/Default")" with your shader reference
            tr_1.material = mat_1;
            tr_1.material.SetFloat("_Fade", 0.5f);
            tr_1.minVertexDistance = 0.01f;
            tr_1.numCapVertices = 640;

            //======
            mat_1.SetColor("_Color", ExtendedColours.orange);
            tr_1.startColor = ExtendedColours.orange;
            tr_1.endColor = Color.white;
            //======
            tr_1.time = 0.15f;
            //======
            tr_1.startWidth = 5.5f;
            tr_1.endWidth = 4.0f;
            tr_1.autodestruct = false;

            var rend = wave.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr_1;
            rend.desiredLength = 6;*/

            //var waveTrailObject = wave.gameObject.AddChild("trail object");
            //waveTrailObject.transform.position = wave.transform.position;
            //waveTrailObject.transform.localPosition = wave.transform.position;

            /*TrailRenderer waveTrail = waveTrailObject.GetOrAddComponent<TrailRenderer>();
            waveTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            waveTrail.receiveShadows = false;
            waveTrail.material = new Material(Shader.Find("Sprites/Default"));

            waveTrail.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            waveTrail.material.SetFloat("_Fade", 0.5f);
            waveTrail.minVertexDistance = 0.1f;
            waveTrail.material.SetColor("_Color", ExtendedColours.orange);
            waveTrail.startColor = ExtendedColours.orange;
            waveTrail.endColor = Color.white;
            waveTrail.time = 0.15f;
            waveTrail.startWidth = 6.0f;
            waveTrail.endWidth = 4.0f;*/

            /*EasyTrailBullet waveTrail = wave.gameObject.AddComponent<EasyTrailBullet>();
            //waveTrail.TrailPos = wave.transform.position;
            waveTrail.StartWidth = 5.5f;
            waveTrail.EndWidth = 4.0f;
            waveTrail.LifeTime = 0.15f; //How long the trail lingers
            // BaseColor sets an overall color for the trail. Start and End Colors are subtractive to it. 
            waveTrail.BaseColor = ExtendedColours.orange; //Set to white if you don't want to interfere with Start/End Colors.
            waveTrail.StartColor = ExtendedColours.orange;
            waveTrail.EndColor = Color.white; //Custom Orange example using r/g/b values*/

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
                    ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire);
                    ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
                    currentOwner.stats.RecalculateStatsWithoutRebuildingGunVolleys(currentOwner);
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

            if (zealCapActivated)
            {
                BraveUtility.Swap(ref this.gun.shootAnimation, ref this.gun.criticalFireAnimation);
                BraveUtility.Swap(ref this.gun.alternateShootAnimation, ref this.gun.finalShootAnimation);
                BraveUtility.Swap(ref this.gun.idleAnimation, ref this.gun.alternateIdleAnimation);

                zealCapActivated = false;
            }
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

            //projectile.gameObject.GetComponent<EasyTrailBullet>().TrailPos = projectile.transform.position;

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
