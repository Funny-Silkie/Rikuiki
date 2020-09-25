using Rikuiki;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rikuiki_Simpson
{
    internal class SiteInfo
    {
        public ReadOnlyDictionary<string, int> Lives { get; }
        private readonly Dictionary<string, int> lives = new Dictionary<string, int>();
        public string Name { get; }
        public SiteInfo(string name)
        {
            Name = name;
            Lives = new ReadOnlyDictionary<string, int>(lives);
        }
        public void Add(string order, int count)
        {
            lives.TryGetValue(order, out var value);
            lives[order] = value + count;
        }
        public decimal? CalcLamda()
        {
            var sum = lives.Sum(x => x.Value);
            if (sum == 1) return null;
            return lives.Sum(x => x.Value * (x.Value - 1)) / sum / (sum - 1);
        }
        public Dictionary<string, decimal> CalcP()
        {
            var result = new Dictionary<string, decimal>(lives.Count);
            var sum = lives.Sum(x => x.Value);
            foreach (var (name, count) in lives) result.Add(name, count / sum);
            return result;
        }
    }
}
