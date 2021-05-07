using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PredPolar
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //Main1(args);
            Main2();
        }
        static void Main1(string[] args)
        {
            Random rnd = new Random();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            Console.WriteLine("Start PredPolar");
            string path = @"C:\Home\data\";
            int num = 0;
            Func<Stream> genStreams = () => File.Open(path + "media" + (num++) + ".bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            int nelements = 10_000_000;

            SequNode db = new SequNode(genStreams);

            bool toload = false;
            if (toload)
            {
                sw.Restart();
                db.Load(Enumerable.Range(0, nelements)
                    .Select(i => new object[] { nelements - i - 1, "p" + (nelements - i - 1), 33 }));
                db.BuildIndexes();
                sw.Stop();
                Console.WriteLine($"load of {nelements} ok. duration: {sw.ElapsedMilliseconds} ms.");
            }
            else
            {
                db.Refresh();
            }


            sw.Restart();
            db.Scan((off, ob) =>
            {
                object[] rec = (object[])ob;
                //Console.WriteLine($"{rec[0]} {rec[1]} {rec[2]}");
                return true;
            });
            sw.Stop();
            Console.WriteLine($"scan of {nelements} ok. duration: {sw.ElapsedMilliseconds} ms.");


            int nprobes = 1000;

            sw.Restart();
            for (int i = 0; i<nprobes; i++)
            {
                int key = rnd.Next(nelements);
                object obj = db.GetByKey(key);
                object[] r = (object[])obj;
                if ((int)r[0] != key) throw new Exception($"Err: wrong value for key {key}: {r[0]} {r[1]} {r[2]}");
            }
            sw.Stop();
            Console.WriteLine($"Search of {nelements} elements, {nprobes} probes. duration: {sw.ElapsedMilliseconds} ms.");

        }

        // Результаты для 10 млн. элементов:
        // load 4506 ms   scan 1547 ms   search1000 99 ms
        // Без загрузки:
        // scan 1525 ms.   search1000 104 ms

        // Результаты для 100 млн. элементов:
        // load 65 s   scan 21 s   search1000 43 s
        // Без загрузки:
        // scan 28 s.   search1000 26 s (Плюс время на Refresh) 

        // Результаты для 100 млн. элементов (после освобождения от лишних программ):
        // load 69 s   scan 15 s   search1000 107 ms.
        // Без загрузки:
        // scan 15 s.   search1000 132 ms. 

    }
}
