using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DecimalExp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mulTimes"></param>
        /// <returns></returns>
        public static decimal Multiply(this decimal value,decimal mulTimes)
        {
            return value * mulTimes;
        }

        /// <summary>
        /// 计算指定小数的整数部分。
        /// </summary>
        /// <param name="value">要截断的数字。</param>
        /// <returns>d 的整数部分（即舍弃小数位后剩余的数）。</returns>
        public static decimal Truncate(this decimal value)
        {
            return Math.Truncate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this decimal value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 计算指定小数的整数部分。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TruncateToInt(this decimal value)
        {
            return Convert.ToInt32(Truncate(value));
        }

        /// <summary>
        /// 计算指定小数的整数部分。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int16 TruncateToInt16(this decimal value)
        {
            return Convert.ToInt16(Truncate(value));
        }

        /// <summary>
        /// 计算指定小数的整数部分。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TruncateToInt32(this decimal value)
        {
            return TruncateToInt(value);
        }
        

        /// <summary>
        /// 将阿拉伯数字换成中文大写
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string[] NunberToChineseUpper(this decimal money)
        {
            string[] retList = new string[15];
            //默认所有值为零
            for (int i = 0; i < retList.Length; i++)
                retList[i] = "零";

            if (money == 0)
            {
                retList[0] = "零";
                return retList;
            }

            string strRmb;
            //将金额精确到小数点后2位
            strRmb = Math.Round(money, 2).ToString("F2").Replace(".", "");
            strRmb = strRmb.TrimStart('0');//去掉左边的0
            for (int i = 0; i < strRmb.Length; i++)
            {
                if (i > 14) continue;
                //从最后开始读，从右到左显示，左侧没有的填充为空白
                string temp = strRmb.Substring(strRmb.Length - i - 1, 1);
                string upperStr = temp.SingleNunberToChineseUpper();
                retList[i] = upperStr;
            }
            return retList;
        }


    }
}
