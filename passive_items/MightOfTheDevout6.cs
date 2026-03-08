using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class MightOfTheDevout6 : TieredPassiveItem
    {
        public static string ItemName = "Might of the Devout VI";

        private static float DamageStat = 1.75f;

        public static int ID;
        public static bool isMightOfTheDevout = true;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/might_of_the_devout_6_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<MightOfTheDevout6>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "+Power";
            string longDesc = "1.75x damage\n" +
                "Complete and absolute belief from your followers increase your power on par with the gods.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, DamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.EXCLUDED;

            item.itemTier = 6;
            item.TierGroupIdentifier = "might_of_the_devout_tiered_item";

            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("tarot_rune_draw", player.gameObject);
            }

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
                if (Owner.HasSynergy(Synergy.MIGHTOFTHEDEVOUT_SIX))
                {
                    //Plugin.Log($"synergy event");
                    Owner.RemovePassiveItem(MightOfTheDevout1.ID);
                    Owner.RemovePassiveItem(MightOfTheDevout2.ID);
                    Owner.RemovePassiveItem(MightOfTheDevout3.ID);
                    Owner.RemovePassiveItem(MightOfTheDevout4.ID);
                    Owner.RemovePassiveItem(MightOfTheDevout5.ID);
                }
            }

            base.Update();
        }
    }
}
