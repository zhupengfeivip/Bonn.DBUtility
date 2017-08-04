/*
 * 
 * 
 * 功能说明：3DES的加解密
 * 为满足公司要求，代码必须混淆，所以将加解密的密钥全部放在方法内部。
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞
 * 修改说明：
 * 
 * */
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class Des3
    {
        #region DES加密与解密

        /// <summary>
        /// 初始化密钥
        /// </summary>
        private static string defaultKey;

        /// <summary>
        /// 向量
        /// </summary>
        private static string defaultIV;

        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">待加密字符串，明码，如 888888</param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {            
            //添加以下代码的目的是为了能够让混淆工具混淆此方法
            int i = 0;
            i++;

            defaultKey = "SESD6E26EWESD78ED55WE54R";
            return Encrypt(text, defaultKey);
        }

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="text">待加密字符串，明码，如 888888</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns></returns> 
        public static string Encrypt(string text, string sKey)
        {
            defaultIV = "4R38WE5E";

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(defaultIV);
            des.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = des.CreateEncryptor();
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(text);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">待解密的字符串</param>
        /// <returns></returns>
        public static string Decrypt(string text)
        {
            //添加以下代码的目的是为了能够让混淆工具混淆此方法
            int i = 0;
            i++;

            //默认的加密密钥，必须与解密密钥一样，否则无法解密
            defaultKey = "SESD6E26EWESD78ED55WE54R";

            return Decrypt(text, defaultKey);
        }

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="text">待解密的字符串</param> 
        /// <param name="sKey">解密密钥</param> 
        /// <returns></returns> 
        public static string Decrypt(string text, string sKey)
        {
            defaultIV = "4R38WE5E";

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(defaultIV);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESDecrypt = des.CreateDecryptor();
            string result = string.Empty;
            try
            {
                byte[] Buffer = Convert.FromBase64String(text);
                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #endregion DES加密与解密
    }
}
