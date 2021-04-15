using System;
using System.Text;
using System.Security.Cryptography;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Identity
    {
        public const string SEPARATOR = "<ID>";
        private RSAParameters _rsaParameters;
        private string _publicKey;
        private string _address;

        public Identity()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            _rsaParameters = rsa.ExportParameters(true);

            string modulus = Convert.ToHexString(_rsaParameters.Modulus);
            string exponent = Convert.ToHexString(_rsaParameters.Exponent);
            _publicKey = $"{modulus}:{exponent}";
            _address = CryptoUtil.ComputeHashString(_publicKey);
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
                return _publicKey;
            }
        }

        public RSAParameters RSAParameters
        {
            get
            {
                return _rsaParameters;
            }
        }

        public string ToSerializedString()
        {
            return $"{_address}{SEPARATOR}{_publicKey}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}