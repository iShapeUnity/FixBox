using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public readonly struct Size {
        public readonly long Width;
        public readonly long Height;

        public Size(long width, long height) {
            Width = width;
            Height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Area() {
            return Width.Mul(Height);
        }
    }

}