using Alexandria.ItemAPI;
using LOLItems.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// might use a stack based system like puppeteer

namespace LOLItems.passive_items
{
    internal class SilverBolts : PassiveItem
    {
        private int silverBoltsCount = 0;
        private static float silverBoltsPercentHealthDamage = 0.15f;
        private static float silverBoltsBaseDamage = 5f;
        private static List<string> sfxList = new List<string>
        {
            "kraken_slayer_passive_SFX_1",
            "kraken_slayer_passive_SFX_2",
            "kraken_slayer_passive_SFX_3"
        };

        public static int ID;

        public static void Init()
        {
            string itemName = "Silver Bolts";
            string resourceName = "LOLItems/Resources/passive_item_sprites/silver_bolts_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SilverBolts>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            item.quality = PickupObject.ItemQuality.C;

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

        private void OnPostProcessProjectile(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickrate)
        {
            float randomVal = UnityEngine.Random.value;
            if (randomVal <= 0.04f)
            {
                silverBoltsCount++;
                //Plugin.Log($"randomVal: {randomVal}, silverboltscount: {silverBoltsCount}");
            }
            if (silverBoltsCount >= 3)
            {
                /*if (beam.sprite != null)
                {
                    beam.sprite.color = Color.Lerp(beam.sprite.color, Color.white, 0.8f);
                }*/
                //HelpfulMethods.PlayRandomSFX(beam.gameObject, sfxList);
                if (hitRigidbody == null) return;
                AIActor firstEnemy = null;
                if (hitRigidbody.aiActor != null)
                {
                    firstEnemy = hitRigidbody.aiActor;
                }
                else if (hitRigidbody.GetComponentInParent<AIActor>() != null)
                {
                    firstEnemy = hitRigidbody.GetComponentInParent<AIActor>();
                }
                else
                {
                    return;
                }
                if (hitRigidbody.healthHaver != null)
                {
                    // scale damage down by tickrate
                    float damageToDeal = (firstEnemy.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                    // damage is 1/4 against bosses and sub-bosses
                    if (firstEnemy.healthHaver.IsBoss || firstEnemy.healthHaver.IsSubboss)
                    {
                        damageToDeal *= 0.25f;
                    }
                    // calculates additional extra damage to apply to enemy
                    firstEnemy.healthHaver.ApplyDamage(
                        damageToDeal,
                        Vector2.zero,
                        "silver_bolts_damage",
                        CoreDamageTypes.None,
                        DamageCategory.Normal,
                        false
                    );
                    Plugin.Log($"damage dealt: {damageToDeal}");
                }
                silverBoltsCount = 0;
            }
        }
        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Shooter == proj.Owner.specRigidbody)
            {
                silverBoltsCount++;
                if (silverBoltsCount >= 3)
                {
                    if (proj.sprite != null)
                    {
                        // doesnt seem to work for some strange reason
                        //proj.sprite.color = Color.Lerp(proj.sprite.color, Color.white, 0.7f);
                        proj.sprite.color = ExtendedColours.silver;
                    }
                    HelpfulMethods.PlayRandomSFX(proj.gameObject, sfxList);
                    proj.OnHitEnemy += (projHit, enemy, fatal) =>
                    {
                        if (enemy == null) return;
                        if (enemy.aiActor == null && enemy.GetComponentInParent<AIActor>() == null) return;
                        if (enemy.healthHaver != null)
                        {
                            float damageToDeal = (enemy.healthHaver.GetMaxHealth() * silverBoltsPercentHealthDamage) + silverBoltsBaseDamage;
                            // damage is 1/4 against bosses and sub-bosses
                            if (enemy.healthHaver.IsBoss || enemy.healthHaver.IsSubboss)
                            {
                                damageToDeal *= 0.25f;
                            }

                            // calculates additional extra damage to apply to enemy
                            enemy.healthHaver.ApplyDamage(
                                damageToDeal,
                                Vector2.zero,
                                "silver_bolts_damage",
                                CoreDamageTypes.None,
                                DamageCategory.Normal,
                                false
                            );
                            Plugin.Log($"damage dealt: {damageToDeal}");
                        }
                    };
                    silverBoltsCount = 0;
                }
            }
        }
    }
}
