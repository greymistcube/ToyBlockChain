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

            Client client1 = new Client(node);
            Client client2 = new Client(node);
            Miner miner1 = new Miner(node);
            Miner miner2 = new Miner(node);

            Thread c1 = new Thread(client1.Run);
            Thread c2 = new Thread(client2.Run);
            Thread m1 = new Thread(miner1.Run);
            Thread m2 = new Thread(miner2.Run);

            c1.Start();
            c2.Start();
            m1.Start();
            m2.Start();
        }
    }
}
