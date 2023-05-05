using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public readonly struct Velocity {
        
        public static readonly Velocity Zero = new(FixVec.Zero);
    
        public readonly FixVec Linear;
        public readonly long Angular;
    
        public bool isZero => Linear is { x: 0, y: 0 } && Angular == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Velocity(FixVec linear, long angular = 0) {
            Linear = linear;
            Angular = angular;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Velocity Apply(long timeStep, FixVec Gravity) {
            var dA = Gravity * timeStep;
            return new Velocity(dA + Linear, Angular); 
        }
        
        public override string ToString()
        {
            return $"Linear: {Linear} Angular: {Angular}";
        }
    }

}