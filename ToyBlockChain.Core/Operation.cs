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

    public class OperationInvalidInternalException : OperationInvalidException
    {
        public OperationInvalidInternalException()
        {
        }

        public OperationInvalidInternalException(string message) : base(message)
        {
        }
    }

    public class OperationInvalidExternalException : OperationInvalidException
    {
        public OperationInvalidExternalException()
        {
        }

        public OperationInvalidExternalException(string message) : base(message)
        {
        }
    }

    public abstract class Operation
    {
        protected string _target;
        protected string _move;
        protected string _value;

        public const string SEPARATOR = "<O>";

        public Operation(string target, string move, string value)
        {
            _target = target;
            _move = move;
            _value = value;
        }

        public string Target
        {
            get
            {
                return _target;
            }
        }

        public string Move
        {
            get
            {
                return _move;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Target, Move, Value });
        }

        public static Operation OperationFactory(
            string target, string move, string value)
        {
            switch (target)
            {
                case OperationOnUser.TARGET:
                    return OperationOnUser.OperationOnUserFactory(
                        target, move, value);
                case OperationOnContract.TARGET:
                    return OperationOnContract.OperationOnContractFactory(
                        target, move, value);
                default:
                    throw new ArgumentException($"invalid target: {target}");
            }
        }

        public static Operation OperationFactory(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            return OperationFactory(
                substrings[0], substrings[1], substrings[2]);
        }
    }
}