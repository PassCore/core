using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace Passcore
{
    class Roller<T>
    {
        private ShakeDigest shake = new ShakeDigest(256);

        int width;
        T[] table;

        /// <summary>
        /// Create a roller instance
        /// </summary>
        /// <param name="table">Table to rolling</param>
        /// <param name="seed">Seed to use</param>
        public Roller(T[] table, byte[] seed)
        {
            double neededEntropy = Math.Log2(table.Length);
            width = (int)neededEntropy + 1;
            if (width > 32) throw new Exception();
            this.table = table;
            shake.BlockUpdate(seed, 0, seed.Length);
        }

        ulong remain = 0;
        int rdigit = 0;

        // get next output
        public T Next()
        {
            ulong ret;
            do
            {
                // how many bit need to take
                int readBit = width - rdigit;
                // corresponding byte count
                int readCount = (readBit + 7) / 8;
                // read bytes
                for (int i = 0; i < readCount; i++)
                {
                    remain |= (ulong)NextByte() << (64 - 8 - rdigit);
                    rdigit += 8;
                }
                // how many bit left after process
                int excessive = rdigit - width;
                ulong mask = ulong.MaxValue << (64 - width);
                // take bits we need to use
                ret = (mask & remain) >> (64 - width);
                // move remain bits to front
                remain <<= width;
                rdigit = excessive;
                mask = ulong.MaxValue << (64 - excessive);
                remain &= mask;
                // if we overflowed, try next output
            } while ((int)ret >= table.Length);

            return table[ret];
        }

        Stack<byte> cache = new Stack<byte>(64);
        byte[] shakeOut = new byte[64];

        private byte NextByte()
        {
            // generate bytes via SHAKE when necessary - it has "infinite" outout length
            if (cache.Count == 0)
            {
                shake.DoOutput(shakeOut, 0, 64);
                foreach (var b in shakeOut)
                {
                    cache.Push(b);
                }
            }

            return cache.Pop();
        }
    }
}
