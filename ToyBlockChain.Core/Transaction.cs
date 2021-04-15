using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Core
{
    public class Transaction
    {
        public const string SEPARATOR = "<T>";
        private readonly long _timestamp;
        private readonly string _sender;
        private readonly string _publicKey;
        private readonly string _value;
        private readonly string _recipient;
        private string _signature;

        public Transaction(
            string sender,
            string value,
            string recipient,
            long timestamp,
            string publicKey,
            string signature = null)
        {
            _sender = sender;
            _value = value;
            _recipient = recipient;
            _timestamp = timestamp;
            _publicKey = publicKey;
            _signature = signature;
        }

        public Transaction(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _sender = substrings[0];
            _value = substrings[1];
            _recipient = substrings[2];
            _timestamp = Int64.Parse(substrings[3]);
            _publicKey = substrings[4];
            _signature = substrings[5];
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
                Sender == CryptoUtil.ComputeHashString(PublicKey));
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
            return String.Join(
                SEPARATOR,
                new string[] {
                    Sender, Value, Recipient, Timestamp.ToString(),
                    PublicKey, Signature });
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

        public string SignatureInputString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Sender, Value, Recipient, Timestamp.ToString(),
                    PublicKey });
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
                return Convert.ToHexString(HashBytes);
            }
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
        }

        public string Value
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
    }
}
