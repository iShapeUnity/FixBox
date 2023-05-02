using System.Runtime.CompilerServices;
using iShape.FixFloat;
using UnityEngine;

namespace iShape.FixBox.Collision {

    public enum ContactType {
        Outside,
        Inside,
        Collide
    }

    public readonly struct Contact {
        public static readonly Contact Outside = new Contact(FixVec.Zero, FixVec.Zero, 0, ContactType.Outside);

        public readonly FixVec Point;
        public readonly FixVec Normal;
        public readonly long Penetration;
        public readonly ContactType Type;
        public FixVec Correction => Penetration * Normal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact(FixVec point, FixVec normal, long penetration, ContactType type) {
            Point = point;
            Normal = normal;
            Penetration = penetration;
            Type = type;
        }


        public void Log() {
            var result = "Point(" + Point.x + "," + Point.y + ") Normal(" + Normal.x + "," + Normal.y + ") Penetration: " + Penetration + " Correction(" + Correction.x + "," + Correction.y + ")";
            Debug.Log(result);
        }
    }

}