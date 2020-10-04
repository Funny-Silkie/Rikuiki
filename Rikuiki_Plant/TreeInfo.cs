using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rikuiki_Plant
{
    internal class TreeInfo
    {
        private readonly List<decimal> dbhs = new List<decimal>();
        private readonly HashSet<string> tags = new HashSet<string>();
        public int Count => tags.Count;
        public ReadOnlyCollection<decimal> DBHs { get; }
        public string Name { get; }
        public IEnumerable<string> Tags
        {
            get
            {
                foreach (var tag in tags) yield return tag;
            }
        }
        public TreeInfo(string name)
        {
            Name = name;
            DBHs = dbhs.AsReadOnly();
        }
        public void Add(string tag, decimal dbh)
        {
            dbhs.Add(dbh);
            tags.Add(tag);
        }
        public bool Calc(out decimal ba, out decimal dbhMax, out decimal dbhAve)
        {
            if (dbhs.Count == 0)
            {
                ba = default;
                dbhMax = default;
                dbhAve = default;
                return false;
            }
            dbhMax = decimal.MinValue;
            var sum = 0m;
            ba = 0m;
            foreach (var current in dbhs)
            {
                if (dbhMax < current) dbhMax = current;
                sum += current;
                ba += current * current / 4m * (decimal)Math.PI / 10000m;
            }
            dbhAve = sum / dbhs.Count;
            return true;
        }
    }
}
