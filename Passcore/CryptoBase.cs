using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace Passcore
{
    internal static class CryptoBase
    {
        public static byte[] Shake256(byte[] plain, int digit)
        {
            if (digit % 8 != 0)
            {
                Console.Error.WriteLine("requested digit is not aligned");
            }
            byte[] hash = new byte[digit / 8];
            ShakeDigest shake = new ShakeDigest(256);
            shake.BlockUpdate(plain, 0, plain.Length);
            shake.DoFinal(hash, 0);
            return hash;
        }

        public static byte[] Sha3_512(byte[] plain)
        {
            Sha3Digest sha3 = new Sha3Digest(512);
            byte[] hash = new byte[sha3.GetDigestSize()];
            sha3.BlockUpdate(plain, 0, plain.Length);
            sha3.DoFinal(hash, 0);
            return hash;
        }

        public static byte[] HmacSha3_512(byte[] plain, byte[] key)
        {
            HMac mac = new HMac(new Sha3Digest(512));
            mac.Init(new KeyParameter(key));
            byte[] hash = new byte[mac.GetMacSize()];
            mac.BlockUpdate(plain, 0, plain.Length);
            mac.DoFinal(hash, 0);
            return hash;
        }

        public static byte[] HkdfSha3_512(byte[] key, byte[] salt, byte[] info)
        {
            byte[] subkey = new byte[256];
            HkdfBytesGenerator hkdf = new HkdfBytesGenerator(new Sha3Digest(512));
            hkdf.Init(new HkdfParameters(key, salt, info));
            hkdf.GenerateBytes(subkey, 0, 256);
            return subkey;
        }

        public static byte[] Random(int len)
        {
            byte[] r = new byte[len];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(r);
            return r;
        }
    }
}
