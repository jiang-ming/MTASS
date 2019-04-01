using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;

namespace WebMap.Web
{
    public static class AESUtil
    {

        private static String AesKey = "u/?~:#vX9K$78MkW6g*q";

        public static String GetSecurityToken(String UserName)
        {
            String ipaddress = HttpContext.Current.Request.UserHostAddress;
            Int32 tokenlife = Convert.ToInt32(ConfigurationManager.AppSettings["TokenExpirationSeconds"].ToString());
            DateTime expiretime = DateTime.Now.AddSeconds(tokenlife);
            return GetSecurityToken(UserName, ipaddress, expiretime);
        }

        public static String GetSecurityToken(String UserName, String IP, DateTime ExpireTime)
        {
            String tokentext = UserName + "|" + IP + "|" + ExpireTime.ToBinary().ToString();
            return EncryptString(tokentext);
        }

        public static String EncryptString(String input)
        {
            Byte[] key = AesKey.ToByteArray(32); //need to convert to 256 bit
            String vector = "dK59cNflX74kZb38";
            Byte[] iv = vector.ToByteArray();
            Byte[] inputbytes = input.ToByteArray();

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(inputbytes, 0, inputbytes.Length);
            csEncrypt.FlushFinalBlock();
            Byte[] encrypted = msEncrypt.ToArray();
            return encrypted.FromByteArrayToHexString();
        }

        public static String DecryptString(String input)
        {
            Byte[] key = AesKey.ToByteArray(32); //need to convert to 256 bit
            String vector = "dK59cNflX74kZb38";
            Byte[] iv = vector.ToByteArray();
            Byte[] inputbytes = input.FromHexStringToByteArray();

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            MemoryStream msDecrypt = new MemoryStream();
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);
            csDecrypt.Write(inputbytes, 0, inputbytes.Length);
            csDecrypt.FlushFinalBlock();
            Byte[] decrypted = msDecrypt.ToArray();
            return decrypted.FromByteArrayToString();
        }

        public static Byte[] ToByteArray(this String StringToConvert)
        {

            char[] CharArray = StringToConvert.ToCharArray();
            byte[] ByteArray = new byte[CharArray.Length];
            for (int i = 0; i < CharArray.Length; i++)
            {
                ByteArray[i] = Convert.ToByte(CharArray[i]);
            }
            return ByteArray;
        }

        public static Byte[] ToByteArray(this String StringToConvert, int length)
        {

            char[] CharArray = StringToConvert.ToCharArray();
            byte[] ByteArray = new byte[length];
            for (int i = 0; i < CharArray.Length; i++)
            {
                ByteArray[i] = Convert.ToByte(CharArray[i]);
            }
            return ByteArray;
        }

        public static Byte[] FromHexStringToByteArray(this String text)
        {
            byte[] bytes = new byte[text.Length / 2];

            for (int i = 0; i < text.Length; i += 2)
            {
                bytes[i / 2] = byte.Parse(text[i].ToString() + text[i + 1].ToString(),
                    System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static String FromCrystoStreamToString(this CryptoStream buff)
        {
            string sbinary = "";
            int b = 0;
            do
            {
                b = buff.ReadByte();
                if (b != -1) sbinary += ((char)b);

            } while (b != -1);
            return (sbinary);
        }

        public static String FromByteArrayToHexString(this Byte[] buff)
        {
            String sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

        public static String FromByteArrayToString(this Byte[] buff)
        {
            return System.Text.Encoding.ASCII.GetString(buff);
        }

    }
}
