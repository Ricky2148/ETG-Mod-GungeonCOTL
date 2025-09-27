using LOLItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

// mainly to add OnPreDamaged event

public class CustomHealthHaver : HealthHaver
{
    public delegate void OnDamagedEvent(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection);

    public delegate void OnHealthChangedEvent(float resultValue, float maxValue);

    public delegate void OnPreDamagedEvent(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection);

    public event OnPreDamagedEvent OnPreDamaged;

    public event OnDamagedEvent OnDamaged;

    public event OnHealthChangedEvent OnHealthChanged;

    // Remove 'override' keyword to fix CS0506, since base method is not virtual/abstract/override
    public /*override*/ void ApplyDamageDirectional(float damage, Vector2 direction, string damageSource, CoreDamageTypes damageTypes, DamageCategory damageCategory = DamageCategory.Normal, bool ignoreInvulnerabilityFrames = false, PixelCollider hitPixelCollider = null, bool ignoreDamageCaps = false)
    {
        if (GetCurrentHealth() > GetMaxHealth())
        {
            Plugin.Log("Something went wrong in HealthHaver, but we caught it! " + currentHealth);
            currentHealth = GetMaxHealth();
        }
        if (PreventAllDamage && damageCategory == DamageCategory.Unstoppable)
        {
            PreventAllDamage = false;
        }
        if (PreventAllDamage || ((bool)m_player && m_player.IsGhost) || (hitPixelCollider != null && DamageableColliders != null && !DamageableColliders.Contains(hitPixelCollider)) || (IsBoss && !BossHealthSanityCheck(damage)) || isFirstFrame)
        {
            return;
        }
        if (ignoreInvulnerabilityFrames)
        {
            if (!vulnerable)
            {
                return;
            }
        }
        else if (!IsVulnerable)
        {
            return;
        }
        if (damage <= 0f)
        {
            return;
        }
        damage *= GetDamageModifierForType(damageTypes);
        damage *= AllDamageMultiplier;
        if (OnlyAllowSpecialBossDamage && (damageTypes & CoreDamageTypes.SpecialBossDamage) != CoreDamageTypes.SpecialBossDamage)
        {
            damage = 0f;
        }
        if (IsBoss && !string.IsNullOrEmpty(damageSource))
        {
            if (damageSource == "primaryplayer")
            {
                damage *= GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
            }
            else if (damageSource == "secondaryplayer")
            {
                damage *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
            }
        }
        if ((bool)m_player && !ignoreInvulnerabilityFrames)
        {
            damage = Mathf.Min(damage, 0.5f);
        }
        if ((bool)m_player && damageCategory == DamageCategory.BlackBullet)
        {
            damage = 1f;
        }
        if (ModifyDamage != null)
        {
            ModifyDamageEventArgs e = new ModifyDamageEventArgs();
            e.InitialDamage = damage;
            e.ModifiedDamage = damage;
            ModifyDamageEventArgs e2 = e;
            ModifyDamage(this, e2);
            damage = e2.ModifiedDamage;
        }
        if (!m_player && !ignoreInvulnerabilityFrames && damage <= 999f && !ignoreDamageCaps)
        {
            if (m_damageCap > 0f)
            {
                damage = Mathf.Min(m_damageCap, damage);
            }
            if (m_bossDpsCap > 0f)
            {
                damage = Mathf.Min(damage, m_bossDpsCap * 3f - m_recentBossDps);
                m_recentBossDps += damage;
            }
        }
        if (damage <= 0f)
        {
            return;
        }
        if (NextShotKills)
        {
            damage = 100000f;
        }
        if (damage > 0f && HasCrest)
        {
            HasCrest = false;
        }
        if (healthIsNumberOfHits)
        {
            damage = 1f;
        }
        if (!NextDamageIgnoresArmor && !NextShotKills && Armor > 0f)
        {
            Armor -= 1f;
            damage = 0f;
            if (isPlayerCharacter)
            {
                m_player.OnLostArmor();
            }
        }
        NextDamageIgnoresArmor = false;
        float num = damage;
        if (num > 999f)
        {
            num = 0f;
        }
        num = Mathf.Min(currentHealth, num);
        if (TrackPixelColliderDamage)
        {
            if (hitPixelCollider != null)
            {
                if (PixelColliderDamage.TryGetValue(hitPixelCollider, out var value))
                {
                    PixelColliderDamage[hitPixelCollider] = value + damage;
                }
            }
            else if (damage <= 999f)
            {
                float num2 = damage * GlobalPixelColliderDamageMultiplier;
                List<PixelCollider> list = new List<PixelCollider>(PixelColliderDamage.Keys);
                for (int i = 0; i < list.Count; i++)
                {
                    PixelCollider key = list[i];
                    PixelColliderDamage[key] += num2;
                }
            }
        }


        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽⓽
        // ONLY CHANGED LINE
        if (this.OnPreDamaged != null)
        {
            this.OnPreDamaged(currentHealth, AdjustedMaxHealth, damageTypes, damageCategory, direction);
        }
        //ONLY CHANGED LINE
        
        
        
        currentHealth -= damage;
        if (isPlayerCharacter)
        {
            Plugin.Log(currentHealth + "||" + damage);
        }
        if (quantizeHealth)
        {
            currentHealth = BraveMathCollege.QuantizeFloat(currentHealth, quantizedIncrement);
        }
        currentHealth = Mathf.Clamp(currentHealth, minimumHealth, AdjustedMaxHealth);
        if (!isPlayerCharacter)
        {
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                GameManager.Instance.AllPlayers[j].OnAnyEnemyTookAnyDamage(num, currentHealth <= 0f && Armor <= 0f, this);
            }
            if (!string.IsNullOrEmpty(damageSource))
            {
                switch (damageSource)
                {
                    case "primaryplayer":
                    case "Player ID 0":
                        GameManager.Instance.PrimaryPlayer.OnDidDamage(damage, currentHealth <= 0f && Armor <= 0f, this);
                        break;
                    case "secondaryplayer":
                    case "Player ID 1":
                        GameManager.Instance.SecondaryPlayer.OnDidDamage(damage, currentHealth <= 0f && Armor <= 0f, this);
                        break;
                }
            }
        }
        if (flashesOnDamage && base.spriteAnimator != null && !m_isFlashing)
        {
            if (m_flashOnHitCoroutine != null)
            {
                StopCoroutine(m_flashOnHitCoroutine);
            }
            m_flashOnHitCoroutine = null;
            if (materialsToFlash == null)
            {
                materialsToFlash = new List<Material>();
                outlineMaterialsToFlash = new List<Material>();
                sourceColors = new List<Color>();
            }
            if ((bool)base.gameActor)
            {
                for (int k = 0; k < materialsToFlash.Count; k++)
                {
                    materialsToFlash[k].SetColor("_OverrideColor", base.gameActor.CurrentOverrideColor);
                }
            }
            if (outlineMaterialsToFlash != null)
            {
                for (int l = 0; l < outlineMaterialsToFlash.Count; l++)
                {
                    if (l >= sourceColors.Count)
                    {
                        UnityEngine.Debug.LogError("NOT ENOUGH SOURCE COLORS");
                        break;
                    }
                    outlineMaterialsToFlash[l].SetColor("_OverrideColor", sourceColors[l]);
                }
            }
            m_flashOnHitCoroutine = StartCoroutine(FlashOnHit(damageCategory, hitPixelCollider));
        }
        if (incorporealityOnDamage && !m_isIncorporeal)
        {
            StartCoroutine("IncorporealityOnHit");
        }
        lastIncurredDamageSource = damageSource;
        lastIncurredDamageDirection = direction;
        if (shakesCameraOnDamage)
        {
            GameManager.Instance.MainCameraController.DoScreenShake(cameraShakeOnDamage, base.specRigidbody.UnitCenter);
        }
        if (NextShotKills)
        {
            Armor = 0f;
        }
        if (this.OnDamaged != null)
        {
            this.OnDamaged(currentHealth, AdjustedMaxHealth, damageTypes, damageCategory, direction);
        }
        if (this.OnHealthChanged != null)
        {
            this.OnHealthChanged(currentHealth, AdjustedMaxHealth);
        }
        if (currentHealth == 0f && Armor == 0f)
        {
            NextShotKills = false;
            if (!SuppressDeathSounds)
            {
                AkSoundEngine.PostEvent("Play_ENM_death", base.gameObject);
                AkSoundEngine.PostEvent(string.IsNullOrEmpty(overrideDeathAudioEvent) ? "Play_CHR_general_death_01" : overrideDeathAudioEvent, base.gameObject);
            }
            Die(direction);
        }
        else if (usesInvulnerabilityPeriod)
        {
            StartCoroutine(HandleInvulnerablePeriod());
        }
        if (damageCategory != DamageCategory.Normal && damageCategory != DamageCategory.Collision)
        {
            return;
        }
        if (currentHealth <= 0f && Armor <= 0f)
        {
            if (!DisableStickyFriction)
            {
                StickyFrictionManager.Instance.RegisterDeathStickyFriction();
            }
        }
        else if (isPlayerCharacter)
        {
            StickyFrictionManager.Instance.RegisterPlayerDamageStickyFriction(damage);
        }
        else
        {
            StickyFrictionManager.Instance.RegisterOtherDamageStickyFriction(damage);
        }
    }
}