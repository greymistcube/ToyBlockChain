using System;

namespace ToyBlockChain.Core
{
    public class UserTargetedOperation : Operation
    {
        public const string REGISTER = "register";
        public const string MESSAGE = "message";

        public UserTargetedOperation(string type, string data)
            : base(type, data)
        {
        }
    }
}