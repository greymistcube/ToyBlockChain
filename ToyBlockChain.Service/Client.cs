using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Client
    {
        public const int ADDRESS_LENGTH = 8;
        private string _address;
        private RSAParameters _rsaParameters;

        public Client()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] addressBytes = new byte[ADDRESS_LENGTH];
            rng.GetBytes(addressBytes);
            _address = Convert.ToBase64String(addressBytes);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            _rsaParameters = rsa.ExportParameters(true);
        }

        public void Run()
        {
            throw new NotImplementedException();
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

        public string Address
        {
            get
            {
                return _address;
            }
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

        public override string ToString()
        {
            return String.Format("Address: {0}\n"
                                 + "Public Key: {1}",
                                 Address, PublicKey);
        }
    }
}
