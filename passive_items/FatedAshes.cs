using Alexandria.ItemAPI;
using LOLItems.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.passive_items
{
    internal class FatedAshes : PassiveItem
    {
        private static float InflameDamagePerSecond = 3f;
        private static float InflameDuration = 3f;

        private static Gun phoenix = PickupObjectDatabase.GetById((int)Items.Phoenix) as Gun;
        private static GameActorFireEffect InflameBurnEffect = new GameActorFireEffect
        {
            duration = InflameDuration,
            DamagePerSecondToEnemies = InflameDamagePerSecond,
            effectIdentifier = "inflame_burn",
            ignitesGoops = false,
            FlameVfx = phoenix.DefaultModule.projectiles[0].fireEffect.FlameVfx
        };

        public static int ID;

        public static void Init()
        {
            string itemName = "Fated Ashes";
            string resourceName = "LOLItems/Resources/passive_item_sprites/fated_ashes_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<FatedAshes>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.D;

            item.UsesCustomCost = true;
            item.CustomCost = 20;

            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.PostProcessProjectile += OnPostProcessProjectile;
            player.PostProcessBeamTick += OnPostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            player.PostProcessProjectile -= OnPostProcessProjectile;
            player.PostProcessBeamTick -= OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            proj.OnHitEnemy += (projHit, enemy, fatal) =>
            {
                if (enemy != null && enemy.aiActor != null)
                {
                    enemy.aiActor.ApplyEffect(InflameBurnEffect);
                }
            };
        }

        private void OnPostProcessProjectile(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickrate)
        {
            if (hitRigidbody != null && hitRigidbody.aiActor != null)
            {
                hitRigidbody.aiActor.ApplyEffect(InflameBurnEffect);
            }
        }
    }
}
