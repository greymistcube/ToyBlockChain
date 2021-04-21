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
    public class Program
    {
        private static bool _seedFlag;
        private static int _logLevel;
        private static bool _minerFlag;
        private static bool _clientFlag;
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
        }

        static void Main(string[] args)
        {
            Options options = new Options();
            ParserResult<Options> result = Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o => {
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

            // Set logging level.
            Logger.LogLevel = _logLevel;

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

            Thread clientThread = null;
            Thread minerThread = null;
            Thread listenThread = null;

            // If this node acts as an active node, create an identity.
            if (_minerFlag || _clientFlag)
            {
                _identity = new Identity();
                _account = new Account(_identity.Address, 0);

                _node.AddAccountToCatalogue(_account);
                outboundPayload = new Payload(
                    Protocol.ANNOUNCE_ACCOUNT, _account.ToSerializedString());
                Announce(outboundPayload);

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

            SyncBlockChain(address);
            SyncAccountCatalogue(address);
            SyncTransactionPool(address);
        }

        private static void SyncBlockChain(Address address)
        {
            Request(
                address, new Payload(Protocol.REQUEST_BLOCKCHAIN, ""));
        }

        private static void SyncAccountCatalogue(Address address)
        {
            Request(
                address, new Payload(Protocol.REQUEST_ACCOUNT_CATALOGUE, ""));
        }

        private static void SyncTransactionPool(Address address)
        {
            Request(
                address, new Payload(Protocol.REQUEST_TRANSACTION_POOL, ""));
        }

        private static Address GetLocalAddress()
        {
            if (_seedFlag)
            {
                return _SEED_ADDRESS;
            }
            else
            {
                // Generate a new random address that is not already in
                // the routing table.
                Random rnd = new Random();
                while (true)
                {
                    Address address = new Address(
                        Const.IP_ADDRESS,
                        rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));
                    if (!_routingTable.Table.Contains(address))
                    {
                        return address;
                    }
                }
            }
        }

        /// <summary>
        /// Get a random address from the routing table excluding
        /// this node's address.
        /// </summary>
        private static Address GetRandomAddress()
        {
            if (_routingTable.Table.Count <= 1)
            {
                throw new MethodAccessException(
                    "invalid access; routing table size too small: "
                    + $"{_routingTable.Table.Count}");
            }
            else
            {
                Address address;
                Random rnd = new Random();
                int idx;

                idx = rnd.Next(_routingTable.Table.Count);
                address = _routingTable.Table[idx];
                if (_address.Equals(address))
                {
                    idx = (idx + 1) + rnd.Next(_routingTable.Table.Count - 1);
                    idx = idx % _routingTable.Table.Count;
                    address = _routingTable.Table[idx];
                }
                return address;
            }
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
                ProcessInboundPayload(stream, inboundPayload);

                stream.Close();
                client.Close();
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
            ProcessInboundPayload(null, inboundPayload);

            // cleanup
            stream.Close();
            client.Close();
        }

        /// <summary>
        /// Announces to all addresses except this node's address.
        /// </summary>
        private static void Announce(Payload outboundPayload)
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

        /// <summary>
        /// Processes an incoming payload.
        /// </summary>
        private static void ProcessInboundPayload(
            NetworkStream stream, Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (Protocol.REQUEST.Contains(header))
            {
                ProcessRequestPayload(stream, inboundPayload);
            }
            else if (Protocol.ANNOUNCE.Contains(header))
            {
                ProcessAnnouncePayload(inboundPayload);
            }
            else if (Protocol.RESPONSE.Contains(header))
            {
                ProcessResponsePayload(inboundPayload);
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }

        /// <summary>
        /// Processes an incoming payload with a request header.
        /// </summary>
        private static void ProcessRequestPayload(
            NetworkStream stream, Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.REQUEST_ROUTING_TABLE)
            {
                Payload outboundPayload = new Payload(
                    Protocol.RESPONSE_ROUTING_TABLE,
                    _routingTable.ToSerializedString());
                StreamHandler.WritePayload(stream, outboundPayload);
            }
            else if (header == Protocol.REQUEST_BLOCKCHAIN)
            {
                lock (_node)
                {
                    Payload outboundPayload = new Payload(
                        Protocol.RESPONSE_BLOCKCHAIN,
                        _node.GetBlockChainSerializedString());
                    StreamHandler.WritePayload(stream, outboundPayload);
                }
            }
            else if (header == Protocol.REQUEST_ACCOUNT_CATALOGUE)
            {
                lock (_node)
                {
                    Payload outboundPayload = new Payload(
                        Protocol.RESPONSE_ACCOUNT_CATALOGUE,
                        _node.GetAccountCatalogueSerializedString());
                    StreamHandler.WritePayload(stream, outboundPayload);
                }
            }
            else if (header == Protocol.REQUEST_TRANSACTION_POOL)
            {
                lock (_node)
                {
                    Payload outboundPayload = new Payload(
                        Protocol.RESPONSE_TRANSACTION_POOL,
                        _node.GetTransactionPoolSerializedString());
                    StreamHandler.WritePayload(stream, outboundPayload);
                }
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }

        /// <summary>
        /// Processes an incoming payload with an announce header.
        /// </summary>
        private static void ProcessAnnouncePayload(Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.ANNOUNCE_ADDRESS)
            {
                Address address = new Address(inboundPayload.Body);
                _routingTable.AddAddress(address);
                Logger.Log(
                    $"[Info] App: Address {address.PortNumber} "
                    + "added to routing table",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.ANNOUNCE_ACCOUNT)
            {
                Account account = new Account(inboundPayload.Body);
                lock (_node)
                {
                    _node.AddAccountToCatalogue(account);
                }
                Logger.Log(
                    $"[Info] App: Account {account.LogId} "
                    + "added to account catalogue",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.ANNOUNCE_TRANSACTION)
            {
                Transaction transaction = new Transaction(
                    inboundPayload.Body);
                try
                {
                    lock (_node)
                    {
                        _node.AddTransactionToPool(transaction);
                    }
                    Logger.Log(
                        $"[Info] App: Transaction {transaction.LogId} "
                        + "added to transaction pool",
                        Logger.INFO, ConsoleColor.Blue);
                    Announce(inboundPayload);
                }
                catch (TransactionSenderInPoolException)
                {
                    Logger.Log(
                        $"[Info] App: Transaction {transaction.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: Transaction sender {transaction.Sender} "
                        + "already in transaction pool",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (TransactionInPoolException)
                {
                    Logger.Log(
                        $"[Info] App: Transaction {transaction.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: Transaction {transaction.HashString} "
                        + "already in transaction pool",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (TransactionInvalidForChainException)
                {
                    Logger.Log(
                        $"[Info] App: Transaction {transaction.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: Transaction {transaction.HashString} "
                        + "already in blockchain",
                        Logger.DEBUG, ConsoleColor.Red);
                }
            }
            else if (header == Protocol.ANNOUNCE_BLOCK)
            {
                Block block = new Block(inboundPayload.Body);
                try
                {
                    lock (_node)
                    {
                        _node.AddBlockToChain(block);
                    }
                    Logger.Log(
                        $"[Info] App: Block {block.LogId} "
                        + "added to blockchain",
                        Logger.INFO, ConsoleColor.Blue);
                    Announce(inboundPayload);
                }
                catch (TransactionNotInPoolException)
                {
                    Logger.Log(
                        $"[Info] App: Block {block.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        "[Debug] App: Transaction "
                        + $"{block.Transaction.HashString} not found in pool.",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (BlockIndexLowForChainException)
                {
                    Logger.Log(
                        $"[Info] App: Block {block.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: Block index too low",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (BlockIndexHighForChainException)
                {
                    throw new NotImplementedException();
                }
                catch (BlockInvalidTimestampException)
                {
                    throw new NotImplementedException();
                }
                catch (BlockPreviousHashMismatchException)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }

        /// <summary>
        /// Processes an incoming payload with a response header.
        /// </summary>
        private static void ProcessResponsePayload(Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable.Sync(inboundPayload.Body);
                Logger.Log(
                    "[Info] App: Routing table synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.RESPONSE_BLOCKCHAIN)
            {
                lock (_node)
                {
                    _node.SyncBlockChain(inboundPayload.Body);
                }
                Logger.Log(
                    "[Info] App: Blockchain synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.RESPONSE_ACCOUNT_CATALOGUE)
            {
                lock (_node)
                {
                    _node.SyncAccountCatalogue(inboundPayload.Body);
                }
                Logger.Log(
                    "[Info] App: Account catalogue synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.RESPONSE_TRANSACTION_POOL)
            {
                lock (_node)
                {
                    _node.SyncTransactionPool(inboundPayload.Body);
                }
                Logger.Log(
                    "[Info] App: Transaction pool synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }
    }
}
