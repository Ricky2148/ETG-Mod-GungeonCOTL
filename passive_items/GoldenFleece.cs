using Alexandria.Integrations; 
using Alexandria.Integrations;
using Alexandria.ItemAPI; 
using Alexandria.Misc;
using Alexandria.VisualAPI;
using GungeonCOTL.custom_class_data;
using Newtonsoft.Json.Serialization;
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

        public tk2dSprite smallStarSprite;

        public tk2dSprite largeStarSprite;

        private static List<string> smallStarVFXSpritePath = "GungeonCOTL/Resources/vfxs/golden_fleece_icons/starbreak/small_star_break".GetResourceFrames(23);

        private static GameObject smallStarVFX;

        private static List<string> largeStarVFXSpritePath = "GungeonCOTL/Resources/vfxs/golden_fleece_icons/starbreak/large_star_break".GetResourceFrames(23);

        private static GameObject largeStarVFX;

        private static CoinBloop coinBlooper;

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

            item.AddItemTip("Increased damage per kill, stacking until the cap. Taking damage hurts more and resets the buff.");

            GameObject smallStarSprite = SpriteBuilder.SpriteFromResource("GungeonCOTL/Resources/vfxs/golden_fleece_icons/star_small_upside_down");
            UnityEngine.Object.DontDestroyOnLoad(smallStarSprite);
            FakePrefab.MarkAsFakePrefab(smallStarSprite);
            smallStarSprite.SetActive(false);
            item.smallStarSprite = smallStarSprite.GetComponent<tk2dSprite>();

            //Plugin.Log($"gameobject is null? {smallStarSprite == null}, tk2dsprite is null? {smallStarSprite.GetComponent<tk2dSprite>() == null}, bravebehavior.sprite is null? {smallStarSprite.GetComponent<BraveBehaviour>().sprite == null}, " +
            //$"tk2dsprite.Collection: {smallStarSprite.GetComponent<tk2dSprite>().collection}, tk2dsprite.spriteId: {smallStarSprite.GetComponent<tk2dSprite>().spriteId}");

            GameObject largeStarSprite = SpriteBuilder.SpriteFromResource("GungeonCOTL/Resources/vfxs/golden_fleece_icons/star_large_sparkly");
            UnityEngine.Object.DontDestroyOnLoad(largeStarSprite);
            FakePrefab.MarkAsFakePrefab(largeStarSprite);
            largeStarSprite.SetActive(false);
            item.largeStarSprite = largeStarSprite.GetComponent<tk2dSprite>();

            Gradient test = new Gradient();

            var colors = new GradientColorKey[3];
            colors[0] = new GradientColorKey(ExtendedColours.paleYellow, 0.0f);
            colors[1] = new GradientColorKey(ExtendedColours.carrionRed, 0.5f);
            colors[2] = new GradientColorKey(ExtendedColours.veryDarkRed, 1.0f);

            // Blend alpha from opaque at 0% to transparent at 100%
            var alphas = new GradientAlphaKey[3];
            alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphas[1] = new GradientAlphaKey(1.0f, 0.5f);
            alphas[2] = new GradientAlphaKey(1.0f, 1.0f);

            test.SetKeys(colors, alphas);

            //item.colorGradient = baseMetronome.colorGradient;
            item.colorGradient = test;

            item.synergyColorGradient = baseMetronome.synergyColorGradient;

            smallStarVFX = VFXBuilder.CreateVFX
            (
                "small_star_vfx",
                smallStarVFXSpritePath,
                15,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                false
            );

            VFXAnchorOnPlayerModule anchor1 = smallStarVFX.GetOrAddComponent<VFXAnchorOnPlayerModule>();

            largeStarVFX = VFXBuilder.CreateVFX
            (
                "large_star_vfx",
                largeStarVFXSpritePath,
                15,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                false
            );

            VFXAnchorOnPlayerModule anchor2 = largeStarVFX.GetOrAddComponent<VFXAnchorOnPlayerModule>();

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false; ID = item.PickupObjectId; item.IgnoredByRat = true;
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
            coinBlooper = player.m_blooper;
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
            //AkSoundEngine.PostEvent("punchy_blessed_choir3", m_player.gameObject);
            float currentMultiplier = GetCurrentMultiplier();
            float time = Mathf.InverseLerp(1f, damageMultiplierCap, currentMultiplier);
            Color tintColor = colorGradient.Evaluate(time);
            if (currentMultiplier >= 2f)
            {
                m_player.BloopItemAboveHead(largeStarSprite, string.Empty, tintColor);
            }
            else
            {
                m_player.BloopItemAboveHead(smallStarSprite, string.Empty, tintColor);
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

                //FIX ANIMATION BY ACTUALLY MAKING AN ANIMATION

                //GameObject gameObject = m_player.PlayEffectOnActor((!(currentMultiplier >= 2f)) ? smallStarSprite.gameObject : largeStarSprite.gameObject, Vector3.up * 1.5f);
                //Plugin.Log($"facing direction: {Owner.FacingDirection}");

                //GameObject gameObject = m_player.PlayEffectOnActor((!(currentMultiplier >= 2f)) ? smallStarVFX : largeStarVFX, (Vector3.up + new Vector3(0f, 0f, 0f) * 1.5f));
                //gameObject.GetComponent<tk2dBaseSprite>().color = color;

                //BloopIconAboveHead(color);
                PlayStarBreakVFXOnActor(m_player, color, currentMultiplier);
            }
            AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0f);
            m_sequentialKills = 0;
            m_player.stats.RecalculateStatsWithoutRebuildingGunVolleys(m_player);
        }

        public static GameObject PlayStarBreakVFXOnActor (PlayerController player, Color tintColor, float currentMultiplier)
        {
            Vector3 vfxOffset = new Vector3(0 / 16f, 24 / 16f, -2f);

            GameObject vfxObject = UnityEngine.Object.Instantiate((!(currentMultiplier >= 2f)) ? smallStarVFX : largeStarVFX, new Vector3(0f, 999f, -2f), Quaternion.identity);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -2f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                /*sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 1f);*/
                sprite.color = tintColor;
            }

            VFXAnchorOnPlayerModule anchor = vfxObject.GetComponent<VFXAnchorOnPlayerModule>();

            anchor.anchorPlayer = player.gameActor;
            anchor.offset = vfxOffset + new Vector3(0, 0);

            return vfxObject;
        }

        public void BloopIconAboveHead(Color tintColor)
        {
            BraveBehaviour blooper = coinBlooper.GetComponent<BraveBehaviour>();
            //blooper.DoBloop(targetSprite, overrideSprite, tintColor, addOutline);

            GameObject gameObject = UnityEngine.Object.Instantiate(smallStarVFX);
            gameObject.transform.parent = blooper.transform.parent.parent;
            gameObject.transform.position = blooper.transform.parent.position;
            CoinBloop componentInParent = gameObject.transform.GetChild(0).GetComponentInParent<CoinBloop>();
            componentInParent.m_player = m_player.GetComponentInParent<PlayerController>();
            gameObject.GetComponent<tk2dBaseSprite>().color = tintColor;

            Bounds bounds = base.sprite.GetBounds();
            float num = bounds.min.x + bounds.extents.x;
            //blooper.transform.parent.position = blooper.transform.parent.position.WithX(BraveMathCollege.QuantizeFloat(blooper.transform.parent.parent.GetComponent<PlayerController>().LockedApproximateSpriteCenter.x - num, 0.0625f));
            blooper.transform.parent.localPosition = blooper.transform.parent.localPosition.WithZ(-5f);
            blooper.transform.parent.localPosition += new Vector3(16f / 16f, 0f, 0f);
            componentInParent.m_cachedParentLocalPosition = blooper.transform.localPosition;
        }

        public void DoBloop(tk2dBaseSprite targetSprite, string overrideSprite, Color tintColor, bool addOutline = false)
        {
            if (m_player == null)
            {
                m_player = GetComponentInParent<PlayerController>();
            }
            GameObject gameObject = UnityEngine.Object.Instantiate(base.transform.parent.gameObject);
            gameObject.transform.parent = base.transform.parent.parent;
            gameObject.transform.position = base.transform.parent.position;
            CoinBloop componentInParent = gameObject.transform.GetChild(0).GetComponentInParent<CoinBloop>();
            componentInParent.m_player = m_player;
            componentInParent.DoBloopInternal(targetSprite, overrideSprite, tintColor, addOutline);
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
