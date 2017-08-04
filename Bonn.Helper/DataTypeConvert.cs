/*
 *  版权所有
 * 
 * 功能说明：进制之间的转换 
 * 
 * 创建标识：朱鹏飞
 * 
 * 修改标识：
 * 修改说明：
 * 
 * */

using System;
using System.Collections.Generic;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class DataTypeConvert
    {
        /// <summary>
        /// 十六进制串转换成二进制串
        /// </summary>
        public static string HexToBit(string hex)
        {
            byte[] b = HexStringToByteArray(hex);
            return ByteArrayToBitString(b);
        }

        /// <summary>
        /// 二进制串转换成十六进制串
        /// </summary>
        public static string BitToHex(string bit)
        {
            byte[] b = BitStringToByteArray(bit);
            return ByteArrayToHexString(b);
        }

        /// <summary>
        /// 十六进制串转换成byte数组
        /// </summary>
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new Exception("十六进制格式不正确");
            }
            byte[] bArray = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                bArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bArray;
        }

        /// <summary>
        /// byte数组转换成二进制串
        /// </summary>
        public static string ByteArrayToBitString(byte[] bArray)
        {
            string[] sArray = new string[bArray.Length];
            for (int i = 0; i < bArray.Length; i++)
            {
                sArray[i] = Convert.ToString(bArray[i], 2).PadLeft(8, '0');
            }
            string bitString = "";
            foreach (string bit in sArray)
            {
                bitString += bit;
            }
            return bitString;
        }

        /// <summary>
        /// 二进制串转换成byte数组
        /// </summary>
        public static byte[] BitStringToByteArray(string bit)
        {
            if (bit.Length % 8 != 0)
            {
                throw new Exception("二进制格式不正确");
            }
            byte[] bArray = new byte[bit.Length / 8];
            for (int i = 0; i < bit.Length / 8; i++)
            {
                bArray[i] = Convert.ToByte(bit.Substring(i * 8, 8), 2);
            }
            return bArray;
        }

        /// <summary>
        /// byte数组转换成十六进制串
        /// </summary>
        public static string ByteArrayToHexString(byte[] bArray)
        {
            string[] sArray = new string[bArray.Length];
            for (int i = 0; i < bArray.Length; i++)
            {
                sArray[i] = Convert.ToString(bArray[i], 16).PadLeft(2, '0').ToUpper();
            }
            string hexString = "";
            foreach (string bit in sArray)
            {
                hexString += bit;
            }
            return hexString;
        }

        /// <summary>
        /// 每8位Bit的String串翻转倒序排列
        /// </summary>
        /// <param name="strBit"></param>
        /// <returns></returns>
        public static string BitStringEvery8ByteDesc(string strBit)
        {
            if (strBit.Length % 8 != 0)
            {
                return null;
            }
            string[] bitArray = new string[strBit.Length / 8];
            for (int i = 0; i < strBit.Length / 8; i++)
            {
                bitArray[i] = strBit.Substring(i * 8, 8);
            }
            for (int j = 0; j < bitArray.Length; j++)
            {
                string oldStr = bitArray[j];
                string newStr = "";
                for (int x = 7; x >= 0; x--)
                {
                    newStr += oldStr[x];
                }
                bitArray[j] = newStr;
            }
            string strDesc = "";
            foreach (string bit in bitArray)
            {
                strDesc += bit;
            }
            return strDesc;
        }
    }
}
