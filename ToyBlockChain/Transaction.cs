using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain
{
    public class Transaction
    {
        private readonly string _sender;
        private readonly string _recipient;
        private readonly float _value;
        private readonly int _timestamp;

        public Transaction(
            int timestamp,
            string sender,
            string recipient,
            float value)
        {
            _timestamp = timestamp;
            _sender = sender;
            _recipient = recipient;
            _value = value;
        }

        public string Sender { get; }
        public string Recipient { get; }
        public float Value { get; }
        public int Timestamp { get; }

        public byte[] HashBytes()
        {
            SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Serialize());
        }

        public string HashString()
        {
            return BitConverter.ToString(HashBytes()).Replace("-", "");
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3}",
                                 Sender, Recipient, Value, Timestamp);
        }
    }
}
