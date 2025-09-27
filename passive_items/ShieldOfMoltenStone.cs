using Alexandria;
using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//uses custom class data to roll a chance to prevent damage when the player is hit

namespace LOLItems.passive_items
{
    internal class ShieldOfMoltenStone : OnPreDamagedPassiveItem
    {
        private static float HealthStat = 1f;
        private static int ArmorStat = 1;

        private static float preDamageProcChance = 0.1f;
        private static float synergyProcChance = 0.3f;

        public static int ID;

        public static void Init()
        {
            string itemName = "Shield of Molten Stone";
            string resourceName = "LOLItems/Resources/passive_item_sprites/shield_of_molten_stone_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<ShieldOfMoltenStone>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "shield";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            item.ArmorToGainOnInitialPickup = ArmorStat;

            item.quality = PickupObject.ItemQuality.B;

            item.procChance = preDamageProcChance;
            item.triggersInvulnerability = true;
            item.invulnerabilityDuration = 1f;
            item.playsSFX = true;
            string[] sfxList = { "stridebreaker_active_SFX" };
            item.updateSFXList(sfxList);
            ID = item.PickupObjectId;
        }

        // updates stats for both items if synergy is active on pickup
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            if (player.PlayerHasActiveSynergy("Shield and Cloak Synergy"))
            {
                this.procChance = synergyProcChance;
                Plugin.Log("Shield of Molten Stone's Synergy activated");

                foreach (PassiveItem item in player.passiveItems)
                {
                    if (item.PickupObjectId == CloakOfStarryNight.ID && item != null)
                    {
                        item.GetComponent<CloakOfStarryNight>().setProcChance(synergyProcChance);
                    }
                }
            }
        }

        // updates stats for both items if synergy is active on drop
        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            this.procChance = preDamageProcChance;
            foreach (PassiveItem item in player.passiveItems)
            {
                if (item.PickupObjectId == CloakOfStarryNight.ID && item != null)
                {
                    item.GetComponent<CloakOfStarryNight>().setProcChance(preDamageProcChance);
                }
            }
        }
    }
}
