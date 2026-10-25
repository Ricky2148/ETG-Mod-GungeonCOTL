using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.custom_class_data
{
    public class VFXAnchorOnPlayerModule : MonoBehaviour
    {
        public GameActor anchorPlayer;

        public Vector3 offset;

        private void LateUpdate()
        {
            if (anchorPlayer != null)
            {
                base.gameObject.transform.position = anchorPlayer.specRigidbody.UnitBottomCenter.ToVector3ZUp() + offset;
                base.gameObject.GetComponent<tk2dSprite>().UpdateZDepth();

                if (!anchorPlayer.healthHaver.IsAlive)
                {
                    Destroy(base.gameObject);
                }
            }
        }
    }
}
