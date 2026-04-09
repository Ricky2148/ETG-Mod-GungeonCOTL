using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class MightOfTheDevout1 : TieredPassiveItem
    {
        public static string ItemName = "Might of the Devout I";

        private static float DamageStat = 1.1f;

        public static int ID;
        public static bool isMightOfTheDevout = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/might_of_the_devout_1_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<MightOfTheDevout1>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "+Power";
            string longDesc = "1.1x damage\n" +
                "Strength of belief from your followers increase your power.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, DamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;

            item.itemTier = 1;
            item.TierGroupIdentifier = "might_of_the_devout_tiered_item";

            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("tarot_rune_draw", player.gameObject);
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
        }
    }
}
