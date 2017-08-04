/*
 * 
 * 
 * 功能说明：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞 2012-03-08
 * 修改说明：
 * 
 * 1、增加DES加密解密函数EncryptBase4，DecryptBase4配对使用  不再对KEY做处理，输出为BASE4编码  ZYP  2013-08-06
 * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Sunlib
{
    public class DES
    {
        #region DES加密与解密

        /// <summary>
        /// 偏移量
        /// </summary>
        const string IV_64 = "SunTront2012";

        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">待加密字符串，明码，如 888888</param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "XinTian_Software");
        }

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text">待加密字符串，明码，如 888888</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(IV_64, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text">待解密的字符串</param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "XinTian_Software");
        }

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text">待解密的字符串</param> 
        /// <param name="sKey">解密密钥</param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(IV_64, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region ========DES加密解密 输出BASE4========
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">KEY值</param>
        /// <returns></returns>
        public static string EncryptBase4(string encryptString, string encryptKey)
        {

            try
            {
                byte[] rgbKey = Encoding.Default.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = { 1, 1, 2, 2, 3, 3, 4, 4 };
                byte[] inputByteArray = Encoding.Default.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                //dCSP.Mode = CipherMode.CBC  ;(默认模式) 
                System.IO.MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();

                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="encryptString">等解密字符串</param>
        /// <param name="encryptKey">KEY值</param>
        /// <returns></returns>
        public static string DecryptBase4(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.Default.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = { 1, 2, 3, 4, 5, 6, 7, 8 };
                byte[] inputByteArray = Convert.FromBase64String(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                // dCSP.Mode = CipherMode.CBC  ;(默认模式) 
                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.Default.GetString(mStream.ToArray());

            }
            catch
            {
                return encryptString;
            }
        }
        #endregion

        #endregion DES加密与解密
    }
}
