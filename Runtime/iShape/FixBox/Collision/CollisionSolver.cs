using iShape.FixBox.Collider;
using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Collision {

    public static class CollisionSolver {

        public static Contact Collide(Body a, Body b) {
            if (!a.Shape.Boundary.IsCollide(b.Shape.Boundary)) {
                return Contact.Outside;
            }

            if (a.Shape.Form == Form.circle) {
                if (b.Shape.Form == Form.circle) {
                    return CollideCircles(a, b);    
                } else {
                    // TODO invert contact
                    return CollidePolygonAndCircle(b, a);
                }
            } else {
                if (b.Shape.Form == Form.circle) {
                    return CollidePolygonAndCircle(a, b);    
                } else {
                    return CollidePolygons(b, a);
                }
            }
        }
        
        private static Contact CollideCircles(Body a, Body b) {
            var circleA = new CircleCollider(center: a.Transform.Position, radius: a.Shape.Radius);
            var circleB = new CircleCollider(center: b.Transform.Position, radius: b.Shape.Radius);

            return circleA.Collide(circleB);
        }
        
        private static Contact CollidePolygonAndCircle(Body a, Body b) {
            if (a.Shape.colliders.Length == 1) {
                return CollideConvexAndCircle(a, b);
            } else {
                return CollideComplexAndCircle(a, b);
            }
        }

        private static Contact CollideConvexAndCircle(Body a, Body b) {
            var pos = a.Transform.ToLocal(b.Transform.Position);
            var circleB = new CircleCollider(center: pos, radius: b.Shape.Radius);

            var contact = a.Shape.colliders[0].Collide(circleB);

            return a.Transform.ToWorld(contact);
        }

        private static Contact CollideComplexAndCircle(Body a, Body b) {
            return Contact.Outside;
        }
        
        private static Contact CollidePolygons(Body a, Body b) {
            return Contact.Outside;
        }
    }

}