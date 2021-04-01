using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain
{
    public class Transaction
    {
        public string Sender;
        public string Recipient;
        public float Value;
        public int Timestamp;

        public Transaction(
            int timestamp,
            string sender,
            string recipient,
            float value)
        {
            Timestamp = timestamp;
            Sender = sender;
            Recipient = recipient;
            Value = value;
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
            return String.Format("{0},{1},{2},{3}",
                                 Sender, Recipient, Value, Timestamp);
        }
    }
}
