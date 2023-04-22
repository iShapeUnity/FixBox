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
                    
                    return new Contact(
                        point: p,
                        type: ContactType.Collide
                    ) {
                        A = new Contact.BodyPoint(nA, Radius),
                        B = new Contact.BodyPoint(nA.Reverse, circle.Radius)
                    };
                }
                else
                {
                    FixVec p = (ca + cb).Half;
                    return new Contact(
                        point: p,
                        type: ContactType.Inside
                    ) {
                        A = new Contact.BodyPoint(FixVec.Zero, Radius),
                        B = new Contact.BodyPoint(FixVec.Zero, circle.Radius)
                    };
                }
            }
            else
            {
                return Contact.Outside;
            }
        }
    }

}
