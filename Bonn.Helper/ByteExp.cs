using System;
using System.Collections;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class ByteExp
    {
        /// <summary>
        /// 数组转换为字符串,如将{0x11,0x22}转换为1122,{0xA1,0xA2}转换为A1A2
        /// </summary>
        /// <param name="bGroup"></param>
        /// <returns></returns>
        public static string ByteToString(this byte[] bGroup)
        {
            string temp = string.Empty;
            foreach (byte b in bGroup)
            {
                temp += b.ToString("X").PadLeft(2, '0');
            }
            return temp;
        }

        /// <summary>
        /// Byte类型转换为16进制字符串 所有值全部转换，字节中间有空格隔开
        /// 如：{0x01,0x02} 转换为 01 02
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexString(this byte[] byteBuf)
        {
            if (byteBuf == null || byteBuf.Length == 0) return string.Empty;

            return ByteToHexString(byteBuf, 0, byteBuf.Length);
        }

        /// <summary>
        /// Byte类型转换为16进制字符串，中间用空格断开
        /// 如：{0x01,0x02} 转换为 01 02
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <param name="begin">转换的开始位置</param>
        /// <param name="len">转换的长度</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexString(this byte[] byteBuf, int begin, int len)
        {
            if (byteBuf == null) return string.Empty;

            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(len * 3);
            for (int i = 0, j = begin; i < len; i++, j++)
            {
                strTmp.AppendFormat("{0:X2} ", byteBuf[j]);
            }
            return strTmp.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bGroup"></param>
        /// <returns></returns>
        public static int Byte3ToInt(this byte[] bGroup)
        {
            byte[] bytes = new byte[4];
            Buffer.BlockCopy(bGroup, 0, bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 数组转换为字符串
        /// </summary>
        /// <param name="bGroup"></param>
        /// <returns></returns>
        public static string ByteToString(this byte bGroup)
        {
            return bGroup.ToString("X").PadLeft(2, '0'); ;
        }


        /// <summary>
        /// ASCII转换为字符串，并去掉结尾的0字节
        /// <para>如将{0x31,0x32}转换为12</para>
        /// </summary>
        /// <param name="bytes"></param>
        public static string ToStringByAscii(this byte[] bytes)
        {
            Encoding ascii = Encoding.ASCII;
            return ascii.GetString(bytes, 0, bytes.Length).Trim('\0');
        }

        /// <summary>
        /// Byte数组转换成 Bit数组
        /// </summary>
        /// <param name="arrByt"></param>
        /// <returns></returns>
        public static byte[] ToArrayBitByte(this byte[] arrByt)
        {
            BitArray bitArray = new BitArray(arrByt);
            byte[] arrByte = new byte[bitArray.Length];
            for (int i = 0; i < bitArray.Length; i++)
            {
                arrByte[i] = bitArray[i] ? (byte)1 : (byte)0;
            }
            return arrByte;
        }

        /// <summary>
        /// 一维数组转换成二维数组
        /// </summary>
        /// <param name="arrByt"></param>
        /// <param name="iWidth">宽</param>
        /// <param name="iHeight">高</param>
        /// <returns></returns>
        public static byte[,] ToDoubleArrayByte(this byte[] arrByt, int iWidth, int iHeight)
        {
            if (arrByt.Length < iWidth * iHeight)
            {
                return null;
            }
            byte[,] arrDblArray = new byte[iHeight, iWidth];
            for (int loopHeight = 0; loopHeight < iHeight; loopHeight++)
            {
                for (int loopWidth = 0; loopWidth < iWidth; loopWidth++)
                {
                    arrDblArray[loopHeight, loopWidth] = arrByt[loopHeight * iWidth + loopWidth];
                }
            }
            return arrDblArray;
        }

        /// <summary>
        /// 对二维数组的行进行交换
        /// </summary>
        /// <param name="arrByt"></param>
        /// <returns></returns>
        public static byte[,] ToChangeArrayByte(this byte[,] arrByt)
        {
            #region

            //20 10
            //0, 20 - 1 - 0 = 19 >= 10
            //1, 20 - 1 - 1 = 18 >= 10
            //2, 20 - 1 - 2 = 17 >= 10
            //8, 20 - 1 - 8 = 11 >= 10
            //9, 20 - 1 - 9 = 10 >= 10
            //10, 20 - 1 - 10 = 9   10

            //21 10
            //0, 21 - 1 - 0 = 20 >= 10
            //1, 21 - 1 - 1 = 19 >= 10
            //2, 21 - 1 - 2 = 18 >= 10
            //8, 21 - 1 - 8 = 12 >= 10
            //9, 21 - 1 - 9 = 11 >= 10
            //10, 21 - 1 - 10 = 10 >=  10

            #endregion
            if (arrByt == null)
            {
                return null;
            }

            int iHeight = arrByt.GetLength(0);
            int iWidth = arrByt.GetLength(1);

            byte[,] arrRtn = new byte[iHeight, iWidth];

            for (int iloopHeight = 0; iloopHeight < iHeight; iloopHeight++)
            {
                for (int iloopWidth = 0; iloopWidth < iWidth; iloopWidth++)
                {
                    arrRtn[iHeight - 1 - iloopHeight, iloopWidth] = arrByt[iloopHeight, iloopWidth];
                }
            }

            return arrRtn;
        }

        /// <summary>
        /// 把byte数组转换为高位4和低位4的分开的数组
        /// </summary>
        /// <param name="arrByt"></param>
        /// <param name="iWidth">宽</param>
        /// <param name="iHeight">高</param>
        /// <returns></returns>
        public static byte[] ToArray4BitByte(this byte[] arrByt, int iWidth, int iHeight)
        {
            //if (arrByt.Length * 2 < iWidth * iHeight)
            //{
            //    return null;
            //}
            byte[] arr4Array = new byte[iWidth * iHeight];
            for (int i = 0; i < arrByt.Length; i++)
            {
                arr4Array[i * 2] = (byte)((arrByt[i] >> 4 & 0x0f) * 15);
                if (i * 2 == iWidth * iHeight)
                    break;
                arr4Array[i * 2 + 1] = (byte)((arrByt[i] & 0x0f) * 15); ;
            }
            return arr4Array;
        }

        /// <summary>
        /// 将 8 位无符号整数的值转换为其等效的二进制字符串表示形式。
        /// <para>如 0x64(10进制100)转换为01100100</para>
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static string ByteToSecString(this byte bt)
        {
            return Convert.ToString(bt, 2).PadLeft(8, '0');
        }

        /// <summary>
        /// 将 8 位无符号整数的值转换为其等效的二进制字符串表示形式。
        /// <para>如 0x64 0x64(10进制100)转换为 0110010001100100</para>
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static string ByteToSecString(this byte[] bs)
        {
            StringBuilder sbOut = new StringBuilder();
            foreach (var b in bs)
            {
                sbOut.Append(ByteToSecString(b));
            }
            return sbOut.ToString();
        }

        /// <summary>
        /// 计算桢校验和字节，返回一个校验字节
        /// </summary>
        /// <param name="userData">校验数据</param>
        /// <returns></returns>
        public static byte GetVerifyFramesSum(this byte[] userData)
        {
            int iNum = 0;
            foreach (byte b in userData)
                iNum = iNum + Convert.ToInt32(b);
            int rem = 0;
            System.Math.DivRem(iNum, 256, out rem);
            return Convert.ToByte(rem);
        }
    }
}
