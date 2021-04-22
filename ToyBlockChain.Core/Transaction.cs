using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Core
{
    public class TransactionInvalidException : Exception
    {
        public TransactionInvalidException()
        {
        }

        public TransactionInvalidException(string message) : base(message)
        {
        }
    }

    public class TransactionInvalidInternalException
        : TransactionInvalidException
    {
        public TransactionInvalidInternalException()
        {
        }

        public TransactionInvalidInternalException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidExternalException
        : TransactionInvalidException
    {
        public TransactionInvalidExternalException()
        {
        }

        public TransactionInvalidExternalException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidForPoolException
        : TransactionInvalidExternalException
    {
        public TransactionInvalidForPoolException()
        {
        }

        public TransactionInvalidForPoolException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidForChainException
        : TransactionInvalidExternalException
    {
        public TransactionInvalidForChainException()
        {
        }

        public TransactionInvalidForChainException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidForCatalogueException
        : TransactionInvalidExternalException
    {
        public TransactionInvalidForCatalogueException()
        {
        }

        public TransactionInvalidForCatalogueException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidForAccountException
        : TransactionInvalidForCatalogueException
    {
        public TransactionInvalidForAccountException()
        {
        }

        public TransactionInvalidForAccountException(string message)
            : base(message)
        {
        }
    }

    public class Transaction
    {
        public const string SEPARATOR = "<T>";
        private readonly string _sender;
        private readonly int _count;
        private readonly string _action;
        private readonly string _recipient;
        private readonly long _timestamp;
        private readonly string _publicKey;
        private string _signature;

        public Transaction(
            string sender,
            int count,
            string action,
            string recipient,
            long timestamp,
            string publicKey,
            string signature = null)
        {
            _sender = sender;
            _count = count;
            _action = action;
            _recipient = recipient;
            _timestamp = timestamp;
            _publicKey = publicKey;
            _signature = signature;
        }

        public Transaction(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _sender = substrings[0];
            _count = Int32.Parse(substrings[1]);
            _action = substrings[2];
            _recipient = substrings[3];
            _timestamp = Int64.Parse(substrings[4]);
            _publicKey = substrings[5];
            _signature = substrings[6];
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
                + "Count: {1}\n"
                + "Action: {2}\n"
                + "Recipient: {3}\n"
                + "Timestamp: {4}\n"
                + "Public Key: {5}\n"
                + "Signature: {6}",
                Sender, Count, Action, Recipient, Timestamp,
                PublicKey, Signature);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Sender, Count.ToString(), Action, Recipient,
                    Timestamp.ToString(), PublicKey, Signature });
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
                    Sender, Count.ToString(), Action, Recipient,
                    Timestamp.ToString(), PublicKey });
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

        public string LogId
        {
            get
            {
                return HashString[0..16];
            }
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public string Action
        {
            get
            {
                return _action;
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
