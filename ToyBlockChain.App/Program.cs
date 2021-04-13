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
                HelpText = "Makes the node run as a seed.")]
            public bool Seed { get; set; }

            [Option('l', "logging",
                Default = false, Required = false,
                HelpText = "Prints output to a console.")]
            public bool Logging { get; set; }

            [Option('v', "verbose",
                Default = false, Required = false,
                HelpText = "Makes the output verbose.")]
            public bool Verbose { get; set; }
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
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Running as a seed node...");
                    Console.ResetColor();
                }

                _address = _seedAddress;
                _routingTable = new RoutingTable();
                _routingTable.AddAddress(_address);
            }
            else
            {
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Running as a non-seed node...");
                    Console.ResetColor();
                }
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
            if (_logging)
            {
                Console.WriteLine("Starting to listen...");
            }

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
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        $"Received: {requestPayload.ToSerializedString()}");
                    Console.ResetColor();
                }

                Payload responsePayload = ProcessRequestPayload(requestPayload);
                if (responsePayload != null)
                {
                    stream.Write(
                        responsePayload.ToSerializedBytes(), 0,
                        responsePayload.ToSerializedBytes().Length);
                    if (_logging)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            $"Sent: {responsePayload.ToSerializedString()}");
                        Console.ResetColor();
                    }
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
            if (_logging)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"Sent: {requestPayload.ToSerializedString()}");
                Console.ResetColor();
            }

            // receive data
            byte[] responseBytes = new byte[Protocol.BUFFER_SIZE];
            string responseString = null;
            int responseLength = stream.Read(
                responseBytes, 0, responseBytes.Length);
            responseString = Encoding.UTF8.GetString(
                responseBytes, 0, responseLength);
            Payload responsePayload = new Payload(responseString);
            if (_logging)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    $"Received: {responsePayload.ToSerializedString()}");
                Console.ResetColor();
            }
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
                    if (_logging)
                    {
                        Console.WriteLine(
                            $"Sent: {requestPayload.ToSerializedString()}");
                    }

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
                return null;
            }
            return null;
        }

        static void ProcessResponsePayload(Payload responsePayload)
        {
            if (responsePayload.Header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable = new RoutingTable(responsePayload.Body);
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Updated: Routing Table");
                    Console.ResetColor();
                }
            }
        }
    }
}
