using System;
using System.IO;

namespace Text_Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            int words_in_lines = 50;
            int number_of_lines = 10000000;

            string[] voc = { "which", "where", "the", "empty", "alone", "dark",
                "coconut", "princess", "stable", "island", "array", 
                "length", "dog", "amber", "lane", "rose", "world",
                "summer", "joy", "knowledge", "password", "smile" };
            Random rnd = new Random();
            using (StreamWriter sw = File.CreateText($"../../../gen_N{number_of_lines}_L{words_in_lines}.txt"))
            {
                for (int j = 0; j < number_of_lines; j++)
                {
                    string line = "";
                    for (int i = 0; i < words_in_lines; i++)
                    {
                        if (i != 0)
                        {
                            line = line + " " + voc[rnd.Next(voc.Length)];
                        }
                        else
                        {
                            line = voc[rnd.Next(voc.Length)];
                        }
                    }
                    sw.WriteLine(line);
                }
            }
        }
    }
}
