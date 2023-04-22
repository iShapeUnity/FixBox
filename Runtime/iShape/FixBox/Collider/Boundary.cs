using System;
using Unity.Mathematics;
using Unity.Collections;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    [Serializable]
    public struct Boundary {
        
        public static readonly Boundary Zero = new Boundary(FixVec.Zero, FixVec.Zero);

        public FixVec Min;
        public FixVec Max;

        public Boundary(FixVec min, FixVec max) {
            this.Min = min;
            this.Max = max;
        }

        public Boundary(long radius) {
            Min = new FixVec(-radius, -radius);
            Max = new FixVec(radius, radius);
        }

        public Boundary(NativeArray<FixVec> points) {
            int n = points.Length;
            int i = 1;

            FixVec p0 = points[0];
            long minX = p0.x;
            long maxX = p0.x;
            long minY = p0.y;
            long maxY = p0.y;

            while (i < n) {
                FixVec p = points[i];
                i += 1;

                minX = math.min(minX, p.x);
                maxX = math.max(maxX, p.x);
                minY = math.min(minY, p.y);
                maxY = math.max(maxY, p.y);
            }

            Min = new FixVec(minX, minY);
            Max = new FixVec(maxX, maxY);
        }

        public Boundary(NativeArray<Boundary> boxes) {
            int n = boxes.Length;

            if (n > 0) {
                Boundary boundary = boxes[0];

                for (int i = 1; i < n; i++) {
                    boundary = boundary.Union(boxes[i]);
                }

                Min = boundary.Min;
                Max = boundary.Max;
            } else {
                Min = FixVec.Zero;
                Max = FixVec.Zero;
            }
        }

        public Boundary Translate(FixVec delta) {
            return new Boundary(Min + delta, Max + delta);
        }

        public Boundary Union(Boundary box) {
            long minX = math.min(Min.x, box.Min.x);
            long minY = math.min(Min.y, box.Min.y);
            long maxX = math.max(Max.x, box.Max.x);
            long maxY = math.max(Max.y, box.Max.y);

            return new Boundary(new FixVec(minX, minY), new FixVec(maxX, maxY));
        }

        public bool IsCollide(Boundary box) {
            if (Max.x < box.Min.x || Min.x > box.Max.x) {
                return false;
            }

            if (Max.y < box.Min.y || Min.y > box.Max.y) {
                return false;
            }

            return true;
        }

        public bool IsCollideCircle(FixVec center, long radius) {
            long cx = math.clamp(center.x, Min.x, Max.x);
            long cy = math.clamp(center.y, Min.y, Max.y);

            long sqrDist = center.SqrDistance(new FixVec(cx, cy));

            return sqrDist <= radius.Sqr();
        }
    }

}