using UnityEditor;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class ContextMenu: Editor {
        
        [MenuItem("FixBox/Create Circle Body")]
        public static void CreateCircleBody() {
            var gameObject = CreateBody("Circle");
            gameObject.AddComponent<FixBoxCircleCollider>();
        }
        
        [MenuItem("GameObject/FixBox/Create Circle Body", false, 0)]
        public static void CreateCircleBody(MenuCommand menuCommand) {
            CreateCircleBody();
        }
        
        [MenuItem("FixBox/Create Rect Body")]
        public static void CreateRectBody() {
            var gameObject = CreateBody("Rect");
            gameObject.AddComponent<FixBoxRectCollider>();
        }
        
        [MenuItem("GameObject/FixBox/Create Rect Body", false, 0)]
        public static void CreateRectBody(MenuCommand menuCommand) {
            CreateRectBody();
        }

        private static GameObject CreateBody(string name) {
            GameObject gameObject = new GameObject(name);
            GameObjectUtility.SetParentAndAlign(gameObject, null);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + name);

            Selection.activeObject = gameObject;

            gameObject.AddComponent<FixBoxBody>();

            return gameObject;
        }
    }

}