using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct BodyIndex {

        internal readonly long Id;
        internal readonly int Index;
        internal readonly BodyType Type;
        internal readonly int TimeStamp;

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