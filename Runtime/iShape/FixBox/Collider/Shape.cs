using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Collider {

    public enum Form {
        circle, polygon
    }
    
    public readonly struct Shape {

        public static readonly Shape Empty = new Shape(0);

        public readonly Form Form;
        public readonly long Area;
        public readonly long Inertia;
        public readonly long Radius;
        public readonly Boundary Boundary;
        public NativeArray<ConvexCollider> colliders { get; }
        
        public Shape(long radius) {
            Radius = radius;
            Form = Form.circle;
            Boundary = new Boundary(radius);
            long rr = radius.Sqr();
            Area = FixNumber.Pi.Mul(rr);
            Inertia = rr >> 1;
            colliders = new NativeArray<ConvexCollider>(0, Allocator.Persistent);
        }
        
        public Shape(ConvexCollider convex) {
            Radius = 0;
            Form = Form.polygon;
            Boundary = convex.Boundary;
            Area = 0;
            Inertia = 0;
            var array = new NativeArray<ConvexCollider>(1, Allocator.Persistent);
            array[1] = convex;
            colliders = array;
        }

        public void Dispose() {
            for (int i = 0; i < colliders.Length; ++i) {
                colliders[i].Dispose();
            }
            colliders.Dispose();
        }

    }

}