using System;
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
                    Console.WriteLine("Running as a seed node...");
                }

                _address = _seedAddress;
                _routingTable = new RoutingTable();
                _routingTable.AddAddress(_address);
            }
            else
            {
                if (_logging)
                {
                    Console.WriteLine("Running as a non-seed node...");
                }

                Random rnd = new Random();
                _address = new Address(
                    Const.IP_ADDRESS,
                    rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));

                string routingTableString = Request(
                    _seedAddress, Protocol.REQUEST_ROUTING_TABLE);
                _routingTable = new RoutingTable(routingTableString);
                _routingTable.AddAddress(_address);

                Announce(_address.ToSerializedString());
            }

            Listen();
        }

        static void Listen()
        {}

        static string Request(Address address, string requestString)
        {
            // connect and prepare to stream
            TcpClient client = new TcpClient(
                address.IpAddress, address.PortNumber);
            NetworkStream stream = client.GetStream();

            // send data
            byte[] requestBytes = Encoding.UTF8.GetBytes(requestString);
            stream.Write(requestBytes, 0, requestBytes.Length);

            // recieve data
            byte[] responseBytes = new byte[Protocol.BUFFER_SIZE];
            string responseString = null;
            int responseLength = stream.Read(
                responseBytes, 0, responseBytes.Length);
            responseString = Encoding.UTF8.GetString(
                responseBytes, 0, responseLength);

            // cleanup
            stream.Close();
            client.Close();

            return responseString;
        }

        static void Announce(string announceString)
        {
            foreach (Address address in _routingTable.Routes)
            {
                // connect and prepare to stream
                TcpClient client = new TcpClient(
                    address.IpAddress, address.PortNumber);
                NetworkStream stream = client.GetStream();

                // send data
                byte[] announceBytes = Encoding.UTF8.GetBytes(announceString);
                stream.Write(announceBytes, 0, announceBytes.Length);

                // cleanup
                stream.Close();
                client.Close();
            }
        }
    }
}
