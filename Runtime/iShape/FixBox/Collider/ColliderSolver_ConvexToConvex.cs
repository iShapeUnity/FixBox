using iShape.FixBox.Collision;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;


namespace iShape.FixBox.Collider {

    public static class ColliderSolver_ConvexToConvex {

        public static Contact Collide(ConvexCollider a, ConvexCollider b, Transform tA, Transform tB) {
            Contact contact;
            Transform t;
            if (a.Points.Length > b.Points.Length) {
                // work in a coord system

                var mt = Transform.ConvertFromBtoA(tB, tA);
                var b2 = new ConvexCollider(mt, b, Allocator.Temp);

                contact = Collide(a, b2);
                b2.Dispose();
                
                t = tA;
            } else {
                // work in b coord system

                var mt = Transform.ConvertFromBtoA(tA, tB);
                var a2 = new ConvexCollider(mt, a, Allocator.Temp);

                contact = Collide(b, a2);
                a2.Dispose();
                
                t = tB;
            }

            if (contact.Type != ContactType.Collide) {
                return contact;
            }
            // return in global coord system
            return t.Convert(contact);
        }

        
        private static Contact Collide(ConvexCollider a, ConvexCollider b) {
            var cA = FindMaxSeparation(a, b);
            if (cA.Penetration > 10) {
                return Contact.Outside;
            }

            var cB = FindMaxSeparation(b, a);

            if (cB.Penetration > cA.Penetration) {
                // A penetrate B
                return cB;
            } else {
                // A penetrate B
                return cA;
            }
        }
        
        private static Contact FindMaxSeparation(ConvexCollider a, ConvexCollider b) {
            var maxSep = long.MinValue;
            var bestNormal = FixVec.Zero;
            var bestVert = FixVec.Zero;
        
            for(int i = 0; i < a.Points.Length; ++i) {
                var na = a.Normals[i];
                var pa = a.Points[i];

                var iMinSep = long.MaxValue;
                var vert = FixVec.Zero;
            
                for(int j = 0; j < b.Points.Length; ++j) {
                    var pb = b.Points[j];
                    var dp = pb - pa;
                    var s = na.DotProduct(dp);

                    if (s < iMinSep) {
                        iMinSep = s;
                        vert = pb;
                    }
                }
            
                if (iMinSep > maxSep) {
                    maxSep = iMinSep;
                    bestNormal = na;
                    bestVert = vert;
                }
            
            }
        
            return new Contact(bestVert, bestNormal, maxSep, ContactType.Collide);
        }
    }

}