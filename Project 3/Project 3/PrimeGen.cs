using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_3
{
    static class PrimeGen
    {
        /// <summary>
        /// This method was given to me by the instructors of the course. It calculates
        /// the modular multiplicative inverse given the E and Phi used in the generation
        /// of keys.
        /// </summary>
        /// <param name="a">The E value used in key generation.</param>
        /// <param name="n">The Phi value used in key generation.</param>
        /// <returns></returns>
        public static BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - (t * x);
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }
        /// <summary>
        /// This method generates a random BigInteger number by generating a specified
        /// amount of random bytes in an array and then giving the array to the BigInteger
        /// constructor. If giving the bytes to BigInteger will result in a negative value
        /// then the byte array is adjusted.
        /// </summary>
        /// <param name="bits">The number of bits specified in the command line arguments</param>
        /// <returns></returns>
        public static BigInteger generateRandom(int bits)
        {
            BigInteger result;
            Byte[] bytes = RandomNumberGenerator.GetBytes(bits / 8);
            if ((bytes[bytes.Length - 1] & 0x80) > 0)       //This if statement was taken from
            {                                               //from the C# docs about the
                byte[] temp = new byte[bytes.Length];       //randomNumberGenerator class
                Array.Copy(bytes, temp, bytes.Length);      //specifically the portion about
                bytes = new byte[temp.Length + 1];          //using byte arrays in the constructor
                Array.Copy(temp, bytes, temp.Length);
            }
            result = new BigInteger(bytes);
            return result;
        }

        /// <summary>
        /// This method takes in a BigInteger and checks to see if it is prime using 
        /// the Miller-Rabin algorithm.
        /// </summary>
        /// <param name="value">The BigInteger to check</param>
        /// <param name="k">K the number of rounds of testing to perform (set to 10 via the HW instructions)</param>
        /// <returns></returns>
        public static Boolean IsProbablyPrime(this BigInteger value, int k = 10)
        {
            BigInteger n = value - 1;
            int r = 0;
            BigInteger d = 0;
            while (n.IsEven)
            {
                n = n / 2;
                d = n;
                r += 1;
            }
            BigInteger a = value;
            BigInteger x;

            for (int i = 0; i < k; i++)
            {
                while (a >= value - 2)
                {
                    a = generateRandom(value.GetByteCount() * 8);
                }
                x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                {
                    continue;
                }
                bool comp = true;
                for (int z = 0; z < r - 1; z++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == value - 1)
                    {
                        comp = false;
                        break;
                    }
                }
                if (comp)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
