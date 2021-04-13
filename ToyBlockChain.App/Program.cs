using System;
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

            Node node = new Node(_logging, _verbose);

            if (_seed)
            {
                _address = new Address(
                    Const.IP_ADDRESS, Const.PORT_NUM_SEED);
                _routingTable = new RoutingTable();
                _routingTable.AddAddress(_address);
            }
            else
            {
                Random rnd = new Random();
                _address = new Address(
                    Const.IP_ADDRESS,
                    rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));
                _routingTable = new RoutingTable();
            }

            Listen();
        }

        static void Listen()
        {}

        static void Announce()
        {}
    }
}
