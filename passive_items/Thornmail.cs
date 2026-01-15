using Alexandria;
using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//health, armor, every time the player is hit, deal set dmg to all enemies in a radius around the player, scales with max health of player

namespace LOLItems
{
    internal class Thornmail : OnDamagedPassiveItem
    {
        public static string ItemName = "Thornmail";

        // stats pool for item
        private static float HealthStat = 1f;
        private static int ArmorStat = 1;

        private static float ThornsDamage = 30f;
        private static float ThornsRadius = 10f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "LOLItems/Resources/passive_item_sprites/thornmail_pixelart_sprite_outline";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Thornmail>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "*social distancing*";
            string longDesc = "+1 Heart and Armor\nTaking damage activates a blank and deals damage to nearby enemies.\n\n" +
                "Armor with spikes. Spikes hurt. Don't touch wearer, spikes will hurt. " +
                "Spikes on armor. Armor under spikes. Armor hurts. Because spikes hurt. Don't touch armor. " +
                "Armor hurts.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            item.ArmorToGainOnInitialPickup = ArmorStat;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.healthHaver.OnDamaged += OnPlayerDamaged;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.healthHaver.OnDamaged -= OnPlayerDamaged;
            }
        }

        // checks for seperate radius around player to deal damage to enemies
        private void DoBlankDamage(PlayerController player)
        {
            if (player.CurrentRoom == null) return;

            List<AIActor> enemyList = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemyList != null)
            {
                foreach (AIActor enemy in enemyList)
                {
                    if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
                    {
                        float dist = Vector2.Distance(player.CenterPosition, enemy.CenterPosition);
                        if (dist <= ThornsRadius)
                        {
                            enemy.healthHaver.ApplyDamage(
                                ThornsDamage,
                                Vector2.zero,
                                "thornmail_blank_damage",
                                CoreDamageTypes.None,
                                DamageCategory.Normal,
                                false
                            );
                        }
                    }
                }
            }
        }

        // when damaged, do blank effect, call DoBlankDamage
        private void OnPlayerDamaged (float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            PlayerController player = this.Owner;
            player.ForceBlank();

            DoBlankDamage(player);
        }
    }
}
