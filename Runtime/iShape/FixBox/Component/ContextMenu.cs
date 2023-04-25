using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class ContextMenu: Editor {
        
        [MenuItem("FixBox/Create World")]
        public static void CreateWorld() {
            var existed = FindObjectOfType<FixBoxWorld>();
            if (existed != null) {
                Debug.LogWarning("FixBoxWorld is already present");
                EditorGUIUtility.PingObject(existed);
                SceneView.lastActiveSceneView.FrameSelected();
                EditorUtility.SetDirty(existed);
            } else {
                GameObject gameObject = new GameObject("FixBoxWorld");
                GameObjectUtility.SetParentAndAlign(gameObject, null);
                Undo.RegisterCreatedObjectUndo(gameObject, "Create FixBoxWorld");

                if (Selection.activeObject != null &&  Selection.activeObject is GameObject) {
                    gameObject.transform.parent = (Selection.activeObject as GameObject)?.transform;
                }

                Selection.activeObject = gameObject;

                gameObject.AddComponent<FixBoxWorld>();                
            }
        }
        
        [MenuItem("GameObject/FixBox/Create World", false, 0)]
        public static void CreateWorld(MenuCommand menuCommand) {
            CreateWorld();
        }
        

    }

}