using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlinkInterpret
{
    public class MySource : IEnumerator
    {
        private string[] words;
        private int nwordsinline, nlines;
        Random rnd, rnd2;
        public MySource(double pspace, int primarytext_len, int nwordsinline, int nlines)
        {
            this.nwordsinline = nwordsinline;
            this.nlines = nlines;
            const string letters = "abcdefghijklmnopqrstuwyxyz";
            char[] chars = new char[primarytext_len];
            rnd = new Random(777777);
            for (int i = 0; i<primarytext_len; i++)
            {
                if (rnd.NextDouble() <= pspace) chars[i] = ' ';
                else chars[i] = letters[rnd.Next(letters.Length)];
            }
            string primarytext = new string(chars);
            words = primarytext.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            rnd2 = new Random(1234567);
        }
        private string line = null;
        private int iline = 0;
        public object Current => line;

        public bool MoveNext()
        {
            if (iline >= nlines) return false;
            iline++;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nwordsinline; i++)
            {
                if (i != 0) sb.Append(' ');
                sb.Append(words[rnd2.Next(words.Length)]);
            }
            line = sb.ToString();
            return true;
        }

        public void Reset()
        {
            iline = 0;
            rnd2 = new Random(1234567);
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        // Для сравнения эффективности
        public IEnumerable<object> Elements()
        {
            for (int j = 0; j < nlines; j++)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nwordsinline; i++)
                {
                    if (i != 0) sb.Append(' ');
                    sb.Append(words[rnd2.Next(words.Length)]);
                }
                yield return sb.ToString();
            }
        }
    }
}
