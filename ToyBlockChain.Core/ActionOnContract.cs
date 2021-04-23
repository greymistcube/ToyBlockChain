namespace ToyBlockChain.Core
{
    public abstract class ActionOnContract : Action
    {
        public const string TARGET = "contract";

        protected string _value;

        public override string Target
        {
            get
            {
                return TARGET;
            }
        }

        public abstract string Move
        {
            get;
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }
    }

    public class ActionOnContractPlay : ActionOnContract
    {
        public const string MOVE = "play";

        public override string Move
        {
            get
            {
                return MOVE;
            }
        }
    }

    public class ActionOnContractReveal : ActionOnContract
    {
        public const string MOVE = "reveal";

        public override string Move
        {
            get
            {
                return MOVE;
            }
        }
    }
}