﻿using System;
using System.Collections.Generic;

namespace Rikuiki
{
    public static class Extension
    {
        public static T[,] Convert<T>(this T[][] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
            var lengthY = array.Length;
            if (lengthY == 0) return new T[0, 0];
            var lengthX = array.Max(x => x.Length);
            //var lengthX = array[0].Length;
            var result = new T[lengthX, lengthY];
            for (int y = 0; y < lengthY; y++)
                for (int x = 0; x < array[y].Length; x++)
                {
                    if (array[y].Length == 0) continue;
                    result[x, y] = array[y][x];
                }
            //for (int x = 0; x < lengthX; x++)
            //{
            //    if (array[y].Length != lengthX) throw new ArgumentException(null, nameof(array));
            //    result[x, y] = array[y][x];
            //}
            return result;
        }
        public static T GenericClone<T>(this T value) where T : ICloneable => value != null ? (T)value.Clone() : default;
        public static int Max<T>(this IEnumerable<T> collection, Func<T, int> converter)
        {
            var result = int.MinValue;
            foreach (var current in collection)
            {
                var value = converter.Invoke(current);
                if (result < value) result = value;
            }
            return result;
        }
        public static decimal Max<T>(this IEnumerable<T> collection, Func<T, decimal> converter)
        {
            var result = decimal.MinValue;
            foreach (var current in collection)
            {
                var value = converter.Invoke(current);
                if (result < value) result = value;
            }
            return result;
        }
        public static int Min<T>(this IEnumerable<T> collection, Func<T, int> converter)
        {
            var result = int.MaxValue;
            foreach (var current in collection)
            {
                var value = converter.Invoke(current);
                if (result > value) result = value;
            }
            return result;
        }
        public static decimal Min<T>(this IEnumerable<T> collection, Func<T, decimal> converter)
        {
            var result = decimal.MaxValue;
            foreach (var current in collection)
            {
                var value = converter.Invoke(current);
                if (result > value) result = value;
            }
            return result;
        }
        public static decimal Sum<T>(this IEnumerable<T> collection, Func<T, decimal> converter)
        {
            var result = 0m;
            foreach (var current in collection) result += converter.Invoke(current);
            return result;
        }
        public static decimal Sum(this IEnumerable<decimal> collection)
        {
            var result = 0m;
            foreach (var current in collection) result += current;
            return result;
        }
    }
}
