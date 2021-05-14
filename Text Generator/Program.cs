using System;
using System.IO;
using FlinkInterpret;

namespace Text_Generator
{
    class Program
    {
        public static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        static void Main(string[] args)
        {
            int words_in_lines = 50;
            int number_of_lines = 100_000;

            var source = new MySource(0.2, 10000, words_in_lines, number_of_lines);
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

            using (StreamWriter sw = File.CreateText($"../../../gen_N{number_of_lines}_L{words_in_lines}.txt"))
            {
                foreach (string v in source.Elements())
                {
                    sw.WriteLine(v);
                }
            }
        }
    }
}
