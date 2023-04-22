using UnityEngine;
using iShape.FixFloat;

namespace iShape.FixBox.Component {

    [CreateAssetMenu(fileName = "FixBoxMaterial", menuName = "FixBox/Material")]
    public class FixBoxMaterial: ScriptableObject {
        
        [Range(min:0, max:1)]
        public float Bounce = 0.5f;
        [Range(min:0, max:1)]
        public float Friction = 0.5f;
        [Range(min:0.1f, max:100)]
        public float Density = 1f;

        public Dynamic.Material Material => new Dynamic.Material(Bounce.ToFix(), Friction.ToFix(), Density.ToFix());
    }

}