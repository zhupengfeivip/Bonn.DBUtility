using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 截取字符串，不足左补齐，默认补0，然后截取指定长度的字符串，保证输出内容长度固定
        /// 不足左补齐，默认补0
        /// 超出长度后，取右边指定长度数据
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lenth"></param>
        /// <param name="padString"></param>
        /// <returns></returns>
        public static string PadLeftSubEnd(this string str, int lenth, char padString = '0')
        {
            string strTemp = str.PadLeft(lenth, padString);
            strTemp = strTemp.Substring(strTemp.Length - lenth, lenth);
            return strTemp;
        }

        /// <summary>
        /// 16进制字符串转换为Byte型数组
        /// </summary>
        /// <param name="hexString">16进制源字符串</param>
        /// <returns>Byte类型数组</returns>
        public static byte[] HexStringToByte(this string hexString)
        {
            #region 函数体
            int len = hexString.Length;
            if (len % 2 != 0)
                return null;
            byte[] bufD = new byte[len / 2];
            byte[] tmpBuf = System.Text.Encoding.UTF8.GetBytes(hexString);
            int i = 0, j = 0;
            for (i = 0; i < len; i++)
            {
                if (tmpBuf[i] >= 0x30 && tmpBuf[i] <= 0x39)
                    tmpBuf[i] -= 0x30;
                else if (tmpBuf[i] >= 0x41 && tmpBuf[i] <= 0x46)
                    tmpBuf[i] -= 0x37;
                else if (tmpBuf[i] >= 0x61 && tmpBuf[i] <= 0x66)
                    tmpBuf[i] -= 0x57;
                else
                    tmpBuf[i] = 0xF;
            }
            for (i = 0, j = 0; i < len; i += 2, j++)
            {
                bufD[j] = (byte)((tmpBuf[i] << 4) | tmpBuf[i + 1]);
            }
            return bufD;
            #endregion
        }
    }
}
