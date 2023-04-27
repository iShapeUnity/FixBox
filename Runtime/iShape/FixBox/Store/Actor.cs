using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public struct Actor {
        
        public BodyIndex Index;
        public Body Body;

        public Actor(BodyIndex index, Body body) {
            Index = index;
            Body = body;
        }
    }

}