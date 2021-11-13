using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestApi
{
    public static class Encryption
    {
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "Velxt3x=A5N/x2sLC|Qnw5lpKR,!T)";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x29, 0x76, 0x61, 0x6e, 0x30, 0x4d, 0x65, 0x34, 0x76, 0x45, 0x64, 0x85, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "Velxt3x=A5N/x2sLC|Qnw5lpKR,!T)";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x29, 0x76, 0x61, 0x6e, 0x30, 0x4d, 0x65, 0x34, 0x76, 0x45, 0x64, 0x85, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
