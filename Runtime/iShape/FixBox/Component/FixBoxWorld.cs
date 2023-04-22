using iShape.FixBox.Collider;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxWorld: MonoBehaviour {

        public static FixBoxWorld Shared;

        public bool IsDebug = true;
        public FixBoxSettings Settings;
        
        [SerializeField, HideInInspector]
        internal float Width = 50.0f;
        
        [SerializeField, HideInInspector]
        internal float Height = 50.0f;
        
        [SerializeField, HideInInspector]
        internal float FreezeMargin = 5.0f;

        [SerializeField, HideInInspector]
        public long FixWidth = 50 * FixNumber.Unit;

        [SerializeField, HideInInspector]
        public long FixHeight = 50 * FixNumber.Unit;
        
        [SerializeField, HideInInspector]
        public long FixFreezeMargin = 5 * FixNumber.Unit;

        public World World;
        
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
        
        public void SetFreezeMargin(float value)
        {
            FreezeMargin = value;
            FixFreezeMargin = value.ToFix();
        }
        
        private void Awake() {
            var a = FixWidth >> 1;
            var b = FixHeight >> 1;
            var Boundary = new Boundary(new FixVec(-a, -b), new FixVec(a, b));
            
            World = new World(Boundary, Settings.Settings, IsDebug, Allocator.Persistent);
            Shared = this;
        }

        private void OnDestroy() {
            Shared = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            var a = FixWidth.ToFloat();
            var b = FixHeight.ToFloat();
            var m = 2 * FixFreezeMargin.ToFloat();

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(a, b, 0));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(a + m, b + m, 0));
        }

        private void Update() {
            World.Iterate();
        }
    }
    
    [CustomEditor(typeof(FixBoxWorld))]
    public class FixBoxWorldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var world = target as FixBoxWorld;
            if (world == null) {
                return;
            }

            EditorGUILayout.Space();
            GUILayout.Label("Boundary", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();

            var newWidth = EditorGUILayout.Slider("Width", world.Width, 10f, 1000f);
            world.SetWidth(newWidth);
            
            var newHeight = EditorGUILayout.Slider("Height", world.Height, 10f, 1000f);
            world.SetHeight(newHeight);
            
            var newFreezeMargin = EditorGUILayout.Slider("FreezeMargin", world.FreezeMargin, 1f, 10f);
            world.SetFreezeMargin(newFreezeMargin);
            
            EditorUtility.SetDirty(target);
            EditorGUI.EndChangeCheck();
        }
    }

}