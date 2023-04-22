using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public readonly struct Material {

        public static readonly Material Ordinary = new Material(FixNumber.Half, FixNumber.Unit, FixNumber.Unit);
        
        public readonly long Bounce;
        public readonly long Friction;
        public readonly long Density;

        public Material(long bounce, long friction, long density) {
            Bounce = bounce;
            Friction = friction;
            Density = density;
        }
    }

}