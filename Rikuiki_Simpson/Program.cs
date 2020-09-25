using Rikuiki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rikuiki_Simpson
{
    class Program
    {
        static void Main(string[] args)
        {
            args ??= Array.Empty<string>();
            if (args.Length == 0)
            {
                Console.WriteLine("読み込むファイルパスを入力してください");
                while (true)
                {
                    var path = Console.ReadLine();
                    if (File.Exists(path))
                    {
                        args = new[] { path };
                        break;
                    }
                    Console.WriteLine("ファイルパスが存在しません");
                }
            }
            var encoding = new UTF8Encoding(true);
            foreach (var path in args)
            {
                Console.WriteLine($"\n{path}");
                if (!File.Exists(path))
                {
                    Console.WriteLine("パスが存在しません");
                    continue;
                }
                Console.WriteLine("Reading File");
                using var reader = new StreamReader(path, encoding);
                var list = new List<string[]>();
                while (!reader.EndOfStream) list.Add(reader.ReadLine().Split(','));
                Table table;
                try
                {
                    table = new Table(list.ToArray().Convert());
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("データに欠落or冗長があります");
                    continue;
                }
                if (table.CountX < 5)
                {
                    Console.WriteLine($"データは5列以上からなる必要があります データ長：{table.CountX}");
                    continue;
                }
                for (int y = 0; y < table.CountY; y++)
                {
                    var str = string.Empty;
                    for (int x = 0; x < table.CountX; x++)
                    {
                        if (x != 0) str += ',';
                        str += table[x, y];
                    }
                    Console.WriteLine(str);
                }
                var dictionary = new Dictionary<string, SiteInfo>(4);
                for (int y = 1; y < table.CountY; y++)
                {
                    if (!dictionary.TryGetValue(table[1, y], out var info))
                    {
                        info = new SiteInfo(table[1, y]);
                        dictionary.Add(info.Name, info);
                    }
                    if (!int.TryParse(table[4, y], out var count))
                    {
                        Console.WriteLine($"個体数の取り出しに失敗しました(X=4,Y={y})");
                        continue;
                    }
                    info.Add(table[2, y].TrimSpecies(), count);
                }
                using var writer = new StreamWriter($"{Path.GetFileNameWithoutExtension(path)}_Calced{Path.GetExtension(path)}", false, encoding);
                writer.WriteLine("Site,Order,Count,p");
                var dDictionary = new Dictionary<string, decimal>(4);
                foreach (var (_, info) in dictionary)
                {
                    var pList = new List<decimal>(dictionary.Count);
                    var pDictionary = info.CalcP();
                    foreach (var (order, count) in info.Lives)
                    {
                        var p = pDictionary[order];
                        pList.Add(p);
                        writer.WriteLine($"{info.Name},{order},{count},{p}");
                    }
                    dDictionary.Add(info.Name, pList.Sum(x => x * x));
                }
                writer.WriteLine();
                writer.WriteLine($"Site,D,λ,1/λ");
                foreach (var (_, info) in dictionary)
                {
                    var D = dDictionary[info.Name];
                    var lamda = info.CalcLamda();
                    writer.WriteLine($"{info.Name},{D},{(lamda.HasValue ? lamda.Value.ToString() : "#Value")},{(!lamda.HasValue || lamda.Value == 0 ? "#VALUE" : (1 / lamda.Value).ToString())}");
                }
                Console.WriteLine("Done");
            }
        }
    }
}
