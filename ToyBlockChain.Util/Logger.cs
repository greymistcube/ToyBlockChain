using System;

namespace ToyBlockChain.Util
{
    public static class Logger
    {
        public const int INFO = 1;
        public const int DEBUG = 2;
        private static int _logLevel = 0;
        private static bool _clear = false;

        public static int LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
            }
        }

        public static bool Clear
        {
            get
            {
                return _clear;
            }
            set
            {
                _clear = value;
            }
        }

        /// <summary>
        /// Simple helper method to log output.
        /// </summary>
        public static void Log(
            string text,
            int textLevel = 1,
            System.ConsoleColor color = ConsoleColor.White)
        {
            if(textLevel <= _logLevel)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }
    }
}
