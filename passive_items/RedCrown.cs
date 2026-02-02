using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class RedCrown : PassiveItem
    {
        public static string ItemName = "Red Crown";

        public float DevotionExpTracker = 0f;

        public float DevotionCurrentThreshold = 1000f;

        public int NumOfDivineInspirations = 0;

        public List<float> DevotionExpThresholdList = new List<float>
        {
            1000f,
            2000f,
            3000f,
            4000f,
            5000f,
            6000f,
            7000f,
            8000f,
            9000f,
            10000f
        };

        private Vector3 DivineInspirationChoiceDecisionLocation = Vector3.zero;

        public List<PickupObject> choices = new List<PickupObject>
        {
            PickupObjectDatabase.GetById(CrownUpgradeResurrection.ID),
            PickupObjectDatabase.GetById(CrownUpgradeDarknessWithin.ID),
            PickupObjectDatabase.GetById(CarefreeMelody.ID)
        };

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/red_crown_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<RedCrown>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            item.quality = PickupObject.ItemQuality.SPECIAL;

            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
            
            //testing
            DevotionExpTracker = 10000f;

            if (NumOfDivineInspirations > DevotionExpThresholdList.Count)
            {
                NumOfDivineInspirations = DevotionExpThresholdList.Count;
            }
            DevotionCurrentThreshold = DevotionExpThresholdList[NumOfDivineInspirations];

            Plugin.Log($"DevotionExpTracker: {DevotionExpTracker}, CurrentThreshold: {DevotionCurrentThreshold}, InspirationCount: {NumOfDivineInspirations}");

            player.OnAnyEnemyReceivedDamage += KillEnemyCount;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.OnAnyEnemyReceivedDamage -= KillEnemyCount;
            }
        }

        private void KillEnemyCount(float damage, bool fatal, HealthHaver enemyHealth)
        {
            if (enemyHealth && fatal && enemyHealth.aiActor != null)
            {
                //Plugin.Log($"enemyHealth: {enemyHealth}");
                float expToGain = 0;
                if (enemyHealth.IsBoss || enemyHealth.IsSubboss)
                {
                    //Plugin.Log("is boss");
                    expToGain = (enemyHealth.aiActor.healthHaver.GetMaxHealth() * 0.25f);
                }
                else
                {
                    expToGain = enemyHealth.aiActor.healthHaver.GetMaxHealth();
                }

                DevotionExpTracker += expToGain;
                Plugin.Log($"Gained {expToGain} Devotion! Current Devotion: {DevotionExpTracker}/{DevotionCurrentThreshold}");
                /*if (DivineAscentExpTracker >= DivineAscentThreshold[DivineAscentFormTracker] && DivineAscentFormTracker < DivineAscentThreshold.Length)
                {
                    TriggerAscent();
                    DivineAscentExpTracker = 0f;
                }*/
                if (DevotionExpTracker >= DevotionCurrentThreshold)
                {
                    TriggerDivineInspiration();
                }
            }
        }

        private void TriggerDivineInspiration()
        {
            DevotionExpTracker = 0;
            DevotionCurrentThreshold = 0;
            if (NumOfDivineInspirations < DevotionExpThresholdList.Count)
            {
                NumOfDivineInspirations++;
            }
            DevotionCurrentThreshold = DevotionExpThresholdList[NumOfDivineInspirations];
            Plugin.Log($"DevotionExpTracker: {DevotionExpTracker}, CurrentThreshold: {DevotionCurrentThreshold}, InspirationCount: {NumOfDivineInspirations}");
            SpewChoicesOntoGround(null, Owner.CenterPosition);
        }

        private void SpewChoicesOntoGround(List<Transform> spawnTransforms, Vector3 spawnLocation)
        {
            List<DebrisObject> list = new List<DebrisObject>();
            for (int i = 0; i < choices.Count; i++)
            {
                List<GameObject> list2 = new List<GameObject>();
                list2.Add(choices[i].gameObject);
                //List<DebrisObject> list3 = LootEngine.SpewLoot(list2, spawnTransforms[i].position);
                List<DebrisObject> list3 = LootEngine.SpewLoot(list2, Vector3.zero);
                list.AddRange(list3);
                for (int j = 0; j < list3.Count; j++)
                {
                    if ((bool)list3[j])
                    {
                        list3[j].PreventFallingInPits = true;
                    }
                    if (!(list3[j].GetComponent<Gun>() != null) && !(list3[j].GetComponent<CurrencyPickup>() != null) && list3[j].specRigidbody != null)
                    {
                        list3[j].specRigidbody.CollideWithOthers = false;
                        DebrisObject debrisObject = list3[j];
                        debrisObject.OnTouchedGround = (Action<DebrisObject>)Delegate.Combine(debrisObject.OnTouchedGround, new Action<DebrisObject>(BecomeViableItem));
                    }
                }
            }
            if (base.transform.position.GetAbsoluteRoom() == GameManager.Instance.Dungeon.data.Entrance)
            {
                GameManager.Instance.Dungeon.StartCoroutine(HandleRainbowRunLootProcessing(list));
            }
        }

        protected void BecomeViableItem(DebrisObject debris)
        {
            debris.OnTouchedGround = (Action<DebrisObject>)Delegate.Remove(debris.OnTouchedGround, new Action<DebrisObject>(BecomeViableItem));
            debris.OnGrounded = (Action<DebrisObject>)Delegate.Remove(debris.OnGrounded, new Action<DebrisObject>(BecomeViableItem));
            debris.specRigidbody.CollideWithOthers = true;
            Vector2 zero = Vector2.zero;
            //zero = ((!(spawnTransform != null)) ? (debris.sprite.WorldCenter - base.sprite.WorldCenter) : (debris.sprite.WorldCenter - spawnTransform.position.XY()));
            zero = debris.sprite.WorldCenter - base.sprite.WorldCenter;
            debris.ClearVelocity();
            debris.ApplyVelocity(zero.normalized * 2f);
        }

        private IEnumerator HandleRainbowRunLootProcessing(List<DebrisObject> items)
        {
            if ((bool)base.majorBreakable)
            {
                base.majorBreakable.Break(Vector2.zero);
            }
            while (true)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if ((bool)items[i])
                    {
                        continue;
                    }
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (i != j)
                        {
                            LootEngine.DoDefaultItemPoof(items[j].transform.position, ignoreTimeScale: false, muteAudio: true);
                            UnityEngine.Object.Destroy(items[j].gameObject);
                        }
                    }
                    if ((bool)this)
                    {
                        LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNotePostRainbow, base.transform.position.XY() + new Vector2(1f, 1.5f), base.transform.position.GetAbsoluteRoom(), doPoof: true);
                    }
                    yield break;
                }
                yield return null;
            }
        }
    }
}
