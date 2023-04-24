using System.Runtime.CompilerServices;
using iShape.FixBox.Dynamic;
using Unity.Collections;
using UnityEngine.Assertions;


namespace iShape.FixBox.Store {
    
    public struct SortedList {
        
        private int timeStamp;
        
        // TODO switch to Native Array
        private NativeList<long> ids;
        public NativeList<Body> Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SortedList(int capacity, Allocator allocator)
        {
            ids = new NativeList<long>(capacity, allocator);
            Items = new NativeList<Body>(capacity, allocator);
            timeStamp = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor Get(BodyIndex bodyIndex)
        {
            int index;
            if (bodyIndex.TimeStamp == timeStamp) {
                index = bodyIndex.Index;
            } else {
                index = ids.FindIndex(value: bodyIndex.Id);    
            }

            var newBodyIndex = new BodyIndex(bodyIndex.Id, index, timeStamp, bodyIndex.Type);

            return new Actor(newBodyIndex, Items[index]);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BodyIndex Set(Actor actor)
        {
            int index;
            if (actor.Index.TimeStamp == timeStamp) {
                index = actor.Index.Index;
            } else {
                index = ids.FindIndex(value: actor.Index.Id);    
            }

            Items[index] = actor.Body;

            return new BodyIndex(actor.Index, index, timeStamp);
        }
        
        public BodyIndex Add(Body body)
        {
            if (ids.Length == 0)
            {
                ids.Add(body.Id);
                Items.Add(body);
                return new BodyIndex(body.Id, 0, timeStamp, body.Type);
            }

            int index = ids.FindFreeIndex(body.Id);
            Assert.AreNotEqual(-1, index, "Index should not be -1");

            if (index != ids.Length) {
                timeStamp += 1;
            }

            ids.Insert(index, body.Id);
            Items.Insert(index, body);
            
            return new BodyIndex(body.Id, index, timeStamp, body.Type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(BodyIndex bodyIndex) {
            int index;
            if (bodyIndex.TimeStamp == timeStamp) {
                index = bodyIndex.Index;
            } else {
                index = ids.FindIndex(value: bodyIndex.Id);    
            }
            
            Assert.AreNotEqual(-1, index, "Index should not be -1");

            timeStamp += 1;
            
            ids.RemoveAt(index);
            Items.RemoveAt(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAll() {
            ids.Clear();
            Items.Clear();
            timeStamp += 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ids.Dispose();
            Items.Dispose();
        }
    }

}

public static class ListExtensions
{
    public static int FindFreeIndex(this NativeList<long> list, long value)
    {
        int left = 0;
        int right = list.Length - 1;
        int j = -1;
        int i = right >> 1;
        long x = list[i];

        while (i != j)
        {
            if (x > value)
            {
                right = i - 1;
            }
            else if (x < value)
            {
                left = i + 1;
            }
            else
            {
                return -1;
            }

            j = i;
            i = (left + right) >> 1;
            x = list[i];
        }

        if (x < value)
        {
            i += 1;
        }

        return i;
    }

    public static int FindIndex(this NativeList<long> list, long value)
    {
        int left = 0;
        int right = list.Length - 1;
        int j = -1;
        int i = right >> 1;
        long x = list[i];

        while (i != j)
        {
            if (x > value)
            {
                right = i - 1;
            }
            else if (x < value)
            {
                left = i + 1;
            }
            else
            {
                return i;
            }

            j = i;
            i = (left + right) / 2;
            x = list[i];
        }

        return -1;
    }
}