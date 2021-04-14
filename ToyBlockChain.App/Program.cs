using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommandLine;
using ToyBlockChain.Service;

namespace ToyBlockChain.App
{
    public class Program
    {
        private static bool _seed;
        private static bool _logging;
        private static bool _verbose;
        private static Address _seedAddress;
        private static Address _address;
        private static RoutingTable _routingTable;

        public class Options
        {
            [Option('s', "seed",
                Default = false, Required = false,
                HelpText = "Make the node run as a seed.")]
            public bool Seed { get; set; }

            [Option('l', "logging",
                Default = false, Required = false,
                HelpText = "Print output to a console.")]
            public bool Logging { get; set; }

            [Option('v', "verbose",
                Default = false, Required = false,
                HelpText = "Make the output verbose.")]
            public bool Verbose { get; set; }

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
            _logging = options.Logging;
            _verbose = options.Verbose;
            _seedAddress = new Address(Const.IP_ADDRESS, Const.PORT_NUM_SEED);

            Node node = new Node(_logging, _verbose);

            if (_seed)
            {
                Log("Running as a seed node...", ConsoleColor.Blue);

                _address = _seedAddress;
                _routingTable = new RoutingTable();
                _routingTable.AddAddress(_address);
            }
            else
            {
                Log("Running as a non-seed node...", ConsoleColor.Blue);
                Payload outboundPayload;
                Payload inboundPayload;

                // Generate a new random address.
                Random rnd = new Random();
                _address = new Address(
                    Const.IP_ADDRESS,
                    rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));

                // Request for the routing table from a seed node.
                outboundPayload = new Payload(
                    Protocol.REQUEST_ROUTING_TABLE, "");
                Request(_seedAddress, outboundPayload);

                // Update the routing table for this node.

                // Add the address for this node and announce the address
                // to update the routing tables accross the network.
                _routingTable.AddAddress(_address);
                inboundPayload = new Payload(
                    Protocol.ANNOUNCE_ADDRESS, _address.ToSerializedString());
                Announce(inboundPayload);
            }

            Listen(_address);
        }

        static void Listen(Address address)
        {
            Log("Starting to listen...");

            TcpListener server = new TcpListener(
                IPAddress.Parse(address.IpAddress), address.PortNumber);
            server.Start();

            int numBytesRead = 0;
            byte[] inboundBytes = new byte[Protocol.BUFFER_SIZE];
            string inboundString = null;
            Payload inboundPayload = null;

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                numBytesRead = stream.Read(
                    inboundBytes, 0, inboundBytes.Length);
                inboundString = Encoding.UTF8.GetString(
                    inboundBytes, 0, numBytesRead);
                inboundPayload = new Payload(inboundString);
                Log($"Received: {inboundPayload.ToSerializedString()}",
                    ConsoleColor.Green);

                Payload outboundPayload = ProcessInboundPayload(inboundPayload);
                if (outboundPayload != null)
                {
                    stream.Write(
                        outboundPayload.ToSerializedBytes(), 0,
                        outboundPayload.ToSerializedBytes().Length);
                    Log($"Sent: {outboundPayload.ToSerializedString()}",
                        ConsoleColor.Red);
                }
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
            stream.Write(
                outboundPayload.ToSerializedBytes(), 0,
                outboundPayload.ToSerializedBytes().Length);
            Log($"Sent: {outboundPayload.ToSerializedString()}",
                ConsoleColor.Red);

            // receive data
            byte[] inboundBytes = new byte[Protocol.BUFFER_SIZE];
            string inboundString = null;
            int numBytesRead = stream.Read(
                inboundBytes, 0, inboundBytes.Length);
            inboundString = Encoding.UTF8.GetString(
                inboundBytes, 0, numBytesRead);
            Payload inboundPayload = new Payload(inboundString);
            Log($"Received: {inboundPayload.ToSerializedString()}",
                ConsoleColor.Green);
            ProcessInboundPayload(inboundPayload);

            // cleanup
            stream.Close();
            client.Close();
        }

        /// <summary>
        /// Announce to all addresses except this node's address.
        /// </summary>
        static void Announce(Payload requestPayload)
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
                    stream.Write(
                        requestPayload.ToSerializedBytes(), 0,
                        requestPayload.ToSerializedBytes().Length);
                    Log($"Sent: {requestPayload.ToSerializedString()}",
                        ConsoleColor.Red);

                    // cleanup
                    stream.Close();
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Processes an incoming payload.
        /// </summary>
        public static Payload ProcessInboundPayload(Payload payload)
        {
            string header = payload.Header;
            if (Protocol.REQUEST.Contains(header))
            {
                return ProcessRequestPayload(payload);
            }
            else if (Protocol.ANNOUNCE.Contains(header))
            {
                ProcessAnnouncePayload(payload);
                return null;
            }
            else if (Protocol.RESPONSE.Contains(header))
            {
                ProcessResponsePayload(payload);
                return null;
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
        private static Payload ProcessRequestPayload(Payload requestPayload)
        {
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
            string header = requestPayload.Header;
            if (header == Protocol.REQUEST_ROUTING_TABLE)
            {
                return new Payload(
                    Protocol.RESPONSE_ROUTING_TABLE,
                    _routingTable.ToSerializedString());
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
        private static void ProcessAnnouncePayload(Payload announcePayload)
        {
            string header = announcePayload.Header;
            if (header == Protocol.ANNOUNCE_ADDRESS)
            {
                _routingTable.AddAddress(
                    new Address(announcePayload.Body));
                Log("Updated: Address added to routing table",
                    ConsoleColor.Yellow);
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
        private static void ProcessResponsePayload(Payload responsePayload)
        {
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
            string header = responsePayload.Header;
            if (header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable = new RoutingTable(responsePayload.Body);
                Log("Updated: Routing table synced", ConsoleColor.Yellow);
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

        /// <summary>
        /// Simple helper method to log output.
        /// </summary>
        static void Log(
            string text,
            System.ConsoleColor color = ConsoleColor.White)
        {
            if (_logging)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }
    }
}
