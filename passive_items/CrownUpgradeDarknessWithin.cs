using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using Dungeonator;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//vfx concept: like a dark wave that goes outwards (can be pretty big, ideally with some kind of dark black and grey vine theming)
// damage enemies in sequence using +0.1s in coroutine start

namespace GungeonCOTL.passive_items
{
    internal class CrownUpgradeDarknessWithin : OnDamagedPassiveItem
    {
        public static string ItemName = "Crown Upgrade Darkness Within";

        private static float DarknessWithinDamage = 15f;

        private static List<string> DiseasedHeartVFXPath = new List<string>
        {
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_001",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_002",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_003",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_004",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_005",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_006",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_007",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_008",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_009",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_010",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_011",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_012",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_013",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_014",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_015",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_016",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_017",
            "GungeonCOTL/Resources/vfxs/diseased_heart_vfx/badheart_018",
            "GungeonCOTL/Resources/one_off_sprites/blank_sprite",
        };

        private static GameObject DiseasedHeartVFXEffect;

        public static Vector3 vfxOffset = new Vector3(-1 / 16f, 6 / 16f, 0);

        private static List<string> sfxList = new List<string>
        {
            "punchy_blessed_choir1",
            "punchy_blessed_choir2",
            "punchy_blessed_choir3",
            "punchy_blessed_choir4",
            "punchy_blessed_choir5",
            "punchy_blessed_choir6",
        };

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/darkness_within_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<CrownUpgradeDarknessWithin>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Blackened Heart";
            string longDesc = "Deal damage to all enemies every time you take damage.\n\n" +
                "Bearing the sins of your followers has caused your heart and soul to become blackened with darkness. It leaks out with ill will and lashes out against any who harm them.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.SetName("Darkness Within");
            item.CanBeDropped = false; ID = item.PickupObjectId;
            //Plugin.Log($"ID: {ID}, pickupID: {item.PickupObjectId}");

            DiseasedHeartVFXEffect = VFXBuilder.CreateVFX
            (
                "diseased_heart_vfx_effect",
                DiseasedHeartVFXPath,
                12,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );

            VFXAnchorModule anchor = DiseasedHeartVFXEffect.GetOrAddComponent<VFXAnchorModule>();
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("crown_upgrade_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayCrownUpgradeEffectOnActor(player);
                player.StartCoroutine(VFXPlayerCOTL.HardCodedCrownUpgradeEffectSFXPlayer(player));
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.healthHaver.OnDamaged += OnPlayerDamaged;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            if (player != null)
            {
                player.healthHaver.OnDamaged -= OnPlayerDamaged;
            }
        }

        private void DoBlankDamage(PlayerController player)
        {
            if (player.CurrentRoom == null) return;

            List<AIActor> enemyList = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemyList != null)
            {
                foreach (AIActor enemy in enemyList)
                {
                    if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
                    {
                        enemy.healthHaver.ApplyDamage(
                            DarknessWithinDamage,
                            Vector2.zero,
                            "darkness_within_blank_damage",
                            CoreDamageTypes.None,
                            DamageCategory.Normal,
                            false
                        );
                    }
                }
            }
        }

        private void OnPlayerDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            //Owner.ForceBlank();
            activeVFXObject = UnityEngine.Object.Instantiate(VFXPlayerCOTL.DarknessWithinActivationEffectVFX, Owner.CenterPosition, Quaternion.identity);
            HelpfulMethods.PlayRandomSFX(Owner.gameObject, sfxList);
            //DoBlankDamage(Owner);

            if (Owner.CurrentRoom == null) return;

            List<AIActor> enemyList = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemyList != null)
            {
                float initialTimeDelay = 0f;
                foreach (AIActor enemy in enemyList)
                {
                    enemy.StartCoroutine(DelayDamage(enemy, initialTimeDelay));
                    initialTimeDelay += 0.1f;
                    /*if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
                    {
                        enemy.healthHaver.ApplyDamage(
                            DarknessWithinDamage,
                            Vector2.zero,
                            "darkness_within_blank_damage",
                            CoreDamageTypes.None,
                            DamageCategory.Normal,
                            false
                        );
                    }*/
                }
            }
        }

        private System.Collections.IEnumerator DelayDamage(AIActor enemy, float initialTimeDelay)
        {
            yield return new WaitForSeconds(initialTimeDelay);

            //Vector3 effectOffset = (enemy.sprite.WorldTopCenter - enemy.sprite.WorldCenter).ToVector3ZUp() + new Vector3(0 / 16f, 0 / 16f, 0f);
            //enemy.PlayEffectOnActor(DiseasedHeartVFXEffect, effectOffset, true, false, false);

            //Plugin.Log($"effectOffset: ({enemy.sprite.WorldTopCenter} - {enemy.sprite.WorldCenter}) + {new Vector3(0 / 16f, 0 / 16f, 0f)} = {(enemy.sprite.WorldTopCenter - enemy.sprite.WorldCenter).ToVector3ZUp()}");
            //Plugin.Log($"anchor: {enemy.sprite.WorldCenter.ToVector3ZUp()}");
            
            GameObject vfxObject = UnityEngine.Object.Instantiate(DiseasedHeartVFXEffect, enemy.specRigidbody.UnitBottomCenter.ToVector3ZUp() + vfxOffset, Quaternion.identity);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = 10f;
                sprite.UpdateZDepth();

                sprite.scale *= Mathf.Max(1f, 1f + ((enemy.specRigidbody.UnitDimensions.x - 1f) / 2f));
                //Plugin.Log($"UnitDimensions.x: {targetEnemy.specRigidbody.UnitDimensions.x}, scale mult: {sprite.scale}");
            }

            vfxObject.GetComponent<VFXAnchorModule>().anchorAIActor = enemy;
            vfxObject.GetComponent<VFXAnchorModule>().offset = vfxOffset + new Vector3(0, enemy.specRigidbody.HitboxPixelCollider.UnitDimensions.y);

            yield return new WaitForSeconds(1f);

            if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
            {
                enemy.healthHaver.ApplyDamage(
                    DarknessWithinDamage,
                    Vector2.zero,
                    "darkness_within_diseased_heart_damage",
                    CoreDamageTypes.None,
                    DamageCategory.Normal,
                    false
                );

                HelpfulMethods.PlayRandomSFX(enemy.gameObject, sfxList);
            }
        }
    }
}
