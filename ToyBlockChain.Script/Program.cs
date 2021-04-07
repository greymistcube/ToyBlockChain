using System;
using ToyBlockChain.Service;

namespace ToyBlockChain.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            client.Run();
            Console.WriteLine(client);
        }
    }
}
