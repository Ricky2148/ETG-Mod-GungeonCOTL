using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOLItems.custom_class_data
{
    public class RicochetProjectileModule : BraveBehaviour
    {
        public HashSet<AIActor> visited = new HashSet<AIActor>();
        public bool isRicocheting = false;
        public float ricochetDamageScale;
        public float ricochetSpeedScale;
        public int ricochetRange;

        public void Awake()
        {
            specRigidbody.OnRigidbodyCollision += HasntPierced;
            projectile.OnHitEnemy += OnHitEnemy;
        }

        public void HasntPierced(CollisionData data)
        {
            if (data == null) return;
            if (data.MyRigidbody == null) return;
            if (data.MyRigidbody.projectile != null)
            {
                data.MyRigidbody.projectile.m_hasPierced = false;
            }
        }

        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (!isRicocheting)
            {
                isRicocheting = true;
                proj.baseData.damage *= ricochetDamageScale;
                proj.baseData.speed *= ricochetSpeedScale;
                proj.baseData.force = 0f;
                proj.baseData.range = 999f;
                proj.UpdateSpeed();
            }

            if (enemy == null) return;
            AIActor targetEnemy = null;
            if (enemy.aiActor != null)
            {
                targetEnemy = enemy.aiActor;
            }
            else if (enemy.GetComponentInParent<AIActor>() != null)
            {
                targetEnemy = enemy.GetComponentInParent<AIActor>();
            }
            else
            {
                return;
            }

            if (targetEnemy != null)
            {
                var ricochetModule = proj.GetComponent<RicochetProjectileModule>();
                if (proj.GetComponent<PierceProjModifier>() != null && proj.GetComponent<PierceProjModifier>().penetration > 0)
                {
                    //var dir = UnityEngine.Random.insideUnitCircle;
                    if (targetEnemy.ParentRoom != null && targetEnemy.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
                        /*var t = enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != enemy.aiActor && x.HasBeenEngaged && x.healthHaver != null && x.healthHaver.IsVulnerable);
                        if (t.Count > 0)
                        {
                            dir = BraveUtility.RandomElement(t.ToArray()).CenterPosition - proj.specRigidbody.UnitCenter;
                        }*/

                        AIActor closest = null;
                        float closestDistSq = ricochetRange * ricochetRange;
                        var t = targetEnemy.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != targetEnemy && x.HasBeenEngaged && x.healthHaver != null && x.healthHaver.IsVulnerable);

                        foreach (AIActor target in t)
                        {
                            if (proj.GetComponent<RicochetProjectileModule>().visited.Contains(target) || !target.IsNormalEnemy)
                                continue;

                            float distSq = (target.CenterPosition - targetEnemy.CenterPosition).sqrMagnitude;
                            if (distSq < closestDistSq)
                            {
                                closest = target;
                                closestDistSq = distSq;
                            }
                        }

                        if (closest != null)
                        {
                            var dir = closest.CenterPosition - proj.specRigidbody.UnitCenter;
                            proj.SendInDirection(dir, false);
                            ricochetModule.visited.Add(enemy.aiActor);
                            //Plugin.Log($"Crossblade ricochet to {closest.GetActorName()}");
                        }
                        else
                        {
                            //Plugin.Log($"Crossblade found no ricochet targets");
                            proj.ForceDestruction();
                        }
                    }
                    //proj.SendInDirection(dir, false);
                }
            }
        }
    }
}