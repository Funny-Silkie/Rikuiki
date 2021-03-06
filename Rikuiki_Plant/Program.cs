using Rikuiki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rikuiki_Plant
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
                Console.WriteLine($"Data Count: {table.CountY - 2}");
                var dictionary = new Dictionary<string, TreeInfo>();
                for (int i = 2; i < table.CountY; i++)
                {
                    if (!decimal.TryParse(table[4, i], out var dbh))
                    {
                        Console.WriteLine($"DBHの値を取り出せませんでした(X={5},Y={i + 1})");
                        continue;
                    }
                    if (dbh < 5m) continue;
                    if (!dictionary.TryGetValue(table[3, i], out var info))
                    {
                        info = new TreeInfo(table[3, i]);
                        dictionary.Add(info.Name, info);
                    }
                    info.Add(table[0, i], dbh);
                }
                var BAList = new List<decimal>();
                var entryList = new List<(string name, decimal dphAve, decimal dbhMax, int count, int individuals)>();
                foreach (var (_, info) in dictionary)
                    if (info.Calc(out var ba, out var dbhMax, out var dbhAve))
                    {
                        BAList.Add(ba);
                        entryList.Add((info.Name, dbhAve, dbhMax, info.DBHs.Count, info.Count));
                    }
                var BAArray = BAList.ToArray();
                var entryArray = entryList.ToArray();
                Array.Sort(BAArray, entryArray, new ReverseComparer<decimal>());
                using var writer = new StreamWriter($"{Path.GetFileNameWithoutExtension(path)}_Calced{Path.GetExtension(path)}", false, encoding);
                writer.WriteLine("Species,BA,,,DBH,,Count,,,Individuals");
                writer.WriteLine(",(m^2/1.1ha),(m^2/ha),(%),Ave,Max,(/1.1ha),(%),(/ha)");
                var baSum = BAArray.Sum(x => x / 1.1m);
                var countSum = entryArray.Sum(x => x.count);
                for (int i = 0; i < BAArray.Length; i++)
                {
                    var (name, dbhAve, dbhMax, count, individuals) = entryArray[i];
                    var ba_ha = BAArray[i] / 1.1m;
                    writer.WriteLine($"{name},{BAArray[i]},{ba_ha},{ba_ha * 100m / baSum},{dbhAve},{dbhMax},{count},{count * 100m / countSum},{count / 1.1m},{individuals}");
                }
                writer.WriteLine($"Sum,{baSum * 1.1m},{baSum},,{entryArray.Sum(x => x.dphAve)},{entryArray.Sum(x => x.dbhMax)},{countSum},,{countSum / 1.1m},{entryArray.Sum(x => x.individuals)}");
                writer.WriteLine();
                for (int i = 0; i < 5; i++)
                {
                    var info = dictionary[entryArray[i].name];
                    var dic = new Dictionary<int, int>();
                    foreach (var current in info.DBHs)
                    {
                        var value = (int)current / 5 * 5;
                        Console.WriteLine(value);
                        if (dic.TryGetValue(value, out var count)) dic[value] = count + 1;
                        else dic.Add(value, 1);
                    }
                    var max = dic.Max(x => x.Key);
                    var head = string.Empty;
                    var body = string.Empty;
                    for (int j = 0; j < max / 5 + 1; j++)
                    {
                        if (j != 0)
                        {
                            body += ',';
                            head += ',';
                        }
                        dic.TryGetValue(j * 5, out var count);
                        body += count.ToString();
                        head += ((j + 1) * 5).ToString();
                    }
                    writer.WriteLine(info.Name);
                    writer.WriteLine(head);
                    writer.WriteLine(body);
                }
                Console.WriteLine("Done");
            }
        }
    }
}
