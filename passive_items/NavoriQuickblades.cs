using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.passive_items
{
    internal class NavoriQuickblades : PassiveItem
    {
        public static string ItemName = "Navori Quickblades";

        private static float RateOfFireStat = 1.1f;
        private static float TranscendenceCooldownReductionRatio = 0.03f;

        public bool SPONSOREDBYNAVORIActivated = false;
        private static float SPONSOREDBYNAVORIDamageStat = 1.15f;
        public bool QUICKBLADESANDQUICKBULLETSActivated = false;
        private static float QUICKBLADESANDQUICKBULLETSRateOfFireStatInc = 0.15f;
        public bool LIGHTSLINGERActivated = false;
        private static float LIGHTSLINGERTranscendenceCooldownReductionRatioInc = 0.05f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "LOLItems/Resources/passive_item_sprites/navori_quickblades_pixelart_sprite_outline";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<NavoriQuickblades>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "\"random !^@$*#%& go!\"";
            // maybe add effect explanation?
            string longDesc = "Increase fire rate\nEvery bullet decreases your active item cooldowns by %remaining cooldown.\n\n" +
                "A set of knives that magically come back after they land. Somehow there's always " +
                "at least one in your hand. You wonder what would happen if you threw them all at once but you " +
                "never do.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, RateOfFireStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            // subscribe to reload and projectile events
            player.PostProcessProjectile += OnPostProcessProjectile;
            player.PostProcessBeamTick += OnPostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                // unsubscribe from events
                player.PostProcessProjectile -= OnPostProcessProjectile;
                player.PostProcessBeamTick -= OnPostProcessProjectile;
            }
        }

        public override void Update()
        {
            if (Owner != null)
            {
                if (Owner.HasSynergy(Synergy.SPONSORED_BY_NAVORI) && !SPONSOREDBYNAVORIActivated)
                {
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, SPONSOREDBYNAVORIDamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    SPONSOREDBYNAVORIActivated = true;
                }
                else if (!Owner.HasSynergy(Synergy.SPONSORED_BY_NAVORI) && SPONSOREDBYNAVORIActivated)
                {
                    ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    SPONSOREDBYNAVORIActivated = false;
                }

                if (Owner.HasSynergy(Synergy.QUICKBLADES_AND_QUICKBULLETS) && !QUICKBLADESANDQUICKBULLETSActivated)
                {
                    ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.RateOfFire);
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.RateOfFire, RateOfFireStat + QUICKBLADESANDQUICKBULLETSRateOfFireStatInc, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    QUICKBLADESANDQUICKBULLETSActivated = true;
                }
                else if (!Owner.HasSynergy(Synergy.QUICKBLADES_AND_QUICKBULLETS) && QUICKBLADESANDQUICKBULLETSActivated)
                {
                    ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.RateOfFire);
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.RateOfFire, RateOfFireStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    Owner.stats.RecalculateStatsWithoutRebuildingGunVolleys(Owner);

                    QUICKBLADESANDQUICKBULLETSActivated = false;
                }

                if (Owner.HasSynergy(Synergy.LIGHTSLINGER) && !LIGHTSLINGERActivated)
                {
                    TranscendenceCooldownReductionRatio += LIGHTSLINGERTranscendenceCooldownReductionRatioInc;

                    LIGHTSLINGERActivated = true;
                }
                else if (!Owner.HasSynergy(Synergy.LIGHTSLINGER) && LIGHTSLINGERActivated)
                {
                    TranscendenceCooldownReductionRatio = 0.03f;

                    LIGHTSLINGERActivated = false;
                }
            }

            base.Update();
        }

        private void OnPostProcessProjectile(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickrate)
        {
            if (hitRigidbody == null) return;
            if (hitRigidbody.aiActor == null && hitRigidbody.GetComponentInParent<AIActor>() == null) return;
            if (hitRigidbody.healthHaver != null)
            {
                PlayerController player = this.Owner;
                
                if (!player.CurrentItem.IsCurrentlyActive)
                {
                    // reduces cooldown based on damage cooldown values
                    if (player.CurrentItem.CurrentDamageCooldown > 0)
                    {
                        float currentCooldownValue = player.CurrentItem.CurrentDamageCooldown;
                        float reducedCooldownValue = currentCooldownValue * (1f - (TranscendenceCooldownReductionRatio * tickrate));
                        player.CurrentItem.CurrentDamageCooldown = reducedCooldownValue;
                    }
                    else if (player.CurrentItem.CurrentTimeCooldown > 0)
                    {
                        float currentCooldownValue = player.CurrentItem.CurrentTimeCooldown;
                        float reducedCooldownValue = currentCooldownValue * (1f - (TranscendenceCooldownReductionRatio * tickrate));
                        player.CurrentItem.CurrentTimeCooldown = reducedCooldownValue;
                    }
                }
            }
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Shooter == proj.Owner.specRigidbody)
            {
                proj.OnHitEnemy += (projHit, enemy, fatal) =>
                {
                    if (enemy == null) return;
                    if (enemy.aiActor == null && enemy.GetComponentInParent<AIActor>() == null) return;
                    if (enemy.healthHaver != null)
                    {
                        PlayerController player = this.Owner;
                        if (!player.CurrentItem.IsCurrentlyActive)
                        {
                            // reduces cooldown based on damage cooldown values
                            if (player.CurrentItem.CurrentDamageCooldown > 0)
                            {
                                float currentCooldownValue = player.CurrentItem.CurrentDamageCooldown;
                                float reducedCooldownValue = currentCooldownValue * (1f - TranscendenceCooldownReductionRatio);
                                player.CurrentItem.CurrentDamageCooldown = reducedCooldownValue;
                            }
                            else if (player.CurrentItem.CurrentTimeCooldown > 0)
                            {
                                float currentCooldownValue = player.CurrentItem.CurrentTimeCooldown;
                                float reducedCooldownValue = currentCooldownValue * (1f - TranscendenceCooldownReductionRatio);
                                player.CurrentItem.CurrentTimeCooldown = reducedCooldownValue;
                            }
                        }
                    }
                };
            }
        }
    }
}
