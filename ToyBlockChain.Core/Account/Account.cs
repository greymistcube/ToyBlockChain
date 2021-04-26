using System;
using System.Text;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Represents an account.
    /// </summary>
    public abstract class Account
    {
        public const string SEPARATOR = "<A>";

        protected string _type;
        protected int _nonce;
        protected string _address;
        protected string _state;

        protected Account(string address, string type)
        {
            _address = address;
            _type = type;
            _nonce = 0;
        }

        internal abstract void ConsumeTransactionAsSender(
            Transaction transaction);

        internal abstract void ConsumeTransactionAsRecipient(
            Transaction transaction);

        public string Address
        {
            get
            {
                return _address;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public int Nonce
        {
            get
            {
                return _nonce;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
        }

        public string LogId
        {
            get
            {
                return Address[0..16];
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Address, Type, Nonce.ToString(), State });
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

        public override string ToString()
        {
            return String.Format(
                "Address: {0}\n"
                + "Type: {1}\n"
                + "Count: {2}\n"
                + "State: {3}",
                Address, Type, Nonce, State);
        }

        public static Account AccountFactory(string address, string type)
        {
            switch (type)
            {
                case UserAccount.TYPE:
                    return new UserAccount(address, type);
                case ContractAccount.TYPE:
                    return ContractAccount.ContractAccountFactory(
                        address, type);
                default:
                    throw new NotImplementedException($"invalid type: {type}");
            }
        }
    }
}