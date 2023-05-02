using iShape.FixBox.Collider;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Collision {

    public static class CollisionSolver {

        public static Contact Collide(Body a, Body b) {
            if (!a.Boundary.IsCollide(b.Boundary)) {
                return Contact.Outside;
            }

            if (a.Shape.Form == Form.circle) {
                if (b.Shape.Form == Form.circle) {
                    return CollideCircles(a, b);    
                } else {
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

            return ColliderSolver_CircleToCircle.Collide(circleA, circleB);
        }
        
        private static Contact CollidePolygonAndCircle(Body a, Body b) {
            if (a.Shape.Form == Form.rect) {
                return CollideRectAndCircle(a, b);
            } else {
                return CollideComplexAndCircle(a, b);
            }
        }

        private static Contact CollideRectAndCircle(Body a, Body b) {
            var rect = new ConvexCollider(a.Shape.Size, Allocator.Temp);

            // var baT = Transform.ConvertFromBtoA(b.Transform, a.Transform);
            // var pos = baT.ConvertAsPoint(FixVec.Zero);
            var pos = Transform.ConvertZeroPointBtoA(b.Transform, a.Transform);
            var circle = new CircleCollider(pos, b.Shape.Radius);
            
            var contact = ColliderSolver_ConvexToCircle.Collide(circle, rect);
            rect.Dispose();

            return a.Transform.Convert(contact);
        }

        private static Contact CollideComplexAndCircle(Body a, Body b) {
            return Contact.Outside;
        }
        
        private static Contact CollidePolygons(Body a, Body b) {
            if (a.Shape.Form == Form.rect && b.Shape.Form == Form.rect) {
                var rectA = new ConvexCollider(a.Shape.Size, Allocator.Temp);
                var rectB = new ConvexCollider(b.Shape.Size, Allocator.Temp);

                var contact = ColliderSolver_ConvexToConvex.Collide(rectA, rectB, a.Transform, b.Transform);
                
                rectA.Dispose();
                rectB.Dispose();
                
                contact.Log();
                
                return contact;
            }

            return Contact.Outside;
        }
    }

}