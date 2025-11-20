using Alexandria.BreakableAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.SoundAPI;
using Alexandria.VisualAPI;
using BepInEx;
using Gungeon;
using LOLItems.custom_class_data;
using MonoMod;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalSparksDoer;

namespace LOLItems.weapons
{
    internal class SoulSpear : AdvancedGunBehavior
    {
        public static string internalName; //Internal name of the gun as used by console commands
        public static int ID; //The Gun ID stored by the game.  Can be used by other functions to call your custom gun.
        public static string realName = "Soul Spear"; //The name that shows up in the Ammonomicon and the mod console.

        private static int ammoStat = 600;
        private static float reloadDuration = 0f;
        private static float fireRateStat = 0.6f;
        private static int spreadAngle = 5;

        private static float projectileDamageStat = 12f;
        private static float projectileSpeedStat = 60f; //60f;
        private static float projectileRangeStat = 20f;
        private static float projectileForceStat = 0f;

        private static float dashBaseDuration = 0.3f;
        private static float dashBaseSpeed = 20f;

        private Coroutine dashCoroutine;

        private static List<string> VFXSpritePath = new List<string>
        {
            "LOLItems/Resources/vfxs/vengencespear/vengencespear_vfx"
        };


        private static GameObject EffectVFX = VFXBuilder.CreateVFX
        (
            "soulspear_rend_vfx",
            VFXSpritePath,
            1,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Loop,
            true
        );

        private Dictionary<AIActor, List<GameObject>> activeVFXObjectList = new Dictionary<AIActor, List<GameObject>>();

        //private Dictionary<AIActor, int> enemyRendStacks = new Dictionary<AIActor, int>();
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
            string SPRITENAME = "vengencespear";
            internalName = $"LOLItems:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<SoulSpear>();
            gun.SetShortDescription("idk");
            gun.SetLongDescription("idk");

            gun.SetupSprite(null, $"{SPRITENAME}_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 17);

            gun.SetAnimationFPS(gun.reloadAnimation, 12);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById((int)Items._38Special) as Gun, true, false);
            gun.muzzleFlashEffects = null;

            gun.gunSwitchGroup = $"LOLItems_{FULLNAME.ToID()}"; 
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", null);
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", "Play_WPN_m1911_reload_01");
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_gun_finale_01", null);
            gun.DefaultModule.angleVariance = spreadAngle;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;

            gun.gunClass = GunClass.RIFLE;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoCost = 1;
            gun.reloadTime = reloadDuration;
            gun.DefaultModule.cooldownTime = fireRateStat;
            gun.DefaultModule.numberOfShotsInClip = ammoStat;
            gun.SetBaseMaxAmmo(ammoStat);

            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.carryPixelOffset += new IntVector2(-14, 10);
            gun.carryPixelDownOffset += new IntVector2(18, 4); //offset when aiming down
            gun.carryPixelUpOffset += new IntVector2(12, -22); // offset when aiming up

            gun.barrelOffset.transform.localPosition += new Vector3(32 / 16f, 5 / 16f);
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

            projectile.SetProjectileSpriteRight("vengencespear_projectile_spearonly", 34, 4, true, tk2dBaseSprite.Anchor.MiddleCenter, 32, 3);

            /*
            List<string> projectileSpriteNames = new List<string>
            {
                "vengencespear_projectile_001",
                "vengencespear_projectile_002",
                "vengencespear_projectile_003",
            };
            int projectileFPS = 8;
            List<IntVector2> projectileSizes = new List<IntVector2>
            {
                new IntVector2(34, 12),
                new IntVector2(34, 12),
                new IntVector2(34, 12),
            };
            List<bool> projectileLighteneds = new List<bool>
            {
                true,
                true,
                true,
            };
            List<tk2dBaseSprite.Anchor> projectileAnchors = new List<tk2dBaseSprite.Anchor>
            {
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
            };
            List<bool> projectileAnchorsChangeColiders = new List<bool>
            {
                false,
                false,
                false,
            };
            List<bool> projectilefixesScales = new List<bool>
            {
                false,
                false,
                false,
            };
            List<Vector3?> projectileManualOffsets = new List<Vector3?>
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
            };
            List<IntVector2?> projectileOverrideColliderSizes = new List<IntVector2?>
            {
                new IntVector2(32, 10),
                new IntVector2(32, 10),
                new IntVector2(32, 10),
            };
            List<IntVector2?> projectileOverrideColliderOffsets = new List<IntVector2?>
            {
                null,
                null,
                null,
            };
            List<Projectile> projectileOverrideProjectilesToCopyFrom = new List<Projectile>
            {
                null,
                null,
                null,
            };
            tk2dSpriteAnimationClip.WrapMode ProjectileWrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

            projectile.AddAnimationToProjectile(projectileSpriteNames, projectileFPS, projectileSizes, projectileLighteneds, projectileAnchors, projectileAnchorsChangeColiders, projectilefixesScales,
                                                projectileManualOffsets, projectileOverrideColliderSizes, projectileOverrideColliderOffsets, projectileOverrideProjectilesToCopyFrom, ProjectileWrapMode);
            */

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("soul_spear_ammo",
                "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/vengencespear_ammo_full_001", "LOLItems/Resources/weapon_sprites/CustomGunAmmoTypes/vengencespear_ammo_empty_001");

            gun.shellCasing = null;
            gun.clipObject = null;
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.clipsToLaunchOnReload = 0;
            gun.reloadClipLaunchFrame = 0;

            EasyTrailBullet trail = projectile.gameObject.AddComponent<EasyTrailBullet>();
            trail.TrailPos = projectile.transform.position;
            trail.StartWidth = 0.15f;
            trail.EndWidth = 0f;
            trail.LifeTime = 0.1f;

            trail.BaseColor = Color.cyan;
            trail.StartColor = Color.white;
            trail.EndColor = Color.blue;

            gun.quality = PickupObject.ItemQuality.A;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;

            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            var sprite = EffectVFX.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 0.5f);
            }
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
                    /*if (fatal && enemyRendStacks.ContainsKey(enemy.aiActor))
                    {
                        enemyRendStacks.Remove(enemy.aiActor);
                    }

                    if (!enemyRendStacks.ContainsKey(enemy.aiActor))
                    {
                        enemyRendStacks.Add(enemy.aiActor, 1);
                        //enemy.aiActor.PlayEffectOnActor(EffectVFX, new Vector3(0 / 16f, 0 / 16f), true, false, false);

                    }
                    else
                    {
                        enemyRendStacks[enemy.aiActor] += 1;
                    }*/

                    if (fatal && activeVFXObjectList.ContainsKey(enemy.aiActor))
                    {
                        foreach (GameObject vfxObj in activeVFXObjectList[enemy.aiActor])
                        {
                            if (vfxObj != null)
                            {
                                Destroy(vfxObj);
                            }
                        }

                        activeVFXObjectList.Remove(enemy.aiActor);
                    }

                    Vector3 idk = (enemy as SpeculativeRigidbody).UnitDimensions;
                    //Plugin.Log($"unit dimensions: {idk}");
                    float num = ((idk.x + idk.y) / 2);
                    //idk.x *= UnityEngine.Random.Range(-1.1f, 1.1f);
                    //idk.y *= UnityEngine.Random.Range(-1.1f, 1.1f);

                    Vector3 offset = new Vector3(26 / 16f, 0 / 16f);
                    offset += new Vector3(num * UnityEngine.Random.Range(-0.3f, 0.3f), num * UnityEngine.Random.Range(-0.3f, 0.3f));
                    //Plugin.Log($"offset (randomized): {offset}");

                    //var smth = enemy.aiActor.PlayEffectOnActor(EffectVFX, new Vector3(26 / 16f, 0 / 16f), true, false, true);
                    var smth = enemy.aiActor.PlayEffectOnActor(EffectVFX, offset, true, false, true);
                    var sprite = smth.GetComponent<tk2dSprite>();

                    if (sprite != null)
                    {
                        sprite.transform.rotation = projectile.transform.rotation;
                    }

                    if (!activeVFXObjectList.ContainsKey(enemy.aiActor))
                    {
                        //var smth = enemy.aiActor.PlayEffectOnActor(EffectVFX, new Vector3(0 / 16f, 0 / 16f), true, false, false);
                        
                        activeVFXObjectList.Add(enemy.aiActor, new List<GameObject> {smth});
                        //enemy.aiActor.PlayEffectOnActor(EffectVFX, new Vector3(0 / 16f, 0 / 16f), true, false, false);
                    }
                    else
                    {
                        //var smth = enemy.aiActor.PlayEffectOnActor(EffectVFX, new Vector3(0 / 16f, 0 / 16f), true, false, false);

                        activeVFXObjectList[enemy.aiActor].Add(smth);
                    }
                }
            };

            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressedSafe(PlayerController player, Gun gun, bool manualReload)
        {
            float rendDamagePerStack = gun.DefaultModule.projectiles[0].baseData.damage * rendScale * player.stats.GetBaseStatValue(PlayerStats.StatType.Damage);
            //Plugin.Log($"rend damage per stack: {rendDamagePerStack}");
            /*foreach (KeyValuePair<AIActor, int> target in enemyRendStacks)
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
            */

            foreach (KeyValuePair<AIActor, List<GameObject>> target in activeVFXObjectList)
            {
                float damageToDeal = (target.Value.Count + 1) * rendDamagePerStack;
                //Plugin.Log($"rend stacks: {target.Value.Count}, damage dealt: {damageToDeal}");

                if (target.Key.healthHaver != null && target.Key.gameObject != null)
                {
                    target.Key.healthHaver.ApplyDamage(
                        damageToDeal,
                        Vector2.zero,
                        "soul_spear_rend_damage",
                        CoreDamageTypes.None,
                        DamageCategory.Normal,
                        false
                    );

                    Vector2 unitDimensions = target.Key.specRigidbody.HitboxPixelCollider.UnitDimensions;
                    Vector2 a = unitDimensions / 2f;

                    int num3 = Mathf.Min(target.Value.Count, 50);
                    Vector2 vector = target.Key.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
                    Vector2 vector2 = target.Key.specRigidbody.HitboxPixelCollider.UnitTopRight;
                    PixelCollider pixelCollider = target.Key.specRigidbody.GetPixelCollider(ColliderType.Ground);
                    if (pixelCollider != null && pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
                    {
                        vector = Vector2.Min(vector, pixelCollider.UnitBottomLeft);
                        vector2 = Vector2.Max(vector2, pixelCollider.UnitTopRight);
                    }
                    vector += Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                    vector2 -= Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                    vector2.y -= Mathf.Min(a.y * 0.1f, 0.1f);
                    //GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, Vector2.zero, 5f, 5f, 0.3f, 1, ExtendedColours.skyblue, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    HelpfulMethods.DoRandomParticleBurst(num3, vector, vector2, 1f, 1f, 0.3f, 1, Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    //anglevariance, magnitudevariance, startsize, startlifetime
                    //GlobalSparksDoer.DoRandomParticleBurst(Mathf.Max(target.Value.Count, 30), )
                }

                foreach (GameObject vfxObj in target.Value)
                {
                    if (vfxObj != null)
                    {
                        Destroy(vfxObj);
                    }
                }
            }

            activeVFXObjectList.Clear();
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
            //player.stats.RecalculateStats(player, true, false);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            base.OnSwitchedAwayFromThisGun();
        }

        public System.Collections.IEnumerator MartialPoiseDash(PlayerController player)
        {
            ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed);
            //player.stats.RecalculateStats(player, true, false);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            float statToMod = player.stats.GetStatValue(PlayerStats.StatType.MovementSpeed);
            ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.MovementSpeed, -statToMod, StatModifier.ModifyMethod.ADDITIVE);
            //player.stats.RecalculateStats(player, true, false);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            //Plugin.Log($"modifier: {-statToMod}");

            float duration = dashBaseDuration / player.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
            //float adjSpeed = dashBaseSpeed * player.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
            float adjSpeed = dashBaseSpeed * (1 + ((player.stats.GetStatValue(PlayerStats.StatType.RateOfFire) - 1) * 0.5f));
            float elapsed = -BraveTime.DeltaTime;

            player.healthHaver.TriggerInvulnerabilityPeriod(duration);

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
            //player.stats.RecalculateStats(player, true, false);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);
            //Plugin.Log($"modifier: {1f}");
        }
    }
}
