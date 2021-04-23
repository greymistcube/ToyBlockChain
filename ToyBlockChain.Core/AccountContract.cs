namespace ToyBlockChain.Core
{
    public class AccountContract : Account
    {
        public const string TYPE = "contract";

        public AccountContract(string address, string type, string state)
            : base(address, type, state)
        {
        }

        public AccountContract(string serializedString) : base(serializedString)
        {
        }
    }
}