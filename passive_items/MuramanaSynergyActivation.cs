using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria;
namespace LOLItems.passive_items
{
    internal class MuramanaSynergyActivation : PassiveItem
    {
        // stats pool for item
        public static int ID;

        public static void Init()
        {
            string itemName = "Manaflow Fully Stacked";
            string resourceName = "LOLItems/Resources/black_dot";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<MuramanaSynergyActivation>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "       ";
            string longDesc = "       ";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.EXCLUDED;

            //item.SetName("");
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
    }
}
