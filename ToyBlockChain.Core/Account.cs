using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class Account
    {
        public const string SEPARATOR = "<A>";
        private string _address;
        private int _balance;

        public Account(string address, int balance)
        {
            _address = address;
            _balance = balance;
        }

        public Account(string serializedString)
        {
            string[] strings = serializedString.Split(SEPARATOR);
            _address = strings[0];
            _balance = Int32.Parse(strings[1]);
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
            return $"{_address}{SEPARATOR}{_balance}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

    }
}