using System;

namespace Passcore
{
    class Program
    {
        static void Main(string[] args)
        {

            string pw = Passcore.GeneratePassword(new (char[] dict, int occur)[]
                {
                    (Tables.LatinLower,0),
                    (Tables.LatinUpper,0),
                },
                16,
                "master",
                "info",
                "salt"
            );
            Console.WriteLine(pw);
            string pw2 = Passcore.GeneratePassword(new (char[] dict, int occur)[]
                {
                    (Tables.LatinUpper,0),
                    (Tables.LatinLower,0),
                },
                16,
                "master",
                "info",
                "salt"
            );
            Console.WriteLine(pw2);

            double e = Passcore.PasswordEntropy(new (char[] dict, int occur)[]
            {
                (Tables.Base64,0),
            }, 16);
            Console.WriteLine(e);
            double e2 = Passcore.PasswordEntropy(new (char[] dict, int occur)[]
            {
                (Tables.Number,0),
            }, 16);
            Console.WriteLine(e2);
        }
    }
}
