using iShape.FixBox.Collision;
using iShape.FixFloat;
using Unity.Mathematics;

namespace iShape.FixBox.Dynamic {

    // https://chrishecker.com/Rigid_Body_Dynamics
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
            
            var aRn = aR.CrossProduct(n);
            var iNum = rV1.DotProduct(n).Mul(ke);
            var iDen = a.InvMass + aRn.Sqr().Mul(a.InvInertia);
            var i = iNum.Div(iDen); 

            // new linear velocity
            var aV2 = aV1 + i.Mul(a.InvMass) * n;
            
            // new angular velocity
            var aW2 = aW1 + aRn.Mul(i).Mul(a.InvInertia);

            // tangent vector
            // leaving only the component that is parallel to the contact surface
            var f = rV1 - n * rV1.DotProduct(n);
            var sqrF = f.SqrLength;

            // ignore if it to small
            if (sqrF >= 1) {
                f = f.Normalize;
                
                var aRf = aR.CrossProduct(f);
                var jNum = rV1.DotProduct(f).Mul(ke);
                var jDen = a.InvMass + aRf.Sqr().Mul(a.InvInertia);
                var j = jNum.Div(jDen);

                // friction coefficient A vs B, for performance 
                // for performance use arithmetic mean instead of the geometric mean
                var q = (a.Material.Friction + b.Material.Friction) >> 1;
                
                // can not be more then original impulse
                var maxFi = i.Mul(q);
                j = math.clamp(j, -maxFi, maxFi);
                
                // new linear velocity
                aV2 += j.Mul(a.InvMass) * f;
            
                // new angular velocity
                aW2 += aRf.Mul(j).Mul(a.InvInertia);
            } else if (contact.Count > 1 && contact.Penetration > -4) {
                var vF = aV2.SqrLength;
                var wF = math.abs(aW2);
                if (vF < 100 && wF < 400) {
                    // if body is near to stop, permanently stopping it
                    aW2 = wF < 20 ? 0 : aW2 >> 1;
                    aV2 = vF < 20 ? FixVec.Zero : aV2.Half;
                }
            }

            // apply result
            a.Velocity = new Velocity(aV2, aW2);
            
            // contact.Log();
            if (contact.Penetration != 0) {
                var penetrationSign = (a.Transform.Position - contact.Point).DotProduct(contact.Normal) > 0;
                // fix contact delta
                a.Transform = a.Transform.Apply(contact.Correction(penetrationSign));                
            }

            return true;
        }
        
        public static bool CollideDynamicAndDynamic(ref Body a, ref Body b) {
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
            
            // distance between center of Mass B to contact point
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
            // -(1 + e) * rV1 * n / (1 / Ma + 1 / Mb + (aR * t)^2 / aI)

            var aRn = aR.CrossProduct(n);
            var bRn = bR.CrossProduct(n);
            var iNum = rV1.DotProduct(n).Mul(ke);
            var iDen = a.InvMass + b.InvMass + aRn.Sqr().Mul(a.InvInertia) + bRn.Sqr().Mul(b.InvInertia);
            var i = iNum.Div(iDen); 

            // new linear velocity
            var aV2 = aV1 + i.Mul(a.InvMass) * n;
            var bV2 = bV1 - i.Mul(b.InvMass) * n;
            
            // new angular velocity
            var aW2 = aW1 + aRn.Mul(i).Mul(a.InvInertia);
            var bW2 = bW1 - bRn.Mul(i).Mul(b.InvInertia);

            // tangent vector
            // leaving only the component that is parallel to the contact surface
            var f = rV1 - n * rV1.DotProduct(n);
            var sqrF = f.SqrLength;

            // ignore if it to small
            if (sqrF >= 1) {
                f = f.Normalize;
                
                var aRf = aR.CrossProduct(f);
                var bRf = bR.CrossProduct(f);
                var jNum = rV1.DotProduct(f).Mul(ke);
                var jDen = a.InvMass + b.InvMass + aRf.Sqr().Mul(a.InvInertia) + bRf.Sqr().Mul(b.InvInertia);
                var j = jNum.Div(jDen);

                // friction coefficient A vs B, for performance 
                // for performance use arithmetic mean instead of the geometric mean
                var q = (a.Material.Friction + b.Material.Friction) >> 1;
                
                // can not be more then original impulse
                var maxFi = i.Mul(q);
                j = math.clamp(j, -maxFi, maxFi);
                
                // new linear velocity
                aV2 += j.Mul(a.InvMass) * f;
                bV2 -= j.Mul(b.InvMass) * f;
            
                // new angular velocity
                aW2 += aRf.Mul(j).Mul(a.InvInertia);
                bW2 -= bRf.Mul(j).Mul(b.InvInertia);
            } else if (contact.Count > 1 && contact.Penetration > -4) {
                
                // stop A
                var vFa = aV2.SqrLength;
                var wFa = math.abs(aW2);
                if (vFa < 100 && wFa < 400) {
                    // if body is near to stop, permanently stopping it
                    aW2 = wFa < 20 ? 0 : aW2 >> 1;
                    aV2 = vFa < 20 ? FixVec.Zero : aV2.Half;
                }
                
                // stop B
                var vFb = bV2.SqrLength;
                var wFb = math.abs(bW2);
                if (vFb < 100 && wFb < 400) {
                    // if body is near to stop, permanently stopping it
                    bW2 = wFb < 20 ? 0 : bW2 >> 1;
                    bV2 = vFb < 20 ? FixVec.Zero : bV2.Half;
                }

            }

            // apply result
            a.Velocity = new Velocity(aV2, aW2);
            b.Velocity = new Velocity(bV2, bW2);
            
            if (contact.Penetration != 0) {
                var penetrationSign = (a.Transform.Position - contact.Point).DotProduct(contact.Normal) > 0;
                // fix contact delta
                var kb = a.Mass.Div(a.Mass + b.Mass);
                var ka = FixNumber.Unit - kb;
                
                a.Transform = a.Transform.Apply(contact.Correction(penetrationSign) * ka);
                b.Transform = b.Transform.Apply(contact.Correction(!penetrationSign) * kb);
            }

            return true;
        }
    }

}