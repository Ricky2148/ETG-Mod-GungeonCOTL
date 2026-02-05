using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class MightOfTheDevout2 : TieredPassiveItem
    {
        public static string ItemName = "Might of the Devout II";

        private static float DamageStat = 1.15f;

        public static int ID;
        public static bool isMightOfTheDevout = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<MightOfTheDevout2>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, DamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.SPECIAL;

            item.itemTier = 2;
            item.TierGroupIdentifier = "might_of_the_devout_tiered_item";

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
                if (Owner.HasSynergy(Synergy.MIGHTOFTHEDEVOUT_TWO))
                {
                    //Plugin.Log($"synergy event");
                    Owner.RemovePassiveItem(MightOfTheDevout1.ID);
                }
            }

            base.Update();
        }
    }
}
