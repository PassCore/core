using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Digests;

namespace Passcore
{
    internal class SeededRNG
    {
        private readonly ShakeDigest shake = new ShakeDigest(256);

        public SeededRNG(byte[] seed)
        {
            shake.BlockUpdate(seed, 0, seed.Length);
        }

        private ulong remain = 0;
        private int rdigit = 0;

        // get next output
        public int NextBits(int width)
        {

            ulong ret;

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

            return (int)ret;
        }

        private readonly Stack<byte> cache = new Stack<byte>(64);
        private readonly byte[] shakeOut = new byte[64];

        private byte NextByte()
        {
            // generate bytes via SHAKE when necessary - it has "infinite" outout length
            if (cache.Count == 0)
            {
                shake.DoOutput(shakeOut, 0, 64);
                foreach (byte b in shakeOut)
                {
                    cache.Push(b);
                }
            }

            return cache.Pop();
        }
    }
}
