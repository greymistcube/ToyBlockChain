using System;

namespace ToyBlockChain.Core
{
    public abstract class ActionOnContract : Action
    {
        public const string TARGET = "contract";

        public ActionOnContract(string target, string move, string value)
            : base(target, move, value)
        {
        }

        public static Action ActionOnContractFactory(
            string target, string move, string value)
        {
            switch (move)
            {
                case ActionOnContractPlay.MOVE:
                    return new ActionOnContractPlay(target, move, value);
                case ActionOnContractReveal.MOVE:
                    return new ActionOnContractReveal(target, move, value);
                default:
                    throw new ArgumentException($"invalid move: {move}");
            }
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