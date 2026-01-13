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
        public static string ItemName = "Refillable Potion";

        public static int ID;

        public float duration = 12f;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "LOLItems/Resources/active_item_sprites/refillable_potion_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RefillablePotion>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "\"this healed me\"";
            string longDesc = "A potion that heals you.\n\n" +
                "Used to be a commercial product sold with the slogan \"It's so refreshing, you'll want to drink it twice! Thankfully, you can never finish it in one go!\"\n" +
                "The product also sold with a lifetime warranty of infinite refills anytime anywhere, which completely bankrupted the company.\n\n" +
                "Somehow, the refill service is still active but only between floors.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 25);

            item.numDrinksPerFloor = 2;
            item.healingAmount = 0.5f;
            item.drinkDuration = 0f;

            item.healVFX = (PickupObjectDatabase.GetById((int)Items.OldKnightsFlask) as EstusFlaskItem).healVFX;
            
            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.D;
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

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.healthHaver.GetCurrentHealthPercentage() >= 1.0f)
            {
                return false;
            }
            return base.m_remainingDrinksThisFloor > 0;
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
                //AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", base.gameObject);
                AkSoundEngine.PostEvent("Health_Potion_active_SFX", base.gameObject);

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
