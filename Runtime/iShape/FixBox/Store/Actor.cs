using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct Actor {
        
        public readonly BodyIndex Index;
        public readonly Body Body;

        public Actor(BodyIndex index, Body body) {
            Index = index;
            Body = body;
        }
    }

}