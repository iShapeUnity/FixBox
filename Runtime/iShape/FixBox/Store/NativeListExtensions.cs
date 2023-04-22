using System;
using Unity.Collections;

namespace iShape.FixBox.Store {

    public static class NativeListExtensions
    {
        public static void Insert<T>(this NativeList<T> list, int index, T item) where T : unmanaged
        {
            int n = list.Length;
            if (index < 0 || index > n)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            if (index == n)
            {
                list.Add(item);
            }
            else
            {
                list.Add(default); // Add a default value to the end to resize the list

                var array = list.AsArray();
                int length = n - index;
                array.Slice(index + 1, length).CopyFrom(array.Slice(index, length));

                list[index] = item; // Set the value at the insertion index
            }
        }
    }

}