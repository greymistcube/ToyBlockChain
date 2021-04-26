using System;

namespace ToyBlockChain.Core
{
    public abstract class ContractTargetedOperation : Operation
    {
        public const string PLAY = "play";
        public const string REVEAL = "reveal";

        public ContractTargetedOperation(string type, string data)
            : base(type, data)
        {
        }
    }
}