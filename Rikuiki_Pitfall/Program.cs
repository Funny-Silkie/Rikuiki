using Rikuiki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rikuiki_Pitfall
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
                if (table.CountX < 6)
                {
                    Console.WriteLine($"データは6列以上からなる必要があります データ長：{table.CountX}");
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
                    if (!int.TryParse(table[5, y], out var count))
                    {
                        Console.WriteLine($"個体数の取り出しに失敗しました(X=5,Y={y})");
                        continue;
                    }
                    if (!int.TryParse(table[0, y], out var day))
                    {
                        Console.WriteLine($"日付の取り出しに失敗しました(X=0,Y={y})");
                        continue;
                    }
                    info.Add(table[3, y].TrimSpecies(), table[2, y], count, day - 20200909 + 1);
                }
                using var writer = new StreamWriter($"{Path.GetFileNameWithoutExtension(path)}_Calced{Path.GetExtension(path)}", false, encoding);
                writer.WriteLine("Site,Order,Count");
                writer.WriteLine(",,Day1,Day2,Day3");
                foreach (var (_, info) in dictionary)
                {
                    var set = new HashSet<string>();
                    foreach (var ((order, _), _) in info.Lives)
                    {
                        if (!set.Add(order)) continue;
                        var body = $"{info.Name},{order}";
                        for (int i = 1; i <= 3; i++)
                        {
                            info.Lives.TryGetValue((order, i), out var count);
                            body += $",{count}";
                        }
                        writer.WriteLine(body);
                    }
                }
                foreach (var (_, info) in dictionary)
                {
                    writer.WriteLine($"\n{info.Name}");
                    writer.WriteLine(",A,B,C,D");
                    for (int i = 1; i <= 4; i++)
                    {
                        info.Traps.TryGetValue($"A{i}", out var A);
                        info.Traps.TryGetValue($"B{i}", out var B);
                        info.Traps.TryGetValue($"C{i}", out var C);
                        info.Traps.TryGetValue($"D{i}", out var D);
                        writer.WriteLine($"{i},{A},{B},{C},{D}");
                    }
                    writer.WriteLine($"m*,{info.CalcMStar()}");
                }
                Console.WriteLine("Done");
            }
        }
    }
}
