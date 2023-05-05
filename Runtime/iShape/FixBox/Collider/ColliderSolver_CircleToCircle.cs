using iShape.FixBox.Collision;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public static class ColliderSolver_CircleToCircle {
        
        public static Contact Collide(CircleCollider a, CircleCollider b)
        {
            FixVec ca = a.Center;
            FixVec cb = b.Center;

            long ra = a.Radius;
            long rb = b.Radius;

            long sqrC = ca.SqrDistance(cb);

            if ((ra + rb).Sqr() >= sqrC) {
                long penetration = ra + rb - sqrC.Sqrt();
                    
                long sqrA = ra.Sqr();
                long sqrB = rb.Sqr();

                FixVec dv = ca - cb;
                
                if (sqrC >= sqrA && sqrC >= sqrB)
                {
                    long k = (sqrB - sqrA + sqrC).Div(2 * sqrC);

                    FixVec p = cb + dv * k;

                    var nA = dv.Normalize;
                    
                    return new Contact(
                        p,
                        nA,
                        penetration,
                        1,
                        ContactType.Collide
                        );
                }
                else {
                    var p = sqrB > sqrA ? ca : cb;
                    var n = dv.SqrLength != 0 ? dv.Normalize : new FixVec(0, FixNumber.Unit);
                        
                    return new Contact(
                        p,
                        n,
                        penetration,
                        1,
                        ContactType.Inside
                        );
                }
            }
            else
            {
                return Contact.Outside;
            }
        }
    }

}