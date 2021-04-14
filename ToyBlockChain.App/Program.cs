using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommandLine;
using ToyBlockChain.Network;
using ToyBlockChain.Service;
using ToyBlockChain.Util;

namespace ToyBlockChain.App
{
    public class Program
    {
        private static bool _seed;
        private static int _logLevel;
        private static Address _seedAddress;
        private static Address _address;
        private static RoutingTable _routingTable;

        public class Options
        {
            [Option('s', "seed",
                Default = false, Required = false,
                HelpText = "Make the node run as a seed.")]
            public bool Seed { get; set; }

            [Option('l', "loglevel",
                Default = 0, Required = false,
                HelpText = "Logging level.")]
            public int LogLevel { get; set; }

            [Option('r', "clear",
                Default = false, Required = false,
                HelpText = "Screen clear between outputs.")]
            public bool Clear { get; set; }
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

            _seed = options.Seed;
            _logLevel = options.LogLevel;
            Logger.LogLevel = _logLevel;

            _seedAddress = new Address(Const.IP_ADDRESS, Const.PORT_NUM_SEED);

            Payload outboundPayload;

            Node node = new Node();

            // Get address for this node and sync routing table if necessary.
            if (_seed)
            {
                Logger.Log(
                    "Running as a seed node...",
                    Logger.INFO, ConsoleColor.Blue);

                _address = _seedAddress;
                _routingTable = new RoutingTable();
            }
            else
            {
                Logger.Log(
                    "Running as a non-seed node...",
                    Logger.INFO, ConsoleColor.Blue);

                // TODO: Handle port collision.
                // Generate a new random address.
                Random rnd = new Random();
                _address = new Address(
                    Const.IP_ADDRESS,
                    rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));

                // Request for the routing table from a seed node.
                outboundPayload = new Payload(
                    Protocol.REQUEST_ROUTING_TABLE, "");
                Request(_seedAddress, outboundPayload);
            }

            // Add the address for this node and announce the address
            // to update the routing tables accross the network.
            _routingTable.AddAddress(_address);
            outboundPayload = new Payload(
                Protocol.ANNOUNCE_ADDRESS, _address.ToSerializedString());
            Announce(outboundPayload);

            Listen(_address);
        }

        static void Listen(Address address)
        {
            Logger.Log(
                "Starting to listen...",
                Logger.INFO, ConsoleColor.White);

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
        static void Request(Address address, Payload outboundPayload)
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
        /// Announce to all addresses except this node's address.
        /// </summary>
        static void Announce(Payload outboundPayload)
        {
            foreach (Address address in _routingTable.Routes)
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
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
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
                throw new NotImplementedException(
                    $"invalid protocol header: {header}");
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
                _routingTable.AddAddress(
                    new Address(inboundPayload.Body));
                Logger.Log(
                    "Updated: Address added to routing table",
                    Logger.INFO, ConsoleColor.Yellow);
            }
            else if (header == Protocol.ANNOUNCE_TRANSACTION)
            {
                throw new NotImplementedException(
                    $"invalid protocol header: {header}");
            }
            else if (header == Protocol.ANNOUNCE_BLOCK)
            {
                throw new NotImplementedException(
                    $"invalid protocol header: {header}");
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
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
            string header = inboundPayload.Header;
            if (header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable = new RoutingTable(inboundPayload.Body);
                Logger.Log(
                    "Updated: Routing table synced",
                    Logger.INFO, ConsoleColor.Yellow);
            }
            else if (header == Protocol.RESPONSE_BLOCKCHAIN)
            {
                throw new NotImplementedException(
                    $"invalid protocol header: {header}");
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }
    }
}
