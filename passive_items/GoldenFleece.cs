using Alexandria.ItemAPI;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

//finalize vfx and colors, also pick a better sfx

namespace GungeonCOTL.passive_items
{
    internal class GoldenFleece : PassiveItem
    {
        public static string ItemName = "Golden Fleece";

        private int StackCount = 0;
        private float DamageIncreasePerStack = 0.02f;

        public float damageBoostPerKill = 0.05f;

        //public float damageBoostPerKillSynergy = 0.04f;

        public float damageMultiplierCap = 3f;

        //public float synergyMultiplierCap = 5f;

        public tk2dSprite eighthNoteSprite;

        public tk2dSprite doubleEighthNoteSprite;

        public Gradient colorGradient;

        public Gradient synergyColorGradient;

        [NonSerialized]
        private int m_sequentialKills;

        [NonSerialized]
        private PlayerController m_player;

        private static MetronomeItem baseMetronome = PickupObjectDatabase.GetById((int)Items.Metronome) as MetronomeItem;

        public static bool isFleece = true;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/golden_fleece_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<GoldenFleece>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Embrace Greed!";
            string longDesc = "Increased damage per kill, stacking until the cap. Taking damage hurts more and resets the buff.\n\n" +
                "The Fleece of a revered Golden Lamb. Despite its \"holiness,\" this Golden Fleece was procured from The Lamb's own follower. " +
                "Those born with a golden fleece are always treated with favor and praise; that is, until the cult murders them for their precious skin.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            GameObject smallStarSprite = SpriteBuilder.SpriteFromResource("GungeonCOTL/Resources/vfxs/golden_fleece_icons/star_small_upside_down");
            UnityEngine.Object.DontDestroyOnLoad(smallStarSprite);
            FakePrefab.MarkAsFakePrefab(smallStarSprite);
            smallStarSprite.SetActive(false);
            item.eighthNoteSprite = smallStarSprite.GetComponent<tk2dSprite>();

            //Plugin.Log($"gameobject is null? {smallStarSprite == null}, tk2dsprite is null? {smallStarSprite.GetComponent<tk2dSprite>() == null}, bravebehavior.sprite is null? {smallStarSprite.GetComponent<BraveBehaviour>().sprite == null}, " +
            //$"tk2dsprite.Collection: {smallStarSprite.GetComponent<tk2dSprite>().collection}, tk2dsprite.spriteId: {smallStarSprite.GetComponent<tk2dSprite>().spriteId}");

            GameObject largeStarSprite = SpriteBuilder.SpriteFromResource("GungeonCOTL/Resources/vfxs/golden_fleece_icons/star_large_sparkly");
            UnityEngine.Object.DontDestroyOnLoad(largeStarSprite);
            FakePrefab.MarkAsFakePrefab(largeStarSprite);
            largeStarSprite.SetActive(false);
            item.doubleEighthNoteSprite = largeStarSprite.GetComponent<tk2dSprite>();

            item.colorGradient = baseMetronome.colorGradient;
            item.synergyColorGradient = baseMetronome.synergyColorGradient;

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId;
        }

        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);

            data.Add(m_sequentialKills);
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            if (!m_player || m_player.inventory == null || data.Count != 2)
            {
                return;
            }
            m_sequentialKills = (int)data[1];
            int num = (int)data[0];
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }

                AkSoundEngine.PostEvent("crown_upgrade_pickup", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayCrownUpgradeEffectOnActor(player);
                player.StartCoroutine(VFXPlayerCOTL.HardCodedCrownUpgradeEffectSFXPlayer(player));
            }

            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
            
            m_player = player;
            player.OnKilledEnemy += OnKilledEnemy;
            player.healthHaver.OnDamaged += OnReceivedDamage;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnKilledEnemy -= OnKilledEnemy;
            player.healthHaver.OnDamaged -= OnReceivedDamage;
            debrisObject.GetComponent<MetronomeItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            if (player != null)
            {
                player.OnKilledEnemy -= OnKilledEnemy;
                player.healthHaver.OnDamaged -= OnReceivedDamage;
            }
        }

        private void DoMetronomeUp()
        {
            m_sequentialKills++;
            ItemBuilder.RemovePassiveStatModifier(this, PlayerStats.StatType.Damage);
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Damage, GetCurrentMultiplier(), StatModifier.ModifyMethod.MULTIPLICATIVE);
            m_player.stats.RecalculateStatsWithoutRebuildingGunVolleys(m_player);

            AkSoundEngine.SetRTPCValue("Pitch_Metronome", m_sequentialKills);
            AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", m_player.gameObject);
            float currentMultiplier = GetCurrentMultiplier();
            float time = Mathf.InverseLerp(1f, damageMultiplierCap, currentMultiplier);
            Color tintColor = colorGradient.Evaluate(time);
            if (currentMultiplier >= 2f)
            {
                m_player.BloopItemAboveHead(doubleEighthNoteSprite, string.Empty, tintColor);
            }
            else
            {
                m_player.BloopItemAboveHead(eighthNoteSprite, string.Empty, tintColor);
            }
        }

        private void DoMetronomeBroken(Gun current)
        {
            if (m_player.healthHaver.IsAlive)
            {
                //m_player.healthHaver.ApplyDamage(0.5f, Vector2.zero, ItemName);
                m_player.healthHaver.ForceSetCurrentHealth(m_player.healthHaver.currentHealth - 0.5f);
            }

            float currentMultiplier = GetCurrentMultiplier();
            if (currentMultiplier > 1f)
            {
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", m_player.gameObject);
                float time = Mathf.InverseLerp(1f, damageMultiplierCap, currentMultiplier);
                Color color = colorGradient.Evaluate(time);
                GameObject gameObject = m_player.PlayEffectOnActor((!(currentMultiplier >= 2f)) ? eighthNoteSprite.gameObject : doubleEighthNoteSprite.gameObject, Vector3.up * 1.5f);
                gameObject.GetComponent<tk2dBaseSprite>().color = color;
            }
            AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0f);
            m_sequentialKills = 0;
            m_player.stats.RecalculateStatsWithoutRebuildingGunVolleys(m_player);
        }

        public float GetCurrentMultiplier()
        {
            return Mathf.Clamp(1f + (float)m_sequentialKills * damageBoostPerKill, 0f, damageMultiplierCap);
        }

        private void OnReceivedDamage(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            DoMetronomeBroken(Owner.CurrentGun);
        }

        private void OnKilledEnemy(PlayerController source)
        {
            DoMetronomeUp();
        }
    }
}
