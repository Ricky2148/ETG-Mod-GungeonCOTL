using Alexandria;
using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// some vfx and sfx work

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
            if (hitRigidbody != null || hitRigidbody.aiActor != null && hitRigidbody.healthHaver.IsAlive)
            {
                float dmgToStore = beam.Gun.DefaultModule.projectiles[0].baseData.damage * TheBombDmgScale * tickrate;
                if (hitRigidbody.healthHaver.IsBoss || hitRigidbody.healthHaver.IsSubboss)
                {
                    dmgToStore *= 0.25f;
                }
                if (!enemyTheBombDmgStored.ContainsKey(hitRigidbody.aiActor))
                {
                    enemyTheBombDmgStored.Add(hitRigidbody.aiActor, dmgToStore);
                    enemyTheBombCoroutine.Add(hitRigidbody.aiActor, null);
                }
                else
                {
                    enemyTheBombDmgStored[hitRigidbody.aiActor] += dmgToStore;
                }

                //Plugin.Log($"enemyTheBombDmgStored: {enemyTheBombDmgStored[hitRigidbody.aiActor]}, enemy hp: {hitRigidbody.aiActor.healthHaver.GetCurrentHealth()}");

                // if the hit enemy's stack count is at max stacks, trigger charm effect and cooldown
                if (enemyTheBombDmgStored[hitRigidbody.aiActor] >= hitRigidbody.aiActor.healthHaver.GetCurrentHealth() && hitRigidbody.aiActor.healthHaver.GetCurrentHealth() != 0)
                {
                    DetonateTheBomb(hitRigidbody.aiActor);

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
                    if (enemyTheBombCoroutine[hitRigidbody.aiActor] != null)
                    {
                        StopCoroutine(enemyTheBombCoroutine[hitRigidbody.aiActor]);
                    }
                    enemyTheBombCoroutine[hitRigidbody.aiActor] = StartCoroutine(TheBombCooldown(hitRigidbody.aiActor));
                }
            }
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Shooter == proj.Owner.specRigidbody)
            {
                proj.OnHitEnemy += (projHit, enemy, fatal) =>
                {
                    if (enemy != null || enemy.aiActor != null && enemy.healthHaver.IsAlive)
                    {
                        float dmgToStore = projHit.baseData.damage * TheBombDmgScale;
                        if (enemy.healthHaver.IsBoss || enemy.healthHaver.IsSubboss)
                        {
                            dmgToStore *= 0.25f;
                        }
                        if (!enemyTheBombDmgStored.ContainsKey(enemy.aiActor))
                        {
                            enemyTheBombDmgStored.Add(enemy.aiActor, dmgToStore);
                            enemyTheBombCoroutine.Add(enemy.aiActor, null);
                        }
                        else
                        {
                            enemyTheBombDmgStored[enemy.aiActor] += dmgToStore;
                        }

                        //Plugin.Log($"enemyTheBombDmgStored: {enemyTheBombDmgStored[enemy.aiActor]}, enemy hp: {enemy.aiActor.healthHaver.GetCurrentHealth()}");
                    
                        // if the hit enemy's stack count is at max stacks, trigger charm effect and cooldown
                        if (enemyTheBombDmgStored[enemy.aiActor] >= enemy.aiActor.healthHaver.GetCurrentHealth() && enemy.aiActor.healthHaver.GetCurrentHealth() != 0)
                        {
                            DetonateTheBomb(enemy.aiActor);

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
                            if (enemyTheBombCoroutine[enemy.aiActor] != null)
                            {
                                StopCoroutine(enemyTheBombCoroutine[enemy.aiActor]);
                            }
                            enemyTheBombCoroutine[enemy.aiActor] = StartCoroutine(TheBombCooldown(enemy.aiActor));
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
