using Alexandria.ItemAPI;
using GungeonCOTL.active_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class HeartOfTheFaithful5 : TieredPassiveItem
    {
        public static string ItemName = "Heart of the Faithful V";

        private static int BlankStat = 2;
        private static int ArmorStat = 2;
        private static float HealthStat = 1f;

        public static int ID;
        public static bool isHeartOfTheFaithful = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/heart_of_the_faithful_5_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HeartOfTheFaithful5>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "+Defense";
            string longDesc = "+2 Blanks per floor, +2 Armor per floor, +1 Heart\n" +
                "Insane strength of faith from your followers drastically increase your defenses.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, BlankStat, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            item.ArmorToGainOnInitialPickup = ArmorStat;

            item.quality = PickupObject.ItemQuality.EXCLUDED;

            item.itemTier = 5;
            item.TierGroupIdentifier = "heart_of_the_faithful_tiered_item";

            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("tarot_rune_draw", player.gameObject);
            }

            base.Pickup(player);
            GameManager.Instance.OnNewLevelFullyLoaded += GainArmorOnLevelLoad;
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            GameManager.Instance.OnNewLevelFullyLoaded -= GainArmorOnLevelLoad;
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
        }
        public void GainArmorOnLevelLoad()
        {
            Owner.healthHaver.Armor += ArmorStat;
        }

        public override void Update()
        {
            if (Owner != null)
            {
                if (Owner.HasSynergy(Synergy.HEARTOFTHEFAITHFUL_FIVE))
                {
                    //Plugin.Log($"synergy event");
                    Owner.RemovePassiveItem(HeartOfTheFaithful1.ID);
                    Owner.RemovePassiveItem(HeartOfTheFaithful2.ID);
                    Owner.RemovePassiveItem(HeartOfTheFaithful3.ID);
                    Owner.RemovePassiveItem(HeartOfTheFaithful4.ID);
                }
            }

            base.Update();
        }
    }
}
