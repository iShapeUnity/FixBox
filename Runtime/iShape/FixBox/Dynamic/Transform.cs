using System;
using System.Runtime.CompilerServices;
using iShape.FixBox.Collider;
using iShape.FixBox.Collision;
using iShape.FixFloat;
using UnityEngine;

namespace iShape.FixBox.Dynamic {

    public readonly struct Transform {
        
        public static readonly Transform Zero = new Transform(position: FixVec.Zero, angle: 0);
        public readonly FixVec Position;
        public readonly long Angle;
        public readonly FixVec Rotator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform(FixVec position, long angle) {
            Position = position;
            Angle = angle;
            Rotator = angle.RadToFixAngle().Rotator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Transform(FixVec position, long angle, FixVec rotator) {
            Position = position;
            Angle = angle;
            Rotator = rotator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ConvertAsPoint(FixVec point) {
            return ConvertAsVector(point) + Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ConvertAsVector(FixVec vector) {
            long x = (Rotator.x * vector.x - Rotator.y * vector.y) >> 10;
            long y = (Rotator.y * vector.x + Rotator.x * vector.y) >> 10;
            return new FixVec(x, y);
        }

        public Boundary Convert(Boundary boundary) {
            if (Angle == 0) {
                return boundary.Translate(delta: Position);
            } else {
                FixVec a0 = boundary.Min;
                FixVec a1 = new FixVec(boundary.Min.x, boundary.Max.y);
                FixVec a2 = boundary.Max;
                FixVec a3 = new FixVec(boundary.Max.x, boundary.Min.y);

                FixVec b0 = ConvertAsPoint(a0);
                FixVec b1 = ConvertAsPoint(a1);
                FixVec b2 = ConvertAsPoint(a2);
                FixVec b3 = ConvertAsPoint(a3);

                long minX = Math.Min(Math.Min(b0.x, b1.x), Math.Min(b2.x, b3.x));
                long minY = Math.Min(Math.Min(b0.y, b1.y), Math.Min(b2.y, b3.y));

                long maxX = Math.Max(Math.Max(b0.x, b1.x), Math.Max(b2.x, b3.x));
                long maxY = Math.Max(Math.Max(b0.y, b1.y), Math.Max(b2.y, b3.y));

                return new Boundary(new FixVec(minX, minY), new FixVec(maxX, maxY));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact Convert(Contact contact) {
            FixVec point = ConvertAsPoint(contact.Point);
            FixVec normal = ConvertAsVector(contact.Normal);

            return new Contact(point, normal, contact.Penetration, contact.Count, contact.Type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform Apply(Velocity v, int timeScale) {
            FixVec dv = v.Linear.DivTwo(timeScale);
            FixVec p = Position + dv;

            if (v.Angular != 0) {
                long a = Angle + (v.Angular >> timeScale);
                return new Transform(p, a);
            } else {
                return new Transform(p, Angle, Rotator);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform Apply(FixVec delta) {
            return new Transform(Position + delta, Angle, Rotator);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ConvertFromBtoA(Transform b, Transform a) {
            var ang = b.Angle - a.Angle;
            var rot = ang.RadToFixAngle().Rotator();

            var cosA = a.Rotator.x;
            var sinA = a.Rotator.y;

            var dv = b.Position - a.Position;

            var x = (cosA * dv.x + sinA * dv.y) >> 10;
            var y = (cosA * dv.y - sinA * dv.x) >> 10;

            return new Transform(new FixVec(x, y), ang, rot);
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVec ConvertZeroPointBtoA(Transform b, Transform a) {
            var cosA = a.Rotator.x;
            var sinA = a.Rotator.y;

            var dv = b.Position - a.Position;

            var x = (cosA * dv.x + sinA * dv.y) >> 10;
            var y = (cosA * dv.y - sinA * dv.x) >> 10;

            return new FixVec(x, y);
        }
    }

}