using Alexandria.ItemAPI;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//needs an activation sfx
// likely has bugs, bug test this

namespace GungeonCOTL.active_items
{
    internal class RiteOfWrath : PlayerItem
    {
        public static string ItemName = "Rite of Wrath";

        private static float WrathDamageStat = 2.5f;
        private static float WrathRateOfFireStat = 1.5f;
        private static float WrathCurseStat = 3f;

        private static float WrathDuration = 20 * 60f;

        private GameObject activeVFXObject;

        private Coroutine activeParticleEmitter;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/rite_of_wrath_nobrown_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RiteOfWrath>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("ritual_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayRitualActivationEffectOnActor(player, true, false, false);
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }
            return base.Drop(player);
        }

        public override void DoEffect(PlayerController player)
        {
            base.DoEffect(player);

            player.StartCoroutine(ApplyWrathBuff(player, WrathDuration));

            AkSoundEngine.PostEvent("wrath_activation", player.gameObject);

            player.RemoveActiveItem(ID);
        }

        private System.Collections.IEnumerator ApplyWrathBuff(PlayerController player, float duration)
        {
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, WrathDamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.RateOfFire, WrathRateOfFireStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            Color originalPlayerColor = player.sprite.color;
            player.sprite.color = ExtendedColours.maroon;

            Material mat = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(105f * 0.4f, 7f * 0.4f, 9f * 0.4f));
            }

            //HelpfulMethods.EmitFromAttachedPlayer(GlobalSparksDoer.EmitRegionStyle.RANDOM, 5f, WrathDuration, player, 1f, 1f, 0.15f, 3f, ExtendedColours.maroon, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);

            //activeParticleEmitter = player.StartCoroutine(HelpfulMethods.HandleEmitFromAttachedPlayer(GlobalSparksDoer.EmitRegionStyle.RANDOM, 5f, duration, player, 1f, 1f, 0.15f, 3f, ExtendedColours.maroon, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE));

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;

                if (activeParticleEmitter == null)
                {
                    //Plugin.Log($"elapsed: {elapsed}, started emitting for: {duration - elapsed}");

                    activeParticleEmitter = player.StartCoroutine(HelpfulMethods.HandleEmitFromAttachedPlayer(GlobalSparksDoer.EmitRegionStyle.RANDOM, 5f, duration - elapsed, player, 1f, 1f, 0.15f, 3f, ExtendedColours.maroon, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE));
                }

                yield return null;
            }

            //yield return new WaitForSeconds(WrathDuration);

            // removes all stat buffs for both base item and overdrive effect
            ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
            ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.RateOfFire);

            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Curse, WrathCurseStat);

            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            player.sprite.color = originalPlayerColor;

            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
            }
        }
    }
}
