using iShape.FixFloat;

namespace iShape.FixBox.Collider {

    public readonly struct CircleCollider
    {
        public FixVec Center { get; }
        public long Radius { get; }

        public CircleCollider(FixVec center, long radius)
        {
            Center = center;
            Radius = radius;
        }
    }

}
