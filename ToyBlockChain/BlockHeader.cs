using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain
{
    public class BlockHeader
    {
        public int Index;
        public string PreviousHashString;
        public string TransactionHashString;
        public int Difficulty;
        public int Nonce;
        public int Timestamp;

        public BlockHeader(
            int index,
            string previousHashString,
            string transactionHashString,
            int timestamp,
            int nonce,
            int difficulty)
        {
            Index = index;
            PreviousHashString = previousHashString;
            TransactionHashString = transactionHashString;
            Timestamp = timestamp;
            Nonce = nonce;
            Difficulty = difficulty;
        }

        public bool IsValid()
        {
            return true;
        }

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
            return String.Format("{0},{1},{2},{3},{4},{5}",
                                 Index,
                                 PreviousHashString, TransactionHashString,
                                 Nonce, Difficulty, Timestamp);
        }
    }
}
