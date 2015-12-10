namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class ArrayUtility
    {
        public static void Add<T>(ref T[] array, T item)
        {
            Array.Resize<T>(ref array, array.Length + 1);
            array[array.Length - 1] = item;
        }

        public static void AddRange<T>(ref T[] array, T[] items)
        {
            int length = array.Length;
            Array.Resize<T>(ref array, array.Length + items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                array[length + i] = items[i];
            }
        }

        public static bool ArrayEquals<T>(T[] lhs, T[] rhs)
        {
            if (lhs.Length != rhs.Length)
            {
                return false;
            }
            for (int i = 0; i < lhs.Length; i++)
            {
                if (!lhs[i].Equals(rhs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void Clear<T>(ref T[] array)
        {
            Array.Clear(array, 0, array.Length);
            Array.Resize<T>(ref array, 0);
        }

        public static bool Contains<T>(T[] array, T item)
        {
            List<T> list = new List<T>(array);
            return list.Contains(item);
        }

        public static T Find<T>(T[] array, Predicate<T> match)
        {
            List<T> list = new List<T>(array);
            return list.Find(match);
        }

        public static List<T> FindAll<T>(T[] array, Predicate<T> match)
        {
            List<T> list = new List<T>(array);
            return list.FindAll(match);
        }

        public static int FindIndex<T>(T[] array, Predicate<T> match)
        {
            List<T> list = new List<T>(array);
            return list.FindIndex(match);
        }

        public static int IndexOf<T>(T[] array, T value)
        {
            List<T> list = new List<T>(array);
            return list.IndexOf(value);
        }

        public static void Insert<T>(ref T[] array, int index, T item)
        {
            ArrayList list = new ArrayList();
            list.AddRange(array);
            list.Insert(index, item);
            array = list.ToArray(typeof(T)) as T[];
        }

        public static int LastIndexOf<T>(T[] array, T value)
        {
            List<T> list = new List<T>(array);
            return list.LastIndexOf(value);
        }

        public static void Remove<T>(ref T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Remove(item);
            array = list.ToArray();
        }

        public static void RemoveAt<T>(ref T[] array, int index)
        {
            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            array = list.ToArray();
        }
    }
}

