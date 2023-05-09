using UnityEngine;
using iShape.FixFloat;

namespace iShape.FixBox.Component {

    [CreateAssetMenu(fileName = "FixBoxMaterial", menuName = "FixBox/Material")]
    public class FixBoxMaterial: ScriptableObject, ISerializationCallbackReceiver {
        
        [Range(min:0, max:1)]
        public float Bounce = 0.5f;
        
        [Range(min:0, max:1)]
        public float Friction = 0.5f;
        
        [Range(min:1, max:20)]
        public int Density = 1;
        
        [Range(min:0, max:24)]
        public long AirLinearFriction = 24;
        
        [Range(min:0, max:24)]
        public long AirAngularFriction = 24;
        
        public long FixBounce = FixNumber.Half;
        public long FixFriction = FixNumber.Half;
        public long FixDensity = FixNumber.Unit;
        public long FixAirLinearFriction = FixNumber.Unit;
        public long FixAirAngularFriction = FixNumber.Unit;
        
        public Dynamic.Material Material => new (FixBounce, FixFriction, FixDensity, FixAirLinearFriction, FixAirAngularFriction);
        public void OnBeforeSerialize() {
            FixBounce = Bounce.ToFix();
            FixFriction = Friction.ToFix();
            FixDensity = Density.ToFix();
            FixAirLinearFriction = 1000 + AirLinearFriction;
            FixAirAngularFriction = 1000 + AirAngularFriction;
        }

        public void OnAfterDeserialize() { }
    }

}