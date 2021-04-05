using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain
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
            return String.Format(
                "{0},{1},{2},{3},{4},{5}",
                Index, PreviousHashString, TransactionHashString,
                Nonce, Timestamp,
                Difficulty);
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public string HashInputString()
        {
            return String.Format(
                "{0},{1},{2},{3},{4}",
                Index, PreviousHashString, TransactionHashString,
                Nonce, Timestamp);
        }

        public byte[] HashInputBytes()
        {
            return Encoding.UTF8.GetBytes(HashInputString());
        }
    }
}
