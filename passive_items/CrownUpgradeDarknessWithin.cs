using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class CrownUpgradeDarknessWithin : OnDamagedPassiveItem
    {
        public static string ItemName = "Crown Upgrade Darkness Within";

        private static float DarknessWithinDamage = 15f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/darkness_within_placeholder_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<CrownUpgradeDarknessWithin>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.SetName("Darkness Within");
            ID = item.PickupObjectId;
            //Plugin.Log($"ID: {ID}, pickupID: {item.PickupObjectId}");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.healthHaver.OnDamaged += OnPlayerDamaged;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

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

            DoBlankDamage(Owner);
        }
    }
}
