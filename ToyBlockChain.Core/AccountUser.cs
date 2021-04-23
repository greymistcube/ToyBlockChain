namespace ToyBlockChain.Core
{
    public class AccountUser : Account
    {
        public const string TYPE = "user";

        public AccountUser(string address, string state) : base(address, state)
        {
        }

        public AccountUser(string serializedString) : base(serializedString)
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