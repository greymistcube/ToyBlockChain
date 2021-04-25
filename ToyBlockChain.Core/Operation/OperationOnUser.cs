using System;

namespace ToyBlockChain.Core
{
    public abstract class OperationOnUser : Operation
    {
        public const string TARGET = "user";

        public OperationOnUser(string target, string move, string value)
            : base(target, move, value)
        {
        }

        public static Operation OperationOnUserFactory(
            string target, string move, string value)
        {
            switch (move)
            {
                case OperationOnUserRegister.MOVE:
                    return new OperationOnUserRegister(target, move, value);
                case OperationOnUserMessage.MOVE:
                    return new OperationOnUserMessage(target, move, value);
                default:
                    throw new ArgumentException($"invalid move: {move}");
            }
        }
    }

    public class OperationOnUserRegister : OperationOnUser
    {
        public const string MOVE = "register";

        public OperationOnUserRegister(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }

    public class OperationOnUserMessage : OperationOnUser
    {
        public const string MOVE = "message";

        public OperationOnUserMessage(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }
}