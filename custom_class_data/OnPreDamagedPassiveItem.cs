// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AutoblankVestItem
using Alexandria.Misc;
using LOLItems;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OnPreDamagedPassiveItem : PassiveItem
{
    public float procChance = 0.1f;

    public bool triggerBlank = false;

    public bool playsSFX = false;

    public List<string> sfxPath = new List<string>();

    public bool triggersInvulnerability = false;

    public float invulnerabilityDuration = 1f;

    //private System.Random rand = new System.Random();

    public override void Pickup(PlayerController player)
    {
        if (!m_pickedUp)
        {
            base.Pickup(player);
            HealthHaver obj = player.healthHaver;
            obj.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(obj.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(HandleEffect));
        }
    }

    public void updateSFXList(string[] newSFXList)
    {
        foreach (string i in newSFXList)
        {
            sfxPath.Add(i);
        }
    }

    public void setProcChance(float newProcChance)
    {
        procChance = newProcChance;
    }

    private void HandleEffect(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
    {
        if (args == EventArgs.Empty || args.ModifiedDamage <= 0f || !source.IsVulnerable)
        {
            return;
        }
        // useless since no interactino with elder blank
        /*
        if ((bool)m_owner)
        {
            for (int i = 0; i < m_owner.activeItems.Count; i++)
            {
                if (m_owner.activeItems[i].PickupObjectId == ElderBlankID && !m_owner.activeItems[i].IsOnCooldown)
                {
                    source.TriggerInvulnerabilityPeriod();
                    m_owner.ForceBlank();
                    m_owner.activeItems[i].ForceApplyCooldown(m_owner);
                    args.ModifiedDamage = 0f;
                    return;
                }
            }
        }*/
        //var rand = new System.Random();
        //float randValue = rand.Next(100);
        float randValue = UnityEngine.Random.value;
        //Plugin.Log($"rolled {randValue} for proc chance of {procChance}");

        if (randValue <= procChance && (bool)m_owner && !m_owner.IsFalling)
        {
            //Plugin.Log($"predamage event triggered for {base.EncounterNameOrDisplayName}");
            if (playsSFX && sfxPath.Count > 0)
            {
                HelpfulMethods.PlayRandomSFX(this.Owner, sfxPath);
            }
            if (triggersInvulnerability)
            {
                PlayerController player = source.GetComponent<PlayerController>();
                //source.TriggerInvulnerabilityPeriod(invulnerabilityDuration);
                player.TriggerInvulnerableFrames(invulnerabilityDuration);
            }
            if (triggerBlank)
            {
                m_owner.ForceBlank();
            }
            args.ModifiedDamage = 0f;
        }
        else 
        {
            //rand = new System.Random();
            //randValue = rand.Next(100);
        }
    }

    public override DebrisObject Drop(PlayerController player)
    {
        DebrisObject debrisObject = base.Drop(player);
        OnPreDamagedPassiveItem component = debrisObject.GetComponent<OnPreDamagedPassiveItem>();
        HealthHaver obj = player.healthHaver;
        obj.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(obj.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(HandleEffect));
        component.m_pickedUpThisRun = true;
        return debrisObject;
    }

    public override void OnDestroy()
    {
        if ((bool)m_owner)
        {
            HealthHaver obj = m_owner.healthHaver;
            obj.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(obj.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(HandleEffect));
        }
        base.OnDestroy();
    }
}
