using iShape.FixFloat;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxRectCollider : MonoBehaviour, ISerializationCallbackReceiver {

        [SerializeField]
        public float Width = 1.0f;
        
        [SerializeField]
        public float Height = 1.0f;

        // [HideInInspector]
        public long FixWidth = FixNumber.Unit;

        // [HideInInspector]
        public long FixHeight = FixNumber.Unit;
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(Width, Height, 0));
        }
        
        public void OnBeforeSerialize() {
            FixWidth = Width.ToFix();
            FixHeight = Height.ToFix();
        }

        public void OnAfterDeserialize() {}
        
    }

}