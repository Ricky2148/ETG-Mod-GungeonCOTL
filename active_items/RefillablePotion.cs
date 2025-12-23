using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using LOLItems.custom_class_data;
using LOLItems.guon_stones;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.active_items
{
    internal class RefillablePotion : EstusFlaskItem
    {
        public static int ID;

        public float duration = 12f;

        public static void Init()
        {
            string itemName = "Refillable Potion";
            string resourceName = "LOLItems/Resources/active_item_sprites/refillable_potion_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RefillablePotion>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 25);

            item.numDrinksPerFloor = 2;
            item.healingAmount = 0.5f;
            item.drinkDuration = 0f;

            item.healVFX = (PickupObjectDatabase.GetById((int)Items.OldKnightsFlask) as EstusFlaskItem).healVFX;
            
            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;
        }

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

        public override void DoEffect(PlayerController user)
        {
            if (m_remainingDrinksThisFloor > 0)
            {
                if (healVFX != null)
                {
                    user.PlayEffectOnActor(healVFX, Vector3.zero);
                }

                m_remainingDrinksThisFloor--;
                user.healthHaver.ApplyHealing(healingAmount);
                AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", base.gameObject);

                StartCoroutine(HandleDuration(user));
            }
            if (m_remainingDrinksThisFloor <= 0)
            {
                base.sprite.SetSprite(NoDrinkSprite);
            }
        }

        private IEnumerator HandleDuration(PlayerController user)
        {
            if (base.IsCurrentlyActive)
            {
                Debug.LogError("Using a ActiveBasicStatItem while it is already active!");
                yield break;
            }
            base.IsCurrentlyActive = true;
            m_activeElapsed = 0f;
            m_activeDuration = duration;
            while (m_activeElapsed < m_activeDuration && base.IsCurrentlyActive)
            {
                yield return null;
            }
            base.IsCurrentlyActive = false;
        }
    }
}
