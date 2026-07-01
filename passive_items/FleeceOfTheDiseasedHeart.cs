using Alexandria.ItemAPI;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class FleeceOfTheDiseasedHeart : PassiveGooperItem
    {
        public static string ItemName = "Fleece of the Diseased Heart";

        private static List<GoopDefinition> goopList = new List<GoopDefinition>
        {
            GoopUtility.FireDef,
            GoopUtility.PoisonDef,
            GoopUtility.WebDef,
        };

        public static bool isFleece = true;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/fleece_of_the_diseased_heart_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<FleeceOfTheDiseasedHeart>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            item.condition = Condition.OnDamaged;
            item.goopType = GoopUtility.MimicSpitDef;
            item.goopRadius = 5f;
            item.TranslatesGleepGlorp = true;
            item.modifiers = null;

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.OnReceivedDamage += UpdateGoopType;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.OnReceivedDamage -= UpdateGoopType;
            }
        }

        private void UpdateGoopType(PlayerController player)
        {
            int randVal = UnityEngine.Random.Range(0, goopList.Count - 1);

            m_cachedGoopType = goopList[randVal]; 
            goopType = goopList[randVal];

            Plugin.Log($"chose {randVal}, goopType: {goopType}, cachedGoopType: {m_cachedGoopType}");
        }
    }
}