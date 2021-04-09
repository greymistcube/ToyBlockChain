using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Crypto
{
    public static class CryptoUtil
    {
        public static readonly int NONCE_LENGTH = 16;
        private static RandomNumberGenerator _rng;
        private static SHA256 _sha256;

        static CryptoUtil()
        {
            _rng = RandomNumberGenerator.Create();
            _sha256 = SHA256.Create();
        }

        public static byte[] ComputeHashBytes(byte[] bytes)
        {
            return _sha256.ComputeHash(bytes);
        }

        public static string ComputeHashString(string str)
        {
            return Convert.ToHexString(
                _sha256.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        public static string Sign(string data, RSAParameters rsaParameters)
        {
            // TODO: Possible Security hole.
            // Private key information should be funged.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            rsa.ImportParameters(rsaParameters);
            byte[] signatureBytes = rsa.SignData(dataBytes, _sha256);
            return Convert.ToHexString(signatureBytes);
        }

        public static bool Verify(
            string data, string signature, RSAParameters rsaParameters)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromHexString(signature);

            rsa.ImportParameters(rsaParameters);
            return rsa.VerifyData(dataBytes, _sha256, signatureBytes);
        }

        public static RSAParameters ExtractRSAParameters(string publicKeyString)
        {
            RSAParameters parameters = new RSAParameters();

            string[] pairString = publicKeyString.Split(":");
            byte[] modulus = Convert.FromHexString(pairString[0]);
            byte[] exponent = Convert.FromHexString(pairString[1]);

            parameters.Modulus = modulus;
            parameters.Exponent = exponent;

            return parameters;
        }

        public static string GenerateNonce()
        {
            byte[] nonce = new byte[NONCE_LENGTH];
            _rng.GetBytes(nonce);
            return Convert.ToHexString(nonce);
        }
    }
}
