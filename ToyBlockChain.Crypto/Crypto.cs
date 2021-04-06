using System;
using System.Text;
using System.Security.Cryptography;

namespace ToyBlockChain.Crypto
{
    public static class Crypto
    {
        private static RSACryptoServiceProvider _rsa;
        private static SHA256 _sha256;

        static Crypto()
        {
            _rsa = new RSACryptoServiceProvider();
            _sha256 = SHA256.Create();
        }
        static string Sign(string str, RSAParameters rsaParameters)
        {
            byte[] bytesToSign = Encoding.UTF8.GetBytes(str);

            _rsa.ImportParameters(rsaParameters);
            byte[] signatureBytes = _rsa.SignData(bytesToSign, _sha256);
            return Convert.ToBase64String(signatureBytes);
        }

        static bool Verify(string str, RSAParameters rsaParameters)
        {
            throw new NotImplementedException();
        }
    }
}
