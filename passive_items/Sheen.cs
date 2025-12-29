using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria;

namespace LOLItems.passive_items
{
    internal class Sheen : PassiveItem
    {
        private bool shouldApplySpellblade = false;
        private static float spellbladeDmg = 10f;

        public static int ID;

        public static void Init()
        {
            string itemName = "Sheen";
            string resourceName = "LOLItems/Resources/passive_item_sprites/sheen_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Sheen>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "who makes a sword out of ice?";
            string longDesc = "A sword made out of ice... It's been magically enchanted to mend itself when shattered, but since it's made of ice, it always shatters...\n";

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

            player.OnReloadedGun += OnGunReloaded;
            player.PostProcessProjectile += OnPostProcessProjectile;
            shouldApplySpellblade = true;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            player.OnReloadedGun -= OnGunReloaded;
            player.PostProcessProjectile -= OnPostProcessProjectile;
            shouldApplySpellblade = false;
        }

        private void OnGunReloaded(PlayerController player, Gun gun)
        {
            shouldApplySpellblade = true;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            //Plugin.Log($"spellblade proc: {shouldApplySpellblade}");
            if (proj.Shooter == proj.Owner.specRigidbody && shouldApplySpellblade)
            {
                proj.OnHitEnemy += (projHit, enemy, fatal) =>
                {
                    if (enemy == null) return;
                    if (enemy.aiActor == null && enemy.GetComponentInParent<AIActor>() == null) return;
                    if (enemy.healthHaver != null)
                    {
                        enemy.healthHaver.ApplyDamage(
                            spellbladeDmg,
                            Vector2.zero,
                            "sheen_spellblade_damage",
                            CoreDamageTypes.None,
                            DamageCategory.Normal,
                            false
                        );
                    }
                };

                shouldApplySpellblade = false;
            }
        }
    }
}
