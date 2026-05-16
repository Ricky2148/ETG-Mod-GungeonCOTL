using Alexandria.ItemAPI;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.active_items
{
    internal class AscendGunRitual : PlayerItem
    {
        public static string ItemName = "Ascend Gun Ritual";

        private static float DamageStat = 1.15f;
        private static float RateOfFireStat = 1.15f;
        private static float ReloadStat = 0.75f;
        private static float Accuracy = 0.75f;
        private static float ProjectileSpeedStat = 1.5f;
        private static float ClipAndAmmoCapacityStat = 1.2f;
        private static float ChargeAmountStat = 1.5f;
        private static float RangeMultiplier = 1.2f;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/ascend_gun_ritual_alt_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<AscendGunRitual>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Bestow Divine Power";
            string longDesc = "Upgrades your current weapon with various buffs\n\n" +
                "Ascends a weapon to the heavens, granting it divine strength and knowledge. No matter how much you pry, the weapon will never tell you what knowledge it received.\n" +
                "\nInitial item use starts the ritual. Press the item use button again while holding the weapon you wish to select. " +
                "If you try to activate the ritual on a starter weapon, it will cancel the ritual without being consumed.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            //Plugin.Log($"{m_pickedUpThisRun}");
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                //Plugin.Log($"initial pickup");
                AkSoundEngine.PostEvent("ritual_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayRitualActivationEffectOnActor(player, true, false, false);
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.GunChanged += ReattachSacrificeVFX;
            //player.OnDidUnstealthyAction += ResetRitual;
            player.OnNewFloorLoaded += ResetRitual;
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            player.GunChanged -= ReattachSacrificeVFX;
            //player.OnDidUnstealthyAction -= ResetRitual;
            player.OnNewFloorLoaded -= ResetRitual;
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }
            return base.Drop(player);
        }

        public override void DoEffect(PlayerController player)
        {
            base.DoEffect(player);

            IsCurrentlyActive = true;

            Plugin.Log($"initial activation");
            AkSoundEngine.PostEvent("sacrifice_start", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop", player.gameObject);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            activeVFXObject = VFXPlayerCOTL.PlaySacrificeEventEffectOnGun(player.CurrentGun);
        }

        private void ReattachSacrificeVFX(Gun previous, Gun current, bool newGun)
        {
            //activeVFXObject.transform.SetParent(current.transform, false);

            if (IsCurrentlyActive) activeVFXObject.GetComponent<VFXAnchorOnGunModule>().gun = current;

            //Plugin.Log($"gun name: {current.EncounterNameOrDisplayName}, , {current.gameObject}, , {current.sprite}, {current.transform}");
        }

        public override void DoActiveEffect(PlayerController player)
        {
            if (player == null || player.CurrentGun == null)
            {
                return;
            }

            if (player.CurrentGun.quality == ItemQuality.SPECIAL)
            {
                ResetRitual(player);
                return;
            }

            base.DoActiveEffect(player);

            /*
             * private static float DamageStat = 1.15f;
             * private static float RateOfFireStat = 1.15f;
             * private static float ReloadStat = 0.75f;
             * private static float Accuracy = 0.75f;
             * private static float ProjectileSpeedStat = 1.5f;
             * private static float ClipAndAmmoCapacityStat = 1.2f;
             * private static float ChargeAmountStat = 1.5f;
             * private static float RangeMultiplier = 1.2f;
            */

            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.Damage, DamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.RateOfFire, RateOfFireStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.ReloadSpeed, ReloadStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.Accuracy, Accuracy, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.ProjectileSpeed, ProjectileSpeedStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.AdditionalClipCapacityMultiplier, ClipAndAmmoCapacityStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.AmmoCapacityMultiplier, ClipAndAmmoCapacityStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.ChargeAmountMultiplier, ChargeAmountStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddCurrentGunStatModifier(player.CurrentGun, PlayerStats.StatType.RangeMultiplier, RangeMultiplier, StatModifier.ModifyMethod.MULTIPLICATIVE);

            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            //Plugin.Log($"final activation: {player.CurrentGun}");

            AkSoundEngine.PostEvent("ascend_start", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop" + "_stop", player.gameObject);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            IsCurrentlyActive = false;

            player.RemoveActiveItem(ID);
        }

        private void ResetRitual(PlayerController player)
        {
            AkSoundEngine.PostEvent("sacrifice_loop" + "_stop", player.gameObject);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }
            IsCurrentlyActive = false;
        }
    }
}
