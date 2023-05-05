using iShape.FixBox.Collision;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;


namespace iShape.FixBox.Collider {

    public static class ColliderSolver_ConvexToConvex {
        
        public static Contact Collide(ConvexCollider a, ConvexCollider b, Transform tA, Transform tB) {
            var ciA = new CircleCollider(tA.Position, a.Radius);
            var ciB = new CircleCollider(tB.Position, b.Radius);
            var ciContact = ColliderSolver_CircleToCircle.Collide(ciA, ciB);
            if (ciContact.Type != ContactType.Outside) {
                return ciContact;
            }
            
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
                return Contact.Outside;
            }
            
            // return in global coord system
            return t.Convert(contact);
        }

        
        private static Contact Collide(ConvexCollider a, ConvexCollider b) {
            var cA = FindContact(a, b);
            var cB = FindContact(b, a);
            
            if (cA.Type == ContactType.Collide && cB.Type == ContactType.Collide) {
                var middle = cA.Point.Middle(cB.Point);
                var penetration = (cA.Penetration + cB.Penetration) / 2;
                var count = (cA.Count + cB.Count) >> 1;
                FixVec normal = cB.Penetration < cA.Penetration ? cB.Normal : cA.Normal;

                return new Contact(middle, normal, penetration, count, ContactType.Collide);
            } else if (cB.Type == ContactType.Collide) {
                return cB;
            } else if (cA.Type == ContactType.Collide) {
                return cA;
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