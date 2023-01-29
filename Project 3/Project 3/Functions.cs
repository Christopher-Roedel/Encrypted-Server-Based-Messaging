using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Project_3
{
    internal class Functions
    {
        static readonly HttpClient client = new HttpClient();
        static BigInteger messageChecker = new BigInteger();
        /// <summary>
        /// getKey takes in an email that is provided by the user and then requests
        /// for the server to send the public key associated with that user.
        /// Saves the public key in a file called "email".key
        /// </summary>
        /// <param name="email">The email given by the user</param>
        /// <returns>Returns nothing of value</returns>
        public static async Task getKey(string email)
        {
            try
            {
                string request = "http://kayrun.cs.rit.edu:5000/Key/" + email;
                using HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    var strings = JsonArray.Parse(responseString);
                    if(strings == null)
                    {
                        Console.WriteLine("No key found for user: " + email);
                    }
                    File.WriteAllText(email + ".key", strings.ToString());
                }
                else
                {
                    Console.WriteLine("Can not find user: " + email);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Takes in an int representing the number of bits to split the private and public
        /// keys into. Then follows the RSA formula generating values for p, q, n, e, phi, and d
        /// then creates the two keys according to the servers specifications and saves them
        /// in files pubic.key, and private.key
        /// </summary>
        /// <param name="bits">The number of bits to be used for the creating of the prime numbers
        /// p and q</param>
        public static void keyGen(int bits)
        {
            int pSize = (int)((bits / 2) * 1.3);
            int qSize = bits - pSize;
            BigInteger p;
            while (true)
            {
                
                p = PrimeGen.generateRandom(pSize);
                if (p % 2 == 1)
                {
                    if (PrimeGen.IsProbablyPrime(p))
                    {
                        break;
                    }
                }
            }
            BigInteger q;
            while (true)
            {
                q = PrimeGen.generateRandom(qSize);
                if (q % 2 == 1)
                {
                    if (PrimeGen.IsProbablyPrime(q))
                    {
                        break;
                    }
                }
            }
            BigInteger N = p * q;
            BigInteger r = ((p - 1) * (q - 1));
            BigInteger E;
            while (true)
            {
                E = PrimeGen.generateRandom(16);
                BigInteger mod = BigInteger.ModPow(r, 1, E);
                if (E % 2 == 1)
                {
                    if (PrimeGen.IsProbablyPrime(E) && mod != 0)
                    {
                        break;
                    }
                }
            }
            BigInteger D = PrimeGen.modInverse(E, r);
            int n = N.GetByteCount();
            int e = E.GetByteCount();
            int d = D.GetByteCount();
            byte[] NArray = N.ToByteArray();
            byte[] nArrayTemp = BitConverter.GetBytes(n);
            byte[] EArray = E.ToByteArray();
            byte[] eArrayTemp = BitConverter.GetBytes(e);
            byte[] DArray = D.ToByteArray();
            byte[] dArrayTemp = BitConverter.GetBytes(d);
            byte[] nArray = new byte[4];
            byte[] eArray = new byte[4];
            byte[] dArray = new byte[4];

            for(int i = 0; i < 4; i++)
            {
                nArray[3 - i] = nArrayTemp[i];
                eArray[3 - i] = eArrayTemp[i];
                dArray[3 - i] = dArrayTemp[i];
            }

            byte[] publicKeyByteArray = new byte[8 + n + e];
            for(int i = 0; i < 8 + n + e; i++)
            {
                if(i < 4)
                {
                    publicKeyByteArray[i] = eArray[i];
                }
                else if(i < 4 + e)
                {
                    publicKeyByteArray[i] = EArray[i - 4];
                }
                else if(i < 8 + e)
                {
                    publicKeyByteArray[i] = nArray[i - (4 + e)];
                }
                else
                {
                    publicKeyByteArray[i] = NArray[i - (8 + e)];
                }
            }
            string publicKey = Convert.ToBase64String(publicKeyByteArray);

            byte[] privateKeyByteArray = new byte[8 + d + n];
            for(int i = 0; i < 8 + d + n; i++)
            {
                if(i < 4)
                {
                    privateKeyByteArray[i] = dArray[i];
                }
                else if(i < 4 + d)
                {
                    privateKeyByteArray[i] = DArray[i - 4];
                }
                else if(i < 8 + d)
                {
                    privateKeyByteArray[i] = nArray[i - (4 + d)];
                }
                else
                {
                    privateKeyByteArray[i] = NArray[i - (8 + d)];
                }
            }
            
            string privateKey = Convert.ToBase64String(privateKeyByteArray);

            PublicKey publicKeyObj = new PublicKey();
            publicKeyObj.key = publicKey;

            PrivateKey privateKeyObj = new PrivateKey();
            privateKeyObj.key = privateKey;

            string publicString = JsonSerializer.Serialize<PublicKey>(publicKeyObj);
            var publicArray = JsonArray.Parse(publicString);

            string privateString = JsonSerializer.Serialize<PrivateKey>(privateKeyObj);
            var privateArray = JsonArray.Parse(privateString);

            File.WriteAllText("public.key", publicArray.ToString());
            File.WriteAllText("private.key", privateArray.ToString());
        }

        /// <summary>
        /// sendKey takes in an email given by the user and then sends the public key to them
        /// on the server which now makes the available for messaging. The user is added to the
        /// of emails stored in private.key which also means that they are open for messaging.
        /// </summary>
        /// <param name="email">The email specified by the user</param>
        /// <returns></returns>
        public static async Task sendKey(string email)
        {
            try
            {
                string request = "http://kayrun.cs.rit.edu:5000/Key/" + email;
                PublicKey? publicKey = JsonSerializer.Deserialize<PublicKey>(JsonArray.Parse(File.ReadAllText("public.key")));
                publicKey.email = email;
                var content = new StringContent(JsonSerializer.Serialize<PublicKey>(publicKey), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PutAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    PrivateKey? privateKey = JsonSerializer.Deserialize<PrivateKey>(JsonArray.Parse(File.ReadAllText("private.key")));
                    if(privateKey.email == null)
                    {
                        privateKey.email = new List<string>();
                    }
                    bool contains = false;
                    foreach (string s in privateKey.email)
                    {
                        if (s.Contains(email)){
                            contains = true;
                        }
                    }

                    if (!contains){
                        privateKey.email.Add(email);
                    }
                    string privateString = JsonSerializer.Serialize<PrivateKey>(privateKey);
                    var privateArray = JsonArray.Parse(privateString);

                    File.WriteAllText("private.key", privateArray.ToString());
                    Console.WriteLine("Key Saved");
                }
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("Public and/or private key does not exist. Please generate a new key.");
            }
        }

        /// <summary>
        /// getMsg takes in an email from the user and tries to retrieve the last message that has been sent to that
        /// user and decrypt it. In order for getMsg to work properly the user must be sent a key and sent a message.
        /// </summary>
        /// <param name="email">Email provided by the user</param>
        /// <returns></returns>
        public static async Task getMsg(string email)
        {
            try
            {
                PrivateKey? privateKey = JsonSerializer.Deserialize<PrivateKey>(JsonArray.Parse(File.ReadAllText("private.key")));
                if(privateKey.email == null)
                {
                    Console.WriteLine("No private key found for user: " + email);
                    return;
                }
                else if (!privateKey.email.Contains(email))
                {
                    Console.WriteLine("No private key found for user: " + email);
                    return;
                }
                string request = "http://kayrun.cs.rit.edu:5000/Message/" + email;
                using HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Message? message = JsonSerializer.Deserialize<Message>(responseString);
                    var bytes = Convert.FromBase64String(message.content);
                    BigInteger encrypted = new BigInteger(bytes);
                    var bytesPrivateKey = Convert.FromBase64String(privateKey.key);
                    byte[] bytes2 = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        bytes2[3 - i] = bytesPrivateKey[i];
                    }
                    int dCount = BitConverter.ToInt32(bytes2, 0);
                    byte[] bytes3 = new byte[dCount];
                    for (int i = 4; i < 4 + dCount; i++)
                    {
                        bytes3[i - 4] = bytesPrivateKey[i];
                    }
                    BigInteger D = new BigInteger(bytes3);
                    byte[] bytes4 = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        bytes4[3 - i] = bytesPrivateKey[i + dCount + 4];
                    }
                    int nCount = BitConverter.ToInt32(bytes4, 0);
                    byte[] bytes5 = new byte[nCount];
                    for (int i = 0; i < nCount; i++)
                    {
                        bytes5[i] = bytesPrivateKey[i + dCount + 8];
                    }
                    BigInteger N = new BigInteger(bytes5);
                    BigInteger decrypted = BigInteger.ModPow(encrypted, D, N);
                    var decryptedBytes = decrypted.ToByteArray();
                    string decryptedMessage = Encoding.ASCII.GetString(decryptedBytes);
                    Console.WriteLine(decryptedMessage);
                }
            }
            catch
            {
                Console.WriteLine("No private key found locally, please generate a new keyset");
            }
        }
        
        /// <summary>
        /// sendMsg takes in a user email and a plaintext message. It then encrypts the message using
        /// the public key for the user and then sends the encrypted message to the server. This method
        /// requires that the user has been sent a key, and has their key grabbed in order to work properly.
        /// </summary>
        /// <param name="email">Email provided by user</param>
        /// <param name="plaintext">Message the user wishes to send</param>
        /// <returns></returns>
        public static async Task sendMsg(string email, string plaintext)
        {
            try
            {
                PublicKey? publicKey = JsonSerializer.Deserialize<PublicKey>(JsonArray.Parse(File.ReadAllText(email + ".key")));
                var bytesMessage = Encoding.ASCII.GetBytes(plaintext);
                BigInteger bigIntMessage = new BigInteger(bytesMessage);
                var bytes = Convert.FromBase64String(publicKey?.key);
                byte[] bytes2 = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes2[3 - i] = bytes[i];
                }
                int e = BitConverter.ToInt32(bytes2, 0);
                byte[] bytes3 = new byte[e];
                for (int i = 4; i < 4 + e; i++)
                {
                    bytes3[i - 4] = bytes[i];
                }
                BigInteger E = new BigInteger(bytes3);
                byte[] bytes4 = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes4[3 - i] = bytes[i + e + 4];
                }
                int n = BitConverter.ToInt32(bytes4, 0);
                byte[] bytes5 = new byte[n];
                for (int i = 0; i < n; i++)
                {
                    bytes5[i] = bytes[i + e + 8];
                }
                BigInteger N = new BigInteger(bytes5);
                BigInteger encrypted = BigInteger.ModPow(bigIntMessage, E, N);
                messageChecker = encrypted;
                var encyrptedBytes = encrypted.ToByteArray();
                string encryptedString = Convert.ToBase64String(encyrptedBytes);
                Message message = new Message();
                message.email = email;
                message.content = encryptedString;
                message.time = DateTime.Now;
                var content = new StringContent(JsonSerializer.Serialize<Message>(message), Encoding.UTF8, "application/json");
                string request = "http://kayrun.cs.rit.edu:5000/Message/" + email;
                using HttpResponseMessage response = await client.PutAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message written");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No public key found for user: " + email);
            }
        }
    }
}
