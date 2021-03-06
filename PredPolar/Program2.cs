﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredPolar
{
    partial class Program
    {
        public static void Main2()
        {
            Console.WriteLine("Start Main2");
            Random rnd = new Random();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            Console.WriteLine("Start PredPolar");
            string path = @"D:\Home\data\PredPolarData\";
            int num = 0;
            Func<Stream> genStreams = () => File.Open(path + "media" + (num++) + ".bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            int nbits = 4; // Сколько младших битов идет на номер секции
            int nsections = 1 << nbits;
            int nelements = 2_000_000_000;

            SequNode[] db_arr = Enumerable.Repeat(0, nsections).Select(i => new SequNode(genStreams)).ToArray();

            bool toload = true;
            if (toload)
            {
                sw.Restart();
                var dataflow = Enumerable.Range(0, nelements)
                    .Select(i => new object[] { nelements - i - 1, "p" + (nelements - i - 1), 33 });
                foreach (var db in db_arr) db.Clear();
                //db.Load();
                foreach (var d in dataflow)
                {
                    int key = (int)((object[])d)[0];
                    int nsec = key & (nsections - 1);
                    db_arr[nsec].AppendElement(d);
                }
                sw.Stop();
                Console.WriteLine($"after load  {nelements} elements. duration: {sw.ElapsedMilliseconds} ms.");
                sw.Restart();
                foreach (var db in db_arr) db.Flush();
                foreach (var db in db_arr) db.BuildIndexes();
                sw.Stop();
                Console.WriteLine($"Build indexed ok. duration: {sw.ElapsedMilliseconds} ms.");
            }
            else
            {
                //db.Refresh();
            }

            sw.Restart();
            object[] res = new object[0];
            foreach (var db in db_arr)
            {
                res = res.Concat(db.ScanFilter(ob =>
                    {
                        object[] rec = (object[])ob;
                        string s = (string)rec[1];
                        if (s == "p777777") return true;
                        //if (s.StartsWith("p777777")) return true;
                        else return false;
                    })).ToArray();
            }
            sw.Stop();
            Console.WriteLine($"scan of {nelements} ok. duration: {sw.ElapsedMilliseconds} ms.");
            foreach (object[] rec in res)
            {
                Console.WriteLine($"{rec[0]} {rec[1]} {rec[2]}");
            }

            int nprobes = 1000;

            sw.Restart();
            for (int i = 0; i < nprobes; i++)
            {
                int key = rnd.Next(nelements);
                int nsec = key & (nsections - 1);
                object obj = db_arr[nsec].GetByKey(key);
                object[] r = (object[])obj;
                if ((int)r[0] != key) throw new Exception($"Err: wrong value for key {key}: {r[0]} {r[1]} {r[2]}");
            }
            sw.Stop();
            Console.WriteLine($"Search of {nelements} elements, {nprobes} probes. duration: {sw.ElapsedMilliseconds} ms.");

        }
    }
    // 10 млн., рабочий компьютер, 16 Гб ОЗУ -- 5.6 s, 1.5 s, 100 ms
    // 100 млн.  -- 113 s, 16 s, 122 ms
    // 1 млрд.  -- 2941 s, 658 s, 49 s (диска: 29 Гб, ОЗУ: 3 Гб)
    // 1 млрд. (8 секций)  -- 626+733 s, 369 s, 48 s (диска: 29 Гб, ОЗУ: 2.8 Гб)
    // 1 млрд. (8 секций)  -- 496+690 s, 369 s, 49.7 s (диска: 29 Гб, ОЗУ: 2.8 Гб)
    // 2 млрд. (8 секций)  -- 2961+2026 s, 1471 s, 66 s (диска: 58 Гб, ОЗУ: 3.9 Гб)
    // 2 млрд. (16 секций)  -- 2107+2362 s, 1643 s, 59 s (диска: 58 Гб, ОЗУ: ?? Гб)
}
