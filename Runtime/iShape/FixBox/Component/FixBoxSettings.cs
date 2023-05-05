using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    [CreateAssetMenu(fileName = "FixBoxSettings", menuName = "FixBox/Settings")]
    public class FixBoxSettings: ScriptableObject, ISerializationCallbackReceiver {

        [Range(min: 8, max: 32)]
        public long TimeStep = 16;
        
        [Range(min: 1, max: 4)]
        public int BodyTimeScale = 2;
        public bool IsBulletVsBullet = false;
        public bool IsPlayerVsPlayer = false;
        [Range(min: 16, max: 64)]
        public int LandCapacity = 64;
        [Range(min: 1, max: 64)]
        public int PlayerCapacity = 32;
        [Range(min: 1, max: 64)]
        public int BulletCapacity = 64;
        [Range(min: 3, max: 6)]
        public int GridSpaceFactor = 4;
        [Range(min: 1, max: 10)]
        public float FreezeMargin;
        
        [HideInInspector]
        public long FixFreezeMargin;
        
        public WorldSettings Settings => new WorldSettings(
            TimeStep,
            BodyTimeScale,
            IsBulletVsBullet,
            IsPlayerVsPlayer,
            LandCapacity,
            PlayerCapacity,
            BulletCapacity,
            GridSpaceFactor,
            FixFreezeMargin
            );
        
        public void OnBeforeSerialize() {
            FixFreezeMargin = FreezeMargin.ToFix();
        }

        public void OnAfterDeserialize() { }
    }
    
    [CustomEditor(typeof(FixBoxSettings))]
    [CanEditMultipleObjects]
    public class FixBoxSettingsEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            var fixBoxSettings = target as FixBoxSettings;
            if (fixBoxSettings == null) {
                return;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label("Time Step: ", EditorStyles.boldLabel);
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            float timeStep = fixBoxSettings.TimeStep.ToFloat();
            GUILayout.Label(timeStep.ToString("F8"), EditorStyles.label);
            EditorGUILayout.Space();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label("Body Step: ", EditorStyles.boldLabel);
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            float bodyStep = (fixBoxSettings.TimeStep / fixBoxSettings.BodyTimeScale).ToFloat();
            GUILayout.Label(bodyStep.ToString("F8"), EditorStyles.label);
            EditorGUILayout.Space();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }

}