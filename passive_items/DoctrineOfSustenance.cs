using Alexandria.ItemAPI;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class DoctrineOfSustenance : PassiveItem
    {
        public static string ItemName = "Doctrine of Sustenance";
    	
        public float ChanceToImproveHealing = 0.15f;

        public float HealingImprovedBy = 0.5f;

        public GameObject OnImprovedHealingVFX;

        public float ChanceToGainMoney = 0.15f;

        public int MoneyGiven = 10;

        public float ChanceToGainAmmo = 0.15f;

        public float AmmoRestorePercentage = 0.10f;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/doctrine_of_sustenance_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DoctrineOfSustenance>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, DiscountValue, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.OnImprovedHealingVFX = (PickupObjectDatabase.GetById((int)Items.Antibody) as HealingReceivedModificationItem).OnImprovedHealingVFX;

            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                AkSoundEngine.PostEvent("doctrine_piece", player.gameObject);
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
            if (args != EventArgs.Empty && UnityEngine.Random.value < ChanceToImproveHealing)
            {
                if (OnImprovedHealingVFX != null)
                {
                    source.GetComponent<PlayerController>().PlayEffectOnActor(OnImprovedHealingVFX, Vector3.zero);
                }
                args.ModifiedHealing += HealingImprovedBy;
            }
            else if (args != EventArgs.Empty && UnityEngine.Random.value < (ChanceToImproveHealing + ChanceToGainMoney))
            {
                //Plugin.Log($"spawn money");
                Owner.StartCoroutine(HelpfulMethods.SpawnMoney(Owner, MoneyGiven, 0.1f));
            }
            else if (args != EventArgs.Empty && UnityEngine.Random.value < (ChanceToImproveHealing + ChanceToGainMoney + ChanceToGainAmmo))
            {
                //Plugin.Log($"restore ammo");
                HelpfulMethods.RestorePercentAmmo(Owner, AmmoRestorePercentage);
            }
        }
    }
}