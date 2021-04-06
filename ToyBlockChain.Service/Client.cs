using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Core;

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

            Transaction transaction = new Transaction(_address,
                                                      value,
                                                      recipient,
                                                      timestamp,
                                                      PublicKey,
                                                      signature);
            return transaction;
        }

        private string Sign(
            string sender, float value, string recipient,
            long timestamp, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            SHA256 sha256 = SHA256.Create();
            string stringToSign = String.Format("{0},{1},{2},{3},{4}");
            byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            rsa.ImportParameters(_rsaParameters);
            byte[] signatureBytes = rsa.SignData(bytesToSign, sha256);
            return Convert.ToBase64String(signatureBytes);
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
