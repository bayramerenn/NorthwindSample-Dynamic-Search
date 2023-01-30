using Microsoft.Extensions.Options;
using NorthwindSample.Encrypt;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
    public class Encryption : IEncryption
    {
        private readonly IOptions<NrtConfig> _nrtConfig;

        public Encryption(IOptions<NrtConfig> nrtConfig)
        {
            _nrtConfig = nrtConfig;
        }

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {

                using (CryptoStream cs = new CryptoStream(ms, Aes.Create().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    var toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new CryptoStream(ms, Aes.Create().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        public string EncryptText(string text, string privateKey = "")
        {
            if (string.IsNullOrEmpty(text) || text == "null")
                return string.Empty;

            if (string.IsNullOrEmpty(privateKey))
                privateKey = _nrtConfig.Value.PrivateKey;

            using (var provider = Aes.Create())
            {
                provider.Key = Encoding.ASCII.GetBytes(privateKey.Substring(0, 16));
                provider.IV = Encoding.ASCII.GetBytes(privateKey.Substring(0, 16));

                var encryptedBinary = EncryptTextToMemory(text, provider.Key, provider.IV);
                return Convert.ToBase64String(encryptedBinary);
            }
        }

        //Example Password: 123456 ==> MTIzNDU2
        public string DecryptText(string text, string privateKey = "")
        {
            try
            {
                if (string.IsNullOrEmpty(text) || text == "null")
                    return string.Empty;

                if (string.IsNullOrEmpty(privateKey))
                    privateKey = _nrtConfig.Value.PrivateKey;

                using (var provider = Aes.Create())
                {
                    provider.Key = Encoding.ASCII.GetBytes(privateKey.Substring(0, 16));
                    provider.IV = Encoding.ASCII.GetBytes(privateKey.Substring(0, 16));

                    var buffer = Convert.FromBase64String(text);
                    return DecryptTextFromMemory(buffer, provider.Key, provider.IV);
                }
            }
            catch
            {
                throw new InvalidTokenException();
            }
        }
    }
}