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
    }
}