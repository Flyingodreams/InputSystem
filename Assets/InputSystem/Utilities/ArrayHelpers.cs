using System;
using System.Collections.Generic;
using System.Linq;

namespace ISX.Utilities
{
    // A collection of utility functions to work with arrays.
    internal static class ArrayHelpers
    {
        public static bool Contains<TValue>(TValue[] array, TValue value)
        {
            if (array == null)
                return false;

            var comparer = EqualityComparer<TValue>.Default;
            for (var i = 0; i < array.Length; ++i)
                if (comparer.Equals(array[i], value))
                    return true;

            return false;
        }

        public static int IndexOf<TValue>(ref TValue[] array, TValue value)
        {
            if (array == null)
                return -1;

            var length = array.Length;
            var comparer = EqualityComparer<TValue>.Default;
            for (var i = 0; i < length; ++i)
                if (comparer.Equals(array[i], value))
                    return i;

            return -1;
        }

        public static int Append<TValue>(ref TValue[] array, TValue value)
        {
            if (array == null)
            {
                array = new TValue[1];
                array[0] = value;
                return 0;
            }

            var length = array.Length;
            Array.Resize(ref array, length + 1);
            array[length] = value;
            return length;
        }

        public static int Append<TValue>(ref TValue[] array, IEnumerable<TValue> values)
        {
            if (array == null)
            {
                array = values.ToArray();
                return 0;
            }

            var oldLength = array.Length;
            var valueCount = values.Count();

            Array.Resize(ref array, oldLength + valueCount);

            var index = oldLength;
            foreach (var value in values)
                array[index++] = value;

            return oldLength;
        }

        // Append to an array that is considered immutable. This allows using 'values' as is
        // if 'array' is null.
        // Returns the index of the first newly added element in the resulting array.
        public static int AppendToImmutable<TValue>(ref TValue[] array, TValue[] values)
        {
            if (array == null)
            {
                array = values;
                return 0;
            }

            if (values != null && values.Length > 0)
            {
                var oldCount = array.Length;
                var valueCount = values.Length;
                Array.Resize(ref array, oldCount + valueCount);
                Array.Copy(values, 0, array, oldCount, valueCount);
                return oldCount;
            }

            return array.Length;
        }

        public static int AppendWithCapacity<TValue>(ref TValue[] array, ref int count, TValue value, int capacityIncrement = 10)
        {
            if (array == null)
            {
                array = new TValue[capacityIncrement];
                array[0] = value;
                ++count;
                return 0;
            }

            var capacity = array.Length;
            if (capacity == count)
            {
                capacity += capacityIncrement;
                Array.Resize(ref array, capacity);
            }

            var index = count;
            array[index] = value;
            ++count;

            return index;
        }

        public static void InsertAt<TValue>(ref TValue[] array, int index, TValue value)
        {
            if (array == null)
            {
                ////REVIEW: allow growing array to specific size by inserting at arbitrary index?
                if (index != 0)
                    throw new IndexOutOfRangeException();

                array = new TValue[1];
                array[0] = value;
                return;
            }

            // Reallocate.
            var oldLength = array.Length;
            Array.Resize(ref array, oldLength + 1);

            // Make room for element.
            if (index != oldLength)
                Array.Copy(array, index, array, index + 1, oldLength - index);

            array[index] = value;
        }

        // Adds 'count' entries to the array. Returns first index of newly added entries.
        public static int GrowBy<TValue>(ref TValue[] array, int count)
        {
            if (array == null)
            {
                array = new TValue[count];
                return 0;
            }

            var oldLength = array.Length;
            Array.Resize(ref array, oldLength + count);
            return oldLength;
        }

        public static TValue[] Join<TValue>(TValue value, params TValue[] values)
        {
            // Determine length.
            var length = 0;
            if (value != null)
                ++length;
            if (values != null)
                length += values.Length;

            if (length == 0)
                return null;

            var array = new TValue[length];

            // Populate.
            var index = 0;
            if (value != null)
                array[index++] = value;

            if (values != null)
                Array.Copy(values, 0, array, index, length);

            return array;
        }

        public static TValue[] Merge<TValue>(TValue[] first, TValue[] second)
            where TValue : IEquatable<TValue>
        {
            if (first == null)
                return second;
            if (second == null)
                return null;

            var merged = new List<TValue>();
            merged.AddRange(first);

            for (var i = 0; i < second.Length; ++i)
            {
                var secondValue = second[i];
                if (!merged.Exists(x => x.Equals(secondValue)))
                {
                    merged.Add(secondValue);
                }
            }

            return merged.ToArray();
        }

        public static TValue[] Merge<TValue>(TValue[] first, TValue[] second, IEqualityComparer<TValue> comparer)
        {
            if (first == null)
                return second;
            if (second == null)
                return null;

            var merged = new List<TValue>();
            merged.AddRange(first);

            for (var i = 0; i < second.Length; ++i)
            {
                var secondValue = second[i];
                if (!merged.Exists(x => comparer.Equals(secondValue)))
                {
                    merged.Add(secondValue);
                }
            }

            return merged.ToArray();
        }

        public static void EraseAt<TValue>(ref TValue[] array, int index)
        {
            if (array == null)
                return;

            var length = array.Length;

            if (index >= length)
                throw new IndexOutOfRangeException();

            if (index == 0 && length == 1)
            {
                array = null;
                return;
            }

            if (index < length - 1)
                Array.Copy(array, index + 1, array, index, length - index - 1);

            Array.Resize(ref array, length - 1);
        }

        public static void Erase<TValue>(ref TValue[] array, TValue value)
        {
            var index = IndexOf(ref array, value);
            if (index != -1)
                EraseAt(ref array, index);
        }

        public static TValue[] Clone<TValue>(TValue[] array)
            where TValue : ICloneable
        {
            if (array == null)
                return null;

            var count = array.Length;
            var result = new TValue[count];

            for (var i = 0; i < count; ++i)
                result[i] = (TValue)array[i].Clone();

            return result;
        }

        public static TNew[] Select<TOld, TNew>(TOld[] array, Func<TOld, TNew> converter)
        {
            if (array == null)
                return null;

            var length = array.Length;
            var result = new TNew[length];

            for (var i = 0; i < length; ++i)
                result[i] = converter(array[i]);

            return result;
        }
    }
}
