using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RsaDecoder
{
    class Program
    {
        private static bool IsPrime(long number)
        {
            if (number == 1) return false;
            if (number == 2) return true;

            for (long i = 3; i <= number / 2; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;

        }

        private static long GetGcd(long value1, long value2)
        {
            while (value1 != 0 && value2 != 0)
            {
                if (value1 > value2)
                    value1 %= value2;
                else
                    value2 %= value1;
            }
            return Math.Max(value1, value2);
        }

        private static bool IsCoprime(long value1, long value2)
        {
            return GetGcd(value1, value2) == 1;
        }

        private static IEnumerable<long> SplitMessage(string msg, long n)
        {
            long num = -1;
            int k = 1;
            var result = new List<long>();
            while (msg.Length >= k)
            {
                long tmp = long.Parse(msg.Substring(0, k));
                if (tmp > n)
                {
                    msg = msg.Substring(k - 1);
                    k = 1;
                    result.Add(num);
                }
                else
                {
                    num = tmp;
                    if (k == msg.Length)
                        result.Add(num);
                    ++k;
                }
            }
            return result;
        }

        private static string Decrypt(string text, long d, long n)
        {
            var numbers = SplitMessage(text, n);
            var decryptedNumbers = numbers.Select(c => BigInteger.ModPow(c, d, n).ToString());

            var chars = new List<char>();
            foreach (var str in decryptedNumbers)
            {
                chars.AddRange(SplitMessage(str, 91).Select(s => (char)s));
            }
            return new string(chars.ToArray());
        }

        private static long? GetP(long n)
        {
            for (var p = 3; p <= n / 2; p += 2)
            {
                if (n % p != 0) continue;
                if (!IsPrime(p)) continue;
                if (!IsPrime(n / p)) continue;
                return p;
            }
            return null;
        }

        private static long? GetD(long phi, long e)
        {

            for (long k = 1; k <= long.MaxValue / phi; k++)
            {
                if ((k * phi + 1) % e != 0) continue;
                return (k * phi + 1) / e;
            }
            return null;
        }

        private static void Start(long n, long e, string text)
        {
            var tmp = GetP(n);
            if (tmp == null)
            {
                Console.WriteLine("Can't find 'p'.");
                return;
            }
            long p = tmp.Value;
            long q = n / p;

            Console.WriteLine("p = {0}", p);
            Console.WriteLine("q = {0}", q);

            var phi = (p - 1) * (q - 1);
            Console.WriteLine("phi = {0}", phi);
            if (IsCoprime(e, phi))
            {
                Console.WriteLine("The numbers 'e' and 'phi' are coprime.");
            }

            tmp = GetD(phi, e);
            if (tmp == null)
            {
                Console.WriteLine("Can't find 'd'.");
                return;
            }
            long d = tmp.Value;
            Console.WriteLine("d = {0}", d);
            Console.WriteLine(Decrypt(text, d, n));
        }

        static void Main(string[] args)
        {
            const long n = 274607103517687;
            const long e = 11119;
            string text = "4472019868828421289843038617813192879612275852133249779907137882545473750";
            Start(n, e, text);
            Console.ReadLine();
        }
    }
}
