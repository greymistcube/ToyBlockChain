using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Core
{
    public class Transaction
    {
        private readonly long _timestamp;
        private readonly string _sender;
        private readonly string _publicKey;
        private readonly float _value;
        private readonly string _recipient;
        private readonly string _signature;

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

        public string Sender { get; }
        public float Value { get; }
        public string Recipient { get; }
        public long Timestamp { get; }
        public string PublicKey { get; }
        public string Signature { get; }

        public byte[] HashBytes
        {
            get
            {
                SHA256 sha256 = SHA256.Create();
                return sha256.ComputeHash(SignatureHashInputBytes());
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
            SHA256 sha256 = SHA256.Create();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaParameters = PublicKeyParameters();
            rsa.ImportParameters(rsaParameters);
            byte[] signatureBytes = Convert.FromBase64String(Signature);

            return rsa.VerifyData(SignatureHashInputBytes(),
                                  sha256, signatureBytes);
        }

        private RSAParameters PublicKeyParameters()
        {
            RSAParameters parameters = new RSAParameters();
            parameters.Modulus = PublicKeyPairBytes()[0];
            parameters.Exponent = PublicKeyPairBytes()[1];
            return parameters;
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

        public string SignatureHashInputString()
        {
            return String.Format("{0},{1},{2},{3},{4}",
                                 Sender, Value, Recipient, Timestamp,
                                 PublicKey);
        }

        public byte[] SignatureHashInputBytes()
        {
            return Encoding.UTF8.GetBytes(SignatureHashInputString());
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
