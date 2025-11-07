using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static HealthHaver;

//health, active targets a circle zone that applies a heal to players and dmg to enemies, the effect casts down after a delay
// NOT COMPLETE

namespace LOLItems
{
    internal class Redemption : TargetedAttackPlayerItem
    {
        private static float HealthStat = 1f;

        private static float InterventionHealAmount = 0.5f;
        private static float InterventionDamageScale = 0.1f;
        private static float InterventionActivationRange = 10f;
        private static float InterventionEffectRadius = 10f;
        private static float InterventionCooldown = 1f; //90f

        private static List<string> VFXSpritePath = new List<string>
            {
                "LOLItems/Resources/vfxs/test_vfx/image (1)",
                "LOLItems/Resources/vfxs/test_vfx/image (2)",
                "LOLItems/Resources/vfxs/test_vfx/image (3)",
                "LOLItems/Resources/vfxs/test_vfx/image (4)",
                "LOLItems/Resources/vfxs/test_vfx/image (5)",
                "LOLItems/Resources/vfxs/test_vfx/image (6)",
                "LOLItems/Resources/vfxs/test_vfx/image (7)",
                "LOLItems/Resources/vfxs/test_vfx/image (8)",
                "LOLItems/Resources/vfxs/test_vfx/image (9)",
            };

        private static GameObject EffectVFX = VFXBuilder.CreateVFX
        (
            "intervention_vfx",
            VFXSpritePath,
            30,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Loop,
            true
        );

        private GameObject activeVFXObject;

        public static int ID;

        public static void Init()
        {
            string itemName = "Redemption";
            string resourceName = "LOLItems/Resources/active_item_sprites/redemption_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Redemption>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Break their stride.";
            string longDesc = "Increases health, damage, and fire rate. Active attacks in a circle around the player, slowing enemies hit and dealing set damage.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, InterventionCooldown);
            item.consumable = false;

            item.minDistance = 0f;
            item.maxDistance = InterventionActivationRange;

            item.reticleQuad = (PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).reticleQuad;
            item.doesStrike = false;
            item.doesGoop = false;
            item.DoScreenFlash = false;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, HealthStat, StatModifier.ModifyMethod.ADDITIVE);

            item.quality = PickupObject.ItemQuality.A;
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

        /*public override void DoEffect(PlayerController player)
        {
            IsCurrentlyActive = true;
            m_currentUser = player;
            GameObject reticleObject = (PickupObjectDatabase.GetById(462) as TargetedAttackPlayerItem).reticleQuad;
            m_extantReticleQuad = reticleObject.GetComponent<tk2dBaseSprite>();
            m_currentAngle = BraveMathCollege.Atan2Degrees(m_currentUser.unadjustedAimPoint.XY() - m_currentUser.CenterPosition);
            m_currentDistance = 5f;
            UpdateReticlePosition();
            spriteAnimator.Play("Activate");
        }*/

        public override void DoActiveEffect(PlayerController player)
        {
            /*if (user && user.CurrentRoom != null)
            {
                Vector2 cachedPosition = m_extantReticleQuad.gameObject.transform.position + new Vector3(0, 0.25f);
                if (m_extantReticleQuad) { Destroy(m_extantReticleQuad.gameObject); }
                IsCurrentlyActive = true;
                AkSoundEngine.PostEvent("Play_OBJ_computer_boop_01", user.gameObject);

                Exploder.Explode(cachedPosition, new ExplosionData
                {
                    damage = 40f,
                    damageRadius = 5f,
                    doDamage = true,
                    doForce = true,
                    force = 25f,
                    debrisForce = 10f,
                    preventPlayerForce = true,
                    doScreenShake = true,
                    playDefaultSFX = true
                }, Vector2.zero);

                IsCurrentlyActive = false;
                user.DropActiveItem(this);
            }*/

            tk2dBaseSprite cursor = this.m_extantReticleQuad;
            Vector2 overridePos = cursor.WorldCenter; //this sets the vector2 to the bottom left of the reticle sprite, not to the actual cursor

            //Plugin.Log("before active effect");

            base.DoActiveEffect(player);

            Plugin.Log("after active effect");

            StartCoroutine(DoInterventionEvent(player, overridePos));

            /*Exploder.Explode(overridePos, new ExplosionData
            {
                damageRadius = 3f,
                damageToPlayer = 0f,
                doDamage = true,
                damage = 20f,
                doDestroyProjectiles = true,
                doForce = true,
                debrisForce = 0f,
                preventPlayerForce = true,
                explosionDelay = 0f,
                usesComprehensiveDelay = false,
                doScreenShake = false,
                playDefaultSFX = true,
                breakSecretWalls = true,
                secretWallsRadius = 3,
                force = 2,
                forceUseThisRadius = true
            }, Vector2.zero, null, false, CoreDamageTypes.None, false);*/

            /*foreach (AIActor enemy in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
                {
                    float distance = Vector2.Distance(cursor.WorldCenter, enemy.CenterPosition);
                    // scale damage with player damage modifiers
                    //float ShockwaveDamage = ShockwaveBaseDamage * player.stats.GetStatValue(PlayerStats.StatType.Damage);

                    Plugin.Log($"cursor: {cursor.WorldCenter.ToString()}, enemy: {enemy.CenterPosition.ToString()}, distance: {distance}");

                    if (distance <= 6f)
                    {
                        float damageToDeal = enemy.healthHaver.GetMaxHealth() * InterventionDamageScale;

                        enemy.healthHaver.ApplyDamage(
                            damageToDeal,
                            Vector2.zero,
                            "Stridebreaker",
                            CoreDamageTypes.None,
                            DamageCategory.Normal,
                            false
                        );
                        //enemy.ApplyEffect(slowEffect, 1f, null);
                        //AkSoundEngine.PostEvent("stridebreaker_active_hit_SFX", player.gameObject);
                    }
                }
            }*/
        }

        private System.Collections.IEnumerator DoInterventionEvent(PlayerController player, Vector2 cursorPos)
        {
            Vector2 initialCursorPos = cursorPos;

            activeVFXObject = UnityEngine.Object.Instantiate(EffectVFX, cursorPos, Quaternion.identity);

            var sprite = activeVFXObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -50f; //-50f

                //sprite.scale = new Vector3(3.1f, 3.1f, 1f);

                sprite.UpdateZDepth();

                //sprite.usesOverrideMaterial = true;

                //sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                //sprite.renderer.material.SetFloat("_Fade", 0.8f);
            }

            yield return new WaitForSeconds(2.5f);

            if (activeVFXObject != null)
            {
                Destroy(activeVFXObject);
            }

            foreach (AIActor enemy in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if (enemy != null && enemy.healthHaver != null && enemy.healthHaver.IsVulnerable)
                {
                    float distance = Vector2.Distance(initialCursorPos, enemy.CenterPosition);
                    // scale damage with player damage modifiers
                    //float ShockwaveDamage = ShockwaveBaseDamage * player.stats.GetStatValue(PlayerStats.StatType.Damage);

                    Plugin.Log($"cursor: {initialCursorPos.ToString()}, enemy: {enemy.CenterPosition.ToString()}, distance: {distance}");

                    if (distance <= InterventionEffectRadius)
                    {
                        float damageToDeal = enemy.healthHaver.GetMaxHealth() * InterventionDamageScale;

                        enemy.healthHaver.ApplyDamage(
                            damageToDeal,
                            Vector2.zero,
                            "Stridebreaker",
                            CoreDamageTypes.None,
                            DamageCategory.Normal,
                            false
                        );
                        //enemy.ApplyEffect(slowEffect, 1f, null);
                        //AkSoundEngine.PostEvent("stridebreaker_active_hit_SFX", player.gameObject);
                    }
                }
            }

            if (Vector2.Distance(initialCursorPos, player.CenterPosition) <= InterventionEffectRadius)
            {
                if (player.healthHaver.isPlayerCharacter && player.healthHaver.currentHealth < player.healthHaver.GetMaxHealth())
                {
                    player.healthHaver.ForceSetCurrentHealth(player.healthHaver.currentHealth + InterventionHealAmount);
                }
            }
        }
    }
}
