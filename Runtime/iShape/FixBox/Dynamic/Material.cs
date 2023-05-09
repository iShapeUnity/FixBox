using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public readonly struct Material {

        public static readonly Material Ordinary = new Material(FixNumber.Half, FixNumber.Unit, FixNumber.Unit, FixNumber.Unit, FixNumber.Unit);
        
        public readonly long Bounce;
        public readonly long Friction;
        public readonly long Density;
        public readonly long AirLinearFriction;
        public readonly long AirAngularFriction;

        public Material(long bounce, long friction, long density, long airLinearFriction, long airAngularFriction) {
            Bounce = bounce;
            Friction = friction;
            Density = density;
            AirLinearFriction = airLinearFriction;
            AirAngularFriction = airAngularFriction;
        }
    }

}