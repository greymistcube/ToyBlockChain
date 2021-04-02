using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain
{
    public class Transaction
    {
        private readonly int _timestamp;
        private readonly string _sender;
        private readonly string _publicKey;
        private readonly float _value;
        private readonly string _recipient;
        private readonly string _signature;

        public Transaction(
            int timestamp,
            string sender,
            string publicKey,
            float value,
            string recipient,
            string signature)
        {
            _timestamp = timestamp;
            _sender = sender;
            _publicKey = publicKey;
            _value = value;
            _recipient = recipient;
            _signature = signature;
        }

        public int Timestamp { get; }
        public string Sender { get; }
        public string PublicKey { get; }
        public float Value { get; }
        public string Recipient { get; }
        public string Signature { get; }

        public byte[] HashBytes
        {
            get
            {
                SHA256 sha256 = SHA256.Create();
                return sha256.ComputeHash(Serialize());
            }
        }

        public string HashString
        {
            get
            {
                return BitConverter.ToString(HashBytes).Replace("-", "");
            }
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
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
