using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredPolar
{
    /// <summary>
    /// Класс определяет узел, в котором накапливается, индексируется и используется последовательность записей из 
    /// трех полей: целочисленного id, строкового name, целочисленного age. Поле id является первичным ключом. 
    /// По нему выстраивается индекс.
    /// </summary>
    public class SequNode
    {
        private Stream stream;
        private int nelements = -1;
        public int Count { get { return nelements; } }

        private BinaryWriter bw;
        private BinaryReader br;

        private StreamArr offsets, keys;

        /// <summary>
        /// Конструктор создает узел формирует через генератор стримов или подключается к нему, если он есть. 
        /// </summary>
        /// <param name="genMedia"></param>
        public SequNode(Func<Stream> genMedia)
        {
            stream = genMedia();
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);

            if (stream.Length >= 8) 
            {
                stream.Position = 0L;
                nelements = (int)br.ReadInt64();
            }
            offsets = new StreamArr(genMedia, true);
            keys = new StreamArr(genMedia, false);
        }

        public void Load(IEnumerable<object> records)
        {
            stream.Position = 0L;
            nelements = 0;
            bw.Write(0L);
            foreach (object[] rec in records)
            {
                bw.Write((int)rec[0]);
                bw.Write((string)rec[1]);
                bw.Write((int)rec[2]);
                nelements++;
            }
            stream.Position = 0L;
            bw.Write((long)nelements);
            stream.Flush();
        }


        // Альтернативный способ загрузки Clear(), AppendElement ..., Flush - метод понадобится когда узлов будет несколько
        public void Clear() 
        {
            stream.Position = 0L;
            nelements = 0;
            bw.Write(0L);
        }
        public long AppendElement(object record)
        {
            long off = stream.Position;

            object[] rec = (object[])record;
            bw.Write((int)rec[0]);
            bw.Write((string)rec[1]);
            bw.Write((int)rec[2]);
            nelements++;

            return off;
        }
        public void Flush()
        {
            stream.Position = 0L;
            bw.Write((long)nelements);
            stream.Flush();
        }

        public void Scan(Func<long, object, bool> handler)
        {
            long off = 0L;
            stream.Position = off;
            long longnelements = br.ReadInt64();
            if (longnelements != nelements) throw new Exception("Err in stream nelements record");
            object[] rec = new object[3];
            for (int i=0; i<nelements; i++)
            {
                off = stream.Position;
                rec[0] = br.ReadInt32();
                rec[1] = br.ReadString();
                rec[2] = br.ReadInt32();
                bool ok = handler(off, rec);
                if (!ok) break; // завершение сканирования если хендле выработал ложь
            }
        }
        public IEnumerable<object> ScanFilter(Func<object, bool> predicate)
        {
            long off = 0L;
            stream.Position = off;
            long longnelements = br.ReadInt64();
            if (longnelements != nelements) throw new Exception("Err in stream nelements record");
            
            for (int i = 0; i < nelements; i++)
            {
                object[] rec = new object[3];
                rec[0] = br.ReadInt32();
                rec[1] = br.ReadString();
                rec[2] = br.ReadInt32();
                bool ok = predicate(rec);
                if (ok) yield return rec;
            }
        }

        public void BuildIndexes()
        {
            // Построение индекса по нулевому элементу записи индекс состоит из друх сериализованных массивов - 
            // массива офсетов элементов последовательности и массива ключей

            // Более простой способ построение индексных массивов - через создание массивов в оперативной памяти
            int[] keys_arr = new int[nelements];
            long[] offsets_arr = new long[nelements];
            
            // Заполнение массивов
            int nom = 0;
            Scan((off, record) =>
            {
                keys_arr[nom] = (int)((object[])record)[0];
                offsets_arr[nom] = off;
                nom++;
                return true;
            });
            
            // Сортировка
            Array.Sort(keys_arr, offsets_arr);

            // инициализация индексных массивов и заполнение
            keys.LoadInt32(keys_arr);
            offsets.LoadInt64(offsets_arr);
        }

        private object Get(long offset)
        {
            stream.Position = offset;
            object[] rec = new object[3];
            rec[0] = br.ReadInt32();
            rec[1] = br.ReadString();
            rec[2] = br.ReadInt32();
            return rec;
        }
        // Выборка по ключу
        public object GetByKey(int key)
        {
            var nom = keys.BinarySearch32(key);
            long offset = offsets.GetInt64(nom);
            return Get(offset);
        } 

        public void Refresh()
        {
            stream.CopyTo(Stream.Null); // Это решение плохо тем, что работает с полным файлом даже если занята только часть
            offsets.Refresh();
            keys.Refresh();
        }
    }
}
