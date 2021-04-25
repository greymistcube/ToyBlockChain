namespace ToyBlockChain.Core
{
    /// <summary>
    /// Represents an account of a user.
    /// </summary>
    public class UserAccount : Account
    {
        public const string TYPE = "user";

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
                throw new TransactionInvalidForAccountException(
                    "transaction count is invalid");
            }
            else
            {
                _nonce = _nonce + 1;
            }
        }

        internal override void ConsumeTransactionAsRecipient(
            Transaction transaction)
        {
            // TODO: Not implemented.
        }
    }
}