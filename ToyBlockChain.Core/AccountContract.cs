namespace ToyBlockChain.Core
{
    public class AccountContract : Account
    {
        public const string TYPE = "contract";

        public AccountContract(string address, string state)
            : base(address, state)
        {
        }

        public AccountContract(string serializedString) : base(serializedString)
        {
        }

        public override string Type
        {
            get
            {
                return TYPE;
            }
        }
    }
}