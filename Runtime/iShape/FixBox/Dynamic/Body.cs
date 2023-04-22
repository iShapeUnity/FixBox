using iShape.FixBox.Collider;
using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {
    
    public struct Body {
        
        public long Id { get; }
        public readonly BodyType Type;

        public Shape Shape { get; private set; }
        public Material Material;
        public long Mass;
        public long Inertia;
        public Velocity Velocity;
        public Transform Transform;
        public Boundary Boundary;
        public bool IsAlive;

        public Body(long id, BodyType type, Material material) {
            Id = id;
            Type = type;

            Material = material;
            Shape = Shape.Empty;
            Mass = 0;
            Inertia = 0;
            Velocity = Velocity.Zero;
            Transform = Transform.Zero;
            Boundary = Boundary.Zero;
            IsAlive = true;
        }
        
        public Body(long id, BodyType type) {
            Id = id;
            Type = type;

            Material = Material.Ordinary;
            Shape = Shape.Empty;
            Mass = 0;
            Inertia = 0;
            Velocity = Velocity.Zero;
            Transform = Transform.Zero;
            Boundary = Boundary.Zero;
            IsAlive = true;
        }

        public void Dispose() {
            Shape.Dispose();
        }
        
        public void Attach(Shape shape) {
            Shape.Dispose();
            Shape = shape;
            if (Type != BodyType.land) {
                Mass = shape.Area.Mul(Material.Density);
                Inertia = shape.Inertia.Mul(Material.Density);
            }
            Boundary = Transform.ToWorld(shape.Boundary);
        }

        internal void Iterate(int timeScale) {
            Transform = Transform.Apply(Velocity, timeScale);
            Boundary = Transform.ToWorld(Shape.Boundary);
        }
    }

}