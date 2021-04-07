using System;
using ToyBlockChain.Service;

namespace ToyBlockChain.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            Node node = new Node(true);

            Client client = new Client(node);
            Miner miner = new Miner(node);

            client.Run();
            Console.WriteLine(client);
        }
    }
}
