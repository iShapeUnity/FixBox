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
        private long Mass;
        private long UnitInertia;
        public long InvInertia;
        private Acceleration Acceleration;
        public Velocity Velocity;
        public Transform Transform;
        public Boundary Boundary;
        public readonly bool ApplyGravity;
        public bool IsAlive;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Body(long id, BodyType type, Material material, bool applyGravity = true) {
            Id = id;
            Type = type;

            Material = material;
            Shape = Shape.Empty;
            InvMass = 0;
            Mass = 0;
            UnitInertia = 0;
            InvInertia = 0;
            Velocity = Velocity.Zero;
            Transform = Transform.Zero;
            Boundary = Boundary.Zero;
            IsAlive = true;
            ApplyGravity = applyGravity;
            Acceleration = Acceleration.Zero;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(Shape shape) {
            Shape = shape;
            if (Type != BodyType.land) {
                Mass = shape.Area.Mul(Material.Density);
                InvMass = FixNumber.Unit.Div(Mass);
                UnitInertia = shape.UnitInertia.Mul(Mass);
                InvInertia = FixNumber.Unit.Div(UnitInertia.Mul(Mass));
            }
            Boundary = Transform.Convert(shape.Boundary);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void IterateStatic(long timeStep) {
            Transform = Transform.Apply(Velocity, timeStep);
            Boundary = Transform.Convert(Shape.Boundary);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void IterateDynamic(long timeStep) {
            Velocity = Velocity.Apply(timeStep, Acceleration);
            Transform = Transform.Apply(Velocity, timeStep);
            Boundary = Transform.Convert(Shape.Boundary);
        }

        public void AddForce(FixVec force, FixVec point) {
            if (point == Transform.Position) {
                AddAccelerationToCenterOfMass(force * InvMass);
                return;
            }

            var r = point - Transform.Position;
            var n = r.Normalize;
            var projF = force.DotProduct(n); 
            var a = projF.Mul(InvMass) * n;
            var moment = -force.CrossProduct(r); 
            var wa = moment.Mul(InvInertia);

            Acceleration = new Acceleration(Acceleration.Linear + a, Acceleration.Angular + wa);
        }

        public void AddAcceleration(FixVec acceleration, FixVec point) {
            if (point == Transform.Position) {
                AddAccelerationToCenterOfMass(acceleration);
                return;
            }
            
            var r = point - Transform.Position;
            var n = r.Normalize;
            var a = acceleration.DotProduct(n) * n;
            var wa = -acceleration.CrossProduct(r).Mul(UnitInertia);
            
            Acceleration = new Acceleration(Acceleration.Linear + a, Acceleration.Angular + wa);
        }
        
        public void AddVelocity(FixVec velocity, FixVec point) {
            if (point == Transform.Position) {
                AddVelocityToCenterOfMass(velocity);
                return;
            }
            
            var r = point - Transform.Position;
            var n = r.Normalize;
            var v = velocity.DotProduct(n) * n;
            var w = -velocity.CrossProduct(r);

            Velocity = new Velocity(Velocity.Linear + v, Velocity.Angular + w);
        }
        
        public void AddAccelerationToCenterOfMass(FixVec acceleration) {
            Acceleration = new Acceleration(Acceleration.Linear + acceleration, Acceleration.Angular);
        }
        
        public void AddVelocityToCenterOfMass(FixVec velocity) {
            Velocity = new Velocity(Velocity.Linear + velocity, Velocity.Angular);
        }

        public void PostIterate(FixVec gravity) {
            Acceleration = new Acceleration(gravity);
            Velocity = new Velocity(Velocity.Linear * Material.AirLinearFriction, Velocity.Angular.Mul(Material.AirAngularFriction));
        }
        
        public override string ToString()
        {
            return $"Transform: ({Transform}) Velocity: ({Velocity})";
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