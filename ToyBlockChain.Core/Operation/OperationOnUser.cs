using System;

namespace ToyBlockChain.Core
{
    public abstract class OperationOnUser : Operation
    {
        public OperationOnUser(string type, string data) : base(type, data)
        {
        }
    }

    public class OperationOnUserRegister : OperationOnUser
    {
        public const string TYPE = "register";

        public OperationOnUserRegister(string type, string data)
            : base(type, data)
        {
        }
    }

    public class OperationOnUserMessage : OperationOnUser
    {
        public const string TYPE = "message";

        public OperationOnUserMessage(string type, string data)
            : base(type, data)
        {
        }
    }
}