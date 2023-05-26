using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Collections;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public readonly struct Boundary {
        
        public static readonly Boundary Zero = new Boundary(FixVec.Zero, FixVec.Zero);

        public readonly FixVec Min;
        public readonly FixVec Max;

        public FixVec Size => Max - Min;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary(FixVec min, FixVec max) {
            this.Min = min;
            this.Max = max;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary(long radius) {
            Min = new FixVec(-radius, -radius);
            Max = new FixVec(radius, radius);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary(Size size) {
            var a = size.Width >> 1;
            var b = size.Height >> 1;
            Min = new FixVec(-a, -b);
            Max = new FixVec(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary(NativeArray<FixVec> points) {
            FixVec p0 = points[0];
            long minX = p0.x;
            long maxX = p0.x;
            long minY = p0.y;
            long maxY = p0.y;

            for (int i = 1; i < points.Length; ++i) {
                FixVec p = points[i];

                minX = math.min(minX, p.x);
                maxX = math.max(maxX, p.x);
                minY = math.min(minY, p.y);
                maxY = math.max(maxY, p.y);
            }

            Min = new FixVec(minX, minY);
            Max = new FixVec(maxX, maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary Translate(FixVec delta) {
            return new Boundary(Min + delta, Max + delta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boundary Union(Boundary box) {
            long minX = math.min(Min.x, box.Min.x);
            long minY = math.min(Min.y, box.Min.y);
            long maxX = math.max(Max.x, box.Max.x);
            long maxY = math.max(Max.y, box.Max.y);

            return new Boundary(new FixVec(minX, minY), new FixVec(maxX, maxY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCollide(Boundary box) {
            if (Max.x < box.Min.x || Min.x > box.Max.x) {
                return false;
            }

            if (Max.y < box.Min.y || Min.y > box.Max.y) {
                return false;
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsContain(FixVec p) {
            return Min.x <= p.x && p.x <= Max.x && Min.y <= p.y && p.y <= Max.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCollideCircle(FixVec center, long radius) {
            long cx = math.clamp(center.x, Min.x, Max.x);
            long cy = math.clamp(center.y, Min.y, Max.y);

            long sqrDist = center.SqrDistance(new FixVec(cx, cy));

            return sqrDist <= radius.Sqr();
        }
    }

}