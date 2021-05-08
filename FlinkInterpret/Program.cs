using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlinkInterpret
{

    class Program
    {
        public static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        static void Main(string[] args)
        {
            Console.WriteLine("Start Flink Interpret");
            sw.Restart();
            var source = new MySource(0.2, 10000, 50, 200_000);
            sw.Stop(); Console.WriteLine("=== duration=" + sw.ElapsedMilliseconds);

            sw.Restart();
            long cnt_symbls = 0;
            string line = "start";
            while (source.MoveNext())
            {
                //Console.WriteLine(source.Current);
                cnt_symbls += ((string)source.Current).Length;
                line = (string)source.Current;
            }
            Console.WriteLine(cnt_symbls + " symbols");
            Console.WriteLine(line);
            sw.Stop(); Console.WriteLine("=== duration=" + sw.ElapsedMilliseconds);
            var elements = source.Elements();
            sw.Restart();
            source.Reset();
            long cnt = 0;
            foreach (string v in source.Elements())
            {
                cnt += v.Length;
                line = v;
            }
            Console.WriteLine(cnt + " symbols");
            Console.WriteLine(line);
            sw.Stop(); Console.WriteLine("=== duration=" + sw.ElapsedMilliseconds);

            // =============== Попытка посчитать количество слов =================
            sw.Restart();
            source.Reset();
            cnt = 0;
            Dictionary<string, int> hash_dictionary = new Dictionary<string, int>();
            while (source.MoveNext())
            {
                line = (string)source.Current;
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    cnt++;
                    string key = word;
                    if (!hash_dictionary.ContainsKey(key))
                    {
                        hash_dictionary.Add(key, 1);
                    }
                    else
                    {
                        hash_dictionary[key] += 1;
                    }
                }
            }
            Console.WriteLine($"total words: {cnt}");
            sw.Stop(); Console.WriteLine("=== duration=" + sw.ElapsedMilliseconds);
            //foreach (var pair in hash_dictionary)
            //{
            //    Console.WriteLine($"{pair.Key} {pair.Value}");
            //}

        }
    }
}
