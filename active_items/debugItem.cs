using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using Dungeonator;
using GungeonCOTL.passive_items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.active_items
{
    internal class debugItem : PlayerItem
    {
        public List<PickupObject> choices = new List<PickupObject>
        {
            PickupObjectDatabase.GetById(CrownUpgradeResurrection.ID),
            PickupObjectDatabase.GetById(CrownUpgradeDarknessWithin.ID),
            PickupObjectDatabase.GetById(CarefreeMelody.ID)
        };

        public AnimationCurve spawnCurve = new AnimationCurve
        {

        };

        public Vector2 choicesSpawnLocation;

        public static int ID;

        public static void Init()
        {
            string itemName = "Debug Item";
            string resourceName = "GungeonCOTL/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<debugItem>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);
            //ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.PerRoom, 20f);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            item.consumable = false;

            item.usableDuringDodgeRoll = true;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            ID = item.PickupObjectId;
            //Plugin.Log($"ID: {ID}, pickupID: {item.PickupObjectId}");
        }

        // subscribe to the player events
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            return base.Drop(player);
        }

        public override void DoEffect(PlayerController player)
        {
            choicesSpawnLocation = player.CenterPosition;
            StartCoroutine(PresentItem());

            Plugin.Log("debug item finished");
        }

        private IEnumerator PresentItem()
        {
            bool shouldActuallyPresent = true;
            List<Transform> vfxTransforms = new List<Transform>();
            List<Vector3> vfxObjectOffsets = new List<Vector3>();
            Vector3 attachPoint = Vector3.zero;
            if (shouldActuallyPresent)
            {
                Bounds bounds = base.sprite.GetBounds();
                attachPoint = base.transform.position + bounds.extents;
                
                for (int i = 0; i < choices.Count; i++)
                {
                    PickupObject pickupObject = choices[i];
                    tk2dSprite tk2dSprite2 = pickupObject.GetComponent<tk2dSprite>();
                    if (tk2dSprite2 == null)
                    {
                        tk2dSprite2 = pickupObject.GetComponentInChildren<tk2dSprite>();
                    }
                    GameObject gameObject = new GameObject("VFX_Chest_Item");
                    Transform transform = gameObject.transform;
                    Vector3 vector = Vector3.zero;
                    if (tk2dSprite2 != null)
                    {
                        tk2dSprite tk2dSprite3 = tk2dSprite.AddComponent(gameObject, tk2dSprite2.Collection, tk2dSprite2.spriteId);
                        tk2dSprite3.HeightOffGround = 2f;
                        NotePassiveItem component = tk2dSprite2.GetComponent<NotePassiveItem>();
                        if (component != null && component.ResourcefulRatNoteIdentifier >= 0)
                        {
                            tk2dSprite3.SetSprite(component.GetAppropriateSpriteName(isAmmonomicon: false));
                        }
                        SpriteOutlineManager.AddOutlineToSprite(tk2dSprite3, Color.white, 0.5f);
                        vector = -BraveUtility.QuantizeVector(gameObject.GetComponent<tk2dSprite>().GetBounds().extents);
                        tk2dSprite3.UpdateZDepth();
                    }
                    transform.position = attachPoint + vector;
                    vfxTransforms.Add(transform);
                    vfxObjectOffsets.Add(vector);
                }
                float displayTime = 1f;
                float elapsed = 0f;
                while (elapsed < displayTime)
                {
                    elapsed += BraveTime.DeltaTime * 1.5f;
                    float t = Mathf.Clamp01(elapsed / displayTime);
                    float curveValue = spawnCurve.Evaluate(t);
                    float modT = Mathf.SmoothStep(0f, 1f, t);
                    if (vfxTransforms.Count <= 4)
                    {
                        for (int j = 0; j < vfxTransforms.Count; j++)
                        {
                            float num = ((vfxTransforms.Count != 1) ? (-1f + 2f / (float)(vfxTransforms.Count - 1) * (float)j) : 0f);
                            num = num * ((float)vfxTransforms.Count / 2f) * 1f;
                            Vector3 vector2 = attachPoint + vfxObjectOffsets[j] + new Vector3(Mathf.Lerp(0f, num, modT), curveValue, -2.5f);
                            if (CheckPresentedItemTheoreticalPosition(vector2, vfxObjectOffsets[j]))
                            {
                                vector2 = vfxTransforms[j].position;
                            }
                            vfxTransforms[j].position = vector2;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < vfxTransforms.Count; k++)
                        {
                            float num2 = 360f / (float)vfxTransforms.Count;
                            Vector3 vector3 = Quaternion.Euler(0f, 0f, num2 * (float)k) * Vector3.right;
                            float num3 = 3f;
                            Vector2 b = vector3.XY().normalized * num3;
                            Vector3 vector4 = attachPoint + vfxObjectOffsets[k] + new Vector3(0f, curveValue, -2.5f) + Vector2.Lerp(Vector2.zero, b, modT).ToVector3ZUp();
                            if (CheckPresentedItemTheoreticalPosition(vector4, vfxObjectOffsets[k]))
                            {
                                vector4 = vfxTransforms[k].position;
                            }
                            vfxTransforms[k].position = vector4;
                        }
                    }
                    yield return null;
                }
            }
            SpewChoicesOntoGround(vfxTransforms);
            yield return null;
            for (int l = 0; l < vfxTransforms.Count; l++)
            {
                Plugin.Log("destroying items?");
                UnityEngine.Object.Destroy(vfxTransforms[l].gameObject);
            }
        }

        private void SpewChoicesOntoGround(List<Transform> spawnTransforms)
        {
            Plugin.Log("spew choices onto ground");
            List<DebrisObject> list = new List<DebrisObject>();
            for (int i = 0; i < choices.Count; i++)
            {
                List<GameObject> list2 = new List<GameObject>();
                list2.Add(choices[i].gameObject);
                List<DebrisObject> list3 = LootEngine.SpewLoot(list2, spawnTransforms[i].position);
                //List<DebrisObject> list3 = LootEngine.SpewLoot(list2, Vector3.zero);
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
            if (choicesSpawnLocation.GetAbsoluteRoom() == GameManager.Instance.Dungeon.data.Entrance)
            {
                GameManager.Instance.Dungeon.StartCoroutine(HandleRainbowRunLootProcessing(list));
            }
        }

        private bool CheckPresentedItemTheoreticalPosition(Vector3 targetPosition, Vector3 objectOffset)
        {
            Vector3 pos = targetPosition - new Vector3(objectOffset.x * 2f, 0f, 0f);
            Vector3 pos2 = targetPosition - new Vector3(0f, objectOffset.y * 2f, 0f);
            Vector3 pos3 = targetPosition - new Vector3(objectOffset.x * 2f, objectOffset.y * 2f, 0f);
            if (!CheckCellValidForItemSpawn(targetPosition) || !CheckCellValidForItemSpawn(pos) || !CheckCellValidForItemSpawn(pos2) || !CheckCellValidForItemSpawn(pos3))
            {
                return true;
            }
            return false;
        }

        private bool CheckCellValidForItemSpawn(Vector3 pos)
        {
            IntVector2 vec = pos.IntXY(VectorConversions.Floor);
            Dungeon dungeon = GameManager.Instance.Dungeon;
            if (!dungeon.data.CheckInBoundsAndValid(vec) || dungeon.CellIsPit(pos) || dungeon.data.isTopWall(vec.x, vec.y))
            {
                return false;
            }
            if (dungeon.data.isWall(vec.x, vec.y) && !dungeon.data.isFaceWallLower(vec.x, vec.y))
            {
                return false;
            }
            return true;
        }

        protected void BecomeViableItem(DebrisObject debris)
        {
            Plugin.Log("become viable item");
            debris.OnTouchedGround = (Action<DebrisObject>)Delegate.Remove(debris.OnTouchedGround, new Action<DebrisObject>(BecomeViableItem));
            debris.OnGrounded = (Action<DebrisObject>)Delegate.Remove(debris.OnGrounded, new Action<DebrisObject>(BecomeViableItem));
            debris.specRigidbody.CollideWithOthers = true;
            Vector2 zero = Vector2.zero;
            //zero = ((!(spawnTransform != null)) ? (debris.sprite.WorldCenter - base.sprite.WorldCenter) : (debris.sprite.WorldCenter - spawnTransform.position.XY()));
            //zero = debris.sprite.WorldCenter - base.sprite.WorldCenter;
            zero = debris.sprite.WorldCenter - choicesSpawnLocation;
            debris.ClearVelocity();
            debris.ApplyVelocity(zero.normalized * 2f);
        }

        private IEnumerator HandleRainbowRunLootProcessing(List<DebrisObject> items)
        {
            Plugin.Log("handle rainbow run loot processing");
            while (true)
            {
                Plugin.Log("looping rainbow loot");
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
                        LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNotePostRainbow, choicesSpawnLocation + new Vector2(1f, 1.5f), choicesSpawnLocation.GetAbsoluteRoom(), doPoof: true);
                    }
                    yield break;
                }
                yield return null;
            }
        }
    }
}
