using UnityEngine;
using iShape.FixFloat;

namespace iShape.FixBox.Component {

    [CreateAssetMenu(fileName = "FixBoxMaterial", menuName = "FixBox/Material")]
    public class FixBoxMaterial: ScriptableObject, ISerializationCallbackReceiver {
        
        [Range(min:0, max:1)]
        public float Bounce = 0.5f;
        [Range(min:0, max:1)]
        public float Friction = 0.5f;
        [Range(min:0.1f, max:100)]
        public float Density = 1f;
        
        public long FixBounce = FixNumber.Half;
        public long FixFriction = FixNumber.Half;
        public long FixDensity = FixNumber.Unit;

        public Dynamic.Material Material => new (FixBounce, FixFriction, FixDensity);
        public void OnBeforeSerialize() {
            FixBounce = Bounce.ToFix();
            FixFriction = Friction.ToFix();
            FixDensity = Density.ToFix();
        }

        public void OnAfterDeserialize() { }
    }

}