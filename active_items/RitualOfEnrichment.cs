using Alexandria.ItemAPI;
using Brave.BulletScript;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//try reworking the spawning to be a circle instead of a square

namespace GungeonCOTL.active_items
{
    internal class RitualOfEnrichment : PlayerItem
    {
        public static string ItemName = "Ritual of Enrichment";

        private static int MoneyGiven = 50;

        private static float timeDelay = 0.05f;
        private static float timeDelayRandRatio = 0.4f;
        private static List<string> moneySFXList = new List<string>
        {
            "pop_1",
            "pop_2",
            "pop_3",
            "pop_4",
            "pop_5",
            "pop_6",
            "pop_7",
        };

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/ritual_of_enrichment_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RitualOfEnrichment>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("start_ritual", player.gameObject);
            }

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
            base.DoEffect(player);

            player.StartCoroutine(HelpfulMethods.SpawnMoney(player, MoneyGiven, timeDelay, true, timeDelayRandRatio, true, moneySFXList));

            player.RemoveActiveItem(ID);
        }

        /*private System.Collections.IEnumerator SpawnMoney(PlayerController player, int count)
        {
            //Plugin.Log($"start spawning");
            for (int i = 0;  i < count; i++)
            {
                //Plugin.Log($"i: {i}, count: {count}");
                Vector3 idk = player.specRigidbody.UnitDimensions;
                float num = ((idk.x + idk.y) / 2);
                Vector2 offset = new Vector3(num * UnityEngine.Random.Range(-3f, 3f), (num * UnityEngine.Random.Range(-3f, 3f)) + -1f);
                LootEngine.SpawnCurrency(player.specRigidbody.UnitBottomCenter + offset, 1);
                yield return new WaitForSeconds(0.05f);
            }
            //Plugin.Log($"finish spawning");
            yield return null;
        }*/
    }
}
