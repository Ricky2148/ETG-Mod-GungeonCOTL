using Alexandria;
using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// needs vfx and sfx work
// tune the damage scale on enemies and bosses
// vfx should be a white spark ball looking thing, on effect application, the vfx is thrown above the enemies head, it starts small, then grows larger with more damage
// maybe start changing colors, would want it to instantly change to red when it would detonate to execute the target. Every time you refresh the duration, it just increases the duration without refreshing the vfx or vfx's loop

namespace LOLItems.passive_items
{
    internal class DetonationOrb : PassiveItem
    {
        private static float DamageStat = 1.2f;
        private static float TheBombDmgScale = 0.25f;
        private static float TheBombDuration = 3f;

        private Dictionary<AIActor, float> enemyTheBombDmgStored = new Dictionary<AIActor, float>();

        private Dictionary<AIActor, Coroutine> enemyTheBombCoroutine = new Dictionary<AIActor, Coroutine>();

        public static int ID;

        public static void Init()
        {
            string itemName = "Detonation Orb";
            string resourceName = "LOLItems/Resources/passive_item_sprites/detonation_orb_pixelart_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DetonationOrb>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, DamageStat, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
            player.PostProcessProjectile += OnPostProcessProjectile;
            player.PostProcessBeamTick += OnPostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");
            player.PostProcessProjectile -= OnPostProcessProjectile;
            player.PostProcessBeamTick -= OnPostProcessProjectile;

            enemyTheBombDmgStored.Clear();
            enemyTheBombCoroutine.Clear();
        }

        private void OnPostProcessProjectile(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickrate)
        {
            if (hitRigidbody == null) return;
            AIActor target = null;
            if (hitRigidbody.aiActor != null)
            {
                target = hitRigidbody.aiActor;
                //Plugin.Log($"enemy.aiActor: {target}");
            }
            else if (hitRigidbody.GetComponentInParent<AIActor>() != null)
            {
                target = hitRigidbody.GetComponentInParent<AIActor>();
                //Plugin.Log($"enemy.parentActor: {target}");
            }
            else
            {
                //Plugin.Log("target = null");
                return;
            }
            if (hitRigidbody.healthHaver.IsAlive)
            {
                float dmgToStore = beam.Gun.DefaultModule.projectiles[0].baseData.damage * TheBombDmgScale * tickrate;
                if (hitRigidbody.healthHaver.IsBoss || hitRigidbody.healthHaver.IsSubboss)
                {
                    dmgToStore *= 0.25f;
                }
                if (!enemyTheBombDmgStored.ContainsKey(target))
                {
                    enemyTheBombDmgStored.Add(target, dmgToStore);
                    enemyTheBombCoroutine.Add(target, null);
                }
                else
                {
                    enemyTheBombDmgStored[target] += dmgToStore;
                }

                //Plugin.Log($"enemyTheBombDmgStored: {enemyTheBombDmgStored[hitRigidbody.aiActor]}, enemy hp: {hitRigidbody.aiActor.healthHaver.GetCurrentHealth()}");

                // if the hit enemy's stack count is at max stacks, trigger charm effect and cooldown
                if (enemyTheBombDmgStored[target] >= target.healthHaver.GetCurrentHealth() && target.healthHaver.GetCurrentHealth() != 0)
                {
                    DetonateTheBomb(target);

                    /*enemy.aiActor.healthHaver.ApplyDamage(
                        enemyTheBombDmgStored[enemy.aiActor],
                        Vector2.zero,
                        "the_bomb_detonation_damage",
                        CoreDamageTypes.None,
                        DamageCategory.Normal,
                        ignoreInvulnerabilityFrames: true,
                        hitPixelCollider: null,
                        ignoreDamageCaps: true
                    );

                    enemyTheBombDmgStored.Remove(enemy.aiActor);
                    */
                }
                else
                {
                    if (enemyTheBombCoroutine[target] != null)
                    {
                        StopCoroutine(enemyTheBombCoroutine[target]);
                    }
                    enemyTheBombCoroutine[target] = StartCoroutine(TheBombCooldown(target));
                }
            }
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Shooter == proj.Owner.specRigidbody)
            {
                proj.OnHitEnemy += (projHit, enemy, fatal) =>
                {
                    //Plugin.Log($"enemy curhealth: {enemy.healthHaver.GetCurrentHealth()}, enemy isAlive: {enemy.healthHaver.IsAlive}");

                    if (enemy == null) return;
                    AIActor target = null;
                    if (enemy.aiActor != null)
                    {
                        target = enemy.aiActor;
                        //Plugin.Log($"enemy.aiActor: {target}");
                    }
                    else if (enemy.GetComponentInParent<AIActor>() != null)
                    {
                        target = enemy.GetComponentInParent<AIActor>();
                        //Plugin.Log($"enemy.parentActor: {target}");
                    }
                    else
                    {
                        //Plugin.Log("target = null");
                        return;
                    }
                    if (enemy.healthHaver.IsAlive)
                    {
                        float dmgToStore = projHit.baseData.damage * TheBombDmgScale;
                        if (enemy.healthHaver.IsBoss || enemy.healthHaver.IsSubboss)
                        {
                            dmgToStore *= 0.25f;
                        }
                        if (!enemyTheBombDmgStored.ContainsKey(target))
                        {
                            enemyTheBombDmgStored.Add(target, dmgToStore);
                            enemyTheBombCoroutine.Add(target, null);
                        }
                        else
                        {
                            enemyTheBombDmgStored[target] += dmgToStore;
                        }

                        //Plugin.Log($"enemyTheBombDmgStored: {enemyTheBombDmgStored[target]}, enemy hp: {target.healthHaver.GetCurrentHealth()}");

                        if (enemyTheBombDmgStored[target] >= target.healthHaver.GetCurrentHealth() && target.healthHaver.GetCurrentHealth() != 0)
                        {
                            DetonateTheBomb(target);

                            /*enemy.aiActor.healthHaver.ApplyDamage(
                                enemyTheBombDmgStored[enemy.aiActor],
                                Vector2.zero,
                                "the_bomb_detonation_damage",
                                CoreDamageTypes.None,
                                DamageCategory.Normal,
                                ignoreInvulnerabilityFrames: true,
                                hitPixelCollider: null,
                                ignoreDamageCaps: true
                            );

                            enemyTheBombDmgStored.Remove(enemy.aiActor);
                            */
                        }
                        else
                        {
                            if (enemyTheBombCoroutine[target] != null)
                            {
                                StopCoroutine(enemyTheBombCoroutine[target]);
                            }
                            enemyTheBombCoroutine[target] = StartCoroutine(TheBombCooldown(target));
                        }
                    }
                };
            }
        }

        private System.Collections.IEnumerator TheBombCooldown(AIActor enemyActor) 
        {
            //Plugin.Log("bomb cooldown start");

            yield return new WaitForSeconds(TheBombDuration);

            DetonateTheBomb(enemyActor);

            //StopCoroutine(enemyTheBombCoroutine[enemyActor]);
        }

        private void DetonateTheBomb(AIActor enemyActor)
        {
            //Plugin.Log("bomb detonated");

            enemyActor.healthHaver.ApplyDamage(
                enemyTheBombDmgStored[enemyActor],
                Vector2.zero,
                "the_bomb_detonation_damage",
                CoreDamageTypes.None,
                DamageCategory.Normal,
                ignoreInvulnerabilityFrames: true,
                hitPixelCollider: null,
                ignoreDamageCaps: true
            );

            StopCoroutine(enemyTheBombCoroutine[enemyActor]);
            enemyTheBombCoroutine.Remove(enemyActor);
            enemyTheBombDmgStored.Remove(enemyActor);

            //Plugin.Log($"dmg storage: {enemyTheBombDmgStored}, coroutine storage: {enemyTheBombCoroutine}");
        }
    }
}
