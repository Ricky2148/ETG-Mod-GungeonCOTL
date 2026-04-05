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

namespace GungeonCOTL.passive_items
{
    internal class CrownUpgradeDarknessWithin : OnDamagedPassiveItem
    {
        public static string ItemName = "Crown Upgrade Darkness Within";

        private static float DarknessWithinDamage = 15f;

        private static List<string> DiseasedHeartVFXPath = new List<string>
        {
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_01",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_02",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_03",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_04",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_05",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_06",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_07",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_08",
            "GungeonCOTL/Resources/vfxs/test_vfx/test_vfx_09",
            "GungeonCOTL/Resources/one_off_sprites/blank_sprite",
        };

        private static GameObject DiseasedHeartVFXEffect;

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
            ID = item.PickupObjectId;
            //Plugin.Log($"ID: {ID}, pickupID: {item.PickupObjectId}");

            DiseasedHeartVFXEffect = VFXBuilder.CreateVFX
            (
                "diseased_heart_vfx_effect",
                DiseasedHeartVFXPath,
                10,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );
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

            //DoBlankDamage(Owner);

            if (Owner.CurrentRoom == null) return;

            List<AIActor> enemyList = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemyList != null)
            {
                foreach (AIActor enemy in enemyList)
                {
                    enemy.StartCoroutine(DelayDamage(enemy));

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

        private System.Collections.IEnumerator DelayDamage(AIActor enemy)
        {
            enemy.PlayEffectOnActor(DiseasedHeartVFXEffect, new Vector3(0f, 0f, 0f), true, false, false);

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
            }
        }
    }
}
