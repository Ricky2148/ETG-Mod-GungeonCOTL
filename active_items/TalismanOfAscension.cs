using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using HutongGames.Utility;
using LOLItems.custom_class_data;
using LOLItems.guon_stones;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace LOLItems.active_items
{
    internal class TalismanOfAscension : PlayerItem
    {
        public static int ID;
        public float duration = 5f;

        public List<PlayerStats.StatType> StatTypeList = new List<PlayerStats.StatType>(
            (PlayerStats.StatType[])Enum.GetValues(typeof(PlayerStats.StatType))
            );

        public Dictionary<PlayerStats.StatType, float> StatTypeWeights = new Dictionary<PlayerStats.StatType, float>();

        //public System.Array StatTypeList = Enum.GetValues(typeof(PlayerStats.StatType));

        public static void Init()
        {
            string itemName = "Talisman of Ascension";
            string resourceName = "LOLItems/Resources/active_item_sprites/talisman_of_ascension_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<TalismanOfAscension>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            //ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.PerRoom, 5);
            item.consumable = false;

            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.StatTypeList.Remove(PlayerStats.StatType.Health);
            item.StatTypeList.Remove(PlayerStats.StatType.Coolness);
            item.StatTypeList.Remove(PlayerStats.StatType.AdditionalGunCapacity);
            item.StatTypeList.Remove(PlayerStats.StatType.AdditionalItemCapacity);
            item.StatTypeList.Remove(PlayerStats.StatType.GlobalPriceMultiplier);
            item.StatTypeList.Remove(PlayerStats.StatType.Curse);
            item.StatTypeList.Remove(PlayerStats.StatType.AdditionalBlanksPerFloor);
            item.StatTypeList.Remove(PlayerStats.StatType.ShadowBulletChance);
            item.StatTypeList.Remove(PlayerStats.StatType.ThrownGunDamage);
            item.StatTypeList.Remove(PlayerStats.StatType.DodgeRollDamage);
            item.StatTypeList.Remove(PlayerStats.StatType.EnemyProjectileSpeedMultiplier);
            item.StatTypeList.Remove(PlayerStats.StatType.ExtremeShadowBulletChance);
            item.StatTypeList.Remove(PlayerStats.StatType.RangeMultiplier);
            item.StatTypeList.Remove(PlayerStats.StatType.DodgeRollDistanceMultiplier);
            item.StatTypeList.Remove(PlayerStats.StatType.DodgeRollSpeedMultiplier);
            item.StatTypeList.Remove(PlayerStats.StatType.TarnisherClipCapacityMultiplier);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
            foreach (PlayerStats.StatType a in StatTypeList)
            {
                Plugin.Log($"{a}");
            }

            /*
             * 1. MovementSpeed
             * 2. RateOfFire
             * 3. Accuracy
             * 4. Damage
             * 5. ProjectileSpeed
             * 6. AmmoCapacityMultiplier
             * 7. ReloadSpeed
             * 8. AdditionalShotPiercing
             * 9. KnockbackMultiplier
             * 10. PlayerBulletScale
             * 11. AdditionalClipCapacityMultiplier
             * 12. AdditionalShotBounces
             * 13. DamageToBosses
             * 14. ChargeAmountMultiplier
             * 15. MoneyMultiplierFromEnemies
             */

            //assuming placeholder stat increase value spread of 1.5 - 5.0
            //formula: ([stat increase value] * [stat increase weight]) + (1.0f [to ensure a baseline stat increase and no decreases])
            StatTypeWeights.Clear();
            StatTypeWeights.Add(StatTypeList[0], 1.0f);  //MovementSpeed (special cased to be additive NOT multiplicative)
            StatTypeWeights.Add(StatTypeList[1], 0.3f);  //RateOfFire
            StatTypeWeights.Add(StatTypeList[2], 0.2f);  //Accuracy
            StatTypeWeights.Add(StatTypeList[3], 0.3f);  //Damage
            StatTypeWeights.Add(StatTypeList[4], 0.4f);  //ProjectileSpeed
            StatTypeWeights.Add(StatTypeList[5], 0.6f);  //AmmoCapacityMultiplier
            StatTypeWeights.Add(StatTypeList[6], 1.0f);  //ReloadSpeed (special cased to be inverse?)
            StatTypeWeights.Add(StatTypeList[7], 1.0f);  //AdditionalShotPiercing (special cased to be additive NOT multiplicative)
            StatTypeWeights.Add(StatTypeList[8], 0.5f);  //KnockbackMultiplier
            StatTypeWeights.Add(StatTypeList[9], 0.4f); //PlayerBulletScale
            StatTypeWeights.Add(StatTypeList[10], 0.4f); //AdditionalClipCapacityMultiplier
            StatTypeWeights.Add(StatTypeList[11], 1.0f); //AddtionalShotBounces (special cased to be additive NOT multiplicative)
            StatTypeWeights.Add(StatTypeList[12], 0.6f); //DamageToBosses
            StatTypeWeights.Add(StatTypeList[13], 1.0f); //ChargeAmountMultiplier
            StatTypeWeights.Add(StatTypeList[14], 0.2f); //MoneyMultiplierFromEnemies

            foreach (KeyValuePair<PlayerStats.StatType, float> b in StatTypeWeights)
            {
                Plugin.Log($"{b.Key} | {b.Value}");
            }
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            return base.Drop(player);
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.gameObject);

            //int numOfBuffs = (int)Math.Ceiling(UnityEngine.Random.value * 5);
            int numOfBuffs = StatTypeList.Count;

            //List<PlayerStats.StatType> listOfBuffs = new List<PlayerStats.StatType>();

            List<int> list1 = new List<int>();

            for (int a = 0; a < numOfBuffs; a++)
            {
                //Enum.GetValues(typeof(PlayerStats.StatType))

                //list1.Add(UnityEngine.Random.Range(0, StatTypeList.Count));
                list1.Add(a);
                Plugin.Log($"{list1[a]}");
            }

            int i = 0,j = 0;
            foreach (PlayerStats.StatType statType in StatTypeList)
            {
                ItemBuilder.RemovePassiveStatModifier(this, statType);

                Plugin.Log($"i: {i}, j: {j}");
                
                /*if (list1[j].Equals(i))
                {
                    //ItemBuilder.AddPassiveStatModifier(this, statType, 10, StatModifier.ModifyMethod.MULTIPLICATIVE);

                    Plugin.Log($"{statType}");

                    j++;
                }*/

                i++;
            }

            user.stats.RecalculateStatsWithoutRebuildingGunVolleys(user);

            foreach (int a in list1)
            {
                float statIncValue = 1f;
                Plugin.Log($"statToMod: {StatTypeList[a]}, statIncValue: {statIncValue}, statWeightValue: {StatTypeWeights[StatTypeList[a]]}");
                statIncValue = 1.0f + (statIncValue * StatTypeWeights[StatTypeList[a]]);
                ItemBuilder.AddPassiveStatModifier(this, StatTypeList[a], statIncValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

                //Plugin.Log($"{StatTypeList[a]}");
            }

            user.stats.RecalculateStatsWithoutRebuildingGunVolleys(user);
        }
    }
}
