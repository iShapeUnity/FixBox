using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Collision {

    public enum ContactType {
        Outside,
        Inside,
        Collide
    }

    public struct Contact {
        public static readonly Contact Outside = new Contact(FixVec.Zero, FixVec.Zero, 0, ContactType.Outside);

        public readonly FixVec Point;
        public readonly FixVec Normal;
        public readonly long Penetration;
        public readonly ContactType Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact(FixVec point, FixVec normal, long penetration, ContactType type) {
            Point = point;
            Normal = normal;
            Penetration = penetration;
            Type = type;
        }

    }

}