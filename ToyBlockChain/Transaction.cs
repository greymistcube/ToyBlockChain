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
                return sha256.ComputeHash(HashInputBytes());
            }
        }

        public string HashString
        {
            get
            {
                return Convert.ToBase64String(HashBytes);
            }
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5}",
                                 Sender, Value, Recipient, Timestamp,
                                 PublicKey, Signature);
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public string HashInputString()
        {
            return String.Format("{0},{1},{2},{3}",
                                 Sender, Value, Recipient, Timestamp);
        }

        public byte[] HashInputBytes()
        {
            return Encoding.UTF8.GetBytes(HashInputString());
        }

        public string[] PublicKeyPairString()
        {
            return PublicKey.Split(":");
        }

        public byte[][] PublicKeyPairBytes()
        {
            string[] publicKeyPairString = PublicKeyPairString();
            return new byte[][] {
                Convert.FromBase64String(publicKeyPairString[0]),
                Convert.FromBase64String(publicKeyPairString[1])};
        }
    }
}
