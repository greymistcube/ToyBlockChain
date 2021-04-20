using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class Account
    {
        public const string SEPARATOR = "<A>";
        private int _count;
        private string _address;
        private int _balance;

        public Account(string address, int balance)
        {
            _count = 0;
            _address = address;
            _balance = balance;
        }

        public Account(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _count = Int32.Parse(substrings[0]);
            _address = substrings[1];
            _balance = Int32.Parse(substrings[2]);
        }

        public void ProcessTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
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

        public int Balance
        {
            get
            {
                return _balance;
            }
        }

        public string ToSerializedString()
        {
            return $"{_count}{SEPARATOR}{_address}{SEPARATOR}{_balance}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

    }
}