using System;
using System.Collections.Generic;
using System.IO;

namespace QuickCount
{
    class ProgramQuickCount
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start QuickCount");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Restart();
            string filename = @"..\..\..\..\..\WordCount\Text Generator\gen_N100000_L50.txt";
            TextReader reader = new StreamReader(File.OpenRead(filename));

            Dictionary<String, int> pairs = new Dictionary<string, int>();

            string line = null;
            int cnt = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    cnt++;
                    int curr_count = 0;
                    if (pairs.TryGetValue(word, out curr_count))
                    {
                        pairs[word] = curr_count + 1;
                    }
                    else
                    {
                        pairs.Add(word, 1);
                    }
                }
            }
            Console.WriteLine(cnt);
            sw.Stop();
            Console.WriteLine($"duration={sw.ElapsedMilliseconds}");
            Console.WriteLine("Different words: " + pairs.Count);
        }
        // В тесте число строк 100 тыс. (по 50 слов), итого 5 млн. слов
        // Сканирование строк 91 мс.
        // Сканирование слов 280 мс.
        // Сканирование и собирание словаря 557 мс.
    }
}
