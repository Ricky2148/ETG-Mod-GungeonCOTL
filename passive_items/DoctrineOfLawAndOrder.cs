using Alexandria.ItemAPI;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class DoctrineOfLawAndOrder : PassiveItem
    {
        public static string ItemName = "Doctrine of Law And Order";

	    public float percentageHealthReduction = 0.1f;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_law_and_order_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfLawAndOrder>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, DiscountValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.SetName("Doctrine of Law & Order");

            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("doctrine_piece", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayDoctrineEffectOnActor(player, true, false, false);
            }

            if (!m_pickedUp)
            {
                base.Pickup(player);
                AIActor.HealthModifier *= Mathf.Clamp01(1f - percentageHealthReduction);
            }

            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public override DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            DebrisObject debrisObject = base.Drop(player);
            //debrisObject.GetComponent<GundromedaStrain>().m_pickedUpThisRun = true;
            debrisObject.GetComponent<DoctrineOfLawAndOrder>().m_pickedUpThisRun = true;
            AIActor.HealthModifier /= Mathf.Clamp01(1f - percentageHealthReduction);
            return debrisObject;
        }
    }
}