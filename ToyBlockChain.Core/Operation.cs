using System;

namespace ToyBlockChain.Core
{
    public abstract class Operation
    {
        protected string _target;
        protected string _move;
        protected string _value;

        public const string SEPARATOR = "<A>";

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
    }
}