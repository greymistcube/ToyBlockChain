using System;
using ToyBlockChain.Service;

namespace ToyBlockChain.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            Node node = new Node();
            Client client = new Client(node);

            Console.WriteLine(client);
        }
    }
}
