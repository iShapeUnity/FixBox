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

            // normal to contact point
            var n = contact.Normal;
            
            // start linear and angular velocity for A and B
            var aV1 = a.Velocity.Linear;
            var aW1 = a.Velocity.Angular;

            var bV1 = b.Velocity.Linear;
            var bW1 = b.Velocity.Angular;
            
            // distance between center of Mass A to contact point
            var aR = contact.Point - a.Transform.Position;
            
            // distance between center of Mass A to contact point
            var bR = contact.Point - b.Transform.Position;
            
            // relative velocity
            var rV1 = aV1 - bV1 + aR.CrossProduct(aW1) - bR.CrossProduct(bW1); 

            // only if getting closer
            if (rV1.DotProduct(n) > 0) {
                return false;
            }

            // bounce coefficient A vs B
            var e = math.max(a.Material.Bounce, b.Material.Bounce);

            // -(1 + e)
            var ke = -e - FixNumber.Unit;
            
            // normal impulse
            // -(1 + e) * rV1 * n / (1 / Ma + (aR * t)^2 / aI)
            
            var iNum = rV1.DotProduct(n).Mul(ke);
            var iDen = a.InvMass + aR.CrossProduct(n).Sqr().Mul(a.InvInertia);
            var i = iNum.Div(iDen); 

            // new linear velocity
            var aV2 = aV1 + i.Mul(a.InvMass) * n;
            
            // new angular velocity
            var aW2 = aW1 + aR.CrossProduct(n).Mul(i).Mul(a.InvInertia);

            // tangent vector
            // leaving only the component that is parallel to the contact surface
            var f = rV1 - n * rV1.DotProduct(n);
            var sqrF = f.SqrLength;

            // ignore if it to small
            if (sqrF >= 8) {
                // friction coefficient A vs B, for performance 
                // for performance use arithmetic mean instead of the geometric mean
                var q = (a.Material.Friction + b.Material.Friction) >> 1;
                
                f = f.Normalize;
                
                var jNum = rV1.DotProduct(f).Mul(ke);
                var jDen = a.InvMass + aR.CrossProduct(f).Sqr().Mul(a.InvInertia);
                var j = jNum.Div(jDen);

                // can not be more then original impulse
                var maxFi = i.Mul(q);
                j = math.clamp(j, -maxFi, maxFi);
                
                // new linear velocity
                aV2 += j.Mul(a.InvMass) * f;
            
                // new angular velocity
                aW2 += aR.CrossProduct(f).Mul(j).Mul(a.InvInertia);
            }


            // apply result
            a.Velocity = new Velocity(aV2, aW2);

            // fix contact delta
            a.Transform = a.Transform.Apply(contact.Correction);

            return true;
        }
    }

}