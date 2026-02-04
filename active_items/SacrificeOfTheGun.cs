using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.active_items
{
    internal class SacrificeOfTheGun : PlayerItem
    {
        public static string ItemName = "Sacrifice of the Gun";

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SacrificeOfTheGun>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

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
            base.DoEffect(player);

            IsCurrentlyActive = true;

            Plugin.Log($"initial activation");
        }

        public override void DoActiveEffect(PlayerController player)
        {
            if (player.CurrentGun.quality == ItemQuality.EXCLUDED || player.CurrentGun.quality == ItemQuality.SPECIAL)
            {
                return;
            }

            base.DoActiveEffect(player);

            Gun sacrificedGun = player.CurrentGun;
            ItemQuality qual = sacrificedGun.quality;

            DebrisObject droppedGun = player.ForceDropGun(sacrificedGun);
            UnityEngine.Object.Destroy(droppedGun.gameObject);

            System.Random rand = new System.Random();

            if (qual == ItemQuality.S)
            {
                PickupObject newGunToSpawn = PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(1342), new List<int>(), qual);
                PickupObject newGunToSpawn2 = PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(63540), new List<int>(), qual);
                LootEngine.SpewLoot(newGunToSpawn.gameObject, player.CenterPosition + new Vector2(-2f, 0));
                LootEngine.SpewLoot(newGunToSpawn2.gameObject, player.CenterPosition + new Vector2(2f, 0));
            }
            else
            {
                PickupObject newGunToSpawn = PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(), new List<int>(), qual + 1);
                LootEngine.SpewLoot(newGunToSpawn.gameObject, player.CenterPosition);
            }

            IsCurrentlyActive = false;

            player.RemoveActiveItem(ID);
        }
    }
}
