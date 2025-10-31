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

        public void Awake()
        {
            specRigidbody.OnRigidbodyCollision += HasntPierced;
            projectile.OnHitEnemy += OnHitEnemy;
        }

        public void HasntPierced(CollisionData data)
        {
            if (data != null && data.MyRigidbody != null && data.MyRigidbody.projectile != null)
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
            }
        }
    }
}