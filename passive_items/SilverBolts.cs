using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using LOLItems.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// might use a stack based system like puppeteer

namespace LOLItems.passive_items
{
    internal class SilverBolts : PassiveItem
    {
        private int silverBoltsCount = 0;
        private static float silverBoltsPercentHealthDamage = 0.15f;
        private static float silverBoltsBaseDamage = 5f;

        private AIActor lastHitEnemy;

        private static List<string> sfxList = new List<string>
        {
            "kraken_slayer_passive_SFX_1",
            "kraken_slayer_passive_SFX_2",
            "kraken_slayer_passive_SFX_3"
        };

        private static List<string> VFXSpritePath = new List<string>
            {
                "LOLItems/Resources/vfxs/silver_bolts_vfx/ring_inner_001",
            };

        private static GameObject EffectVFX;

        private static List<string> SecondVFXSpritePath = new List<string>
            {
                "LOLItems/Resources/vfxs/silver_bolts_vfx/ring_double_001",
            };

        private static GameObject SecondEffectVFX;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = "Silver Bolts";
            string resourceName = "LOLItems/Resources/passive_item_sprites/silver_bolts_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SilverBolts>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.C;

            ID = item.PickupObjectId;

            EffectVFX = VFXBuilder.CreateVFX
            (
                "silver_bolts_ring_inner",
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

            var sprite = EffectVFX.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -50f;
                sprite.UpdateZDepth();
            }

            SecondEffectVFX = VFXBuilder.CreateVFX
            (
                "silver_bolts_ring_double",
                SecondVFXSpritePath,
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

            sprite = SecondEffectVFX.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -50f;
                sprite.UpdateZDepth();
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.PostProcessProjectile += OnPostProcessProjectile;
            player.PostProcessBeamTick += OnPostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            player.PostProcessProjectile -= OnPostProcessProjectile;
            player.PostProcessBeamTick -= OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickrate)
        {
            float randomVal = UnityEngine.Random.value;
            if (randomVal <= 0.04f)
            {
                //Plugin.Log($"randomVal: {randomVal}, silverboltscount: {silverBoltsCount}");

                if (hitRigidbody == null) return;
                AIActor targetEnemy = null;
                if (hitRigidbody.aiActor != null)
                {
                    targetEnemy = hitRigidbody.aiActor;
                }
                else if (hitRigidbody.GetComponentInParent<AIActor>() != null)
                {
                    targetEnemy = hitRigidbody.GetComponentInParent<AIActor>();
                }
                else
                {
                    return;
                }
                if (hitRigidbody.healthHaver != null)
                {
                    if (targetEnemy != lastHitEnemy)
                    {
                        silverBoltsCount = 0;
                        lastHitEnemy = targetEnemy;
                    }

                    if (activeVFXObject != null)
                    {
                        Destroy(activeVFXObject);
                    }

                    silverBoltsCount++;

                    switch (silverBoltsCount)
                    {
                        case 1:
                            //Plugin.Log($"{silverBoltsCount}");
                            activeVFXObject = targetEnemy.PlayEffectOnActor(EffectVFX, new Vector3(0, 0, 0), true, false, false);
                            break;
                        case 2:
                            //Plugin.Log($"{silverBoltsCount}");
                            activeVFXObject = targetEnemy.PlayEffectOnActor(SecondEffectVFX, new Vector3(0, 0, 0), true, false, false);
                            break;
                        case 3:
                            //Plugin.Log($"{silverBoltsCount}");
                            float damageToDeal = (hitRigidbody.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                            // damage is 1/4 against bosses and sub-bosses
                            if (hitRigidbody.healthHaver.IsBoss || hitRigidbody.healthHaver.IsSubboss)
                            {
                                damageToDeal *= 0.25f;
                            }

                            // calculates additional extra damage to apply to enemy
                            hitRigidbody.healthHaver.ApplyDamage(
                                damageToDeal,
                                Vector2.zero,
                                "silver_bolts_damage",
                                CoreDamageTypes.None,
                                DamageCategory.Normal,
                                false
                            );
                            //Plugin.Log($"damage dealt: {damageToDeal}");
                            silverBoltsCount = 0;
                            break;
                    }
                }
            }
            /*if (silverBoltsCount >= 3)
            {
                if (beam.sprite != null)
                {
                    beam.sprite.color = Color.Lerp(beam.sprite.color, Color.white, 0.8f);
                }
                //HelpfulMethods.PlayRandomSFX(beam.gameObject, sfxList);
                if (hitRigidbody == null) return;
                AIActor firstEnemy = null;
                if (hitRigidbody.aiActor != null)
                {
                    firstEnemy = hitRigidbody.aiActor;
                }
                else if (hitRigidbody.GetComponentInParent<AIActor>() != null)
                {
                    firstEnemy = hitRigidbody.GetComponentInParent<AIActor>();
                }
                else
                {
                    return;
                }
                if (hitRigidbody.healthHaver != null)
                {
                    // scale damage down by tickrate
                    float damageToDeal = (firstEnemy.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                    // damage is 1/4 against bosses and sub-bosses
                    if (firstEnemy.healthHaver.IsBoss || firstEnemy.healthHaver.IsSubboss)
                    {
                        damageToDeal *= 0.25f;
                    }
                    // calculates additional extra damage to apply to enemy
                    firstEnemy.healthHaver.ApplyDamage(
                        damageToDeal,
                        Vector2.zero,
                        "silver_bolts_damage",
                        CoreDamageTypes.None,
                        DamageCategory.Normal,
                        false
                    );
                    Plugin.Log($"damage dealt: {damageToDeal}");
                }
                silverBoltsCount = 0;
            }*/
        }

        // TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING 
        private static GameActorSpeedEffect slowEffect = new GameActorSpeedEffect
        {
            duration = 100f,
            effectIdentifier = "botrk_slow",
            resistanceType = EffectResistanceType.Freeze,
            AppliesOutlineTint = true,
            OutlineTintColor = Color.cyan,
            SpeedMultiplier = 0f,
        };
        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Shooter == proj.Owner.specRigidbody)
            {
                /*silverBoltsCount++;
                if (silverBoltsCount >= 3)
                {
                    if (proj.sprite != null)
                    {
                        // doesnt seem to work for some strange reason
                        //proj.sprite.color = Color.Lerp(proj.sprite.color, Color.white, 0.7f);
                        proj.sprite.color = ExtendedColours.silver;
                    }
                    HelpfulMethods.PlayRandomSFX(proj.gameObject, sfxList);
                    proj.OnHitEnemy += (projHit, enemy, fatal) =>
                    {
                        if (enemy == null) return;
                        if (enemy.aiActor == null && enemy.GetComponentInParent<AIActor>() == null) return;
                        if (enemy.healthHaver != null)
                        {
                            float damageToDeal = (enemy.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                            // damage is 1/4 against bosses and sub-bosses
                            if (enemy.healthHaver.IsBoss || enemy.healthHaver.IsSubboss)
                            {
                                damageToDeal *= 0.25f;
                            }

                            // calculates additional extra damage to apply to enemy
                            enemy.healthHaver.ApplyDamage(
                                damageToDeal,
                                Vector2.zero,
                                "silver_bolts_damage",
                                CoreDamageTypes.None,
                                DamageCategory.Normal,
                                false
                            );
                            Plugin.Log($"damage dealt: {damageToDeal}");
                        }
                    };
                    silverBoltsCount = 0;
                }*/

                proj.OnHitEnemy += (projHit, enemy, fatal) =>
                {
                    if (enemy == null) return;
                    AIActor targetEnemy = null;
                    if (enemy.aiActor != null)
                    {
                        targetEnemy = enemy.aiActor;
                    }
                    else if (enemy.GetComponentInParent<AIActor>() != null)
                    {
                        targetEnemy = enemy.GetComponentInParent<AIActor>();
                    }
                    else
                    {
                        return;
                    }
                    if (enemy.healthHaver != null)
                    {
                        if (targetEnemy != lastHitEnemy)
                        {
                            silverBoltsCount = 0;
                            lastHitEnemy = targetEnemy;
                        }

                        if (activeVFXObject != null)
                        {
                            Destroy(activeVFXObject);
                        }

                        // TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING TESTING 
                        targetEnemy.ApplyEffect(slowEffect);

                        silverBoltsCount++;

                        switch (silverBoltsCount)
                        {
                            case 1:
                                //Plugin.Log($"{silverBoltsCount}");

                                /*activeVFXObject = targetEnemy.PlayEffectOnActor(EffectVFX, new Vector3(0 / 16f, 0 / 16f, 0f), true, false, true);

                                var sprite = activeVFXObject.GetComponent<tk2dSprite>();

                                if (sprite != null)
                                {
                                    sprite.HeightOffGround = -10f;
                                    sprite.UpdateZDepth();
                                }*/

                                GameObject gameObject = SpawnManager.SpawnVFX(EffectVFX);
                                tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                                component.transform.position = targetEnemy.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0, -2f);
                                //Plugin.Log($"sprite.worldCenter: {targetEnemy.sprite.WorldCenter.ToVector3ZUp()}, specRigidBody.HitBoxPicelCollider.unitCenter: {targetEnemy.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp()}, specRigidBody.UnitCenter: {targetEnemy.specRigidbody.UnitCenter}");
                                //gameObject.transform.parent = targetEnemy.transform;
                                component.HeightOffGround = -10f;
                                targetEnemy.sprite.AttachRenderer(component);

                                activeVFXObject = gameObject;

                                break;
                            case 2:
                                //Plugin.Log($"{silverBoltsCount}");
                                /*activeVFXObject = targetEnemy.PlayEffectOnActor(SecondEffectVFX, new Vector3(0 / 16f, 0 / 16f, 0f), true, false, true);

                                sprite = activeVFXObject.GetComponent<tk2dSprite>();

                                if (sprite != null)
                                {
                                    sprite.HeightOffGround = -10f;
                                    sprite.UpdateZDepth();
                                }*/

                                gameObject = SpawnManager.SpawnVFX(SecondEffectVFX);
                                component = gameObject.GetComponent<tk2dBaseSprite>();
                                component.transform.position = targetEnemy.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0, -2f);
                                //Plugin.Log($"sprite.worldCenter: {targetEnemy.sprite.WorldCenter.ToVector3ZUp()}, specRigidBody.HitBoxPicelCollider.unitCenter: {targetEnemy.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp()}, specRigidBody.UnitCenter: {targetEnemy.specRigidbody.UnitCenter}");
                                //gameObject.transform.parent = targetEnemy.transform;
                                component.HeightOffGround = -10f;
                                targetEnemy.sprite.AttachRenderer(component);

                                activeVFXObject = gameObject;

                                break;
                            case 3:
                                //Plugin.Log($"{silverBoltsCount}");
                                HelpfulMethods.PlayRandomSFX(this.gameObject, sfxList);
                                float damageToDeal = (enemy.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                                // damage is 1/4 against bosses and sub-bosses
                                if (enemy.healthHaver.IsBoss || enemy.healthHaver.IsSubboss)
                                {
                                    damageToDeal *= 0.25f;
                                }

                                damageToDeal = 0;

                                // calculates additional extra damage to apply to enemy
                                enemy.healthHaver.ApplyDamage(
                                    damageToDeal,
                                    Vector2.zero,
                                    "silver_bolts_damage",
                                    CoreDamageTypes.None,
                                    DamageCategory.Normal,
                                    false
                                );
                                //Plugin.Log($"damage dealt: {damageToDeal}");
                                silverBoltsCount = 0;
                                break;
                        }
                    }
                };
            }
        }
    }
}
