using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Client
    {
        private string _address;
        private RSAParameters _rsaParameters;
        public Client()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            _rsaParameters = rsa.ExportParameters(true);
        }

        public Transaction CreateTransaction(float value, string recipient)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string signature = Sign(_address, value, recipient, timestamp, PublicKey);

            Transaction transaction = new Transaction(_address, value,
                                                      recipient, timestamp,
                                                      PublicKey, signature);
            return transaction;
        }

        private string Sign(
            string sender, float value, string recipient,
            long timestamp, string publicKey)
        {
            string stringToSign = String.Format("{0},{1},{2},{3},{4}",
                                                sender, value, recipient,
                                                timestamp, publicKey);
            return CryptoUtil.Sign(stringToSign, _rsaParameters);
        }

        public string PublicKey
        {
            get
            {
                string modulus = Convert.ToBase64String(_rsaParameters.Modulus);
                string exponent = Convert.ToBase64String(_rsaParameters.Exponent);
                return $"{modulus}:{exponent}";
            }
        }
    }
}