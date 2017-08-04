/*
 * 
 * 
 * 功能说明：字符串处理助手
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞
 * 修改说明：
 * 
 * */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Bonn.Helper
{
    /// <summary>
    /// 字符串处理助手
    /// </summary>
    public static class StrHelper
    {
        /// <summary>
        /// 数组转换为字符串
        /// </summary>
        /// <param name="bGroup"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bGroup)
        {
            string temp = string.Empty;
            foreach (byte b in bGroup)
            {
                temp += b.ToString("X").PadLeft(2, '0');
            }
            return temp;
        }

        /// <summary>
        ///字符串转换为数组
        /// 如 1234 转换为{ 0x12, 0x34 }
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string str)
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
        /// 解析字符串组对象
        /// </summary>
        /// <param name="oldString">字符串组对象，如 A||BCD||E</param>
        /// <param name="spPara">分隔符 如 |</param>
        /// <returns></returns>
        public static List<string> Split(string oldString, string spPara)
        {
            List<string> List = new List<string>();

            int index = 0;
            int len = 0;
            do
            {
                index = oldString.IndexOf("||", index);
                if (index == -1)
                {
                    break;
                }
                List.Add(oldString.Substring(len, index - len));
                index += 2;
                len = index;

            } while (index != -1);

            return List;
        }

        /// <summary>
        /// 汉字转换
        /// </summary>
        /// <param name="strChines"></param>
        /// <returns></returns>
        public static string ChinesToString(string strChines)
        {
            byte[] bGroup = Encoding.Unicode.GetBytes(strChines);
            return ByteToString(bGroup);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CutString(string str, int len)
        {
            if (str == null || str.Length <= len)
            {
                return str;
            }
            return str.Substring(0, len);
        }

        /// <summary>
        /// 检查字符串出现的次数
        /// </summary>
        /// <param name="allStr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Count(string allStr, string str)
        {
            int iCount = 0;
            if (string.IsNullOrEmpty(allStr) == true)
            {
                return -1;
            }
            if (string.IsNullOrEmpty(str) == true)
            {
                return 0;
            }
            if (allStr.Contains(str) == false)
            {
                return -1;
            }
            int index = 0;
            while (index > 0)
            {
                index = allStr.IndexOf(str, index, System.StringComparison.Ordinal);
                if (index == -1)
                {
                    break;
                }
                iCount++;
            }
            return iCount;
        }

        /// <summary>
        /// 字符串转换为Int32，字符串为空或者NULL时返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int StrToInt(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return 0;
                }
                if (str == "&nbsp;")
                {
                    return 0;
                }
                return Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 字符串转换为double型,，字符串为空、NULL、nbsp;时返回 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static double StrToDouble(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            else if (text == "&nbsp;")
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(Convert.ToDouble(text).ToString());
            }
        }

        /// <summary>
        /// 字符串转换为Decimal型,，字符串为空、NULL、nbsp;时返回 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal StrToDecimal(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            else if (text == "&nbsp;")
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(Convert.ToDecimal(text).ToString());
            }
        }

        /// <summary>
        /// 字符串转换成DateTime类型
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(string text)
        {
            if ((string.IsNullOrEmpty(text)) || (text == "&nbsp;"))
            {
                return DateTime.Now.Date;
            }
            else
            {
                return Convert.ToDateTime(text);
            }
        }

        /// <summary>
        /// 验证日期字符串是否为yyyy-MM-dd（yyyy/MM/dd或yyyy.MM.dd）格式
        /// </summary>
        /// <param name="strDateTime">日期字符串</param>
        /// <returns></returns>
        public static bool isDateTime(string strDateTime)
        {
            if (strDateTime.Length != 10)
            {
                return false;
            }
            if (strDateTime.Substring(4, 1) != "-" &&
                strDateTime.Substring(4, 1) != "/" &&
                strDateTime.Substring(4, 1) != ".")
            {
                return false;
            }
            if (strDateTime.Substring(7, 1) != "-" &&
                strDateTime.Substring(7, 1) != "/" &&
                strDateTime.Substring(7, 1) != ".")
            {
                return false;
            }
            try
            {
                DateTime.Parse(strDateTime);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证字符串是否是数字
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsInt(string strValue)
        {
            int outValeu;
            return int.TryParse(strValue, out outValeu);
        }

        /// <summary>
        /// 判断字符串中是否含有指定字符
        /// </summary>
        /// <param name="myStr">字符串</param>
        /// <param name="cs">指定字符或字符串</param>
        /// <returns></returns>
        public static bool StrContains(string myStr, string[] cs)
        {
            foreach (string str in cs)
            {
                if (myStr.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将字符串转换为JS识别的字符串
        /// <para>由于JS中不识别'，所以将'转换为\'</para>
        /// <para>由于JS中不识别;，所以将'转换为\;'</para> 
        /// <para>由于JS中不识别换行\r\n，所以将换行转换为空格。如果不转换可能会出现不执行JS脚本的现象。</para>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string JsEncode(string str)
        {
            str = str.Replace("'", "\\'");
            str = str.Replace(":", "\\:");
            str = str.Replace("\r\n", "\\r\\n");
            return str;
        }

        /// <summary>
        /// 格式化（小写转大写）
        /// </summary>
        /// <param name="numRmb"></param>
        /// <returns></returns>
        public static string Format(int numRmb)
        {
            try
            {
                if (0 == numRmb)
                    return "零元整";

                StringBuilder szRmb = new StringBuilder();

                //乘100以格式成整型，便于处理
                ulong iRmb = Convert.ToUInt64(numRmb * 100);

                szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 100), -2));

                //去掉原来的小数位
                iRmb = iRmb / 100;

                int iUnit = 0;

                //以每4位为一个单位段进行处理，所以下边除以10000
                while (iRmb != 0)
                {
                    szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 10000), iUnit));
                    iRmb = iRmb / 10000;
                    iUnit += 4;
                }

                string strRmb = szRmb.ToString();

                //格式修正
                strRmb = Regex.Replace(strRmb, "零+", "零");
                strRmb = strRmb.Replace("元零整", "元整");
                strRmb = strRmb.Replace("零元", "元");

                return strRmb.Trim('零');
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 格式化（小写转大写），如 0 转换为 零元整
        /// </summary>
        /// <param name="numRmb"></param>
        /// <returns></returns>
        public static string Format(decimal numRmb)
        {
            try
            {
                if (0 == numRmb)
                    return "零元整";

                StringBuilder szRmb = new StringBuilder();

                //乘100以格式成整型，便于处理
                ulong iRmb = Convert.ToUInt64(Math.Abs(numRmb * 100));

                szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 100), -2));

                //去掉原来的小数位
                iRmb = iRmb / 100;

                int iUnit = 0;

                //以每4位为一个单位段进行处理，所以下边除以10000
                while (iRmb != 0)
                {
                    szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 10000), iUnit));
                    iRmb = iRmb / 10000;
                    iUnit += 4;
                }

                string strRmb = szRmb.ToString();

                //格式修正
                strRmb = Regex.Replace(strRmb, "零+", "零");
                strRmb = strRmb.Replace("元零整", "元整");
                strRmb = strRmb.Replace("零元", "元");

                return strRmb.Trim('零');
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 格式化（小写转大写）
        /// </summary>
        /// <param name="numRmb"></param>
        /// <returns></returns>
        public static string Format(double numRmb)
        {
            try
            {
                if (numRmb.Equals(0))
                    return "零元整";

                StringBuilder szRmb = new StringBuilder();

                //乘100以格式成整型，便于处理
                ulong iRmb = Convert.ToUInt64(numRmb * 100);

                szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 100), -2));

                //去掉原来的小数位
                iRmb = iRmb / 100;

                int iUnit = 0;

                //以每4位为一个单位段进行处理，所以下边除以10000
                while (iRmb != 0)
                {
                    szRmb.Insert(0, ToUpper(Convert.ToInt32(iRmb % 10000), iUnit));
                    iRmb = iRmb / 10000;
                    iUnit += 4;
                }

                string strRmb = szRmb.ToString();

                //格式修正
                strRmb = Regex.Replace(strRmb, "零+", "零");
                strRmb = strRmb.Replace("元零整", "元整");
                strRmb = strRmb.Replace("零元", "元");

                return strRmb.Trim('零');
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 计算表达式（大写表达式求值）
        /// </summary>
        /// <param name="strRmb"></param>
        /// <returns></returns>
        private static double Eval(string strRmb)
        {
            try
            {
                if (null == strRmb)
                    return 0;

                strRmb = Replace(strRmb, false);

                if ("" == strRmb)
                    return 0;

                //基础指数
                int basicExp = 0;
                //当前指数
                int currExp = 0;

                double numRmb = 0;

                for (int i = strRmb.Length - 1; i > -1; i--)
                {
                    char temp = strRmb[i];

                    if (temp == '元' || temp == '万' || temp == '亿' || temp == '圆' || temp == '萬')
                    {
                        basicExp = GetExp(temp);
                        currExp = 0;

                        continue;
                    }
                    else
                    {
                        if (Regex.IsMatch(temp.ToString(), "^\\d"))
                        {
                            numRmb = numRmb + Convert.ToInt32(temp.ToString()) * Math.Pow(10, (basicExp + currExp));
                        }
                        else
                        {
                            currExp = GetExp(temp);
                        }

                    }
                }

                return numRmb;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 计算表达式（小写数值求大写字符串）
        /// </summary>
        /// <param name="numRmb"></param>
        /// <param name="iUnit"></param>
        /// <returns></returns>
        private static string ToUpper(int numRmb, int iUnit)
        {
            try
            {
                if (0 == numRmb)
                {
                    if (iUnit == -2)
                    {
                        return "整";
                    }

                    if (iUnit == 0)
                    {
                        return "元";
                    }

                    return "零";
                }

                StringBuilder szRmb = new StringBuilder();

                string strRmb = "";

                if (iUnit == -2)
                {
                    int jiao = numRmb / 10;
                    int fen = numRmb % 10;

                    if (jiao > 0)
                    {
                        szRmb.Append(jiao);
                        szRmb.Append(GetUnit(-1));

                        if (fen > 0)
                        {
                            szRmb.Append(fen);
                            szRmb.Append(GetUnit(-2));
                        }
                    }
                    else
                    {
                        szRmb.Append(fen);
                        szRmb.Append(GetUnit(-2));
                    }

                    return Replace(szRmb.ToString(), true);
                }

                strRmb = numRmb.ToString("0000");

                //前一位是否是0
                bool hasZero = false;

                for (int i = 0; i < strRmb.Length; i++)
                {
                    //只有四位，最高位为‘千’，所以下边的3-i为单位修正
                    if ((3 - i) > 0)
                    {
                        if ('0' != strRmb[i])
                        {
                            szRmb.Append(strRmb[i]);
                            szRmb.Append(GetUnit(3 - i));
                            hasZero = false;
                        }
                        else
                        {
                            if (!hasZero)
                                szRmb.Append(strRmb[i]);

                            hasZero = true;
                        }
                    }
                    //最后一位，特别格式处理
                    //如最后一位是零，则单位应在零之前
                    else
                    {
                        if ('0' != strRmb[i])
                        {
                            szRmb.Append(strRmb[i]);
                            szRmb.Append(GetUnit(iUnit));
                            hasZero = false;
                        }
                        else
                        {
                            if (hasZero)
                            {
                                szRmb.Insert(szRmb.Length - 1, GetUnit(iUnit));
                            }
                            else
                            {
                                szRmb.Append(GetUnit(iUnit));
                                szRmb.Append(strRmb[i]);
                            }
                        }
                    }
                }

                //转换大写后返回
                return Replace(szRmb.ToString(), true);

            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 将中文大写换成阿拉伯数字
        /// </summary>
        /// <param name="strRmb"></param>
        /// <param name="toUpper">true--转换为大写/false--转换为小写</param>
        /// <returns></returns>
        private static string Replace(string strRmb, bool toUpper)
        {
            if (toUpper)
            {
                strRmb = strRmb.Replace("0", "零");
                strRmb = strRmb.Replace("1", "壹");
                strRmb = strRmb.Replace("2", "贰");
                strRmb = strRmb.Replace("3", "叁");
                strRmb = strRmb.Replace("4", "肆");
                strRmb = strRmb.Replace("5", "伍");
                strRmb = strRmb.Replace("6", "陆");
                strRmb = strRmb.Replace("7", "柒");
                strRmb = strRmb.Replace("8", "捌");
                strRmb = strRmb.Replace("9", "玖");
            }
            else
            {
                strRmb = strRmb.Replace("零", "0");
                strRmb = strRmb.Replace("壹", "1");
                strRmb = strRmb.Replace("贰", "2");
                strRmb = strRmb.Replace("叁", "3");
                strRmb = strRmb.Replace("肆", "4");
                strRmb = strRmb.Replace("伍", "5");
                strRmb = strRmb.Replace("陆", "6");
                strRmb = strRmb.Replace("柒", "7");
                strRmb = strRmb.Replace("捌", "8");
                strRmb = strRmb.Replace("玖", "9");
            }
            return strRmb;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        /// <param name="iCode"></param>
        /// <returns></returns>
        private static string GetUnit(int iCode)
        {
            switch (iCode)
            {
                case -2:
                    return "分";
                case -1:
                    return "角";
                case 0:
                    return "元";
                case 1:
                    return "拾";
                case 2:
                    return "佰";
                case 3:
                    return "仟";
                case 4:
                    return "萬";
                case 8:
                    return "亿";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 获取位权指数
        /// </summary>
        /// <param name="cUnit"></param>
        /// <returns></returns>
        private static int GetExp(char cUnit)
        {
            switch (cUnit)
            {
                case '分':
                    return -2;
                case '角':
                    return -1;
                case '元':
                case '圆':
                    return 0;
                case '十':
                case '拾':
                    return 1;
                case '百':
                case '佰':
                    return 2;
                case '千':
                case '仟':
                    return 3;
                case '万':
                case '萬':
                    return 4;
                case '亿':
                    return 8;
                default:
                    return 0;
            }

        }


        #region 基本方法
        /// <summary>
        /// 将Byte类型数组转换为UTF8编码格式的String类型字符串
        /// </summary>
        /// <param name="trunNull"></param>
        /// <param name="byteBuf">Byte类型数组</param>
        /// <returns>String类型字符串</returns>
        public static string ByteToUTF8String(bool trunNull, byte[] byteBuf)
        {
            return ByteToUTF8String(trunNull, byteBuf, 0, byteBuf.Length);
        }

        /// <summary>
        /// 将Byte类型数组转换为UTF8编码格式的String类型字符串
        /// </summary>
        /// <param name="trunNull"></param>
        /// <param name="byteBuf">Byte类型数组</param>
        /// <param name="begin">指定取字符的位置</param>
        /// <param name="len">取字符的长度</param>
        /// <returns>String类型字符串</returns>
        public static string ByteToUTF8String(bool trunNull, byte[] byteBuf, int begin, int len)
        {
            string strTmp = new string(System.Text.Encoding.UTF8.GetChars(byteBuf, begin, len)).Trim();
            if (trunNull)
            {
                int iPos = strTmp.IndexOf('\0');
                if (iPos < 0)
                    return strTmp;
                else if (iPos == 0)
                    return "";
                return strTmp.Substring(0, iPos);
            }
            return strTmp;
        }

        /// <summary>
        /// 将Byte类型数组转换为系统当前编码格式的String类型字符串
        /// </summary>
        /// <param name="trunNull"></param>
        /// <param name="byteBuf">Byte类型数组</param>
        /// <returns>String类型字符串</returns>
        public static string ByteToDefaultString(bool trunNull, byte[] byteBuf)
        {
            return ByteToDefaultString(trunNull, byteBuf, 0, byteBuf.Length);
        }

        /// <summary>
        /// 将Byte类型数组转换为系统当前编码格式的String类型字符串
        /// </summary>
        /// <param name="trunNull"></param>
        /// <param name="byteBuf">Byte类型数组</param>
        /// <param name="begin">指定取字符的位置</param>
        /// <param name="len">取字符的长度</param>
        /// <returns>String类型字符串</returns>
        public static string ByteToDefaultString(bool trunNull, byte[] byteBuf, int begin, int len)
        {
            string strTmp = new string(System.Text.Encoding.Default.GetChars(byteBuf, begin, len)).Trim();
            if (trunNull)
            {
                int iPos = strTmp.IndexOf('\0');
                if (iPos < 0)
                    return strTmp;
                else if (iPos == 0)
                    return "";
                return strTmp.Substring(0, iPos);
            }
            return strTmp;
        }

        /// <summary>
        /// 将当前系统编码格式的字符串转换为Byte类型数组
        /// </summary>
        /// <param name="chinaStr">string字符串</param>
        /// <returns>Byte类型数组</returns>
        public static byte[] DefaultStringToByte(string chinaStr)
        {
            byte[] ret = System.Text.Encoding.Default.GetBytes(chinaStr);
            return ret;
        }

        /// <summary>
        /// 将当前编码格式的字符串转换到指定的Byte类型数组中指定的位置
        /// </summary>
        /// <param name="chinaStr">当前编码格式的字符串</param>
        /// <param name="chinaByte">接收的Byte类型数组</param>
        /// <param name="begin">接收的Byte类型数组chinaByte的开始位置</param>
        /// <param name="len">接收的长度</param>
        /// <returns></returns>
        public static bool DefaultStringToByte(string chinaStr, byte[] chinaByte, int begin, int len)
        {
            chinaStr = chinaStr.PadRight(len);
            byte[] ret = System.Text.Encoding.Default.GetBytes(chinaStr);
            Array.Copy(ret, 0, chinaByte, begin, len);
            return true;
        }

        /// <summary>
        /// Byte类型转换为16进制字符串，中间用空格断开
        /// 如：{0x01,0x02} 转换为 01 02
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <param name="begin">转换的开始位置</param>
        /// <param name="len">转换的长度</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexString(byte[] byteBuf, int begin, int len)
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
        /// Byte类型转换为16进制字符串
        /// 如：{0x01,0x02} 转换为 0102
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <param name="begin">转换的开始位置</param>
        /// <param name="len">转换的长度</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexStringNoSpace(byte[] byteBuf, int begin, int len)
        {
            if (byteBuf == null) return string.Empty;

            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(len * 2);
            for (int i = 0, j = begin; i < len; i++, j++)
            {
                strTmp.AppendFormat("{0:X2}", byteBuf[j]);
            }
            return strTmp.ToString();
        }

        /// <summary>
        /// Byte类型转换为16进制字符串
        /// 如：0x01 转换为 01
        /// </summary>
        /// <param name="bt">欲转换的Byte类型数组</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexStringNoSpace(byte bt)
        {
            #region 函数体
            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(2);
            strTmp.AppendFormat("{0:X2}", bt);
            return strTmp.ToString();
            #endregion
        }

        /// <summary>
        /// Byte类型转换为16进制字符串 所有值全部转换，字节中间有空格隔开
        /// 如：{0x01,0x02} 转换为 01 02
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexString(byte[] byteBuf)
        {
            if (byteBuf == null || byteBuf.Length == 0) return string.Empty;

            return ByteToHexString(byteBuf, 0, byteBuf.Length);
        }


        /// <summary>
        /// Byte类型转换为16进制字符串 所有值全部转换
        /// 如：{0x01,0x02} 转换为 0102
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexStringNoSpace(byte[] byteBuf)
        {
            if (byteBuf == null || byteBuf.Length == 0) return string.Empty;

            return ByteToHexStringNoSpace(byteBuf, 0, byteBuf.Length);
        }

        /// <summary>
        /// Byte类型转换为16进制字符串
        /// 如：{0x01,0x02} 转换为 0102
        /// </summary>
        /// <param name="byteBuf">欲转换的Byte类型数组</param>
        /// <param name="begin">转换的开始位置</param>
        /// <param name="len">转换的长度</param>
        /// <param name="lenFillSpace">在指定长度时添加空格</param>
        /// <returns>String类型16进制数</returns>
        public static string ByteToHexString(byte[] byteBuf, int begin, int len, byte lenFillSpace)
        {
            #region 函数体
            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(len * 3);
            for (int i = 0, j = begin; i < len; i++, j++)
            {
                strTmp.AppendFormat("{0:X2}", byteBuf[j]);
                if (i > 0 && lenFillSpace > 0 && (i % lenFillSpace == 0))
                    strTmp.Append(" ");
            }
            return strTmp.ToString();
            #endregion
        }
        /// <summary>
        /// 标准的字节数据转换成HexString格式的字节数据,字节长度的合法性本函数不验证.
        /// 例子：[0x78,0xB6]=>[0x37,0x38,0x42,0x36]既['7','8','B','6']
        /// </summary>
        /// <param name="byteBufS">标准的字节数据源</param>
        /// <param name="beginS">起始位,注意基数从0开始</param>
        /// <param name="lenS">长度</param>
        /// <param name="byteHexStrBufD">转换后的数据</param>
        /// <param name="beginD">起始位,注意基数从0开始</param>
        public static void ByteToHexStrBuf(byte[] byteBufS, int beginS, int lenS, byte[] byteHexStrBufD, int beginD)
        {
            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(lenS * 2 + 1);
            for (int i = 0, j = beginS; i < lenS; i++, j++)
            {
                strTmp.AppendFormat("{0:X2}", byteBufS[j]);
            }
            byte[] tmpBuf = System.Text.Encoding.UTF8.GetBytes(strTmp.ToString());
            Array.Copy(tmpBuf, 0, byteHexStrBufD, beginD, lenS * 2);
        }
        /// <summary>
        /// HexString格式的字节数据转换成标准的字节数据,字节长度的合法性本函数不验证.
        /// 例子：[0x37,0x38,0x42,0x36]既['7','8','B','6']=>[0x78,0xB6]
        /// </summary>
        /// <param name="byteHexStrBufS">HexString格式的字节数据源</param>
        /// <param name="beginS">起始位,注意基数从0开始</param>
        /// <param name="lenS">长度,应该是偶数</param>
        /// <param name="byteBufD">转换后的数据</param>
        /// <param name="beginD">起始位,注意基数从0开始</param>
        public static void HexStrBufToBuf(byte[] byteHexStrBufS, int beginS, int lenS, byte[] byteBufD, int beginD)
        {
            byte[] tmpBuf = new byte[lenS + 1];
            Array.Copy(byteHexStrBufS, beginS, tmpBuf, 0, lenS);
            int i = 0, j = 0;
            for (i = 0; i < lenS; i++)
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
            for (i = 0, j = beginD; i < lenS; i += 2, j++)
            {
                byteBufD[j] = (byte)((tmpBuf[i] << 4) | tmpBuf[i + 1]);
            }
        }

        /// <summary>
        /// String类型转换为16进制字符串
        /// </summary>
        /// <param name="strS">准备转换的源字符串</param>
        /// <returns>String类型16进制数</returns>
        public static string stringToHexString(string strS)
        {
            #region 函数体
            byte[] byteBuf = System.Text.Encoding.UTF8.GetBytes(strS.ToUpper());
            System.Text.StringBuilder strTmp = new System.Text.StringBuilder(byteBuf.Length * 2 + 1);
            foreach (byte tmp in byteBuf)
            {
                strTmp.AppendFormat("{0:X2}", tmp);
            }
            return strTmp.ToString();
            #endregion
        }

        /// <summary>
        /// 16进制字符串转换为Byte型数组
        /// </summary>
        /// <param name="hexString">16进制源字符串</param>
        /// <param name="bufD">Byte型目标数组</param>
        /// <param name="begin">开始位置</param>
        /// <returns>如果源字符串的长度为2的整数倍返回hexString长度/2，否则返回0</returns>
        public static int HexStringToByte(string hexString, byte[] bufD, int begin)
        {
            int len = hexString.Length;
            if (len % 2 != 0)
                return 0;
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
            for (i = 0, j = begin; i < len; i += 2, j++)
            {
                bufD[j] = (byte)((tmpBuf[i] << 4) | tmpBuf[i + 1]);
            }
            return len / 2;
        }

        /// <summary>
        /// 16进制字符串转换为Byte型数组
        /// </summary>
        /// <param name="hexString">16进制源字符串</param>
        /// <returns>Byte类型数组</returns>
        public static byte[] HexStringToByte(string hexString)
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

        /// <summary>
        /// 数字转换为16进制字符串
        /// 如：0x01->"01",0x0A->"0A"
        /// </summary>
        /// <param name="data">Byte型数据</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(byte data)
        {
            return string.Format("{0:X02}", data).PadLeft(2, '0');
        }

        /// <summary>
        /// 数字转换为16进制字符串
        /// </summary>
        /// <param name="data">Byte型数据</param>
        /// <param name="fillLen">转换后的长度，不足补空格</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(byte data, int fillLen)
        {
            fillLen = fillLen < 2 ? 2 : fillLen;
            return string.Format("{0:X02}", data).PadRight(fillLen);
        }

        /// <summary>
        /// 数字转换为16进制字符串
        /// </summary>
        /// <param name="data">short(Int16)型数据</param>
        /// <param name="fillLen">转换后的长度，不足补空格</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(short data, int fillLen)
        {
            fillLen = fillLen < 4 ? 4 : fillLen;
            return string.Format("{0:X04}", data).PadRight(fillLen);
        }

        /// <summary>
        /// 数字转换为16进制字符串
        /// </summary>
        /// <param name="data">int(Int32)型数据</param>
        /// <param name="fillLen">转换后的长度，不足补空格</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(int data, int fillLen)
        {
            #region 函数体
            fillLen = fillLen < 8 ? 8 : fillLen;
            return string.Format("{0:X08}", data).PadRight(fillLen);
            #endregion
        }

        /// <summary>
        /// 数字转换为16进制字符串
        /// </summary>
        /// <param name="data">long(Int64)型数据</param>
        /// <param name="fillLen">转换后的长度，不足补空格</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(long data, int fillLen)
        {
            #region 函数体
            fillLen = fillLen < 16 ? 16 : fillLen;
            return string.Format("{0:X16}", data).PadRight(fillLen);
            #endregion
        }

        /// <summary>
        /// BCD编码 数字字符串转换为字节数组
        /// 如：数字“1234”转换为{0x12,0x34}
        /// </summary>
        /// <param name="numS">要转换的数字字符串</param>
        /// <param name="bcdBuf">输出的BCD数组</param>
        /// <param name="begin">bcdBuf 中从零开始的字节偏移量。</param>
        public static void BCDToBytes(string numS, byte[] bcdBuf, int begin)
        {
            int strLen = numS.Length;
            if ((strLen % 2) != 0)
                numS = numS.PadLeft(++strLen, ' ');
            byte[] numBuf = System.Text.Encoding.UTF8.GetBytes(numS);
            byte byteH, byteL;
            for (int i = 0, j = begin; i < strLen; i += 2, j++)
            {
                if (numBuf[i] >= 0x61)         //'a' 
                    byteH = (byte)(numBuf[i] - 0x61 + 10);
                else if (numBuf[i] >= 0x41)   //'A'
                    byteH = (byte)(numBuf[i] - 0x41 + 10);
                else if (numBuf[i] >= 0x30)
                    byteH = (byte)(numBuf[i] - 0x30);
                else
                    byteH = 0;
                if (numBuf[i + 1] >= 0x61)         //'a' 
                    byteL = (byte)(numBuf[i + 1] - 0x61 + 10);
                else if (numBuf[i + 1] >= 0x41)   //'A'
                    byteL = (byte)(numBuf[i + 1] - 0x41 + 10);
                else if (numBuf[i + 1] >= 0x30)
                    byteL = (byte)(numBuf[i + 1] - 0x30);
                else

                    byteL = 0;
                bcdBuf[j] = (byte)(((byteH & 0xF) << 4) | (byteL & 0xF));
            }
        }

        /// <summary>
        /// BCD编码 数字字符串转换为字节数组
        /// 如：数字“1234”转换为{0x12,0x34}
        /// </summary>
        /// <param name="numS">要转换的数字字符串</param>
        /// <param name="bcdBuf">输出的BCD数组</param>
        public static void BCDToBytes(string numS, byte[] bcdBuf)
        {
            BCDToBytes(numS, bcdBuf, 0);
        }

        /// <summary>
        /// BCD码转换为INT类型数值，数值包含有非BCD码时返回0
        /// </summary>
        /// <param name="bcdNum"></param>
        /// <example>
        /// BcdToInt(""0101)
        /// 输出:101
        /// </example>
        /// <returns></returns>
        public static int BcdToInt(string bcdNum)
        {
            try
            {
                return Convert.ToInt32(bcdNum);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// BCD码转换为INT类型数值，数值包含有非BCD码时返回0
        /// </summary>
        /// <param name="bcdNum"></param>
        /// <returns></returns>
        public static UInt32 BcdToUInt(string bcdNum)
        {
            try
            {
                return Convert.ToUInt32(bcdNum);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// ASCII转换为字符串
        /// <para>如将{0x31,0x32}转换为12</para>
        /// </summary>
        /// <param name="bytes"></param>
        public static string GetAsciiString(byte[] bytes)
        {
            Encoding ascii = Encoding.ASCII;
            return ascii.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 字符串转换为ASCII
        /// <para>如将12转换为{0x31,0x32}</para>
        /// </summary>
        /// <param name="str"></param>
        public static byte[] ToAscii(this string str)
        {
            Encoding ascii = Encoding.ASCII;
            return ascii.GetBytes(str);
        }

        /// <summary>
        /// 字符串反转，如“12345678”转换为“21436587”
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <example> 
        /// StringReverse("12345678");
        /// 输出:21436587 
        /// </example>
        /// <returns>转换后的字符串</returns>
        public static string StringReverse(string str)
        {
            StringBuilder sbOut = new StringBuilder();
            for (int i = 0; i < str.Length; )
            {
                sbOut.AppendFormat("{0}{1}", str.Substring(i + 1, 1), str.Substring(i, 1));
                i = i + 2;
            }
            return sbOut.ToString();
        }

        /// <summary>
        /// 字符串反转，低字节在前，如“12345678”转换为“78563412”
        /// </summary>
        /// <param name="strOrg">原始字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string StringReverse2(string strOrg)
        {
            if ((strOrg.Length % 2) != 0)
                throw new Exception("字符串长度必须是2的整数倍。");

            StringBuilder sbOut = new StringBuilder();
            for (int i = strOrg.Length; i > 0; )
            {
                i = i - 2;
                sbOut.AppendFormat("{0}", strOrg.Substring(i, 2));
            }
            return sbOut.ToString();
        }

        /// <summary>
        /// 将 8 位无符号整数的值转换为其等效的二进制字符串表示形式。
        /// <para>如 0x64(10进制100)转换为01100100</para>
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static string ByteToSecString(byte bt)
        {
            return Convert.ToString(bt, 2).PadLeft(8, '0');
        }

        /// <summary>
        /// 将 8 位无符号整数的值转换为其等效的二进制字符串表示形式。
        /// <para>如 0x64 0x64(10进制100)转换为 0110010001100100</para>
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static string ByteToSecString(byte[] bs)
        {
            StringBuilder sbOut = new StringBuilder();
            foreach (var b in bs)
            {
                sbOut.Append(ByteToSecString(b));
            }
            return sbOut.ToString();
        }

        /// <summary>
        /// 将二进制字符串转换为数组
        /// <para>如 0x64 0x64(10进制100)转换为 0110010001100100</para>
        /// </summary>
        /// <param name="secString"></param>
        /// <returns></returns>
        public static byte[] SecStringToBytes(string secString)
        {
            byte[] tempBytes = new byte[secString.Length / 8];
            for (int i = 0; i < tempBytes.Length; i++)
            {
                tempBytes[i] = Convert.ToByte(secString.Substring(i * 8, 8), 2);
            }
            return tempBytes;
        }


        /// <summary>
        /// UCS2解码 
        /// </summary>
        /// <param name="src"> UCS2 源串 </param>
        /// <returns> 解码后的UTF-16BE字符串</returns>
        public static string DecodeUCS2(string src)
        {
            string decucs = string.Empty;
            if (src.IndexOf("/r") != -1)
            {
                decucs = src.Remove(src.IndexOf("/r"));
            }
            else
            {
                decucs = src;
            }
            string pstr = "^[0-9a-fA-F]+$";
            if (!Regex.IsMatch(decucs, pstr))
            {
                return "非法字符串无法解析！";
            }

            StringBuilder builer = new StringBuilder();

            for (int i = 0; i < decucs.Length; i += 4)
            {
                int unicode_nu = Int32.Parse(decucs.Substring(i, 4), System.Globalization.NumberStyles.HexNumber);
                builer.Append(string.Format("{0}", (char)unicode_nu));
            }

            return builer.ToString();
        }

        /// <summary>
        /// UCS2编码，如 你好 转换为 F60597D
        /// </summary>
        /// <param name="src"> UTF-16BE编码的源串</param>
        /// <returns>编码后的UCS2串 </returns>
        public static string EncodeUCS2(string src)
        {
            StringBuilder builer = new StringBuilder();
            //builer.Append("000800");
            byte[] tmpSmsText = Encoding.Unicode.GetBytes(src);
            //builer.Append(tmpSmsText.Length.ToString("X2")); //正文内容长度
            for (int i = 0; i < tmpSmsText.Length; i += 2) //高低字节对调 
            {
                builer.Append(tmpSmsText[i + 1].ToString("X2"));//("X2")转为16进制
                builer.Append(tmpSmsText[i].ToString("X2"));
            }
            //builer = builer.Remove(0, 8);

            return builer.ToString();
        }

        /// <summary>
        /// PDU字符串用到的7bit的加密函数
        /// </summary>
        /// <param name="strUserData">数据部分PDU字符串</param>
        /// <returns></returns>
        public static string Pdu7BitContentEncoder(this string strUserData)
        {
            string result = string.Empty;
            string resultLength = strUserData.Length.ToString("X2");                  //7bit编码 宿舍数据长度：源字符串长度

            Encoding encodingAsscii = Encoding.ASCII;
            byte[] bytes = encodingAsscii.GetBytes(strUserData);

            string temp = string.Empty;                                     //存储中间字符串 二进制串
            string tmp;
            for (int i = strUserData.Length; i > 0; i--)                          //高低交换 二进制串
            {
                tmp = Convert.ToString(bytes[i - 1], 2);
                while (tmp.Length < 7)                                      //不够7位，补齐
                {
                    tmp = "0" + tmp;
                }
                temp += tmp;
            }

            for (int i = temp.Length; i > 0; i -= 8)                    //每8位取位为一个字符 即完成编码
            {
                if (i > 8)
                {
                    result += Convert.ToInt32(temp.Substring(i - 8, 8), 2).ToString("X2");
                }
                else
                {
                    result += Convert.ToInt32(temp.Substring(0, i), 2).ToString("X2");
                }
            }

            return result;
        }

        /// <summary>
        /// PDU字符串用到的7bit的解密函数
        /// </summary>
        /// <param name="strUserData">数据部分PDU字符串</param>
        /// <returns></returns>
        public static string Pdu7BitContentDecoder(string strUserData)
        {
            string result = string.Empty;
            //byte[] b;

            //b = Hex2Bin(strUserData);

            //Array.Reverse(b);       //字节串翻转

            //result = Bin2BinStringof8Bit(b);

            //result = BinStringof8Bit2AsciiwithReverse(result);

            return result;
        }

        /// <summary>
        /// Decimal类型数据转换为String类型，用于界面显示
        /// <para>如 1.0000转为为1，1.10000转换为1.1</para>
        /// </summary>
        /// <param name="decValue"></param>
        /// <returns></returns>
        public static string DecimalToString(decimal? decValue)
        {
            return string.Format("{0:0.000000}", decValue).TrimEnd('0').TrimEnd('.');
        }

        #endregion


        #region 短信息解码

        /// <summary>
        /// 对整个短信息进行解码
        /// </summary>
        /// <param name="s">要解码的信息</param>
        /// <param name="phone">解码后的电话号码,本短信发送方的来源号码</param>
        /// <param name="text">解码后的短信内容</param>
        /// <param name="sendTime">短信时间戳</param>
        /// <param name="code">使用的编码方式</param>
        /// <param name="sca"></param>
        /// <returns>成功返回true</returns>
        public static bool DecodingMsg(string s, ref string phone, ref string text, ref DateTime sendTime, ref GSMCode code, ref string sca)
        {
            try
            {
                //短信息中心
                int iLength = int.Parse(s.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                if (iLength > 0)
                {
                    if (s.Substring(2, 2) == "91")
                    {
                        sca += "+";
                        iLength--;
                    }
                    for (int i = 0; i < iLength * 2; i += 2)
                    {
                        sca += s.Substring(5 + i, 1);
                        sca += s.Substring(4 + i, 1);
                    }
                    if (sca.EndsWith("F")) sca = sca.Remove(sca.Length - 1, 1);
                }
                s = s.Remove(0, iLength * 2 + 6);
                //发送方号码
                iLength = int.Parse(s.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                if (s.Substring(2, 2) == "91")
                {
                    phone = "+";
                }
                if (iLength % 2 == 1) iLength++;
                for (int i = 0; i < iLength; i += 2)
                {
                    phone += s.Substring(5 + i, 1);
                    phone += s.Substring(4 + i, 1);
                }
                if (phone.EndsWith("F"))
                {
                    phone = phone.Remove(phone.Length - 1, 1);
                }
                s = s.Remove(0, iLength + 6);
                //编码方式
                if (s.Substring(0, 2) == "08")
                    code = GSMCode.UCS2;
                else if (s.Substring(0, 2) == "00")
                    code = GSMCode.Bit7;
                else
                    code = GSMCode.Bit8;
                s = s.Remove(0, 2);
                //时间戳
                sendTime = new DateTime(int.Parse("20" + s.Substring(1, 1) + s.Substring(0, 1)),
                                        int.Parse(s.Substring(3, 1) + s.Substring(2, 1)),
                                        int.Parse(s.Substring(5, 1) + s.Substring(4, 1)),
                                        int.Parse(s.Substring(7, 1) + s.Substring(6, 1)),
                                        int.Parse(s.Substring(9, 1) + s.Substring(8, 1)),
                                        int.Parse(s.Substring(11, 1) + s.Substring(10, 1)));
                s = s.Remove(0, 16);
                //收到的信息
                if (code == GSMCode.Bit7)
                {
                    text = DecodingBit7(s);
                }
                else if (code == GSMCode.UCS2)
                {
                    text = DecodingUCS2(s);
                }
                else if (code == GSMCode.Bit8)
                {
                    text = s;
                }
                else
                {
                    text = s;
                }
                //去掉后面无效字符
                text = text.TrimEnd('\0').TrimEnd('\r');
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 对短信息中心进行编码
        /// </summary>
        /// <param name="s">要编码的号码</param>
        /// <returns>编码后的号码</returns>
        public static string EncodingSCA(string s)
        {
            StringBuilder sb = new StringBuilder();
            if (s.Length == 0)
            {
                sb.Append("00");
                return sb.ToString();
            }
            if (s.StartsWith("+"))
            {
                sb.Append("91"); //用国际格式号码(在前面加‘+’)
                s = s.Remove(0, 1);
            }
            else
            {
                sb.Append("C8");
            }
            if (s.Length % 2 == 1) s += "F";
            for (int i = 0; i < s.Length; i += 2)
            {
                sb.Append(s.Substring(i + 1, 1));
                sb.Append(s.Substring(i, 1));
            }
            string len = (sb.Length / 2).ToString("X2");
            return len + sb.ToString();
        }

        /// <summary>
        /// 对电话号码进行编码
        /// </summary>
        /// <param name="mobileNo">要编码的电话号码</param>
        /// <returns>编码后的电话号码</returns>
        public static string EncodingNumber(string mobileNo)
        {
            StringBuilder sb = new StringBuilder();
            if (mobileNo.StartsWith("+"))
            {
                sb.Append("91");
                mobileNo = mobileNo.Remove(0, 1);
            }
            else
            {
                sb.Append("C8");
            }
            string len = mobileNo.Length.ToString("X2");
            if (mobileNo.Length % 2 == 1) mobileNo += "F";
            for (int i = 0; i < mobileNo.Length; i += 2)
            {
                sb.Append(mobileNo.Substring(i + 1, 1));
                sb.Append(mobileNo.Substring(i, 1));
            }
            return len + sb.ToString();
        }

        /// <summary>
        /// 使用7-bit进行编码
        /// </summary>
        /// <param name="s">要编码的英文字符串</param>
        /// <returns>信息长度及编码后的字符串</returns>
        public static string EncodingBit7(string s)
        {
            int iLeft = 0;
            string sReturn = "";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // 取源字符串的计数值的最低3位
                int iChar = i & 7;
                byte bSrc = (byte)char.Parse(s.Substring(i, 1));
                // 处理源串的每个字节
                if (iChar == 0)
                {
                    // 组内第一个字节，只是保存起来，待处理下一个字节时使用
                    iLeft = (int)char.Parse(s.Substring(i, 1));
                }
                else
                {
                    // 组内其它字节，将其右边部分与残余数据相加，得到一个目标编码字节
                    sReturn = (bSrc << (8 - iChar) | iLeft).ToString("X4");
                    // 将该字节剩下的左边部分，作为残余数据保存起来
                    iLeft = bSrc >> iChar;
                    // 修改目标串的指针和计数值 pDst++;
                    sb.Append(sReturn.Substring(2, 2));
                }
            }
            sb.Append(sReturn.Substring(0, 2));
            return (sb.Length / 2).ToString("X2") + sb.ToString();
        }

        /// <summary>
        /// 对7-bit编码进行解码
        /// </summary>
        /// <param name="s">要解码的字符串</param>
        /// <returns>解码后的英文字符串</returns>
        public static string DecodingBit7(string s)
        {
            int iByte = 0;
            int iLeft = 0;
            // 将源数据每7个字节分为一组，解压缩成8个字节
            // 循环该处理过程，直至源数据被处理完
            // 如果分组不到7字节，也能正确处理
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i += 2)
            {
                byte bSrc = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
                // 将源字节右边部分与残余数据相加，去掉最高位，得到一个目标解码字节
                sb.Append((((bSrc << iByte) | iLeft) & 0x7f).ToString("X2"));
                // 将该字节剩下的左边部分，作为残余数据保存起来
                iLeft = bSrc >> (7 - iByte);
                // 修改字节计数值
                iByte++;
                // 到了一组的最后一个字节
                if (iByte == 7)
                {
                    // 额外得到一个目标解码字节
                    sb.Append(iLeft.ToString("X2"));
                    // 组内字节序号和残余数据初始化
                    iByte = 0;
                    iLeft = 0;
                }
            }
            string sReturn = sb.ToString();
            byte[] buf = new byte[sReturn.Length / 2];
            for (int i = 0; i < sReturn.Length; i += 2)
            {
                buf[i / 2] = byte.Parse(sReturn.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// 使用8-bit进行编码
        /// </summary>
        /// <param name="s">要编码的字符串</param>
        /// <returns>信息长度及编码后的字符串</returns>
        public static string EncodingBit8(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = Encoding.ASCII.GetBytes(s);
            sb.Append(buf.Length.ToString("X2"));
            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 使用8-bit进行解码
        /// </summary>
        /// <param name="s">要解码的字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodingBit8(string s)
        {
            byte[] buf = new byte[s.Length / 2];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i += 2)
            {
                buf[i / 2] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// 中文短信息UCS2编码
        /// 如 你好 转换为 4F60597D
        /// </summary>
        /// <param name="s">要编码的中文字符串</param>
        /// <returns>信息长度及编码后的字符串</returns>
        public static string EncodingUCS2(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = Encoding.Unicode.GetBytes(s);
            sb.Append(buf.Length.ToString("X2"));
            for (int i = 0; i < buf.Length; i += 2)
            {
                sb.Append(buf[i + 1].ToString("X2"));
                sb.Append(buf[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 中文短信息UCS2解码
        /// 如 4F60597D 转换为 你好
        /// </summary>
        /// <param name="s">要解码的信息</param>
        /// <returns>解码后的中文字符串</returns>
        public static string DecodingUCS2(string s)
        {
            byte[] buf = new byte[s.Length];
            for (int i = 0; i < s.Length; i += 4)
            {
                buf[i / 2] = byte.Parse(s.Substring(2 + i, 2), NumberStyles.AllowHexSpecifier);
                buf[i / 2 + 1] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
            }
            return Encoding.Unicode.GetString(buf);
        }

        #endregion




        /// <summary>
        /// 反转二进制数据 如11100000->00000111
        /// </summary>
        /// <param name="secString"></param>
        /// <returns></returns>
        public static string ResvSec(string secString)
        {
            string outString = string.Empty;
            for (int i = secString.Length - 1; i >= 0; i--)
            {
                outString += secString.Substring(i, 1);
            }
            return outString;
        }



    }

    /// <summary>
    /// 编码格式
    /// </summary>
    public enum GSMCode
    {
        Bit7 = 0,

        Bit8 = 1,

        UCS2 = 2,

        Unknown = -1
    }
}
