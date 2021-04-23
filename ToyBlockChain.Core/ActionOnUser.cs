namespace ToyBlockChain.Core
{
    public abstract class ActionOnUser : Action
    {
        public const string TARGET = "user";

        public ActionOnUser(string target, string move, string value)
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

    public class ActionOnUserRegister : ActionOnUser
    {
        public const string MOVE = "register";

        public ActionOnUserRegister(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }
}