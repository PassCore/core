using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Passcore
{
    class Passcore
    {
        public static byte[] DeriveKey(string master, string info, string salt = "")
        {
            string[] input = { master, info, salt };
            byte[][] inputByte = input.Select(Utils.StringToBytes).ToArray();
            bool salted = !string.IsNullOrEmpty(salt);

            // SHA3-512 all input
            byte[][] inputHash = inputByte.Select(CryptoBase.Sha3_512).ToArray();

            byte[] _salt = salted ? inputHash[2] : new byte[64];
            if (salted)
            {
                // if salted, HMAC-SHA3-512 all input instead
                inputHash = inputByte.Select(s => CryptoBase.HmacSha3_512(s, _salt)).ToArray();
            }

            // Use HKDF to get key
            byte[] key = CryptoBase.HkdfSha3_512(inputHash[0], inputHash[2], inputHash[1]);

            return key;
        }
    }
}
