using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Core
{
    public class BlockHeader
    {
        private readonly int _index;
        private readonly string _previousHashString;
        private readonly string _transactionHashString;
        private readonly int _difficulty;
        private readonly int _nonce;
        private readonly int _timestamp;

        public BlockHeader(
            int index,
            string previousHashString,
            string transactionHashString,
            int timestamp,
            int nonce,
            int difficulty)
        {
            _index = index;
            _previousHashString = previousHashString;
            _transactionHashString = transactionHashString;
            _timestamp = timestamp;
            _nonce = nonce;
            _difficulty = difficulty;
        }

        public int Index { get; }
        public string PreviousHashString { get; }
        public string TransactionHashString { get; }
        public int Difficulty { get; }
        public int Nonce { get; }
        public int Timestamp { get; }

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

        public bool IsValid()
        {
            byte[] bytes = HashBytes;
            for (int i = 0; i < Difficulty; i++)
            {
                if (bytes[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return String.Format("Index: {0}\n"
                                 + "Previous Hash: {1}\n"
                                 + "Transaction Hash: {2}\n"
                                 + "Timestamp: {3}\n"
                                 + "Nonce: {4}\n"
                                 + "Difficulty: {5}",
                                 Index,
                                 PreviousHashString, TransactionHashString,
                                 Timestamp, Nonce, Difficulty);
        }

        public string ToSerializedString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5}",
                                 Index,
                                 PreviousHashString, TransactionHashString,
                                 Timestamp, Nonce, Difficulty);
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}
