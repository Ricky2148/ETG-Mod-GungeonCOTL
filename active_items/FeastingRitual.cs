using Alexandria.ItemAPI;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.active_items
{
    internal class FeastingRitual : HealPlayerItem
    {
        public static string ItemName = "Feasting Ritual";

        private static StatModifier ownerlessCurseModifier = StatModifier.Create(PlayerStats.StatType.Curse, StatModifier.ModifyMethod.ADDITIVE, 1.0f);

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/feasting_ritual_pixelart_sprite_corpse";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<FeastingRitual>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "yum...";
            string longDesc = "Fully heals the player and +1 curse on use\n\n" +
                "A freshly prepared c*r**e, cooked and seasoned to perfection. It tastes really good, however each bite makes you feel more and more guilty.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.healVFX = (PickupObjectDatabase.GetById((int)Items.Meatbun) as HealPlayerItem).healVFX;
            //item.healVFX = null;
            item.healingAmount = 30f;

            item.consumable = true;
            item.usableDuringDodgeRoll = false;
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

                AkSoundEngine.PostEvent("ritual_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayRitualActivationEffectOnActor(player, true, false, false);
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }
            return base.Drop(player);
        }

        public override bool CanBeUsed(PlayerController player)
        {
            //Plugin.Log($"hp %: {player.healthHaver.GetCurrentHealthPercentage()}");
            if (player.healthHaver.GetCurrentHealthPercentage() >= 1.0)
            {
                return false;
            }
            return base.CanBeUsed(player);
        }

        public override void DoEffect(PlayerController player)
        {
            player.healthHaver.ApplyHealing(GetHealingAmount(player));
            if (healVFX != null)
            {
                player.PlayEffectOnActor(healVFX, Vector3.zero);
            }
            AkSoundEngine.PostEvent("feasting_ritual_activation", base.gameObject);

            player.ownerlessStatModifiers.Add(ownerlessCurseModifier);
        }
    }
}
