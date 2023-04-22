using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public struct Velocity {
        public static readonly Velocity Zero = new Velocity(FixVec.Zero);
    
        public FixVec Linear;
        public long Angular;
    
        public bool isZero => Linear is { x: 0, y: 0 } && Angular == 0;

        public Velocity(FixVec linear, long angular = 0) {
            Linear = linear;
            Angular = angular;
        }
    }

}