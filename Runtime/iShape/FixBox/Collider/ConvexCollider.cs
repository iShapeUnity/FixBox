using iShape.FixBox.Collision;
using iShape.FixFloat;
using Unity.Collections;
using UnityEngine.Assertions;

namespace iShape.FixBox.Collider {

    public readonly struct ConvexCollider {
        
        // In local parent coordinate system
        public readonly FixVec Center;
        public NativeArray<FixVec> Points { get; }
        public NativeArray<FixVec> Normals { get; }
        public readonly Boundary Boundary;

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
        }

        public void Dispose() {
            Points.Dispose();
            Normals.Dispose();
        }
        
        // Do not work correctly with degenerate points
        public Contact Collide(CircleCollider circle) {
            // Find the min separating edge.
            int normalIndex = 0;
            long separation = long.MinValue;

            long r = circle.Radius + 100;

            for(int i = 0; i < Points.Length; ++i) {
                FixVec d = circle.Center - Points[i];
                long s = Normals[i].DotProduct(d);

                if (s > r) {
                    return Contact.Outside;
                }

                if (s > separation) {
                    separation = s;
                    normalIndex = i;
                }
            }

            // Vertices that subtend the incident face.
            int vertIndex1 = normalIndex;
            int vertIndex2 = (vertIndex1 + 1) % Points.Length;
            FixVec v1 = Points[vertIndex1];
            FixVec v2 = Points[vertIndex2];
            FixVec n1 = Normals[vertIndex1];

            FixVec faceCenter = v1.Middle(v2);

            long delta = circle.Radius - separation;
            
            // If the center is inside the polygon ...
            if (separation < 0) {
                return new Contact(faceCenter, delta, ContactType.Inside) {
                    A = new Contact.BodyPoint(n1.Reverse, 0),
                    B = new Contact.BodyPoint(n1, circle.Radius)
                };
            }

            // Compute barycentric coordinates
            long sqrRadius = circle.Radius.Sqr();

            long u1 = (circle.Center - v1).DotProduct(v2 - v1);

            if (u1 <= 0) {
                if (circle.Center.SqrDistance(v1) > sqrRadius) {
                    return Contact.Outside;
                }

                var nB = (circle.Center - v1).Normalize;
                return new Contact(v1, delta, ContactType.Collide) {
                    A = new Contact.BodyPoint(nB.Reverse, 0),
                    B = new Contact.BodyPoint(nB, circle.Radius)
                };
            }

            long u2 = (circle.Center - v2).DotProduct(v1 - v2);

            if (u2 <= 0) {
                if (circle.Center.SqrDistance(v2) > sqrRadius) {
                    return Contact.Outside;
                }
                
                var nB = (circle.Center - v2).Normalize;
                return new Contact(v2, delta, ContactType.Collide) {
                    A = new Contact.BodyPoint(nB.Reverse, 0),
                    B = new Contact.BodyPoint(nB, circle.Radius)
                };
            }

            long sc = (circle.Center - faceCenter).DotProduct(n1);
            if (sc > circle.Radius) {
                return Contact.Outside;
            }

            long dc = (circle.Center - v2).DotProduct(n1);
            FixVec m = circle.Center - dc * n1;

            return new Contact(m, delta, ContactType.Collide) {
                A = new Contact.BodyPoint(n1.Reverse, 0),
                B = new Contact.BodyPoint(n1, circle.Radius)
            };
        }
    }

}
