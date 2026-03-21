using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using Brave.BulletScript;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//try reworking the spawning to be a circle instead of a square
//add activation sfx and vfx

namespace GungeonCOTL.active_items
{
    internal class RitualOfEnrichment : PlayerItem
    {
        public static string ItemName = "Ritual of Enrichment";

        private static int MoneyGiven = 50;

        private static float timeDelay = 0.05f;
        private static float timeDelayRandRatio = 0.4f;
        private static List<string> moneySFXList = new List<string>
        {
            "pop_1",
            "pop_2",
            "pop_3",
            "pop_4",
            "pop_5",
            "pop_6",
            "pop_7",
        };

        private static List<string> BronzeEnrichmentActivationVFXSpritePath = "GungeonCOTL/Resources/vfxs/casing_vanish_vfxs/casingVanish_bronze/casingVanish".GetResourceFrames(62);

        private static GameObject BronzeEnrichmentActivationVFX;

        private static List<string> SilverEnrichmentActivationVFXSpritePath = "GungeonCOTL/Resources/vfxs/casing_vanish_vfxs/casingVanish_silver/casingVanish".GetResourceFrames(62);

        private static GameObject SilverEnrichmentActivationVFX;

        private static List<string> GoldEnrichmentActivationVFXSpritePath = "GungeonCOTL/Resources/vfxs/casing_vanish_vfxs/casingVanish_gold/casingVanish".GetResourceFrames(62);

        private static GameObject GoldEnrichmentActivationVFX;

        private static List<string> MixEnrichmentActivationVFXSpritePath = "GungeonCOTL/Resources/vfxs/casing_vanish_vfxs/casingVanish_mix/casingVanish".GetResourceFrames(62);

        private static GameObject MixEnrichmentActivationVFX;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/ritual_of_enrichment_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RitualOfEnrichment>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "received a donation!";
            string longDesc = "Command your followers to \"donate\" their casings for the gods. You assure them that the funds will go towards all things divine.\n\n" +
                "somehow it ends up in your pockets...\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;

            BronzeEnrichmentActivationVFX = VFXBuilder.CreateVFX
            (
                "bronze_enrichment_activation_vfx",
                BronzeEnrichmentActivationVFXSpritePath,
                18,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );

            SilverEnrichmentActivationVFX = VFXBuilder.CreateVFX
            (
                "Silver_enrichment_activation_vfx",
                SilverEnrichmentActivationVFXSpritePath,
                18,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );

            GoldEnrichmentActivationVFX = VFXBuilder.CreateVFX
            (
                "Gold_enrichment_activation_vfx",
                GoldEnrichmentActivationVFXSpritePath,
                18,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );

            MixEnrichmentActivationVFX = VFXBuilder.CreateVFX
            (
                "Mix_enrichment_activation_vfx",
                MixEnrichmentActivationVFXSpritePath,
                18,
                new IntVector2(0, 0),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Once,
                true
            );
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

            //testing
            //player.PlayEffectOnActor(EnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);

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

            float randVal = UnityEngine.Random.value;
            Plugin.Log($"rand val: {randVal}");

            switch (randVal)
            {
                //gold (5 loops at 1s) = 5s
                case < 0.01f:
                    Plugin.Log($"gold: {randVal}");
                    //player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, (MoneyGiven / 50) * 5, timeDelay * 20f, false, timeDelayRandRatio, true, moneySFXList, 50));

                    //bronze casing spam ver
                    player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, (int)(MoneyGiven * 5f), timeDelay * 0.6f, true, timeDelayRandRatio, true, moneySFXList));
                    player.PlayEffectOnActor(GoldEnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);
                    break;
                //mix (30 loops at 0.1.25s) 3.75s
                case < 0.10f:
                    Plugin.Log($"mix: {randVal}");
                    //player.StartCoroutine(SpawnRandomCasingsInDonut(player, (int)((MoneyGiven / 2) * 1.2f), timeDelay * 2.5f, true, timeDelayRandRatio, true, moneySFXList));

                    //bronze casing spam ver
                    player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, (int)(MoneyGiven * 2.5f), timeDelay * 0.8f, true, timeDelayRandRatio, true, moneySFXList));
                    player.PlayEffectOnActor(MixEnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);
                    break;
                //silver (15 loops at 0.20s) = 3.0s
                case < 0.30f:
                    Plugin.Log($"silver: {randVal}");
                    //player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, (int)((MoneyGiven / 5) * 1.5f), timeDelay * 4f, true, timeDelayRandRatio, true, moneySFXList, 5));

                    //bronze casing spam ver
                    player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, (int)(MoneyGiven * 1.5f), timeDelay, true, timeDelayRandRatio, true, moneySFXList));
                    player.PlayEffectOnActor(SilverEnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);
                    break;
                //bronze (50 loops at 0.05s) = 2.5s
                default:
                    Plugin.Log($"bronze: {randVal}");
                    player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, MoneyGiven, timeDelay, true, timeDelayRandRatio, true, moneySFXList));
                    player.PlayEffectOnActor(BronzeEnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);
                    break;
            }

            //player.StartCoroutine(HelpfulMethods.SpawnMoneyInDonut(player, MoneyGiven, timeDelay, true, timeDelayRandRatio, true, moneySFXList));
            //player.PlayEffectOnActor(BronzeEnrichmentActivationVFX, new Vector3(19 / 16f, 25 / 16f, 1f), true, false, false);

            AkSoundEngine.PostEvent("enrichment_activation", player.gameObject);

            player.RemoveActiveItem(ID);
        }

        public static System.Collections.IEnumerator SpawnRandomCasingsInDonut(PlayerController player, int count, float spawnDelay, bool randSpawn = false, float randRatio = 0f, bool playSFX = false, List<string> SFXList = null)
        {
            //Plugin.Log($"start spawning");
            float timeDelayUsed = spawnDelay;
            for (int i = 0; i < count; i++)
            {
                //Plugin.Log($"i: {i}, count: {count}");
                Vector3 idk = player.specRigidbody.UnitDimensions;
                float num = ((idk.x + idk.y) / 2);

                //Vector2 offset = new Vector3(num * UnityEngine.Random.Range(-4f, 4f), (num * UnityEngine.Random.Range(-4f, 4f)) + -0.5f);

                /*Vector2 offset = UnityEngine.Random.insideUnitCircle;
                offset.Scale(new Vector3(num * 6f, num * 5f, 0));
                offset.y -= 0.5f;

                Vector2 offsetExclusionMin = new Vector3(num * -3f, (num * -2.5f) + -1f);
                Vector2 offsetExclusionMax = new Vector3(num * 3f, (num * 2.5f) + -1f);

                while (offset.IsWithin(offsetExclusionMin, offsetExclusionMax))
                {
                    offset = UnityEngine.Random.insideUnitCircle;
                    offset.Scale(new Vector3(num * 6f, num * 5f, 0));
                    offset.y -= 0.5f;
                }*/

                Vector2 offset = HelpfulMethods.RandomPointInDonut(Vector2.zero, (num * 3f), (num * 5f), 1.3f, 1);
                //Plugin.Log($"offset: {offset}, min: {num * 1.5f}, max: {num * 3f}");
                offset.y -= 0.5f;

                float randVal = UnityEngine.Random.value;
                int casingValue = 1;

                switch (randVal)
                {
                    case < 0.05f:
                        casingValue = 25;
                        break;
                    case < 0.10f:
                        casingValue = 12;
                        break;
                    case < 0.20f:
                        casingValue = 8;
                        break;
                    case < 0.40f:
                        casingValue = 3;
                        break;
                    default:
                        casingValue = 1;
                        break;
                }

                LootEngine.SpawnCurrency(player.specRigidbody.UnitBottomCenter + offset, casingValue);

                if (playSFX && SFXList != null && SFXList.Count > 0)
                {
                    HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);
                }

                if (randSpawn)
                {
                    timeDelayUsed = spawnDelay * (UnityEngine.Random.Range(1f - randRatio, 1f + randRatio));
                }
                yield return new WaitForSeconds(timeDelayUsed);
            }
            //Plugin.Log($"finish spawning");
            yield return null;
        }

        /*private System.Collections.IEnumerator SpawnMoney(PlayerController player, int count)
        {
            //Plugin.Log($"start spawning");
            for (int i = 0;  i < count; i++)
            {
                //Plugin.Log($"i: {i}, count: {count}");
                Vector3 idk = player.specRigidbody.UnitDimensions;
                float num = ((idk.x + idk.y) / 2);
                Vector2 offset = new Vector3(num * UnityEngine.Random.Range(-3f, 3f), (num * UnityEngine.Random.Range(-3f, 3f)) + -1f);
                LootEngine.SpawnCurrency(player.specRigidbody.UnitBottomCenter + offset, 1);
                yield return new WaitForSeconds(0.05f);
            }
            //Plugin.Log($"finish spawning");
            yield return null;
        }*/
    }
}
