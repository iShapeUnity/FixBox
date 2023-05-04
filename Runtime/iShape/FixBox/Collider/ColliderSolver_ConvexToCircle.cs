using iShape.FixBox.Collision;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public static class ColliderSolver_ConvexToCircle {
        
        // Do not work correctly with degenerate points
        public static Contact Collide(CircleCollider circle, ConvexCollider convex) {
            // Find the min separating edge.
            int normalIndex = 0;
            long separation = long.MinValue;

            long r = circle.Radius + 100;

            for(int i = 0; i < convex.Points.Length; ++i) {
                FixVec d = circle.Center - convex.Points[i];
                long s = convex.Normals[i].DotProduct(d);

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
            int vertIndex2 = (vertIndex1 + 1) % convex.Points.Length;
            FixVec v1 = convex.Points[vertIndex1];
            FixVec v2 = convex.Points[vertIndex2];
            FixVec n1 = convex.Normals[vertIndex1];

            long delta = circle.Radius - separation;
            
            // If the center is inside the polygon ...
            if (separation < 0) {
                return new Contact(
                    circle.Center,
                    n1,
                    delta,
                    1,
                    ContactType.Inside
                    );
            }

            // Compute barycentric coordinates
            long sqrRadius = circle.Radius.Sqr();

            long u1 = (circle.Center - v1).DotProduct(v2 - v1);

            if (u1 <= 0) {
                if (circle.Center.SqrDistance(v1) > sqrRadius) {
                    return Contact.Outside;
                }

                var nB = (circle.Center - v1).Normalize;
                return new Contact(v1, nB, delta, 1, ContactType.Collide);
            }

            long u2 = (circle.Center - v2).DotProduct(v1 - v2);

            if (u2 <= 0) {
                if (circle.Center.SqrDistance(v2) > sqrRadius) {
                    return Contact.Outside;
                }
                
                var nB = (circle.Center - v2).Normalize;
                return new Contact(v2, nB, delta,1, ContactType.Collide);
            }
            
            FixVec faceCenter = v1.Middle(v2);

            long sc = (circle.Center - faceCenter).DotProduct(n1);
            if (sc > circle.Radius) {
                return Contact.Outside;
            }

            long dc = (circle.Center - v2).DotProduct(n1);
            FixVec m = circle.Center - dc * n1;

            return new Contact(m, n1, delta, 1, ContactType.Collide);
        }
    }

}