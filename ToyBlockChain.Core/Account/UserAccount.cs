using System;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Represents an account of a user.
    /// </summary>
    public class UserAccount : Account
    {
        public const string TYPE = "user";
        public const string INIT_STATE = "";

        public UserAccount(string address, string type, string state)
            : base(address, type, state)
        {
        }

        public UserAccount(string serializedString) : base(serializedString)
        {
        }

        internal override void ConsumeTransactionAsSender(
            Transaction transaction)
        {
            if (transaction.Nonce != _nonce)
            {
                throw new ArgumentException("transaction nonce is invalid");
            }
            else
            {
                _nonce = _nonce + 1;
            }
        }

        internal override void ConsumeTransactionAsRecipient(
            Transaction transaction)
        {
            if (transaction.Operation.Target != OperationOnUser.TARGET)
            {
                throw new ArgumentException(
                    "invalid target for operation: "
                    + $"{transaction.Operation.Target}");
            }
            else
            {
                switch (transaction.Operation.Move)
                {
                    case OperationOnUserRegister.MOVE:
                        return;
                    case OperationOnUserMessage.MOVE:
                        _state = transaction.Operation.Value;
                        return;
                    default:
                        throw new ArgumentException(
                            "unknown operation given");
                }
            }
        }
    }
}