using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace iShape.FixBox.Collider {

    public readonly struct ConvexCollider {
        
        // In local parent coordinate system
        public readonly FixVec Center;
        public NativeArray<FixVec> Points { get; }
        public NativeArray<FixVec> Normals { get; }
        public readonly Boundary Boundary;
        public readonly long Radius;

        public CircleCollider CircleCollider => new CircleCollider(Center, Radius);
        
        public ConvexCollider(Size size, Allocator allocator) {
            long a = (size.Width + 1) >> 1;
            long b = (size.Height + 1) >> 1;
            var points = new NativeArray<FixVec>(4, allocator);
            points[0] = new FixVec(-a, -b);
            points[1] = new FixVec(-a, b);
            points[2] = new FixVec(a, b);
            points[3] = new FixVec(a, -b);

            var normals = new NativeArray<FixVec>(4, allocator);

            normals[0] = new FixVec(-FixNumber.Unit, 0);
            normals[1] = new FixVec(0, FixNumber.Unit);
            normals[2] = new FixVec(FixNumber.Unit, 0);
            normals[3] = new FixVec(0, -FixNumber.Unit);

            Center = new FixVec(0, 0);
            Points = points;
            Normals = normals;
            Boundary = new Boundary(min: new FixVec(-a, -b), max: new FixVec(a, b));
            Radius = math.min(a, b);
        }

        public ConvexCollider(NativeArray<FixVec> points, Allocator allocator) {
            Assert.IsTrue(points.Length >= 3, "At least 3 points are required");

            var normals = new NativeArray<FixVec>(points.Length, allocator);

            FixVec centroid = FixVec.Zero;
            long area = 0;

            int j = points.Length - 1;
            FixVec p0 = points[j];

            for (int i = 0; i < points.Length; ++i) {
                FixVec p1 = points[i];
                FixVec e = p1 - p0;

                FixVec nm = new FixVec(-e.y, e.x).Normalize;
                normals[j] = nm;

                long crossProduct = p1.CrossProduct(p0);
                area += crossProduct;

                FixVec sp = p0 + p1;
                centroid += sp * crossProduct;

                p0 = p1;

                j = i;
            }

            area >>= 1;
            long s = 6 * area;

            long x = centroid.x.Div(s);
            long y = centroid.y.Div(s);

            Center = new FixVec(x, y);
            Points = new NativeArray<FixVec>(points.Length, allocator);
            
            points.CopyTo(Points);
            Normals = normals;
            Boundary = new Boundary(points: points);

            var minR = long.MaxValue;
        
            for (int i = 0; i < points.Length; ++i) {
                var p = points[i];
                var n = normals[i];

                var v = p - Center;
                var r = math.abs(v.DotProduct(n));
            
                if (r < minR) {
                    minR = r;
                }
            }

            Radius = minR;
        }

        public ConvexCollider(Transform transform, ConvexCollider collider, Allocator allocator) {
            int n = collider.Points.Length;
            var points = new NativeArray<FixVec>(n, allocator);
            var normals = new NativeArray<FixVec>(n, allocator);
            for(int i = 0; i < n; ++i) {
                points[i] = transform.ConvertAsPoint(collider.Points[i]);
                normals[i] = transform.ConvertAsVector(collider.Normals[i]);
            }

            Points = points;
            Normals = normals;
            Boundary = transform.Convert(collider.Boundary);
            Center = transform.ConvertAsPoint(collider.Center);
            Radius = collider.Radius;
        }
        
        public void Dispose() {
            Points.Dispose();
            Normals.Dispose();
        }

        public bool IsContain(FixVec point) {
            if (!Boundary.IsContain(point)) {
                return false;
            }

            return Points.IsPointInsideConvexPolygon(point);
        }

    }

}
