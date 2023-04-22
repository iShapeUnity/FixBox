using iShape.FixBox.Collision;
using iShape.FixBox.Store;
using iShape.FixFloat;
using Unity.Mathematics;

namespace iShape.FixBox.Dynamic {

    public static class WorldCollision {
        
        public static void Collide(this World world, BodyHandler a, BodyHandler b) {

            var contact = CollisionSolver.Collide(a.Body, b.Body);
            
            if (contact.Type != ContactType.Outside) {
                return;
            }

            var vA = a.Body.Velocity.Linear;
            var aNy = contact.A.Normal;

            var aPy = vA.DotProduct(aNy);
        
            if (aPy >= 0) {
                return;
            }

            var aNx = new FixVec(aNy.y, -aNy.x);

            var aPx = vA.DotProduct(aNx);

            var vNy = aNy * aPy;
            var vNx = aNx * aPx;

            var kb = math.max(a.Body.Material.Bounce, b.Body.Material.Bounce);
            
            var new_aVel = vNx - kb * vNy;
            var new_aBody = a.Body;
            new_aBody.Velocity.Linear = new_aVel;
            
            world.bodyStore.SetBody(new BodyHandler(a.Index, new_aBody));
            
            // TODO set B
        }
        
    }

}