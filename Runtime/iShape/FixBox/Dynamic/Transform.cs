using System;
using System.Runtime.CompilerServices;
using iShape.FixBox.Collider;
using iShape.FixBox.Collision;
using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {

    public readonly struct Transform {
        
        public static readonly Transform Zero = new Transform(position: FixVec.Zero, angle: 0);
        public readonly FixVec Position;
        public readonly long Angle;

        private readonly RotationMatrix rotator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform(FixVec position, long angle) {
            Position = position;
            Angle = angle;
            if (angle != 0) {
                rotator = angle.RadToFixAngle().Rotator();
            } else {
                rotator = new RotationMatrix(sin: 0, cos: FixNumber.Unit);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Transform(FixVec position, long angle, RotationMatrix rotator) {
            Position = position;
            Angle = angle;
            this.rotator = rotator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ToLocal(FixVec point) {
            return rotator.RotateForward(point - Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ToWorld(FixVec point) {
            return rotator.RotateBack(point) + Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ToLocalVector(FixVec vector) {
            return rotator.RotateForward(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVec ToWorldVector(FixVec vector) {
            return rotator.RotateBack(vector);
        }

        public Boundary ToWorld(Boundary boundary) {
            if (Angle == 0) {
                return boundary.Translate(delta: Position);
            } else {
                FixVec a0 = new FixVec(boundary.Min.x, boundary.Min.y);
                FixVec a1 = new FixVec(boundary.Min.x, boundary.Max.y);
                FixVec a2 = new FixVec(boundary.Max.x, boundary.Max.y);
                FixVec a3 = new FixVec(boundary.Max.x, boundary.Min.y);

                FixVec b0 = ToWorld(a0);
                FixVec b1 = ToWorld(a1);
                FixVec b2 = ToWorld(a2);
                FixVec b3 = ToWorld(a3);

                long minX = Math.Min(Math.Min(b0.x, b1.x), Math.Min(b2.x, b3.x));
                long minY = Math.Min(Math.Min(b0.y, b1.y), Math.Min(b2.y, b3.y));

                long maxX = Math.Max(Math.Max(b0.x, b1.x), Math.Max(b2.x, b3.x));
                long maxY = Math.Max(Math.Max(b0.y, b1.y), Math.Max(b2.y, b3.y));

                return new Boundary(min: new FixVec(minX, minY), max: new FixVec(maxX, maxY));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact ToWorld(Contact contact) {
            FixVec point = ToWorld(contact.Point);
            FixVec normalA = ToWorldVector(contact.A.Normal);

            return new Contact(point, contact.Type) {
                A = new Contact.BodyPoint(normalA, contact.A.Radius),
                B = new Contact.BodyPoint(normalA.Reverse, contact.B.Radius)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform Apply(Velocity v, int timeScale) {
            FixVec dv = v.Linear.DivTwo(timeScale);
            FixVec p = Position + dv;

            if (v.Angular != 0) {
                long a = Angle + (v.Angular >> timeScale);
                return new Transform(p, a);
            } else {
                return new Transform(p, Angle, rotator);
            }
        }
    }

}