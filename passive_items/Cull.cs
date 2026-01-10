using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.passive_items
{
    internal class Cull : PassiveItem
    {
        private static int ReapKillThreshold = 5;
        private static int ReapCountMax = 200;
        private static int ReapCountMaxMoney = 15;
        private bool ReapCountMaxReached = false;

        private int ReapCount = 0;

        public static int ID;

        public static void Init()
        {
            string itemName = "Cull";
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

        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemy)
        {
            if (enemy.aiActor != null && enemy && fatal)
            {
                ReapCount++;
                //Plugin.Log($"reapcount: {ReapCount}");

                //spawns gold every 4 (reapkillthreshold) kills
                if (ReapCount % ReapKillThreshold == 0)
                {
                    LootEngine.SpawnCurrency(enemy.specRigidbody.UnitCenter, 1, false);
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
