namespace ToyBlockChain.Core
{
    public abstract class ActionOnUser : Action
    {
        public const string TARGET = "user";

        public override string Target
        {
            get
            {
                return TARGET;
            }
        }
    }

    public class ActionMessageUser : ActionOnUser
    {
    }

    public class ActionRegisterUser : ActionOnUser
    {
    }
}