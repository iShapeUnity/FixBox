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
        public Velocity Apply(long timeStep, Acceleration acceleration) {
            var dA = acceleration.Linear * timeStep;
            var dWa = acceleration.Angular * timeStep;
            return new Velocity(Linear + dA, Angular + dWa); 
        }
        
        public override string ToString()
        {
            return $"Linear: {Linear} Angular: {Angular}";
        }
    }

}