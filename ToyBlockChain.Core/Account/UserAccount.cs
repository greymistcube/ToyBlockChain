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

        public UserAccount(string address, string type)
            : base(address, type)
        {
            _state = INIT_STATE;
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
            switch (transaction.Operation.Type)
            {
                case OperationOnUserRegister.TYPE:
                    return;
                case OperationOnUserMessage.TYPE:
                    _state = transaction.Operation.Data;
                    return;
                default:
                    throw new ArgumentException(
                        "unknown operation given");
            }
        }
    }
}