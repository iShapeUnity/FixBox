using iShape.FixBox.Collider;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Collision {

    public static class CollisionSolver_ConvexToConvex {
        
        public static Contact Collide(ConvexCollider a, ConvexCollider b, Transform tA, Transform tB) {
            if (a.Points.Length > b.Points.Length) {
                // work in A coord system

                var mt = Transform.ConvertFromBtoA(tB, tA);
                
                var b2 = new ConvexCollider(mt, b, Allocator.Temp);

                var contact = Collide(a, b2);
                b2.Dispose();

                // return in global coord system
                return tA.Convert(contact);
            } else {
                // work in B coord system

                var mt = Transform.ConvertFromBtoA(tA, tB);
                
                var a2 = new ConvexCollider(mt, a, Allocator.Temp);

                var contact = Collide(a2, b);
                a2.Dispose();
                
                if (contact.Type != ContactType.Collide) {
                    return Contact.Outside;
                }

                // return in global coord system
                return tB.Convert(contact);
            }
        }

        // Normal is always look at A * <-| * B        
        private static Contact Collide(ConvexCollider a, ConvexCollider b) {
            var cA = FindContact(a, b);
            var cB = FindContact(b, a);
            
            if (cA.Type == ContactType.Collide && cB.Type == ContactType.Collide) {
                var middle = cA.Point.Middle(cB.Point);
                var penetration = (cA.Penetration + cB.Penetration) / 2;
                var count = (cA.Count + cB.Count) >> 1;

                FixVec normal;
                if (cA.Penetration < cB.Penetration) {
                    normal = cA.Normal.Negative;
                } else {
                    normal = cB.Normal;
                }
                return new Contact(middle, normal, penetration, count, ContactType.Collide);
            } else if (cB.Type == ContactType.Collide) {
                return cB;
            } else if (cA.Type == ContactType.Collide) {
                return cA.NegativeNormal();
            } else {
                return Contact.Outside;
            }
        }
        
        private static Contact FindContact(ConvexCollider a, ConvexCollider b) {
            var contact_0 = new Contact(FixVec.Zero, FixVec.Zero, long.MaxValue, 1, ContactType.Outside);
            var contact_1 = new Contact(FixVec.Zero, FixVec.Zero, long.MaxValue, 1, ContactType.Outside);

            for (int i = 0; i < b.Points.Length; ++i) {
                var vert = b.Points[i];
                if (!a.IsContain(vert))
                {
                    continue;
                }

                var sv = long.MinValue;
                var nv = FixVec.Zero;
            
                for (int j = 0; j < a.Points.Length; ++j) {
                    var n = a.Normals[j];
                    var p = a.Points[j];

                    var d = vert - p;
                    var s = n.DotProduct(d);
                
                    if (s > sv) {
                        sv = s;
                        nv = n;
                    }
                }
            
                if (sv < contact_1.Penetration) {
                    var newContact = new Contact(vert, nv, sv, 1, ContactType.Collide);

                    if (newContact.Penetration < contact_0.Penetration) {
                        contact_0 = newContact;
                    } else {
                        contact_1 = newContact;
                    }
                }
            }
            
            if (contact_1.Type == ContactType.Collide) {
                var mid = contact_0.Point.Middle(contact_1.Point);
                return new Contact(mid, normal: contact_0.Normal, contact_0.Penetration, 2, ContactType.Collide);
            } else if (contact_0.Type == ContactType.Collide) {
                return contact_0;
            } else {
                return Contact.Outside;                
            }
        }

    }

}