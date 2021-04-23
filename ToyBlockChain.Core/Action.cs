using System;

namespace ToyBlockChain.Core
{
    public abstract class Action
    {
        protected string _target;
        protected string _move;
        protected string _value;

        public const string SEPARATOR = "<A>";

        public Action(string target, string move, string value)
        {
            _target = target;
            _move = move;
            _value = value;
        }

        public string Target
        {
            get
            {
                return _target;
            }
        }

        public string Move
        {
            get
            {
                return _move;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public static Action ActionFactory(
            string target, string move, string value)
        {
            switch (target)
            {
                case ActionOnUser.TARGET:
                    return ActionOnUser.ActionOnUserFactory(
                        target, move, value);
                case ActionOnContract.TARGET:
                    return ActionOnContract.ActionOnContractFactory(
                        target, move, value);
                default:
                    throw new ArgumentException($"invalid target: {target}");
            }
        }
    }
}