using System;

namespace ToyBlockChain.Core
{
    public abstract class OperationOnContract : Operation
    {
        public OperationOnContract(string type, string data) : base(type, data)
        {
        }
    }

    public class OperationOnContractPlay : OperationOnContract
    {
        public const string TYPE = "play";

        public OperationOnContractPlay(string type, string data)
            : base(type, data)
        {
        }
    }

    public class OperationOnContractReveal : OperationOnContract
    {
        public const string TYPE = "reveal";

        public OperationOnContractReveal(string type, string data)
            : base(type, data)
        {
        }
    }
}