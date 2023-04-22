using Unity.Collections;

namespace iShape.FixBox.Collider {

    public readonly struct ComplexCollider {

        public readonly Boundary Boundary;
        public NativeArray<ConvexCollider> Colliders { get; }

        public ComplexCollider(Boundary boundary, NativeArray<ConvexCollider> colliders) {
            Boundary = boundary;
            Colliders = colliders;
        }

        public void Dispose() {
            Colliders.Dispose();
        }
    }

}