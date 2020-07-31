using System;
using System.Text;

namespace Passcore
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Replace with real logic
            byte[] b = Encoding.UTF8.GetBytes("");
            CryptoBase.Shake256(b, 512);
            CryptoBase.Sha3_512(b);

            int[] t = new int[331];
            for (int i = 0; i < 331; i++)
            {
                t[i] = i;
            }
            var k = Passcore.DeriveKey("master", "info", "salt");
            var br = new Roller<int>(t, k);

            int[] ctr = new int[331];
            for (int i = 0; i < 0xffffff; i++)
            {
                 ctr[br.Next()]++;
            }
            for (int i = 0; i < ctr.Length; i++)
            {
                Console.WriteLine($"{i}\t: {ctr[i]}");
            }
        }
    }
}
