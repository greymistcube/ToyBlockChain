using System;
using System.Collections.Generic;
using System.Threading;
using ToyBlockChain.Service;

namespace ToyBlockChain.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            int numClients = 4;
            int numMiners = 4;

            Node node = new Node(true);

            List<Client> clients = new List<Client>();
            List<Miner> miners = new List<Miner>();
            List<Thread> clientThreads = new List<Thread>();
            List<Thread> minerThreads = new List<Thread>();

            for (int i = 0; i < numClients; i++)
            {
                Client client = new Client(node);
                Thread clientThread = new Thread(client.Run);
                clients.Add(client);
                clientThreads.Add(clientThread);
            }
            for (int i = 0; i < numMiners; i++)
            {
                Miner miner = new Miner(node);
                Thread minerThread = new Thread(miner.Run);
                miners.Add(miner);
                minerThreads.Add(minerThread);
            }

            foreach (Thread clientThread in clientThreads)
            {
                clientThread.Start();
            }
            foreach (Thread minerThread in minerThreads)
            {
                minerThread.Start();
            }

            foreach (Thread clientThread in clientThreads)
            {
                clientThread.Join();
            }
            foreach (Thread minerThread in minerThreads)
            {
                minerThread.Join();
            }
        }
    }
}
