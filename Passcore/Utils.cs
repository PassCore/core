using System;
using System.Linq;
using System.Text;

namespace Passcore
{
    public class Utils
    {
        public static string BytesToHex(byte[] bin)
        {
            return BitConverter.ToString(bin).Replace("-", "");
        }

        public static byte[] HexToBytes(string hex)
        {
            string formal = hex.Replace(":", "").Replace("-", "").ToLowerInvariant();
            if (formal.Length % 2 == 1)
            {
                throw new FormatException();
            }

            return Enumerable
                .Range(0, formal.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static byte[] StringToBytes(string str)
        {
            return StringToBytes(str, Encoding.UTF8);
        }

        public static byte[] StringToBytes(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static string BytesToString(byte[] bin)
        {
            return BytesToString(bin, Encoding.UTF8);
        }

        public static string BytesToString(byte[] bin, Encoding encoding)
        {
            return encoding.GetString(bin);
        }
    }
}
