using System.Collections.Generic;
using System.Runtime.CompilerServices;
using iShape.FixBox.Collider;
using iShape.FixFloat;

namespace iShape.FixBox.Dynamic {
    
    public struct Body {
        
        public long Id { get; }
        public readonly BodyType Type;

        public Shape Shape { get; private set; }
        public Material Material;
        public long InvMass;
        public long Mass;
        public long Inertia;
        public Velocity Velocity;
        public Transform Transform;
        public Boundary Boundary;
        public bool IsAlive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Body(long id, BodyType type, Material material) {
            Id = id;
            Type = type;

            Material = material;
            Shape = Shape.Empty;
            InvMass = 0;
            Mass = 0;
            Inertia = 0;
            Velocity = Velocity.Zero;
            Transform = Transform.Zero;
            Boundary = Boundary.Zero;
            IsAlive = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(Shape shape) {
            Shape = shape;
            if (Type != BodyType.land) {
                Mass = shape.Area.Mul(Material.Density);
                InvMass = FixNumber.Unit.Div(Mass);
                Inertia = shape.Inertia.Mul(Material.Density);
            }
            Boundary = Transform.ToWorld(shape.Boundary);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Iterate(int timeScale) {
            Transform = Transform.Apply(Velocity, timeScale);
            Boundary = Transform.ToWorld(Shape.Boundary);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Iterate(int timeScale, FixVec Gravity) {
            Velocity = Velocity.Apply(timeScale, Gravity);
            Transform = Transform.Apply(Velocity, timeScale);
            Boundary = Transform.ToWorld(Shape.Boundary);
        }
    }
    
    public class BodyIdComparer : IComparer<Body>
    {
        public int Compare(Body a, Body b)
        {
            return a.Id.CompareTo(b.Id);
        }
    }

}