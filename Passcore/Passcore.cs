using System;
using System.Collections.Generic;
using System.Linq;

namespace Passcore
{
    public class Passcore
    {
        static byte[] DeriveKey(string master, string info, string salt = "")
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

        static int[] GenerateMixSequence(byte[] key, int length, int[] minOccurence)
        {
            byte[] mixSeed = CryptoBase.HmacSha3_512(Utils.StringToBytes("mix"), key);
            int oSum = 0;
            Stack<int> mixItems = new Stack<int>(length);
            // put dictionary id with given number
            for (int i = 0; i < minOccurence.Length; i++)
            {
                oSum += minOccurence[i];
                for (int j = 0; j < minOccurence[i]; j++)
                {
                    mixItems.Push(i);
                }
            }
            if (oSum > length)
            {
                throw new ArgumentOutOfRangeException();
            }
            // fill rest part with random id
            byte[] fillSeed = CryptoBase.HmacSha3_512(Utils.StringToBytes("fill"), key);
            SequenceGenerator fillGenerator = new SequenceGenerator(fillSeed);

            for (int i = 0; i < length - oSum; i++)
            {
                mixItems.Push(fillGenerator.NextInt(minOccurence.Length));
            }
            // shuffle it
            return new SequenceGenerator(mixSeed).Shuffle(mixItems);
        }

        static (int group, int index)[] GeneratePasswordTokens(
            byte[] key,
            int length,
            (int itemCount, int occurence)[] dict
            )
        {
            int[] mixSeq = GenerateMixSequence(key, length, dict.Select(d => d.occurence).ToArray());
            // create sub sequence source
            SequenceGenerator[] generators = new SequenceGenerator[dict.Length];
            for (int i = 0; i < dict.Length; i++)
            {
                byte[] seed = CryptoBase.HmacSha3_512(Utils.StringToBytes($"seq_{i}"), key);
                generators[i] = new SequenceGenerator(seed);
            }
            // replace dictionary id to token id
            return mixSeq.Select(g => (g, generators[g].NextInt(dict[g].itemCount))).ToArray();
        }

        public static string GeneratePassword(
            (char[] dict, int occur)[] dictionaries,
            int length,
            string master,
            string info,
            string salt = ""
            )
        {
            // sort dictionary, make result stable
            (char[] dict, int weight)[] sortedDict = dictionaries
                .Select(d =>
                {
                    // sort each dict
                    IOrderedEnumerable<char> sd = d.dict.OrderBy(k => k);
                    string hash = Utils.BytesToHex(CryptoBase.Sha3_512(Utils.StringToBytes(string.Concat(sd))));
                    return (hash, (dict: sd.ToArray(), d.occur));
                })
                // sort all dict
                .OrderBy(d => d.hash)
                .Select(d => d.Item2)
                .ToArray();

            // get dictionary metadata
            (int, int)[] dictDef = sortedDict.Select(sd => (sd.dict.Length, sd.weight)).ToArray();

            byte[] masterKey = DeriveKey(master, info, salt);
            (int group, int index)[] tokens = GeneratePasswordTokens(masterKey, length, dictDef);

            // replace token with real dictionary items
            return string.Concat(tokens.Select(t => sortedDict[t.group].dict[t.index]));
        }
    }
}
