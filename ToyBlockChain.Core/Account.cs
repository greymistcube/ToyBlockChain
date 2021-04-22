using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class Account
    {
        public const string SEPARATOR = "<A>";
        private int _count;
        private string _address;
        private string _state;

        public Account(string address, string state)
        {
            _count = 0;
            _address = address;
            _state = state;
        }

        public Account(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _count = Int32.Parse(substrings[0]);
            _address = substrings[1];
            _state = substrings[2];
        }

        public void ConsumeTransaction(Transaction transaction)
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

        internal void ConsumeTransactionAsSender(Transaction transaction)
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

        internal void ConsumeTransactionAsRecipient(Transaction transaction)
        {
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public string Address
        {
            get
            {
                return _address;
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
            return $"{_count}{SEPARATOR}{_address}{SEPARATOR}{_state}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

    }
}