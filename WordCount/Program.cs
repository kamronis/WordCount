using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace WordCount
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            //Convert the string into an array of words 
            string dir = "../../../../Text Generator/";
            string input_file = "gen_N1000_L50.txt";

            watch.Start();

            using (StreamReader sr = new StreamReader(dir + input_file))
            {
                string line;
                // Read and display lines from the file until the end of
                // the file is reached.
                
                if (File.Exists(dir + "res_" + input_file))
                {
                    File.Delete(dir + "res_" + input_file);
                }
                Dictionary<String, int> pairs = new Dictionary<string, int>();

                while ((line = sr.ReadLine()) != null)
                {
                    
                    var grouped_lines = line.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).GroupBy(x => x);
                    //Console.WriteLine(line);
                    using (StreamWriter sw = File.AppendText(dir + "res_" + input_file))
                    {
                        foreach (var group in grouped_lines)
                        {
                            int curr_count = 0;
                            bool test = pairs.TryGetValue(group.Key, out curr_count);
                            if (pairs.TryGetValue(group.Key, out curr_count))
                            {
                                curr_count += group.Count();
                                pairs[group.Key] = curr_count;
                            }
                            else
                            {
                                curr_count = group.Count();
                                pairs.Add(group.Key, group.Count());
                            }
                            sw.WriteLine(group.Key + ":" + curr_count);
                        }
                    }
                    
                }
            }
            watch.Stop();
            Console.WriteLine($"duration={watch.ElapsedMilliseconds}");

        }
    }
}
