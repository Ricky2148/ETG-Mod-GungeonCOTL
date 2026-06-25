using Alexandria.ItemAPI;
using GungeonCOTL.active_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class HeartOfTheFaithful4 : TieredPassiveItem
    {
        public static string ItemName = "Heart of the Faithful IV";

        private static int BlankStat = 2;
        private static int ArmorStat = 1;
        private static float HealthStat = 1f;

        public static int ID;
        public static bool isHeartOfTheFaithful = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/heart_of_the_faithful_4_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HeartOfTheFaithful4>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "+Defense";
            string longDesc = "+2 Blanks per floor, +1 Armor per floor, +1 Heart\n" +
                "Immense strength of faith from your followers increase your defenses greatly.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, BlankStat, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);

            item.quality = PickupObject.ItemQuality.EXCLUDED;

            item.itemTier = 4;
            item.TierGroupIdentifier = "heart_of_the_faithful_tiered_item";

            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("tarot_rune_draw", player.gameObject);
                player.m_blanks += BlankStat;
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
                if (Owner.HasSynergy(Synergy.HEARTOFTHEFAITHFUL_FOUR))
                {
                    //Plugin.Log($"synergy event");
                    Owner.RemovePassiveItem(HeartOfTheFaithful1.ID);
                    Owner.RemovePassiveItem(HeartOfTheFaithful2.ID);
                    Owner.RemovePassiveItem(HeartOfTheFaithful3.ID);
                }
            }

            base.Update();
        }
    }
}
