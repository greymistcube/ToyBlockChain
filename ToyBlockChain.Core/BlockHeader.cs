using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Core
{
    public class BlockHeaderInvalidException : Exception
    {
        public BlockHeaderInvalidException()
        {
        }

        public BlockHeaderInvalidException(string message) : base(message)
        {
        }
    }

    public class BlockHeaderUnsoundException : Exception
    {
        public BlockHeaderUnsoundException()
        {
        }

        public BlockHeaderUnsoundException(string message)
            : base(message)
        {
        }
    }

    public class BlockHeader
    {
        public const string SEPARATOR = "<BH>";
        private readonly int _index;
        private readonly string _previousHashString;
        private readonly string _transactionHashString;
        private readonly string _miner;
        private readonly long _timestamp;
        private readonly string _nonce;
        private readonly int _difficulty;

        public BlockHeader(
            int index,
            string previousHashString,
            string transactionHashString,
            string miner,
            long timestamp,
            string nonce,
            int difficulty)
        {
            _index = index;
            _previousHashString = previousHashString;
            _transactionHashString = transactionHashString;
            _miner = miner;
            _timestamp = timestamp;
            _nonce = nonce;
            _difficulty = difficulty;
        }

        public BlockHeader(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _index = Int32.Parse(substrings[0]);
            _previousHashString = substrings[1];
            _transactionHashString = substrings[2];
            _miner = substrings[3];
            _timestamp = Int64.Parse(substrings[4]);
            _nonce = substrings[5];
            _difficulty = Int32.Parse(substrings[6]);
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public string PreviousHashString
        {
            get
            {
                return _previousHashString;
            }
        }

        public string TransactionHashString
        {
            get
            {
                return _transactionHashString;
            }
        }

        public string Miner
        {
            get
            {
                return _miner;
            }
        }

        public int Difficulty
        {
            get
            {
                return _difficulty;
            }
        }

        public string Nonce
        {
            get
            {
                return _nonce;
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
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

        public void CheckSoundness()
        {
            BitArray bits = new BitArray(HashBytes);

            for (int i = 0; i < Difficulty; i++)
            {
                if (bits[i] != false)
                {
                    throw new BlockHeaderUnsoundException(
                        "hash does not satisfy the difficulty requirement");
                }
            }
        }

        public override string ToString()
        {
            return String.Format(
                "Index: {0}\n"
                + "Previous Hash: {1}\n"
                + "Transaction Hash: {2}\n"
                + "Miner: {3}\n"
                + "Timestamp: {4}\n"
                + "Nonce: {5}\n"
                + "Difficulty: {6}",
                Index, PreviousHashString, TransactionHashString, Miner,
                Timestamp, Nonce, Difficulty);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    Index.ToString(), PreviousHashString, TransactionHashString,
                    Miner, Timestamp.ToString(), Nonce,
                    Difficulty.ToString() });
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}
