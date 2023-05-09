using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public readonly struct Acceleration {

        public static readonly Acceleration Zero = new(FixVec.Zero);

        public readonly FixVec Linear;
        public readonly long Angular;

        public bool isZero => Linear is { x: 0, y: 0 } && Angular == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Acceleration(FixVec linear, long angular = 0) {
            Linear = linear;
            Angular = angular;
        }
    
        public override string ToString()
        {
            return $"Linear: {Linear} Angular: {Angular}";
        }
        
    }

}