using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct BodyIndex {

        public static readonly BodyIndex Empty = new BodyIndex(-1, -1, -1, BodyType.player);
        
        internal readonly long Id;
        internal readonly int Index;
        internal readonly BodyType Type;
        internal readonly int TimeStamp;

        public BodyIndex(long id, BodyType type) {
            Index = -1;
            Id = id;
            TimeStamp = -1;
            Type = type;
        }
        
        internal BodyIndex(long id, int index, int timeStamp, BodyType type) {
            Index = index;
            Id = id;
            TimeStamp = timeStamp;
            Type = type;
        }
        
        internal BodyIndex(BodyIndex bodyIndex, int index, int timeStamp) {
            Index = index;
            Id = bodyIndex.Id;
            TimeStamp = timeStamp;
            Type = bodyIndex.Type;
        }
    }
}