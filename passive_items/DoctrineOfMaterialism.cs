using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class DoctrineOfMaterialism : PassiveItem
    {
        public static string ItemName = "Doctrine of Materialism";

        private int NumItemsPurchased = 0;
        private static float DiscountIncPerStack = 0.05f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_materialism_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfMaterialism>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, DiscountValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.OnItemPurchased += ShopItemPurchased;
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.OnItemPurchased -= ShopItemPurchased;
            }
        }

        public void ShopItemPurchased(PlayerController player, ShopItemController itemController)
        {
            ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.GlobalPriceMultiplier);

            NumItemsPurchased++;
            //float actualDiscountVal = 1f - (DiscountIncPerStack * NumItemsPurchased);
            float actualDiscountVal = Mathf.Pow((1f - DiscountIncPerStack), NumItemsPurchased);
            Plugin.Log($"discountVal: {actualDiscountVal}, num of items purchased: {NumItemsPurchased}");
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.GlobalPriceMultiplier, actualDiscountVal, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);
        }
    }
}
