using Alexandria.ItemAPI;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// make there be some kind of cool vfx for when the gun actually gets sacrificed

namespace GungeonCOTL.active_items
{
    internal class SacrificeOfTheGun : PlayerItem
    {
        public static string ItemName = "Sacrifice of the Gun";

        private GameObject activeVFXObject;

        private static StatModifier ownerlessCurseModifier = StatModifier.Create(PlayerStats.StatType.Curse, StatModifier.ModifyMethod.ADDITIVE, 1.0f);

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/active_item_sprites/sacrifice_of_the_gun_alt_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SacrificeOfTheGun>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "to appease the Gods";
            string longDesc = "Sacrifice your current gun and obtains a new weapon of higher rarity. +1 Curse on use\n\n" +
                "Sacrifices a weapon to the gods in honor of their benevolence. In return, they shall reward you accordingly. " +
                "Many devout weapons are happy to give their lives for the good of us all, or so they say...\n" +
                "\nInitial item use starts the ritual. Press the item use button again while holding the weapon you wish to select. " +
                "If you try to activate the ritual on a starter, excluded, or special rarity weapon, it will cancel the ritual without being consumed.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 100);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            item.consumable = false;
            item.consumableOnActiveUse = false;
            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CanBeDropped = false;
            item.CanBeDropped = false; ID = item.PickupObjectId;
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

            player.GunChanged += ReattachSacrificeVFX;
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            player.GunChanged -= ReattachSacrificeVFX;
            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }
            return base.Drop(player);
        }

        public override void DoEffect(PlayerController player)
        {
            base.DoEffect(player);

            IsCurrentlyActive = true;

            //Plugin.Log($"initial activation");
            AkSoundEngine.PostEvent("sacrifice_start", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop", player.gameObject);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            activeVFXObject = VFXPlayerCOTL.PlaySacrificeEventEffectOnGun(player.CurrentGun);
        }

        private void ReattachSacrificeVFX(Gun previous, Gun current, bool newGun)
        {
            //activeVFXObject.transform.SetParent(current.transform, false);

            if (IsCurrentlyActive) activeVFXObject.GetComponent<VFXAnchorOnGunModule>().gun = current;

            //Plugin.Log($"gun name: {current.EncounterNameOrDisplayName}, , {current.gameObject}, , {current.sprite}, {current.transform}");
        }

        public override void DoActiveEffect(PlayerController player)
        {
            if (player == null || player.CurrentGun == null)
            {
                return;
            }

            if (player.CurrentGun.quality == ItemQuality.EXCLUDED || player.CurrentGun.quality == ItemQuality.SPECIAL)
            {
                AkSoundEngine.PostEvent("sacrifice_loop" + "_stop", player.gameObject);

                if (activeVFXObject != null)
                {
                    Destroy(activeVFXObject);
                }
                IsCurrentlyActive = false;
                return;
            }

            base.DoActiveEffect(player);

            Gun sacrificedGun = player.CurrentGun;
            ItemQuality qual = sacrificedGun.quality;

            DebrisObject droppedGun = player.ForceDropGun(sacrificedGun);
            UnityEngine.Object.Destroy(droppedGun.gameObject);

            //use UnityEngine.random
            System.Random rand = new System.Random();

            if (qual == ItemQuality.S)
            {
                PickupObject newGunToSpawn = PickupObjectDatabase.GetRandomGunOfQualities(rand, new List<int>(), qual);
                PickupObject newGunToSpawn2 = PickupObjectDatabase.GetRandomPassiveOfQualities(new System.Random(rand.Next()), new List<int>(), [qual, qual]);
                LootEngine.SpewLoot(newGunToSpawn.gameObject, player.CenterPosition + new Vector2(-2f, 0));
                LootEngine.SpewLoot(newGunToSpawn2.gameObject, player.CenterPosition + new Vector2(2f, 0));
            }
            else
            {
                PickupObject newGunToSpawn = PickupObjectDatabase.GetRandomGunOfQualities(rand, new List<int>(), qual + 1);
                LootEngine.SpewLoot(newGunToSpawn.gameObject, player.CenterPosition);
            }


            AkSoundEngine.PostEvent("sacrifice_gun_activated", player.gameObject);
            AkSoundEngine.PostEvent("sacrifice_loop" + "_stop", player.gameObject);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            player.ownerlessStatModifiers.Add(ownerlessCurseModifier);
            player.stats.RecalculateStatsWithoutRebuildingGunVolleys(player);

            IsCurrentlyActive = false;

            player.RemoveActiveItem(ID);
        }
    }
}
