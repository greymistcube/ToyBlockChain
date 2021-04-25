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

        protected Account(string address, string type, string state)
        {
            _address = address;
            _type = type;
            _nonce = 0;
            _state = state;
        }

        protected Account(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _address = substrings[0];
            _type = substrings[1];
            _nonce = Int32.Parse(substrings[2]);
            _state = substrings[3];
        }

        internal void ConsumeTransaction(Transaction transaction)
        {
            if (transaction.Sender != _address
                && transaction.Recipient != _address)
            {
                throw new TransactionInvalidForAccountException(
                    "transaction does not involve this account");
            }
            else if (transaction.Sender == _address)
            {
                ConsumeTransactionAsSender(transaction);
            }
            else if (transaction.Recipient == _address)
            {
                ConsumeTransactionAsRecipient(transaction);
            }
            else
            {
                throw new ArgumentException(
                    "something went wrong");
            }
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

        public static Account AccountFactory(
            string address, string type, string state)
        {
            switch (type)
            {
                case UserAccount.TYPE:
                    return new UserAccount(address, type, state);
                case ContractAccount.TYPE:
                    return ContractAccount.ContractAccountFactory(
                        address, type, state);
                default:
                    throw new NotImplementedException($"invalid type: {type}");
            }
        }

        public static Account AccountFactory(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            return AccountFactory(substrings[0], substrings[1], substrings[2]);
        }
    }
}