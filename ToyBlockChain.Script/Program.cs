using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using ToyBlockChain.Service;

namespace ToyBlockChain.Script
{
    class Program
    {
        public class Options
        {
            [Option('l', "logging",
                Default = 0, Required = false,
                HelpText = (
                    "Logging level.\n"
                    + "0: None.\n"
                    + "1: Minimal.\n"
                    + "2: Verbose."))]
            public int LogLevel { get; set; }

            [Option('c', "client",
                Default = 1, Required = false,
                HelpText = "Number of clients.")]
            public int NumClients { get; set; }

            [Option('m', "miner",
                Default = 1, Required = false,
                HelpText = "Number of miners.")]
            public int NumMiners { get; set; }
        }

        static void Main(string[] args)
        {
            Options options = new Options();
            ParserResult<Options> result = Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o => {
                    options = o;
                    if (options.NumClients < 1)
                    {
                        throw new ArgumentException(
                            "number of clients must be positive");
                    }
                    else if (options.NumMiners < 1)
                    {
                        throw new ArgumentException(
                            "number of miners must be positive");
                    }
                });
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine("Not Parsed");
                return;
            }

            int logLevel = options.LogLevel;
            int numClients = options.NumClients;
            int numMiners = options.NumMiners;

            Node node = new Node();

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
