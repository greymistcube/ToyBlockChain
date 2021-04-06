using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Crypto
{
    public static class CryptoUtil
    {
        private static RSACryptoServiceProvider _rsa;
        private static SHA256 _sha256;

        static CryptoUtil()
        {
            _rsa = new RSACryptoServiceProvider();
            _sha256 = SHA256.Create();
        }
        static string Sign(string data, RSAParameters rsaParameters)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            _rsa.ImportParameters(rsaParameters);
            byte[] signatureBytes = _rsa.SignData(dataBytes, _sha256);
            return Convert.ToBase64String(signatureBytes);
        }

        static bool Verify(string data, string signature, RSAParameters rsaParameters)
        {
            byte[] dataBytes = Convert.FromBase64String(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            _rsa.ImportParameters(rsaParameters);
            return _rsa.VerifyData(dataBytes, _sha256, signatureBytes);
        }
    }
}
