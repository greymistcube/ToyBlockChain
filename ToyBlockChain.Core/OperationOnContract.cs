using System;

namespace ToyBlockChain.Core
{
    public abstract class OperationOnContract : Operation
    {
        public const string TARGET = "contract";

        public OperationOnContract(string target, string move, string value)
            : base(target, move, value)
        {
        }

        public static Operation OperationOnContractFactory(
            string target, string move, string value)
        {
            switch (move)
            {
                case OperationOnContractPlay.MOVE:
                    return new OperationOnContractPlay(target, move, value);
                case OperationOnContractReveal.MOVE:
                    return new OperationOnContractReveal(target, move, value);
                default:
                    throw new ArgumentException($"invalid move: {move}");
            }
        }
    }

    public class OperationOnContractPlay : OperationOnContract
    {
        public const string MOVE = "play";

        public OperationOnContractPlay(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }

    public class OperationOnContractReveal : OperationOnContract
    {
        public const string MOVE = "reveal";

        public OperationOnContractReveal(
            string target, string move, string value)
            : base(target, move, value)
        {
        }
    }
}