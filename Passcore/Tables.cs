using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Passcore
{
    public static class Tables
    {
        public static char[] FromString(string str)
        {
            return str.AsEnumerable().Distinct().ToArray();
        }

        //public static IEnumerable<string> FromString(string str, string split)
        //{
        //    return str.Split(split).Distinct();
        //}

        public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] ts)
        {
            return ts.SelectMany(t => t).Distinct();
        }

        public static char[] FromFile(string file)
        {
            return FromString(File.ReadAllText(file));
        }

        //public static IEnumerable<string> FromFile(string file, string split)
        //{
        //    return FromString(File.ReadAllText(file), split);
        //}

        public static readonly char[] Base64 = FromString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");
        public static readonly char[] Base64Url = FromString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_");
        public static readonly char[] LatinUpper = FromString("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        public static readonly char[] LatinLower = FromString("abcdefghijklmnopqrstuvwxyz");
        public static readonly char[] Number = FromString("0123456789");
        public static readonly char[] Symbol = FromString("~!@#$%^&*()_+`[]{}\\|;:'\",.<>/?");
    }
}
