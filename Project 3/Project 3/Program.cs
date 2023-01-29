// See https://aka.ms/new-console-template for more information

using System.Buffers.Text;
using System.Net.Security;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Project_3{
    static class Program
    {
        /// <summary>
        /// The main function that runs the desired messaging functions based on
        /// the users input.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <returns>The exit status</returns>
        static async Task Main(String[] args)
        {
            if(args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("Incorrect usage error, acceptable inputs are as follows:");
                Console.WriteLine("dotnet run keyGen <bits>");
                Console.WriteLine("dotnet run sendKey <email>");
                Console.WriteLine("dotnet run getKey <email>");
                Console.WriteLine("dotnet run sendMsg <email> <msg>");
                Console.WriteLine("dotnet run getMsg <email>");
            }
            else if(args.Length == 2)
            {
                if (args[0].Equals("keyGen"))
                {
                    int parsed;
                    if (Int32.TryParse(args[1], out parsed))
                    {
                        Functions.keyGen(parsed);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect usage error: bit count must be an integer");
                    }
                }
                else if (args[0].Equals("sendKey"))
                {
                    await Functions.sendKey(args[1]);
                }
                else if (args[0].Equals("getKey"))
                {
                    await Functions.getKey(args[1]);
                }
                else if (args[0].Equals("getMsg"))
                {
                    await Functions.getMsg(args[1]);
                }
                else
                {
                    Console.WriteLine("Incorrect usage error, acceptable inputs are as follows:");
                    Console.WriteLine("dotnet run keyGen <bits>");
                    Console.WriteLine("dotnet run sendKey <email>");
                    Console.WriteLine("dotnet run getKey <email>");
                    Console.WriteLine("dotnet run sendMsg <email> <msg>");
                    Console.WriteLine("dotnet run getMsg <email>");
                }
            }
            else if (args.Length == 3)
            {
                if (args[0].Equals("sendMsg"))
                {
                    await Functions.sendMsg(args[1], args[2]);
                }
                else
                {
                    Console.WriteLine("Incorrect usage error, acceptable inputs are as follows:");
                    Console.WriteLine("dotnet run keyGen <bits>");
                    Console.WriteLine("dotnet run sendKey <email>");
                    Console.WriteLine("dotnet run getKey <email>");
                    Console.WriteLine("dotnet run sendMsg <email> <msg>");
                    Console.WriteLine("dotnet run getMsg <email>");
                }
            }
            
        }
    }
}