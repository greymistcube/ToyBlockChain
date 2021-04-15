using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class Account
    {
        public const string SEPARATOR = "<A>";
        private int _nonce;
        private string _address;
        private int _balance;

        public Account(string address, int balance)
        {
            _nonce = 0;
            _address = address;
            _balance = balance;
        }

        public Account(string serializedString)
        {
            string[] strings = serializedString.Split(SEPARATOR);
            _nonce = Int32.Parse(strings[0]);
            _address = strings[1];
            _balance = Int32.Parse(strings[2]);
        }

        public void ProcessTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public int Nonce
        {
            get
            {
                return _nonce;
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
            return $"{_nonce}{SEPARATOR}{_address}{SEPARATOR}{_balance}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

    }
}