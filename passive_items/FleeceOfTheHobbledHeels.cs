using Alexandria.ItemAPI;
using GungeonCOTL.custom_class_data;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//buffs the player greatly and heals them per room clear, removes the ability to dodge roll
//add custom vfx and sfx for the heal

namespace GungeonCOTL.passive_items
{
    internal class FleeceOfTheHobbledHeels : PassiveItem
    {
        public static string ItemName = "Fleece of the Hobbled Heels";

        private static float HealthStat = 2f;
        private static float RateOfFireStat = 1.20f;
        private static float MovementSpeedStat = 2.5f;

        private static float HealAmount = 0.5f;

        private static GameObject healVFX;

        public static bool isFleece = true;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/fleece_of_the_hobbled_heels_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<FleeceOfTheHobbledHeels>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Embrace Rollerskates?";
            string longDesc = "Increases fire rate and movement speed. Heal for a half heart upon room clear. Can no longer dodge roll.\n\n" +
                "The fleece of a weirdly spiky enemy you encountered one day. His speed despite being on the verge of death was astonishing to see " +
                "and made you wonder what donning his fleece could do. The secret of his speed appears to be a set of juiced up rollerskates, " +
                "but your inexperience with them prevents you from doing anything but running.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, RateOfFireStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, MovementSpeedStat, StatModifier.ModifyMethod.ADDITIVE);

            healVFX = (PickupObjectDatabase.GetById((int)Items.OldKnightsFlask) as EstusFlaskItem).healVFX;

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("crown_upgrade_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayCrownUpgradeEffectOnActor(player);
                player.StartCoroutine(VFXPlayerCOTL.HardCodedCrownUpgradeEffectSFXPlayer(player));
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            //player.OnPreDodgeRoll += PreventDodgeRoll;
            player.OnRoomClearEvent += HealPlayer;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            if (player != null)
            {
                //player.OnPreDodgeRoll -= PreventDodgeRoll;
                player.OnRoomClearEvent -= HealPlayer;
            }
        }

        private void HealPlayer(PlayerController player)
        {
            player.PlayEffectOnActor(healVFX, Vector3.zero);

            player.healthHaver.ApplyHealing(HealAmount);
            AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", base.gameObject);
        }

        [HarmonyPatch]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.HandleStartDodgeRoll))]
        internal static class PlayerControllerHandleStartDodgeRollPatch
        {
            static bool Prefix(PlayerController __instance, Vector2 direction, ref bool __result)
            {
                if (__instance.HasPassiveItem(ID)) 
                {
                    //Plugin.Log($"does have hobbled heels: {__instance.HasPassiveItem(ID)}");
                    return false;
                }
                return true;
            }
        }
    }
}
