using System;

namespace ToyBlockChain.Core
{
    public abstract class ActionOnUser : Action
    {
        public const string TARGET = "user";

        public ActionOnUser(string target, string move, string value)
            : base(target, move, value)
        {
        }

        public static Action ActionOnUserFactory(
            string target, string move, string value)
        {
            switch (move)
            {
                case ActionOnUserRegister.MOVE:
                    return new ActionOnUserRegister(target, move, value);
                case ActionOnUserMessage.MOVE:
                    return new ActionOnUserMessage(target, move, value);
                default:
                    throw new ArgumentException($"invalid move: {move}");
            }
        }
    }

    public class ActionOnUserRegister : ActionOnUser
    {
        public const string MOVE = "register";

        public ActionOnUserRegister(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }

    public class ActionOnUserMessage : ActionOnUser
    {
        public const string MOVE = "message";

        public ActionOnUserMessage(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }
}