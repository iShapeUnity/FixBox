using iShape.FixFloat;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxCircleCollider : MonoBehaviour, ISerializationCallbackReceiver {

        [Range(min: 0.1f, max: 10)]
        public float Radius = 0.5f;

        [HideInInspector]
        public long FixRadius = FixNumber.Half;
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, Radius);
        }

        public void OnBeforeSerialize() {
            FixRadius = Radius.ToFix();
        }

        public void OnAfterDeserialize() {}
    }

}

