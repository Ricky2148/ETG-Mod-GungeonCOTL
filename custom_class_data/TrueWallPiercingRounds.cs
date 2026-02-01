using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.custom_class_data
{
    public class TrueWallPiercingRounds : MonoBehaviour
    {
        private PlayerController projOwner;
        private Projectile m_projectile;
        public bool isInWall = false;

        public TrueWallPiercingRounds()
        {

        }

        private void Start()
        {
            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            SpeculativeRigidbody specRigidBody = this.m_projectile.specRigidbody;
            this.m_projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
            this.m_projectile.BulletScriptSettings.surviveTileCollisions = true;
            this.m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisionTile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
        }

        private void OnPreCollisionTile(SpeculativeRigidbody myRigidBody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            isInWall = true;
        }

        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (!otherRigidbody.aiActor)
            {
                if (otherRigidbody.minorBreakable == false && otherRigidbody.projectile == false)
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }

        private void Update()
        {
            /*if (isInWall == true)
            {
                if (m_projectile.specRigidbody.HasTriggerCollisions == false)
                {
                    isInWall = false;
                }
            }
            else
            {
                if (m_projectile.LastPosition.GetAbsoluteRoom() != null)
                {
                    if (m_projectile.LastPosition.GetAbsoluteRoom() != this.projOwner.CurrentRoom)
                    {
                        m_projectile.DieInAir();
                    }
                }
            }*/
            keepTrackOfInWallState();
        }

        private void keepTrackOfInWallState()
        {
            if (isInWall == true)
            {
                if (m_projectile.specRigidbody.HasTriggerCollisions == false)
                {
                    isInWall = false;
                }
            }
            else
            {
                if (m_projectile.LastPosition.GetAbsoluteRoom() != null)
                {
                    if (m_projectile.LastPosition.GetAbsoluteRoom() != this.projOwner.CurrentRoom)
                    {
                        //Plugin.Log("wrong room");
                        m_projectile.DieInAir();
                    }
                }
            }
        }
    }
}
