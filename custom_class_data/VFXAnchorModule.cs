using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.custom_class_data
{
    public class VFXAnchorModule : MonoBehaviour
    {
        public AIActor anchorAIActor;

        public Vector3 offset;

        private void Update()
        {
            base.gameObject.transform.position = anchorAIActor.specRigidbody.UnitBottomCenter.ToVector3ZUp() + offset;
        }
    }
}
