using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// add additional sfx for heal and ammo restoration

namespace GungeonCOTL.passive_items
{
    internal class DoctrineOfSustenance : PassiveItem
    {
        public static string ItemName = "Doctrine of Sustenance";
    	
        public float ChanceToImproveHealing = 0.15f;
        public float HealingImprovedBy = 0.5f;
        public GameObject OnImprovedHealingVFX;

        public float ChanceToGainMoney = 0.15f;
        public int MoneyGiven = 15;
        private static float timeDelay = 0.15f;
        private static float timeDelayRandRatio = 0.5f;
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

        public float ChanceToGainAmmo = 0.15f;
        public float AmmoRestorePercentage = 0.15f;

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_sustenance_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfSustenance>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "DEVOUR";
            string longDesc = "Every heal has a chance to heal more, give casings, or restore ammo.\n\n" +
                "\"Instruct them on the liturgies surrounding their daily bread.\"\n\n" +
                "Learn how to be more thankful for the crops which feed you. Every meal makes you grateful.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, DiscountValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.OnImprovedHealingVFX = (PickupObjectDatabase.GetById((int)Items.Antibody) as HealingReceivedModificationItem).OnImprovedHealingVFX;

            item.quality = PickupObject.ItemQuality.SPECIAL;
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

                AkSoundEngine.PostEvent("doctrine_piece", player.gameObject);
                activeVFXObject = VFXPlayerCOTL.PlayDoctrineEffectOnActor(player, true, false, false);
            }

            if (!m_pickedUp)
            {
                HealthHaver obj = player.healthHaver;
                obj.ModifyHealing = (Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>)Delegate.Combine(obj.ModifyHealing, new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(ModifyIncomingHealing));
                base.Pickup(player);
            }

            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public override DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            DebrisObject debrisObject = base.Drop(player);
            HealthHaver obj = player.healthHaver;
            obj.ModifyHealing = (Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>)Delegate.Remove(obj.ModifyHealing, new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(ModifyIncomingHealing));
            debrisObject.GetComponent<HealingReceivedModificationItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        public override void OnDestroy()
        {
            if (m_pickedUp)
            {
                HealthHaver obj = m_owner.healthHaver;
                obj.ModifyHealing = (Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>)Delegate.Combine(obj.ModifyHealing, new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(ModifyIncomingHealing));
            }
            base.OnDestroy();
        }

        private void ModifyIncomingHealing(HealthHaver source, HealthHaver.ModifyHealingEventArgs args)
        {
            //find sfx for each event
            float rand = UnityEngine.Random.value;
            Plugin.Log($"rand: {rand}");

            if (args != EventArgs.Empty && rand < ChanceToImproveHealing)
            {
                Plugin.Log($"rand: {rand}, heal");
                if (OnImprovedHealingVFX != null)
                {
                    source.GetComponent<PlayerController>().PlayEffectOnActor(OnImprovedHealingVFX, Vector3.zero);
                }
                args.ModifiedHealing += HealingImprovedBy;
            }
            else if (args != EventArgs.Empty && rand < (ChanceToImproveHealing + ChanceToGainMoney))
            {
                Plugin.Log($"rand: {rand}, money");
                Owner.StartCoroutine(HelpfulMethods.SpawnMoney(Owner, MoneyGiven, timeDelay, true, timeDelayRandRatio, true, moneySFXList));
            }
            else if (args != EventArgs.Empty && rand < (ChanceToImproveHealing + ChanceToGainMoney + ChanceToGainAmmo))
            {
                Plugin.Log($"rand: {rand}, ammo");
                HelpfulMethods.RestorePercentAmmo(Owner, AmmoRestorePercentage);
            }
        }
    }
}