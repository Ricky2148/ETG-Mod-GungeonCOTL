using Alexandria.ItemAPI;
using Alexandria.Misc;
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

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/ascend_gun_ritual_alt_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<AscendGunRitual>();

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
            //Plugin.Log($"{m_pickedUpThisRun}");
            if (!m_pickedUpThisRun)
            {
                //Plugin.Log($"initial pickup");
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

            IsCurrentlyActive = true;

            Plugin.Log($"initial activation");
            AkSoundEngine.PostEvent("sacrifice_start", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop", player.gameObject);
        }

        public override void DoActiveEffect(PlayerController player)
        {
            if (player == null || player.CurrentGun == null)
            {
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

            Plugin.Log($"final activation: {player.CurrentGun}");

            AkSoundEngine.PostEvent("ascend_start", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop" + "_stop", player.gameObject);

            IsCurrentlyActive = false;

            player.RemoveActiveItem(ID);
        }
    }
}
