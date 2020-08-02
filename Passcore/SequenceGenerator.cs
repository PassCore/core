using System;
using System.Collections.Generic;
using System.Linq;

namespace Passcore
{
    internal class SequenceGenerator
    {
        private readonly SeededRNG rng;

        public SequenceGenerator(byte[] seed)
        {
            rng = new SeededRNG(seed);
        }

        public int NextInt(int max)
        {
            int width = GetNeededBitCount(max);
            int r;
            do
            {
                r = rng.NextBits(width);
            }
            while (r >= max);
            return r;
        }

        public int[] IntSeq(int max, int count)
        {
            int[] ret = new int[count];
            int width = GetNeededBitCount(max);
            for (int i = 0; i < count; i++)
            {
                do
                {
                    ret[i] = rng.NextBits(width);
                }
                while (ret[i] >= max);
            }
            return ret;
        }

        private int GetNeededBitCount(int events)
        {
            return (int)Math.Log2(events) + 1;
        }

        public T[] Shuffle<T>(IEnumerable<T> seq)
        {
            LinkedList<T> s = new LinkedList<T>(seq);
            Stack<T> ret = new Stack<T>(seq.Count());
            while (s.Count > 0)
            {
                int idx = NextInt(s.Count);
                LinkedListNode<T> node = s.First;
                for (int j = 0; j < idx; j++)
                {
                    node = node.Next;
                }

                ret.Push(node.Value);
                s.Remove(node);
            }
            return ret.ToArray();
        }

        public T[] Roll<T>(IEnumerable<T> table, int count)
        {
            T[] seq = table.Distinct().ToArray();
            return IntSeq(seq.Length, count)
                .Select(i => seq[i])
                .ToArray();
        }
    }
}
