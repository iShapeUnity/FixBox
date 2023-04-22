using iShape.FixFloat;
using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxCircleCollider : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        internal float Radius = 0.5f;

        [SerializeField, HideInInspector]
        public long FixRadius = FixNumber.Half;

        public void SetRadius(float value)
        {
            Radius = value;
            FixRadius = value.ToFix();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, Radius);
        }
    }
    
    [CustomEditor(typeof(FixBoxCircleCollider))]
    public class FixBoxCircleColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            var collider = target as FixBoxCircleCollider;
            if (collider == null) {
                return;
            }
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.Space();
            GUILayout.Label("Circle", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var newRadius = EditorGUILayout.Slider("Radius", collider.Radius, 0.1f, 10f);

            collider.SetRadius(newRadius);
            EditorUtility.SetDirty(target);
            EditorGUI.EndChangeCheck();
        }
    }

}

