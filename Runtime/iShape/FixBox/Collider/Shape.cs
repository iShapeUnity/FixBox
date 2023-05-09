using System.Runtime.CompilerServices;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public enum Form {
        circle, rect, polygon
    }
    
    public readonly struct Shape {

        public static readonly Shape Empty = new Shape(-1);

        public readonly Form Form;
        public readonly long Area;
        public readonly long UnitInertia;

        public bool IsNotEmpty => Radius >= 0;
        
        public long Radius => data0;
        public Size Size => new Size(data0, data1);
        public ColliderIndex Index => new ColliderIndex(data0, (int)data1);

        private readonly long data0;
        private readonly long data1;
        
        public readonly Boundary Boundary;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Shape(long radius) {
            data0 = radius;
            data1 = -1;
            Form = Form.circle;
            Boundary = new Boundary(radius);
            long rr = radius.Sqr();
            Area = FixNumber.PI.Mul(rr);
            UnitInertia = rr / 2;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Shape(Size size) {
            data0 = size.Width;
            data1 = size.Height;
            Form = Form.rect;
            Boundary = new Boundary(size);
            Area = size.Area();
            UnitInertia = (size.Width.Sqr() + size.Height.Sqr()).Mul(85); // 85 ~= 1024 / 12 
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Shape(ColliderIndex index, Boundary boundary) {
            data0 = index.Id;
            data1 = index.Index;
            Form = Form.polygon;
            Boundary = boundary;
            Area = 0;
            UnitInertia = 0;
        }

    }

}