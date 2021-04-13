using System;
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
                Payload requestPayload;
                Payload responsePayload;

                // Generate a new random address.
                Random rnd = new Random();
                _address = new Address(
                    Const.IP_ADDRESS,
                    rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));

                // Request for the routing table from a seed node.
                requestPayload = new Payload(
                    Protocol.REQUEST_ROUTING_TABLE, "");
                responsePayload = Request(_seedAddress, requestPayload);

                // Update the routing table for this node.
                _routingTable = new RoutingTable(responsePayload.Body);
                _routingTable.AddAddress(_address);

                // Announce the address for this node to update
                // the routing table accross the network.
                requestPayload = new Payload(
                    Protocol.ANNOUNCE_ADDRESS, _address.ToSerializedString());
                Announce(requestPayload);
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
            byte[] requestBytes = new byte[Protocol.BUFFER_SIZE];
            string requestString = null;
            Payload requestPayload = null;

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                numBytesRead = stream.Read(
                    requestBytes, 0, requestBytes.Length);
                requestString = Encoding.UTF8.GetString(
                    requestBytes, 0, numBytesRead);
                requestPayload = new Payload(requestString);
                Log($"Received: {requestPayload.ToSerializedString()}",
                    ConsoleColor.Green);

                Payload responsePayload = ProcessRequestPayload(requestPayload);
                if (responsePayload != null)
                {
                    stream.Write(
                        responsePayload.ToSerializedBytes(), 0,
                        responsePayload.ToSerializedBytes().Length);
                    Log($"Sent: {responsePayload.ToSerializedString()}",
                        ConsoleColor.Red);
                }
                stream.Close();
                client.Close();
            }
        }

        static Payload Request(Address address, Payload requestPayload)
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

            // receive data
            byte[] responseBytes = new byte[Protocol.BUFFER_SIZE];
            string responseString = null;
            int responseLength = stream.Read(
                responseBytes, 0, responseBytes.Length);
            responseString = Encoding.UTF8.GetString(
                responseBytes, 0, responseLength);
            Payload responsePayload = new Payload(responseString);
            Log($"Received: {responsePayload.ToSerializedString()}",
                ConsoleColor.Green);
            ProcessResponsePayload(responsePayload);

            // cleanup
            stream.Close();
            client.Close();

            return responsePayload;
        }

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

        static Payload ProcessRequestPayload(Payload requestPayload)
        {
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
            if (requestPayload.Header == Protocol.REQUEST_ROUTING_TABLE)
            {
                return new Payload(
                    Protocol.RESPONSE_ROUTING_TABLE,
                    _routingTable.ToSerializedString());
            }
            else if (requestPayload.Header == Protocol.ANNOUNCE_ADDRESS)
            {
                _routingTable.AddAddress(
                    new Address(requestPayload.Body));
                Log("Updated: Routing Table", ConsoleColor.Yellow);
                return null;
            }
            return null;
        }

        static void ProcessResponsePayload(Payload responsePayload)
        {
            // TODO: Below is a placeholder.
            // This should be more fully fledged out.
            if (responsePayload.Header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable = new RoutingTable(responsePayload.Body);
                Log("Updated: Routing Table", ConsoleColor.Yellow);
            }
        }

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
