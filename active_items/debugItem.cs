using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using Dungeonator;
using GungeonCOTL.passive_items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.active_items
{
    internal class debugItem : PlayerItem
    {
        public static int ID;

        public static void Init()
        {
            string itemName = "Debug Item";
            string resourceName = "GungeonCOTL/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<debugItem>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "testing";
            string longDesc = "some BULLSHIT is being run";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            item.consumable = false;

            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        // subscribe to the player events
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            return base.Drop(player);
        }

        public override void DoEffect(PlayerController player)
        {
            PhysicsEngine.SkipCollision = !PhysicsEngine.SkipCollision;
            Plugin.Log($"skipCollision: {PhysicsEngine.SkipCollision}");
        }
    }
}
