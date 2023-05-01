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

            if ((ra + rb).Sqr() >= sqrC)
            {
                long sqrA = ra.Sqr();
                long sqrB = rb.Sqr();

                if (sqrC >= sqrA && sqrC >= sqrB)
                {
                    long k = (sqrB - sqrA + sqrC) / sqrC >> 1;

                    FixVec dv = ca - cb;

                    FixVec p = cb + dv * k;

                    var nA = dv.Normalize;
                    
                    return new Contact(p, nA, 0, ContactType.Collide);
                }
                else
                {
                    FixVec p = (ca + cb).Half;
                    return new Contact(p, FixVec.Zero, 0, ContactType.Inside);
                }
            }
            else
            {
                return Contact.Outside;
            }
        }
    }

}