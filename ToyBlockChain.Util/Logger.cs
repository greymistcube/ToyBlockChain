using System;

namespace ToyBlockChain.Util
{
    public static class Logger
    {
        private static bool? _logging;

        public static bool Logging
        {
            get
            {
                if (_logging.HasValue)
                {
                    return (bool)_logging;
                }
                else
                {
                    throw new MethodAccessException(
                        $"Logging must be set before accessing its value");
                }
            }
            set
            {
                if (_logging.HasValue)
                {
                    throw new MethodAccessException(
                        $"Logging can be set only once: {(bool)_logging}");
                }
                else
                {
                    _logging = value;
                }
            }
        }

        /// <summary>
        /// Simple helper method to log output.
        /// </summary>
        public static void Log(
            string text,
            System.ConsoleColor color = ConsoleColor.White)
        {
            if(Logging)
            {
                    Console.ForegroundColor = color;
                    Console.WriteLine(text);
                    Console.ResetColor();
            }
        }
    }
}
