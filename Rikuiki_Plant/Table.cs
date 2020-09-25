using System;
using System.Collections;
using System.Collections.Generic;

namespace Rikuiki_Plant
{
    [Serializable]
    public class Table : ICollection<string>, IReadOnlyCollection<string>, ICollection
    {
        private readonly string[,] items;
        public int Count => CountX * CountY;
        public int CountX { get; }
        public int CountY { get; }
        bool ICollection<string>.IsReadOnly => true;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null) System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }
        [NonSerialized]
        private object _syncRoot;
        public Table(string[,] array)
        {
            items = array.GenericClone();
            CountX = items.GetLength(0);
            CountY = items.GetLength(1);
        }
        public string this[int x, int y] => items[x, y];
        private static bool IsString(object value, out string item)
        {
            if (value == null)
            {
                item = null;
                return true;
            }
            if (value is string s)
            {
                item = s;
                return true;
            }
            item = null;
            return false;
        }
        void ICollection<string>.Add(string item) => throw new NotSupportedException();
        internal void Clear() => Array.Clear(items, 0, items.Length);
        void ICollection<string>.Clear() => throw new NotSupportedException();
        bool ICollection<string>.Contains(string item) => Array.IndexOf(items, item) >= 0;
        public void CopyTo(string[,] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
            if (array.GetLength(0) < CountX) throw new ArgumentException(null, nameof(array));
            if (array.GetLength(1) < CountY) throw new ArgumentException(null, nameof(array));
            for (int y = 0; y < CountY; y++)
                for (int x = 0; x < CountX; x++)
                    array[x, y] = items[x, y];
        }
        public void CopyTo(string[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
            if (array.Length < Count + arrayIndex) throw new ArgumentException(null, nameof(array));
            for (int y = 0; y < CountY; y++)
                for (int x = 0; x < CountX; x++)
                    array[y * CountX + x + arrayIndex] = items[x, y];
        }
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
            switch (array)
            {
                case string[,] s: CopyTo(s); break;
                case object[,] o:
                    try
                    {
                        if (o.GetLength(0) < CountX) throw new ArgumentException(null, nameof(array));
                        if (o.GetLength(1) < CountY) throw new ArgumentException(null, nameof(array));
                        for (int y = 0; y < CountY; y++)
                            for (int x = 0; x < CountX; x++)
                                o[x, y] = items[x, y];
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(null, nameof(array));
                    }
                    break;
                case string[] s: CopyTo(s, index); break;
                case object[] o:
                    try
                    {
                        if (o.Length < Count + index) throw new ArgumentException(null, nameof(array));
                        for (int y = 0; y < CountY; y++)
                            for (int x = 0; x < CountX; x++)
                                o[y * CountX + x + index] = items[x, y];
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(null, nameof(array));
                    }
                    break;
                default: throw new ArgumentException(null, nameof(array));
            }
        }
        public IEnumerator<string> GetEnumerator()
        {
            for (int y = 0; y < CountY; y++)
                for (int x = 0; x < CountX; x++)
                    yield return items[x, y];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        bool ICollection<string>.Remove(string item) => throw new NotSupportedException();
        [Serializable]
        public sealed class XFixedList : IList<string>, IReadOnlyList<string>, IList
        {
            private readonly Table table;
            public int Count => table.CountY;
            bool IList.IsFixedSize => true;
            bool ICollection<string>.IsReadOnly => true;
            bool IList.IsReadOnly => true;
            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => ((ICollection)table).SyncRoot;
            public int X { get; }
            internal XFixedList(Table table, int x)
            {
                this.table = table;
                X = x;
            }
            public string this[int y] => table.items[X, y];
            object IList.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            string IList<string>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            void ICollection<string>.Add(string item) => throw new NotSupportedException();
            int IList.Add(object value) => throw new NotSupportedException();
            void ICollection<string>.Clear() => throw new NotSupportedException();
            void IList.Clear() => throw new NotSupportedException();
            public bool Contains(string item) => IndexOf(item) >= 0;
            bool IList.Contains(object value) => IsString(value, out var item) && Contains(item);
            public void CopyTo(string[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
                if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length < arrayIndex + Count) throw new ArgumentException(null, nameof(array));
                for (int i = 0; i < Count; i++) array[arrayIndex++] = table.items[X, i];
            }
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
                if (array.Rank != 1) throw new ArgumentException(null, nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length < index + Count || array.GetLowerBound(0) != 0) throw new ArgumentException(null, nameof(array));
                switch (array)
                {
                    case string[] s: CopyTo(s, index); break;
                    case object[] o:
                        try
                        {
                            for (int i = 0; i < Count; i++) o[index++] = table.items[X, i];
                        }
                        catch (ArrayTypeMismatchException)
                        {
                            throw new ArgumentException(null, nameof(array));
                        }
                        break;
                    default: throw new ArgumentException(null, nameof(array));
                }
            }
            public IEnumerator<string> GetEnumerator()
            {
                for (int i = 0; i < Count; i++) yield return table.items[X, i];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public int IndexOf(string item)
            {
                for (int i = 0; i < Count; i++)
                    if (table.items[X, i] == item)
                        return i;
                return -1;
            }
            int IList.IndexOf(object value) => IsString(value, out var item) ? IndexOf(item) : -1;
            void IList.Insert(int index, object value) => throw new NotSupportedException();
            void IList<string>.Insert(int index, string item) => throw new NotSupportedException();
            bool ICollection<string>.Remove(string item) => throw new NotSupportedException();
            void IList.Remove(object value) => throw new NotSupportedException();
            void IList.RemoveAt(int index) => throw new NotSupportedException();
            void IList<string>.RemoveAt(int index) => throw new NotSupportedException();
        }
        [Serializable]
        public sealed class YFixedList : IList<string>, IReadOnlyList<string>, IList
        {
            private readonly Table table;
            public int Count => table.CountX;
            bool IList.IsFixedSize => true;
            bool ICollection<string>.IsReadOnly => true;
            bool IList.IsReadOnly => true;
            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => ((ICollection)table).SyncRoot;
            public int Y { get; }
            internal YFixedList(Table table, int y)
            {
                this.table = table;
                Y = y;
            }
            public string this[int x] => table.items[x, Y];
            object IList.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            string IList<string>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            void ICollection<string>.Add(string item) => throw new NotSupportedException();
            int IList.Add(object value) => throw new NotSupportedException();
            void ICollection<string>.Clear() => throw new NotSupportedException();
            void IList.Clear() => throw new NotSupportedException();
            public bool Contains(string item) => IndexOf(item) >= 0;
            bool IList.Contains(object value) => IsString(value, out var item) && Contains(item);
            public void CopyTo(string[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
                if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length < arrayIndex + Count) throw new ArgumentException(null, nameof(array));
                for (int i = 0; i < Count; i++) array[arrayIndex++] = table.items[i, Y];
            }
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null) throw new ArgumentNullException(nameof(array), "引数がnullです");
                if (array.Rank != 1) throw new ArgumentException(null, nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length < index + Count || array.GetLowerBound(0) != 0) throw new ArgumentException(null, nameof(array));
                switch (array)
                {
                    case string[] s: CopyTo(s, index); break;
                    case object[] o:
                        try
                        {
                            for (int i = 0; i < Count; i++) o[index++] = table.items[i, Y];
                        }
                        catch (ArrayTypeMismatchException)
                        {
                            throw new ArgumentException(null, nameof(array));
                        }
                        break;
                    default: throw new ArgumentException(null, nameof(array));
                }
            }
            public IEnumerator<string> GetEnumerator()
            {
                for (int i = 0; i < Count; i++) yield return table.items[i, Y];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public int IndexOf(string item)
            {
                for (int i = 0; i < Count; i++)
                    if (table.items[i, Y] == item)
                        return i;
                return -1;
            }
            int IList.IndexOf(object value) => IsString(value, out var item) ? IndexOf(item) : -1;
            void IList.Insert(int index, object value) => throw new NotSupportedException();
            void IList<string>.Insert(int index, string item) => throw new NotSupportedException();
            bool ICollection<string>.Remove(string item) => throw new NotSupportedException();
            void IList.Remove(object value) => throw new NotSupportedException();
            void IList.RemoveAt(int index) => throw new NotSupportedException();
            void IList<string>.RemoveAt(int index) => throw new NotSupportedException();
        }
    }
}
