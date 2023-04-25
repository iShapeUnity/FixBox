using iShape.FixBox.Collision;
using iShape.FixFloat;
using Unity.Mathematics;

namespace iShape.FixBox.Dynamic {

    public static class ImpactSolver {
        
        public static bool CollideDynamicAndStatic(ref Body a, Body b) {

            var contact = CollisionSolver.Collide(a, b);
            
            if (contact.Type == ContactType.Outside) {
                return false;
            }

            var vA = a.Velocity.Linear;
            var aNy = contact.A.Normal;

            var aPy = vA.DotProduct(aNy);
        
            // only if opposite to normal
            if (aPy >= 0) {
                return false;
            }

            var aNx = new FixVec(aNy.y, -aNy.x);

            var aPx = vA.DotProduct(aNx);

            var vNy = aNy * aPy;
            var vNx = aNx * aPx;

            var kb = math.max(a.Material.Bounce, b.Material.Bounce);
            
            // apply new velocity
            var new_aVel = vNx - kb * vNy;
            a.Velocity = new Velocity(new_aVel, a.Velocity.Angular);

            // apply to contact delta
            var dNy = contact.Delta * aNy;
            a.Transform = a.Transform.Apply(dNy);

            return true;
        }
    }

}