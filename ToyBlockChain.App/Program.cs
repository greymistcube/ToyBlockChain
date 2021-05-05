using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommandLine;
using ToyBlockChain.Core;
using ToyBlockChain.Network;
using ToyBlockChain.Service;
using ToyBlockChain.Util;

namespace ToyBlockChain.App
{
    public partial class Program
    {
        private static bool _seedFlag;
        private static int _logLevel;
        private static bool _minerFlag;
        private static bool _clientFlag;
        private static bool _clearFlag;
        private static double _failRate;

        private static Address _SEED_ADDRESS = new Address(
            Const.IP_ADDRESS, Const.PORT_NUM_SEED);
        private static Address _address;
        private static RoutingTable _routingTable;
        private static INodeApp _node;
        private static Identity _identity;
        private static Account _account;
        private static Miner _miner;
        private static Client _client;

        public class Options
        {
            [Option('s', "seed",
                Default = false, Required = false,
                HelpText = "Make the node run as a seed.")]
            public bool SeedFlag { get; set; }

            [Option('l', "loglevel",
                Default = 0, Required = false,
                HelpText = "Logging level.")]
            public int LogLevel { get; set; }

            [Option('m', "miner",
                Default = false, Required = false,
                HelpText = "Run as a miner.")]
            public bool MinerFlag { get; set; }

            [Option('c', "client",
                Default = false, Required = false,
                HelpText = "Run as a client.")]
            public bool ClientFlag { get; set; }

            [Option('r', "clear",
                Default = false, Required = false,
                HelpText = "Screen clear between outputs.")]
            public bool ClearFlag { get; set; }

            [Option('f', "fail",
                Default = 0.0, Required = false,
                HelpText = "Failure rate for announcements.")]
            public double FailRate { get; set; }
        }

        static void Main(string[] args)
        {
            Init(args);
            Run();
        }

        static void Init(string[] args)
        {
            Options options = new Options();
            ParserResult<Options> result = Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    options = o;
                });
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine("Not Parsed");
                return;
            }

            _seedFlag = options.SeedFlag;
            _logLevel = options.LogLevel;
            _minerFlag = options.MinerFlag;
            _clientFlag = options.ClientFlag;
            _clearFlag = options.ClearFlag;
            _failRate = options.FailRate;

            // Set logging level.
            Logger.LogLevel = _logLevel;
            Logger.Clear = _clearFlag;

            Payload outboundPayload;

            // Get address for this node and sync routing table if necessary.
            if (_seedFlag)
            {
                Logger.Log(
                    "[Info] App: Running as a seed node...",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else
            {
                Logger.Log(
                    "[Info] App: Running as a non-seed node...",
                    Logger.INFO, ConsoleColor.Blue);
            }

            // Create a new routing table and sync.
            _routingTable = new RoutingTable();
            if (!_seedFlag)
            {
                SyncRoutingTable();
            }

            // Set address for this node.
            _address = GetLocalAddress();

            // Add the address for this node and announce the address
            // to update the routing tables accross the network.
            _routingTable.AddAddress(_address);
            outboundPayload = new Payload(
                Protocol.ANNOUNCE_ADDRESS, _address.ToSerializedString());
            Announce(outboundPayload);

            _node = new Node();
            if (!_seedFlag)
            {
                Address address = GetRandomAddress();
                SyncNode(address);
            }
        }

        static void Run()
        {
            Thread clientThread = null;
            Thread minerThread = null;
            Thread listenThread = null;

            // If this node acts as an active node, create an identity.
            if (_minerFlag || _clientFlag)
            {
                _identity = new Identity();
                _account = Account.AccountFactory(
                    _identity.Address, UserAccount.TYPE);

                if (_minerFlag)
                {
                    _miner = new Miner((INodeMiner)_node, _identity, Announce);
                    minerThread = new Thread(_miner.Run);
                    minerThread.Start();
                }
                if (_clientFlag)
                {
                    _client = new Client(
                        (INodeClient)_node, _identity, Announce);
                    clientThread = new Thread(_client.Run);
                    clientThread.Start();
                }
            }

            listenThread = new Thread(() => Listen(_address));
            listenThread.Start();
        }

        /// <summary>
        /// Syncs the routing table to the one kept by a seed node.
        /// This method makes a request to a seed node to retrieve
        /// the routing table and the local routing table is updated.
        /// Actual update is done when processing the payload received
        /// as a result of making the request.
        /// </summary>
        private static void SyncRoutingTable()
        {
            // Basic sanity check.
            if (_seedFlag)
            {
                throw new ArgumentException(
                    "seed node cannot sync routing table");
            }

            Payload outboundPayload = new Payload(
                Protocol.REQUEST_ROUTING_TABLE, "");
            Request(_SEED_ADDRESS, outboundPayload);
        }

        private static void SyncNode(Address address)
        {
            // Basic sanity check.
            if (_address == address)
            {
                throw new ArgumentException(
                    $"cannot sync to self: {address.ToSerializedString()}");
            }

            _node.Dump();
            SyncBlockChain(address);
            SyncTransactionPool(address);
        }

        private static void SyncBlockChain(Address address)
        {
            Request(
                address, new Payload(Protocol.REQUEST_BLOCKCHAIN, ""));
        }

        private static void SyncTransactionPool(Address address)
        {
            Request(
                address, new Payload(Protocol.REQUEST_TRANSACTION_POOL, ""));
        }

        private static void Listen(Address address)
        {
            Logger.Log(
                "[Info] App: Starting to listen...",
                Logger.INFO, ConsoleColor.Blue);

            TcpListener server = new TcpListener(
                IPAddress.Parse(address.IpAddress), address.PortNumber);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                Payload inboundPayload = StreamHandler.ReadPayload(stream);
                string header = inboundPayload.Header;

                if (Protocol.REQUEST.Contains(header))
                {
                    ProcessRequestPayload(stream, inboundPayload);
                    stream.Close();
                    client.Close();
                }
                else if (Protocol.ANNOUNCE.Contains(header))
                {
                    stream.Close();
                    client.Close();
                    ProcessAnnouncePayload(inboundPayload);
                }
                else
                {
                    throw new ArgumentException(
                        $"invalid protocol header: {header}");
                }
            }
        }

        /// <summary>
        /// Makes a data request to given address.
        /// </summary>
        private static void Request(Address address, Payload outboundPayload)
        {
            // connect and prepare to stream
            TcpClient client = new TcpClient(
                address.IpAddress, address.PortNumber);
            NetworkStream stream = client.GetStream();

            // send data
            StreamHandler.WritePayload(stream, outboundPayload);

            // receive data and process
            Payload inboundPayload = StreamHandler.ReadPayload(stream);
            ProcessResponsePayload(inboundPayload);

            // cleanup
            stream.Close();
            client.Close();
        }

        /// <summary>
        /// Announces to all addresses except this node's address.
        /// </summary>
        private static void Announce(Payload outboundPayload)
        {
            Random random = new Random();
            double p = random.NextDouble();
            // Force address announcement to always succeed.
            if (outboundPayload.Header == Protocol.ANNOUNCE_ADDRESS
                || _failRate < p)
            {
                foreach (Address address in _routingTable.Table)
                {
                    if (!_address.Equals(address))
                    {
                        // connect and prepare to stream
                        TcpClient client = new TcpClient(
                            address.IpAddress, address.PortNumber);
                        NetworkStream stream = client.GetStream();

                        // send data
                        StreamHandler.WritePayload(stream, outboundPayload);

                        // cleanup
                        stream.Close();
                        client.Close();
                    }
                }
            }
        }
    }
}
