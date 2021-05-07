using System;
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
            string path = @"C:\Home\data\";
            int num = 0;
            Func<Stream> genStreams = () => File.Open(path + "media" + (num++) + ".bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            int nbits = 2; // Сколько младших битов идет на номер секции
            int nsections = 1 << nbits;
            int nelements = 1_000_000_000;

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
                foreach (var db in db_arr) db.Flush();
                foreach (var db in db_arr) db.BuildIndexes();
                sw.Stop();
                Console.WriteLine($"load of {nelements} ok. duration: {sw.ElapsedMilliseconds} ms.");
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
}
