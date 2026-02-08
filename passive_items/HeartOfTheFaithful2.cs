using Alexandria.ItemAPI;
using GungeonCOTL.active_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class HeartOfTheFaithful2 : TieredPassiveItem
    {
        public static string ItemName = "Heart of the Faithful II";

        private static float HealthStat = 2f;

        public static int ID;
        public static bool isHeartOfTheFaithful = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/heart_of_the_faithful_2_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HeartOfTheFaithful2>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);

            item.quality = PickupObject.ItemQuality.SPECIAL;

            item.itemTier = 2;
            item.TierGroupIdentifier = "heart_of_the_faithful_tiered_item";

            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
        }

        public override void Update()
        {
            if (Owner != null)
            {
                if (Owner.HasSynergy(Synergy.HEARTOFTHEFAITHFUL_TWO))
                {
                    //Plugin.Log($"synergy event");
                    Owner.RemovePassiveItem(HeartOfTheFaithful1.ID);
                }
            }

            base.Update();
        }
    }
}
