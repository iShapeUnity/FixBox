using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Collision {

    public enum ContactType {
        Outside,
        Inside,
        Collide
    }

    public readonly struct Contact {
        public static readonly Contact Outside = new Contact(FixVec.Zero, FixVec.Zero, 0, 0, ContactType.Outside);

        public readonly FixVec Point;
        public readonly FixVec Normal;
        public readonly long Penetration;
        public readonly int Count;
        public readonly ContactType Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact(FixVec point, FixVec normal, long penetration, int count, ContactType type) {
            Point = point;
            Normal = normal;
            Penetration = penetration;
            Type = type;
            Count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec Correction(bool isDirect) {
            if (isDirect) {
                return -Penetration * Normal;    
            } else {
                return Penetration * Normal;
            }
        }
        
        public override string ToString()
        {
            var cor = Correction(true);
            return $"Point{Point} Normal{Normal} Penetration: {Penetration} Correction{cor})";
        }
    }

}