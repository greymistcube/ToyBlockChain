namespace ToyBlockChain.Core
{
    public class AccountUser : Account
    {
        public const string TYPE = "user";

        public AccountUser(string address, string type, string state)
            : base(address, type, state)
        {
        }

        public AccountUser(string serializedString) : base(serializedString)
        {
        }

        internal override void ConsumeTransactionAsSender(
            Transaction transaction)
        {
            if (transaction.Count != _count + 1)
            {
                throw new TransactionInvalidForAccountException(
                    "transaction count is invalid");
            }
            else
            {
                _count = _count + 1;
            }
        }

        internal override void ConsumeTransactionAsRecipient(
            Transaction transaction)
        {
            // TODO: Not implemented.
        }
    }
}