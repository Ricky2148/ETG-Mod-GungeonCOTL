using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;
using Alexandria;
using LOLItems.custom_class_data;
using Alexandria.Misc;
using LOLItems.passive_items;
using LOLItems.weapons;
using LOLItems.guon_stones;

namespace LOLItems
{
    internal class debugItem : PlayerItem
    {
        public static int ID;

        public static void Init()
        {
            string itemName = "Debug Item";
            string resourceName = "LOLItems/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<debugItem>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            item.consumable = false;

            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }

        // subscribe to the player events
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

        public override void DoEffect(PlayerController player)
        {
            Plugin.Log($"Manamune is {Manamune.ID}");
            Plugin.Log($"Muramana is {Muramana.ID}");
            Plugin.Log($"MuramanaSynergyActivation is {MuramanaSynergyActivation.ID}");
            Plugin.Log($"PowPow is {PowPow.ID}");
            Plugin.Log($"PowPowAltForm is {PowPowAltForm.ID}");
            Plugin.Log($"BladeOfTheRuinedKing is {BladeOfTheRuinedKing.ID}");
            Plugin.Log($"CloakOfStarryNight is {CloakOfStarryNight.ID}");
            Plugin.Log($"ShieldOfMoltenStone is {ShieldOfMoltenStone.ID}");
            Plugin.Log($"BraumsShield is {BraumsShield.ID}");
            //StartCoroutine(EffectCoroutine(player));
        }

        private System.Collections.IEnumerator EffectCoroutine(PlayerController player)
        {
            player.aiAnimator.FpsScale = 0f;
            yield return new WaitForSeconds(2f);
            player.aiAnimator.FpsScale = 1f;
            yield return new WaitForSeconds(0.5f);
        }

        /*private System.Collections.IEnumerator StasisCoroutine(PlayerController player)
        {
            player.healthHaver.TriggerInvulnerabilityPeriod(StasisDuration);
            player.CurrentInputState = PlayerInputState.NoInput;
            player.healthHaver.PreventAllDamage = true;

            Color originalPlayerColor = player.sprite.color;
            Color originalGunColor = player.CurrentGun.sprite.color;

            // find a better color later
            player.sprite.color = ExtendedColours.honeyYellow;
            player.CurrentGun.sprite.color = ExtendedColours.honeyYellow;

            AkSoundEngine.PostEvent("zhonyas_hourglass_activation_SFX", GameManager.Instance.gameObject);

            yield return new WaitForSeconds(StasisDuration);

            player.sprite.color = originalPlayerColor;
            player.CurrentGun.sprite.color = originalGunColor;

            player.ForceBlank();
            player.CurrentInputState = PlayerInputState.AllInput;

            // wait an extra 0.25 seconds to prevent player from immediate collision damage
            yield return new WaitForSeconds(0.25f);
            player.healthHaver.PreventAllDamage = false;

            AkSoundEngine.PostEvent("zhonyas_hourglass_ending_SFX", GameManager.Instance.gameObject);
        }*/
    }
}
