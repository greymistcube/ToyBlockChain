using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Core
{
    public class Transaction
    {
        private readonly long _timestamp;
        private readonly string _sender;
        private readonly string _publicKey;
        private readonly float _value;
        private readonly string _recipient;
        private string _signature = null;

        public Transaction(
            string sender,
            float value,
            string recipient,
            long timestamp,
            string publicKey)
        {
            _sender = sender;
            _value = value;
            _recipient = recipient;
            _timestamp = timestamp;
            _publicKey = publicKey;
        }

        public Transaction(
            string sender,
            float value,
            string recipient,
            long timestamp,
            string publicKey,
            string signature)
        {
            _sender = sender;
            _value = value;
            _recipient = recipient;
            _timestamp = timestamp;
            _publicKey = publicKey;
            _signature = signature;
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
        }

        public string Recipient
        {
            get
            {
                return _recipient;
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public string Signature
        {
            get
            {
                return _signature;
            }
        }

        public byte[] HashBytes
        {
            get
            {
                SHA256 sha256 = SHA256.Create();
                return sha256.ComputeHash(ToSerializedBytes());
            }
        }

        public string HashString
        {
            get
            {
                return Convert.ToBase64String(HashBytes);
            }
        }

        /// <summary>
        /// Signs this transaction with given signature.
        /// Simply overwrites the previous signature.
        /// </summary>
        public void Sign(string signature)
        {
            _signature = signature;
        }

        public bool IsValid()
        {
            bool senderValid = (
                Sender == CryptoUtil.HashString(PublicKey));
            bool signatureValid = CryptoUtil.Verify(
                SignatureInputString(),
                Signature,
                CryptoUtil.ExtractRSAParameters(PublicKey));
            return senderValid && signatureValid;
        }

        public override string ToString()
        {
            return String.Format(
                "Sender: {0}\n"
                + "Value: {1}\n"
                + "Recipient: {2}\n"
                + "Timestamp: {3}\n"
                + "Public Key: {4}\n"
                + "Signature: {5}",
                Sender, Value, Recipient, Timestamp,
                PublicKey, Signature);
        }

        public string ToSerializedString()
        {
            return String.Format(
                "{0},{1},{2},{3},{4},{5}",
                Sender, Value, Recipient, Timestamp, PublicKey, Signature);
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

        public string SignatureInputString()
        {
            return String.Format(
                "{0},{1},{2},{3},{4}",
                Sender, Value, Recipient, Timestamp, PublicKey);
        }
    }
}
