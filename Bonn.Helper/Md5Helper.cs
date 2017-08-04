using System;
using System.Security.Cryptography;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// Md5安全类（加密字符串）
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// 得到MD5哈希值
        /// </summary>
        /// <param name="input">要加密的原始字符串</param>
        /// <returns>返回32个字符的MD5哈希值</returns>
        public static string GetMd5(this string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.

            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes

            // and create a string.

            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 

            // and format each one as a hexadecimal string.

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.

            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        /// <summary>
        /// 验证一个MD5值
        /// </summary>
        /// <param name="input">要验证的明文</param>
        /// <param name="hash">要验证的MD5哈希值</param>
        /// <returns></returns>
        public static bool VerifyMd5Hash(this string input, string hash)
        {
            string hashOfInput = GetMd5(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
