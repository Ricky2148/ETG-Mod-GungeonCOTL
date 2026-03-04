using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.custom_class_data
{
    public class VFXAnchorOnGunModule : MonoBehaviour
    {
        public Gun gun;

        public Vector3 offset;

        private void Update()
        {
            if (gun != null)
            {
                base.gameObject.transform.position = gun.sprite.WorldCenter.ToVector3ZUp() + offset;
                base.gameObject.GetComponent<tk2dSprite>().UpdateZDepth();

                if (gun.CurrentOwner == null)
                {
                    //Plugin.Log($"deleted lol {gun.CurrentOwner}");
                    Destroy(base.gameObject);
                }
            }
        }
    }
}
