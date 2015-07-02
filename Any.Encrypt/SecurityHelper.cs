using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Any.Encrypt
{
    public class SecurityHelper
    {
        private readonly byte[] Key;

        public SecurityHelper(string password)
        {
            const string saltStr = "1agg$wew";

            var salt = new UTF8Encoding(false).GetBytes(saltStr);
            var key = new Rfc2898DeriveBytes(password, salt, 10000);

            Key = key.GetBytes(32);
        }

        public byte[] Encrypt(byte[] data, byte[] key)
        {
            var iv = CreateIV();
            using (var algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            {
                var encryptData = Crypt(data, encryptor);
                var result = new byte[iv.Length + encryptData.Length];
                iv.CopyTo(result, 0);
                encryptData.CopyTo(result, iv.Length);
                return result;
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key)
        {
            var iv = new byte[16];
            var encryptData = new byte[data.Length - iv.Length];
            Array.Copy(data, 0, iv, 0, iv.Length);
            Array.Copy(data, iv.Length, encryptData, 0, encryptData.Length);
            using (var algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            {
                return Crypt(encryptData, decryptor);
            }
        }

        private byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            var m = new MemoryStream();
            using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
            {
                c.Write(data, 0, data.Length);
            }
            return m.ToArray();
        }

        private byte[] CreateIV()
        {
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                var data = new byte[16];
                rngCsp.GetBytes(data);
                return data;
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            return Encrypt(data, Key);
        }

        public byte[] Decrypt(byte[] data)
        {
            return Decrypt(data, Key);
        }
 
    }
}