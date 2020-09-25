using System;
using System.Collections.Generic;

namespace Rikuiki
{
    [Serializable]
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        public IComparer<T> Comparer { get; }
        public ReverseComparer() : this(null) { }
        public ReverseComparer(IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
        }
        public int Compare(T x, T y) => -Comparer.Compare(x, y);
    }
}
