using iShape.FixFloat;

namespace iShape.FixBox.Collision {

    public enum ContactType {
        Outside,
        Inside,
        Collide
    }

    public struct Contact {

        public struct BodyPoint {
            public static readonly BodyPoint Empty = new BodyPoint(FixVec.Zero, 0);
            public readonly FixVec Normal;
            public readonly long Radius;
            
            public BodyPoint(FixVec normal, long radius) {
                Normal = normal;
                Radius = radius;
            }
        }

        public static readonly Contact Outside = new Contact(FixVec.Zero, ContactType.Outside);

        public readonly FixVec Point;
        
        public BodyPoint A;
        public BodyPoint B;
        public readonly ContactType Type;

        public Contact(FixVec point, ContactType type) {
            Point = point;
            Type = type;
            A = BodyPoint.Empty;
            B = BodyPoint.Empty;
        }

    }

}