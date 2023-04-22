using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct BodyHandler {
        
        public readonly int Index;
        public readonly Body Body;

        public BodyHandler(int index, Body body) {
            Index = index;
            Body = body;
        }
    }
}