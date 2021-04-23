namespace ToyBlockChain.Core
{
    public abstract class ActionOnContract : Action
    {
        public const string TARGET = "contract";

        public ActionOnContract(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }

    public class ActionOnContractPlay : ActionOnContract
    {
        public const string MOVE = "play";

        public ActionOnContractPlay(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }

    public class ActionOnContractReveal : ActionOnContract
    {
        public const string MOVE = "reveal";

        public ActionOnContractReveal(string target, string move, string value)
            : base(target, move, value)
        {
        }
    }
}