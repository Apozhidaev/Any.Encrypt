using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Any.Encrypt
{
    public class SecurityHelper
    {
        private readonly byte[] Key;

        private readonly byte[] IV;

        public SecurityHelper(string password)
        {
            const string saltStr = "1agg$wew";
            const string ivStr = "ddfgsd%a";

            var salt = new UTF8Encoding(false).GetBytes(saltStr);
            var key = new Rfc2898DeriveBytes(password, salt);
            var iv = new Rfc2898DeriveBytes(ivStr, salt);

            Key = key.GetBytes(32);
            IV = iv.GetBytes(16);
        }

        public byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            {
                return Crypt(data, encryptor);
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            {
                return Crypt(data, decryptor);
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

        public byte[] Encrypt(byte[] data)
        {
            return Encrypt(data, Key, IV);
        }

        public byte[] Decrypt(byte[] data)
        {
            return Decrypt(data, Key, IV);
        }
 
    }
}