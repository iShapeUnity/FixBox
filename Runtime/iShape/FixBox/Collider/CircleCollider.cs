using iShape.FixBox.Collision;
using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public readonly struct CircleCollider
    {
        // In local parent coordinate system
        public FixVec Center { get; }
        public long Radius { get; }

        public CircleCollider(FixVec center, long radius)
        {
            Center = center;
            Radius = radius;
        }

        public Contact Collide(CircleCollider circle)
        {
            FixVec ca = Center;
            FixVec cb = circle.Center;

            long ra = Radius;
            long rb = circle.Radius;

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
