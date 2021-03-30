using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredPolar
{
    /// <summary>
    /// Класс массив, в котором накапливается, индексируется и используется последовательность Элементов длинного или простого целого типа
    /// </summary>
    public class StreamArr
    {
        private Stream stream;
        private int nelements = -1;
        public int Count { get { return nelements; } }
        private bool longelements = false;

        private BinaryWriter bw;
        private BinaryReader br;

        /// <summary>
        /// Конструктор создает узел формирует через генератор стримов или подключается к нему, если он есть. 
        /// </summary>
        /// <param name="genMedia"></param>
        public StreamArr(Func<Stream> genMedia, bool longelements)
        {
            this.longelements = longelements; 
            stream = genMedia();
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
            if (stream.Length >= 8)
            {
                stream.Position = 0L;
                nelements = (int)br.ReadInt64();
            }

        }

        public void LoadInt32(IEnumerable<int> records)
        {
            stream.Position = 0L;
            nelements = 0;
            bw.Write(0L);
            foreach (int val in records)
            {
                bw.Write(val);
                nelements++;
            }
            stream.Position = 0L;
            bw.Write((long)nelements);
            stream.Flush();
        }
        public void LoadInt64(IEnumerable<long> records)
        {
            stream.Position = 0L;
            nelements = 0;
            bw.Write(0L);
            foreach (long val in records)
            {
                bw.Write(val);
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
        public void AppendInt32(int val)
        {
            long off = stream.Position;
            bw.Write(val);
            nelements++;
        }
        public void AppendInt64(long val)
        {
            long off = stream.Position;
            bw.Write(val);
            nelements++;
        }
        public void Flush()
        {
            stream.Position = 0L;
            bw.Write((long)nelements);
            stream.Flush();
        }

        public int GetInt32(int index)
        {
            stream.Position = 8L + (long)index * 4;
            return br.ReadInt32();
        }
        public long GetInt64(int index)
        {
            stream.Position = 8L + (long)index * 8;
            return br.ReadInt64();
        }

        // нахождение индекса ключа в отсортированном массиве
        public int BinarySearch32(int key) { return BinarySearch32(0, nelements, key); }
        private int BinarySearch32(int start, int number, int key)
        {
            int half = number / 2;
            int middle_keyvalue = GetInt32(start + half);
            if (half == 0) // number = 0, 1
            {
                if (GetInt32(start) == key) return start;
                else if (GetInt32(start + 1) == key) return start + 1;
                else return -1;
            }
            if (middle_keyvalue == key) return start + half;

            int middle = start + half;
            int rest = number - half - 1;
            var middle_depth = middle_keyvalue - key;

            if (middle_depth == 0) // Нашли!
            {
                return middle;
            }
            if (middle_depth < 0)
            {
                return BinarySearch32(middle + 1, rest, key);
            }
            else
            {
                return BinarySearch32(start, half, key);
            }
        }

        public void Refresh()
        {
            stream.CopyTo(Stream.Null); // Это решение плохо тем, что работает с полным файлом даже если занята только часть
        }
    }
}
