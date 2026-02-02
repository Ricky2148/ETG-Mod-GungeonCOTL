using Alexandria;
using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//uses custom class data to roll a chance to prevent damage when the player is hit

namespace GungeonCOTL.passive_items
{
    internal class CarefreeMelody : OnPreDamagedPassiveItem
    {
        private static float preDamageProcChance = 0.15f;

        public static int ID;

        public static void Init()
        {
            string itemName = "Carefree Melody";
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/carefree_melody_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<CarefreeMelody>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "rnjesus";
            string longDesc = "Contains a song of protection that may defend the bearer from damage.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            //item.ArmorToGainOnInitialPickup = ArmorStat;

            item.quality = PickupObject.ItemQuality.C;

            item.procChance = preDamageProcChance;
            item.triggersInvulnerability = true;
            item.effectDuration = 1f;
            item.playsSFX = false;
            string[] sfxList = null;
            //item.updateSFXList(sfxList);
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
