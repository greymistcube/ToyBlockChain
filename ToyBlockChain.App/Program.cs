using System;
using CommandLine;
using ToyBlockChain.Service;

namespace ToyBlockChain.App
{
    public class Program
    {
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

            bool seed = options.Seed;
            bool logging = options.Logging;
            bool verbose = options.Verbose;

            Node node = new Node(logging, verbose);
        }
    }
}
