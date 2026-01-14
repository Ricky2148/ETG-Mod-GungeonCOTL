using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.passive_items
{
    internal class Cull : PassiveItem
    {
        public static string ItemName = "Cull";

        private static int ReapKillThreshold = 5;
        private static int ReapCountMax = 200;
        private int ReapMoney = 1;
        private static int ReapCountMaxMoney = 30;
        private bool ReapCountMaxReached = false;

        private int ReapCount = 0;

        public bool WEAKEARLYGAMEActivated = false;
        public bool BAUSENLAWActivated = false;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "LOLItems/Resources/passive_item_sprites/cull_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Cull>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "\"full clearing\"";
            string longDesc = "Drops an extra casing every few kills. After enough kills, drops a one time lump sum of casings.\n\n" +
                "A simple worn-out scythe used by many to farm crops. Farming enemies seems more efficient now.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.D;

            item.UsesCustomCost = false;
            item.CustomCost = 20;

            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            /*if (!ReapCountMaxReached)
            {
                player.OnAnyEnemyReceivedDamage += KillEnemyCount;
            }*/
            player.OnAnyEnemyReceivedDamage += KillEnemyCount;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.OnAnyEnemyReceivedDamage -= KillEnemyCount;
            }
        }

        public override void Update()
        {
            if (Owner != null)
            {
                if (Owner.HasSynergy(Synergy.WEAK_EARLY_GAME) && !WEAKEARLYGAMEActivated)
                {
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    WEAKEARLYGAMEActivated = true;
                }
                else if (!Owner.HasSynergy(Synergy.WEAK_EARLY_GAME) && WEAKEARLYGAMEActivated)
                {
                    ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    WEAKEARLYGAMEActivated = false;
                }
                if (Owner.HasSynergy(Synergy.BAUSEN_LAW) && !BAUSENLAWActivated)
                {
                    ReapMoney = 3;

                    BAUSENLAWActivated = true;
                }
                else if (!Owner.HasSynergy(Synergy.BAUSEN_LAW) && BAUSENLAWActivated)
                {
                    ReapMoney = 1;

                    BAUSENLAWActivated = false;
                }
            }

            base.Update();
        }

        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemy)
        {
            if (enemy.aiActor != null && enemy && fatal)
            {
                ReapCount++;
                //Plugin.Log($"reapcount: {ReapCount}");

                //spawns gold every (reapkillthreshold) kills
                if (ReapCount % ReapKillThreshold == 0)
                {
                    LootEngine.SpawnCurrency(enemy.specRigidbody.UnitCenter, ReapMoney, false);
                }

                if (ReapCount >= ReapCountMax && !ReapCountMaxReached)
                {
                    LootEngine.SpawnCurrency(enemy.specRigidbody.UnitCenter, ReapCountMaxMoney, false);
                    //Owner.OnAnyEnemyReceivedDamage -= KillEnemyCount;
                    ReapCountMaxReached = true;
                }
            }
        }
    }
}
