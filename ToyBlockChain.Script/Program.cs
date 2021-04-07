using System;
using System.Threading;
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

            Thread t1 = new Thread(client.Run);
            Thread t2 = new Thread(miner.Run);

            t1.Start();
            t2.Start();
        }
    }
}
