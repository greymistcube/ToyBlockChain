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

    public class TransactionUnsoundException
        : Exception
    {
        public TransactionUnsoundException()
        {
        }

        public TransactionUnsoundException(string message)
            : base(message)
        {
        }
    }

    public class TransactionInvalidForPoolException
        : TransactionInvalidException
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
        : TransactionInvalidException
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
        : TransactionInvalidException
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
        private readonly int _nonce;
        private readonly Operation _operation;
        private readonly string _recipient;
        private readonly long _timestamp;
        private readonly string _publicKey;
        private string _signature;

        public Transaction(
            string sender,
            int nonce,
            Operation operation,
            string recipient,
            long timestamp,
            string publicKey,
            string signature = null)
        {
            _sender = sender;
            _nonce = nonce;
            _operation = operation;
            _recipient = recipient;
            _timestamp = timestamp;
            _publicKey = publicKey;
            _signature = signature;
        }

        public Transaction(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _sender = substrings[0];
            _nonce = Int32.Parse(substrings[1]);
            _operation = Operation.OperationFactory(substrings[2]);
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

        public void CheckSoundness()
        {
            if (Sender != CryptoUtil.ComputeHashString(PublicKey))
            {
                throw new TransactionUnsoundException(
                    "sender identity does not match public key");
            }
            else if (!CryptoUtil.Verify(
                SignatureInputString(),
                Signature,
                CryptoUtil.ExtractRSAParameters(PublicKey)))
            {
                throw new TransactionUnsoundException(
                    "signature is not valid");
            }
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
                Sender, Nonce, Operation.ToSerializedString(), Recipient,
                Timestamp, PublicKey, Signature);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Sender, Nonce.ToString(), Operation.ToSerializedString(),
                    Recipient, Timestamp.ToString(), PublicKey, Signature });
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
                    Sender, Nonce.ToString(), Operation.ToSerializedString(),
                    Recipient, Timestamp.ToString(), PublicKey });
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

        public int Nonce
        {
            get
            {
                return _nonce;
            }
        }

        public Operation Operation
        {
            get
            {
                return _operation;
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
