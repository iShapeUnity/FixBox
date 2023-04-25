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

            var aV1 = a.Velocity.Linear;
            var aNy = contact.A.Normal;

            var aPy = aV1.DotProduct(aNy);
        
            // only if opposite to normal
            if (aPy >= 0) {
                return false;
            }

            var kab = math.max(a.Material.Bounce, b.Material.Bounce);
            
            var bV1 = b.Velocity.Linear;
            // relative velocity
            var V1 = aV1 - bV1;

            var noMassImp = -(FixNumber.Unit + kab).Mul(V1.DotProduct(aNy)); 
            
            // new linear velocity
            var adV = noMassImp * aNy;
            var aV2 = aV1 + adV;

            // calculate the contact point relative to the center of mass of body A
            var aP = contact.Point - a.Transform.Position;

            // calculate the torque
            var impulse = noMassImp.Mul(a.Mass);
            var torque = aP.CrossProduct(impulse * aNy);

            // new angular velocity
            var dA = torque.Mul(a.Inertia);
            var new_aAngVel = a.Velocity.Angular + dA;
            
            a.Velocity = new Velocity(aV2, new_aAngVel);
            
            // fix contact delta
            var dNy = contact.Delta * aNy;
            a.Transform = a.Transform.Apply(dNy);

            return true;
        }
    }

}