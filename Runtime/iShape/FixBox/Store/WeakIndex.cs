using iShape.FixBox.Dynamic;

namespace iShape.FixBox.Store {

    public readonly struct WeakIndex {

        internal readonly long Id;
        internal readonly int Index;
        internal readonly BodyType Type;
        internal readonly int TimeStamp;

        internal WeakIndex(long id, int index, int timeStamp, BodyType type) {
            Index = index;
            Id = id;
            TimeStamp = timeStamp;
            Type = type;
        }
        
        internal WeakIndex(WeakIndex weakIndex, int index, int timeStamp) {
            Index = index;
            Id = weakIndex.Id;
            TimeStamp = timeStamp;
            Type = weakIndex.Type;
        }
    }
}