using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct Actor {
        
        public readonly WeakIndex Index;
        public readonly Body Body;

        public Actor(WeakIndex index, Body body) {
            Index = index;
            Body = body;
        }
    }

}