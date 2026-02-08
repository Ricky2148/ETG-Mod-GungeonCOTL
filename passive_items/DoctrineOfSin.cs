using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ShrineCost;

namespace GungeonCOTL.passive_items
{
    internal class DoctrineOfSin : PassiveItem
    {
        public static string ItemName = "Doctrine of Sin";

        private static float CurseToGive = 3f;

        public bool ChestSpawned = false;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_sin_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfSin>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, CurseToGive, StatModifier.ModifyMethod.ADDITIVE);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            if (!ChestSpawned)
            {
                SpawnSinChest();
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
        }

        private void SpawnSinChest()
        {
            if (GameManager.Instance?.PrimaryPlayer == null)
            {
                Plugin.Log("Player doesn't exist!");
                return;
            }
            RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }
            RewardManager rewards = GameManager.Instance?.RewardManager;
            var rand = UnityEngine.Random.Range(0, 99);
            Plugin.Log($"randVal: {rand}");
            Chest chest;
            switch (rand)
            {
                case < 1:
                    chest = rewards?.Rainbow_Chest;
                    break;
                case < 5:
                    chest = rewards?.Synergy_Chest;
                    break;
                case < 30:
                    chest = rewards?.A_Chest;
                    break;
                default:
                    chest = rewards?.S_Chest;
                    break;
            }
            chest.overrideMimicChance = 0;
            chest.IsLocked = false;

            var location = currentRoom.GetBestRewardLocation(new IntVector2(2, 1), Dungeonator.RoomHandler.RewardLocationStyle.PlayerCenter, true);
            var c = Chest.Spawn(chest, location + (IntVector2.Zero), currentRoom);

            ChestSpawned = true;
        }
    }
}
