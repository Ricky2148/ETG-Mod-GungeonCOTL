using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using LOLItems.custom_class_data;

// look into making the aura tint not apply to corpse

namespace LOLItems.passive_items
{
    internal class ZekesConvergence : AuraItem
    {
        // stats pool for item
        private static float HealthStat = 1f;
        //private static int ArmorStat = 1;

        private static float FrostFireTempestDamage = 15f;
        private static float FrostFireTempestRadius = 6f;
        private bool isFrostFireTempestActive = false;

        private static float FrostFireTempestDuration = 10f;
        private static float FrostFireTempestCooldown = 45f;
        private static float FrostFireTempestSlowPercent = 0.5f;

        private static GameObject EffectVFX = ((Gun)PickupObjectDatabase.GetById(596))
            .DefaultModule.projectiles[0]
            .hitEffects.tileMapHorizontal.effects[0]
            .effects[0].effect;

        private static GameActorSpeedEffect FrostFireTempestSlowEffect = new GameActorSpeedEffect
        {
            duration = 1f,
            SpeedMultiplier = FrostFireTempestSlowPercent,
            effectIdentifier = "frostfire_tempest_slow_effect",
            resistanceType = EffectResistanceType.Freeze,
            AppliesTint = true,
            TintColor = ExtendedColours.skyblue
        };

        public static int ID;

        public static void Init()
        {
            string itemName = "Zeke's Convergence";
            string resourceName = "LOLItems/Resources/passive_item_sprites/zekes_convergence_pixelart_sprite_outline";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<ZekesConvergence>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);
            //item.ArmorToGainOnInitialPickup = ArmorStat;

            // sets damage aura stats
            item.AuraRadius = 0f;
            item.DamagePerSecond = 0f;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        // subscribe to the player events
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.OnUsedPlayerItem += ActivateFrostFireTempest;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            player.OnUsedPlayerItem -= ActivateFrostFireTempest;
        }

        // updates the immolate stats based on the player's current health
        private void ActivateFrostFireTempest(PlayerController player, PlayerItem item)
        {
            if (!isFrostFireTempestActive) StartCoroutine(DoFrostFireTempestEffect(player));
        }

        private System.Collections.IEnumerator DoFrostFireTempestEffect(PlayerController player)
        {
            isFrostFireTempestActive = true;

            this.AuraRadius = FrostFireTempestRadius;
            this.DamagePerSecond = FrostFireTempestDamage;

            /*if (EffectVFX != null && player != null)
            {
                this.AuraVFX = EffectVFX;
            }*/

            yield return new WaitForSeconds(FrostFireTempestDuration);

            this.AuraRadius = 0f;
            this.DamagePerSecond = 0f;

            yield return new WaitForSeconds(FrostFireTempestCooldown - FrostFireTempestDuration);

            isFrostFireTempestActive = false;
        }

        public override void DoAura()
        {
            if (m_extantAuraVFX == null)
            {
            }
            didDamageEnemies = false;
            if (AuraAction == null)
            {
                AuraAction = delegate (AIActor actor, float dist)
                {
                    float num = ModifiedDamagePerSecond * BraveTime.DeltaTime;
                    if (DamageFallsOffInRadius)
                    {
                        float t = dist / ModifiedAuraRadius;
                        num = Mathf.Lerp(num, 0f, t);
                    }
                    if (num > 0f)
                    {
                        didDamageEnemies = true;
                    }
                    actor.healthHaver.ApplyDamage(num, Vector2.zero, "Aura", damageTypes);
                    actor.ApplyEffect(FrostFireTempestSlowEffect);
                };
            }
            if (m_owner != null && m_owner.CurrentRoom != null)
            {
                m_owner.CurrentRoom.ApplyActionToNearbyEnemies(m_owner.CenterPosition, ModifiedAuraRadius, AuraAction);
            }
            if (didDamageEnemies)
            {
                m_owner.DidUnstealthyAction();
            }
        }
    }
}
