using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using Bonn.Helper;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExp
    {
        /// <summary>
        /// 当前字符串不是空，IsNotNullOrWhiteSpace为假
        /// </summary>
        /// <param name="orgString"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string orgString)
        {
            return string.IsNullOrWhiteSpace(orgString);
        }

        /// <summary>
        /// 当前字符串不是空，IsNotNullOrWhiteSpace为假
        /// </summary>
        /// <param name="orgString"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string orgString)
        {
            return !string.IsNullOrWhiteSpace(orgString);
        }

        /// <summary>
        ///字符串转换为字节，只处理数字串最左边2个数字，长度小于2返回0x00
        /// 如 1234 转换为{ 0x12 }
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte BcdToByte(this string str)
        {
            byte bOut = new byte();
            if (str.IsNullOrWhiteSpace()) return bOut;
            if (str.Length < 2) return bOut;

            byte[] bGroup = new byte[str.Length / 2];
            int index = 0;
            for (int i = 0; i < bGroup.Length; i++)
            {
                bGroup[i] = Convert.ToByte(int.Parse(str.Substring(index, 2), NumberStyles.HexNumber));
                index = index + 2;
            }
            return bGroup[0];
        }

        /// <summary>
        ///字符串转换为数组
        /// 如 1234 转换为{ 0x12, 0x34 }
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] BcdToBytes(this string str)
        {
            byte[] bGroup = new byte[str.Length / 2];
            int index = 0;
            for (int i = 0; i < bGroup.Length; i++)
            {
                bGroup[i] = Convert.ToByte(int.Parse(str.Substring(index, 2), NumberStyles.HexNumber));
                index = index + 2;
            }
            return bGroup;
        }

        /// <summary>
        /// 字符串反转
        /// </summary>
        /// <param name="oldStr"></param>
        /// <returns></returns>
        public static string Reverse(this string oldStr)
        {
            StringBuilder newStr = new StringBuilder();
            Stack<char> strStack = new Stack<char>();
            foreach (char c in oldStr)
            {
                strStack.Push(c);
            }
            while (strStack.Count > 0)
            {
                newStr.Append(strStack.Pop());
            }
            return newStr.ToString();
        }

        #region 转换

        /// <summary>
        /// 字符串转换为byte，字符串为空或者NULL时返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte SecStringToByte(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;

            if (str == "&nbsp;")
                return 0;

            return Convert.ToByte(str, 2);
        }

        /// <summary>
        /// 10进制字符串转换为byte，字符串为空或者NULL时返回0 ，如"100"->0x64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToByte(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            if (str == "&nbsp;")
                return 0;
            return Convert.ToByte(str);
        }

        /// <summary>
        /// 字符串转换为数组
        /// 如 "1234" 转换为{ 0x12, 0x34 }
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str)
        {
            byte[] bGroup = new byte[str.Length / 2];
            int index = 0;
            for (int i = 0; i < bGroup.Length; i++)
            {
                bGroup[i] = Convert.ToByte(int.Parse(str.Substring(index, 2), NumberStyles.HexNumber));
                index = index + 2;
            }
            return bGroup;
        }

        ///  <summary>
        ///  字符串转换为数组,不足时左补0
        ///  如 "1234" 转换为{ 0x12, 0x34 }
        ///  </summary>
        ///  <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="paddingChar"></param>
        /// <returns></returns>
        public static byte[] PadLeftBytes(this string str, int length, char paddingChar = '0')
        {
            byte[] bGroup = new byte[length / 2];
            int index = 0;
            for (int i = 0; i < bGroup.Length; i++)
            {
                bGroup[i] = Convert.ToByte(int.Parse(str.PadLeft(length, '0').Substring(index, 2), NumberStyles.HexNumber));
                index = index + 2;
            }
            return bGroup;
        }

        /// <summary>
        /// 10进制字符串转换为byte，字符串为空或者NULL时返回0 ，如"100"->0x64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte TryToByte(this string str)
        {
            try
            {
                return ToByte(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// 10进制字符串转换为byte，字符串为空或者NULL时返回0 ，如"100"->0x64
        /// </summary>
        /// <param name="str"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool TryToByte(this string str, out int outValue)
        {
            outValue = 0;
            try
            {
                outValue = ToByte(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 10进制字符串转换为Int32，字符串为空或者NULL时返回0 ，如"100"->0x64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            if (str == "&nbsp;")
                return 0;
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// 字符串转换为Int32，字符串为空或者NULL时返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int TryToInt(this string str)
        {
            try
            {
                return ToInt(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 字符串转换为Int32，字符串为空或者NULL时返回0
        /// </summary>
        /// <param name="str"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool TryToInt(this string str, out int outValue)
        {
            outValue = 0;
            try
            {
                outValue = ToInt(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            if (str == "&nbsp;")
                return 0;
            return Convert.ToUInt16(str);
        }

        /// <summary>
        /// 字符串转换为Decimal型,，字符串为空、NULL、nbsp;时返回 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            if (text == "&nbsp;")
                return 0;

            return Convert.ToDecimal(text.Trim());
        }

        /// <summary>
        /// 字符串转换成DateTime类型
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string text)
        {
            if ((string.IsNullOrEmpty(text)) || (text == "&nbsp;"))
                return DateTime.MinValue;

            return Convert.ToDateTime(text);
        }

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="formmat"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string strDate, string formmat)
        {
            try
            {
                DateTime outDateTime;
                TryToDateTime(strDate, formmat, out outDateTime);
                return outDateTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="formmat"></param>
        /// <param name="outDateTime"></param>
        /// <returns></returns>
        public static bool TryToDateTime(this string strDate, string formmat, out DateTime outDateTime)
        {
            outDateTime = DateTime.MinValue;
            try
            {
                int year;
                int month;
                int day;
                int hour;
                int minute;
                int second;
                switch (formmat)
                {
                    case "yyyyMMddHHmmss":
                        outDateTime = new DateTime(int.Parse(strDate.Substring(0, 4)),
                            int.Parse(strDate.Substring(4, 2)), int.Parse(strDate.Substring(6, 2)),
                            int.Parse(strDate.Substring(8, 2)), int.Parse(strDate.Substring(10, 2)),
                            int.Parse(strDate.Substring(12, 2)));
                        return true;
                    case "yyMMddHHmmss":
                        year = int.Parse("20" + strDate.Substring(0, 2));
                        month = int.Parse(strDate.Substring(2, 2));
                        day = int.Parse(strDate.Substring(4, 2));
                        hour = int.Parse(strDate.Substring(6, 2));
                        minute = int.Parse(strDate.Substring(8, 2));
                        second = int.Parse(strDate.Substring(10, 2));
                        outDateTime = new DateTime(year, month, day, hour, minute, second);
                        return true;
                    case "yyMMddHHmm":
                        year = int.Parse("20" + strDate.Substring(0, 2));
                        month = int.Parse(strDate.Substring(2, 2));
                        day = int.Parse(strDate.Substring(4, 2));
                        hour = int.Parse(strDate.Substring(6, 2));
                        minute = int.Parse(strDate.Substring(8, 2));
                        outDateTime = new DateTime(year, month, day, hour, minute, 0);
                        return true;
                    case "yyMMdd":
                        outDateTime = new DateTime(int.Parse("20" + strDate.Substring(0, 2)),
                            int.Parse(strDate.Substring(2, 2)), int.Parse(strDate.Substring(4, 2)));
                        return true;
                    case "yyyy-MM":
                        string[] strings = strDate.Split('-');
                        outDateTime = new DateTime(int.Parse(strings[0]), int.Parse(strings[1]), 1);
                        return true;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 将阿拉伯数字换成中文大写
        /// </summary>
        /// <param name="strRmb"></param>
        /// <returns></returns>
        public static string SingleNunberToChineseUpper(this string strRmb)
        {
            string outStr = strRmb;
            outStr = outStr.Replace("0", "零");
            outStr = outStr.Replace("1", "壹");
            outStr = outStr.Replace("2", "贰");
            outStr = outStr.Replace("3", "叁");
            outStr = outStr.Replace("4", "肆");
            outStr = outStr.Replace("5", "伍");
            outStr = outStr.Replace("6", "陆");
            outStr = outStr.Replace("7", "柒");
            outStr = outStr.Replace("8", "捌");
            outStr = outStr.Replace("9", "玖");
            return outStr;
        }

        /// <summary>
        /// 将中文大写换成阿拉伯数字
        /// </summary>
        /// <param name="cnString"></param>
        /// <returns></returns>
        private static decimal ChineseUpperToNunber(string cnString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 16进制字符串转换为Byte
        /// </summary>
        /// <param name="hexString">16进制源字符串</param>
        /// <returns>Byte类型数组</returns>
        public static byte HexToByte(this string hexString)
        {
            string strTemp = hexString;
            int len = hexString.Length;
            if (len % 2 != 0)
                strTemp = "0" + hexString;

            return StrHelper.HexStringToByte(strTemp)[0];
        }

        /// <summary>
        /// 16进制字符串转换为Byte
        /// </summary>
        /// <param name="jsonString">16进制源字符串</param>
        /// <param name="appendKey"></param>
        /// <param name="appendText"></param>
        /// <returns>Byte类型数组</returns>
        public static void JsonAppend(ref string jsonString, string appendKey, int appendText)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                jsonString = "{}";
            }
            if (jsonString.EndsWith("}]"))
            {
                jsonString = jsonString.Remove(jsonString.Length - 2, 2);
                jsonString += ",";

                jsonString += "\"" + appendKey + "\":";
                jsonString += "\"" + appendText + "\"";
                jsonString += "}]";
            }
            else if (jsonString.EndsWith("}"))
            {
                jsonString = jsonString.Remove(jsonString.Length - 1, 1);
                if (jsonString != "{")
                    jsonString += ",";

                jsonString += "\"" + appendKey + "\":";
                jsonString += appendText;
                jsonString += "}";
            }
            else
            {
                jsonString += "非正常JSon数据";
                return;
            }
        }
        
        /// <summary>
        /// 16进制字符串转换为Byte
        /// </summary>
        /// <param name="jsonString">16进制源字符串</param>
        /// <param name="appendKey"></param>
        /// <param name="appendText"></param>
        /// <returns>Byte类型数组</returns>
        public static void JsonAppend(ref string jsonString, string appendKey, string appendText)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                jsonString = "{}";
            }
            if (jsonString.EndsWith("}]"))
            {
                jsonString = jsonString.Remove(jsonString.Length - 2, 2);
                jsonString += ",";

                jsonString += "\"" + appendKey + "\":";
                jsonString += "\"" + appendText + "\"";
                jsonString += "}]";
            }
            else if (jsonString.EndsWith("}"))
            {
                jsonString = jsonString.Remove(jsonString.Length - 1, 1);
                if (jsonString != "{")
                    jsonString += ",";

                jsonString += "\"" + appendKey + "\":";
                jsonString += "\"" + appendText + "\"";
                jsonString += "}";
            }
            else
            {
                jsonString += "非正常JSon数据";
                return;
            }
        }
    }
}
