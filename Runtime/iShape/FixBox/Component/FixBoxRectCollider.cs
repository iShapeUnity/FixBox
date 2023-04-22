using iShape.FixFloat;
using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxRectCollider : MonoBehaviour {

        [SerializeField, HideInInspector]
        internal float Width = 1.0f;
        
        [SerializeField, HideInInspector]
        internal float Height = 1.0f;

        [SerializeField, HideInInspector]
        public long FixWidth = FixNumber.Unit;

        [SerializeField, HideInInspector]
        public long FixHeight = FixNumber.Unit;
        
        public void SetWidth(float value)
        {
            Width = value;
            FixWidth = value.ToFix();
        }
        
        public void SetHeight(float value)
        {
            Height = value;
            FixHeight = value.ToFix();
        }
    }

    [CustomEditor(typeof(FixBoxRectCollider))]
    public class FixBoxRectColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var collider = target as FixBoxRectCollider;
            if (collider == null) {
                return;
            }

            EditorGUILayout.Space();
            GUILayout.Label("Rectangle", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();

            var newWidth = EditorGUILayout.Slider("Width", collider.Width, 0.1f, 100f);
            collider.SetWidth(newWidth);
            
            var newHeight = EditorGUILayout.Slider("Height", collider.Height, 0.1f, 100f);
            collider.SetHeight(newHeight);
            
            EditorUtility.SetDirty(target);
            EditorGUI.EndChangeCheck();
        }
    }

}