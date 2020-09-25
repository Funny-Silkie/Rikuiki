using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rikuiki_Pitfall
{
    internal class SiteInfo
    {
        public ReadOnlyDictionary<(string, int), int> Lives { get; }
        private readonly Dictionary<(string, int), int> lives = new Dictionary<(string, int), int>();
        public string Name { get; }
        public ReadOnlyDictionary<string, int> Traps { get; }
        private readonly Dictionary<string, int> traps = new Dictionary<string, int>(16);
        public SiteInfo(string name)
        {
            Name = name;
            Lives = new ReadOnlyDictionary<(string, int), int>(lives);
            Traps = new ReadOnlyDictionary<string, int>(traps);
        }
        public void Add(string order, string trapName, int count, int day)
        {
            lives.TryGetValue((order, day), out var value);
            lives[(order, day)] = value + count;
            traps.TryGetValue(trapName, out value);
            traps[trapName] = value + count;
        }
        public decimal CalcMStar()
        {
            var array = new int[16];
            var sum = 0;
            for (int i = 0; i < 4; i++)
            {
                traps.TryGetValue($"A{i}", out var A);
                traps.TryGetValue($"B{i}", out var B);
                traps.TryGetValue($"C{i}", out var C);
                traps.TryGetValue($"D{i}", out var D);
                sum += A + B + C + D;
                array[0 + i * 4] = A;
                array[1 + i * 4] = B;
                array[2 + i * 4] = C;
                array[3 + i * 4] = D;
            }
            var result = 0m;
            foreach (var current in array) result += current * (current - 1);
            return result / sum;
        }
    }
}
