using System;

namespace ToyBlockChain.Core
{
    public class OperationInvalidException : Exception
    {
        public OperationInvalidException()
        {
        }

        public OperationInvalidException(string message) : base(message)
        {
        }
    }

    public class OperationUnsoundException : Exception
    {
        public OperationUnsoundException()
        {
        }

        public OperationUnsoundException(string message) : base(message)
        {
        }
    }

    public class Operation
    {
        protected string _type;
        protected string _data;

        public const string SEPARATOR = "<O>";

        public Operation(string type, string data)
        {
            _type = type;
            _data = data;
        }

        public Operation(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _type = substrings[0];
            _data = substrings[1];
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public string Data
        {
            get
            {
                return _data;
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Type, Data });
        }
    }
}