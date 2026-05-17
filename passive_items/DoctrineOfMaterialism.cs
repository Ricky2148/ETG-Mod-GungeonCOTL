using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using GungeonCOTL.custom_class_data;
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

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_materialism_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfMaterialism>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "CONSUME";
            string longDesc = "Each purchase decreases current shop prices by 5%\n\n" +
                "\"Preach on the value of earthly goods.\"\n\n" +
                "With each purchase made, making your next purchase gets easier.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, DiscountValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("doctrine_piece", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayDoctrineEffectOnActor(player, true, false, false);
            }

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

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

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
            //Plugin.Log($"discountVal: {actualDiscountVal}, num of items purchased: {NumItemsPurchased}");
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.GlobalPriceMultiplier, actualDiscountVal, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);
        }
    }
}
