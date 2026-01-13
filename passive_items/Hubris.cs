using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria;
using Alexandria.Misc;

//dmg increase, dmg increase increases per kill, scaling infinitely, but very slowly

namespace LOLItems
{
    internal class Hubris : PassiveItem
    {
        public static string ItemName = "Hubris";

        // stats pool for item
        private int eminenceCount = 0;
        private float eminenceDamageIncrease = 0.005f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "LOLItems/Resources/passive_item_sprites/hubris_pixelart_sprite_small";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Hubris>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "A symbol of victory";
            string longDesc = "Every kill permanently increases your damage a little bit.\n\n" +
                "A congratulatory laurel wreath gifted to the victor. With each triumph, one's strength increases. " +
                "Legends speak of a statue that manifests once you reach the pinnacle of victory.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            //player.OnKilledEnemy += KillEnemyCount;
            player.OnAnyEnemyReceivedDamage += KillEnemyCount;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                //player.OnKilledEnemy -= KillEnemyCount;
                player.OnAnyEnemyReceivedDamage -= KillEnemyCount;
            }
        }

        // removes current damage modifier, increments damage increase count, and adds new damage modifier
        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemyHealth)
        {
            /*
            ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
            eminenceCount++;
            float damageIncrease = eminenceCount * eminenceDamageIncrease;
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, 1.0f + damageIncrease, StatModifier.ModifyMethod.MULTIPLICATIVE);
            player.stats.RecalculateStats(player, false, false);
            */

            //float damageIncrease;

            if (enemyHealth.aiActor != null && enemyHealth && fatal)
            {
                if (enemyHealth.aiActor.IsNormalEnemy && (enemyHealth.IsBoss || enemyHealth.IsSubboss))
                {
                    eminenceCount += 5;
                    //Plugin.Log($"is boss: {enemyHealth.IsBoss}, is sub boss: {enemyHealth.IsSubboss}");
                }
                else
                {
                    eminenceCount++;
                    //Plugin.Log($"is normal enemy");
                }

                //Plugin.Log($"is normal enemy: {enemyHealth.aiActor.IsNormalEnemy}");

                ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
                float damageIncrease = eminenceCount * eminenceDamageIncrease;
                ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, 1.0f + damageIncrease, StatModifier.ModifyMethod.MULTIPLICATIVE);
                //Owner.stats.RecalculateStats(Owner, false, false);
                Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);
            }
        }
    }
}
