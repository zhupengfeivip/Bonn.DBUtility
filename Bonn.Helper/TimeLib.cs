using System;
using System.Runtime.InteropServices;

namespace Bonn.Helper
{
    /// <summary>
    /// 日期时间处理组件
    /// </summary>
    public static class TimeLib
    {
        /// <summary>
        /// 
        /// </summary>
        public static DateTime DateTimeOf2000 = new DateTime(2000, 1, 1, 0, 0, 0);

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="strDate">20130919155020</param>
        /// <returns></returns>
        public static DateTime? StrToDateTime(string strDate)
        {
            try
            {
                if (strDate.Length == 14)
                {
                    //yyyyMMddHHmmss
                    DateTime date = new DateTime(int.Parse(strDate.Substring(0, 4)),
                        int.Parse(strDate.Substring(4, 2)), int.Parse(strDate.Substring(6, 2)),
                        int.Parse(strDate.Substring(8, 2)), int.Parse(strDate.Substring(10, 2)),
                        int.Parse(strDate.Substring(12, 2)));
                    return date;
                }
                if (strDate.Length == 12)
                {
                    //yyMMddHHmmss
                    DateTime date = new DateTime(int.Parse("20" + strDate.Substring(0, 2)),
                        int.Parse(strDate.Substring(2, 2)), int.Parse(strDate.Substring(4, 2)),
                        int.Parse(strDate.Substring(6, 2)), int.Parse(strDate.Substring(8, 2)),
                        int.Parse(strDate.Substring(10, 2)));
                    return date;
                }
                if (strDate.Length == 6)
                {
                    //yyMMdd
                    DateTime date = new DateTime(int.Parse("20" + strDate.Substring(0, 2)),
                        int.Parse(strDate.Substring(4, 2)), int.Parse(strDate.Substring(6, 2)));
                    return date;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="formmat"></param>
        /// <param name="outDateTime"></param>
        /// <returns></returns>
        public static bool TryParseDateTime(string strDate, string formmat, out DateTime outDateTime)
        {
            try
            {
                outDateTime = DateTime.MinValue;
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
                                int.Parse(strDate.Substring(4, 2)), int.Parse(strDate.Substring(6, 2)));
                        return true;
                    case "yyyy-MM":
                        string[] strings = strDate.Split('-');
                        outDateTime = new DateTime(int.Parse(strings[0]), int.Parse(strings[1]), 1);
                        return true;
                }

                return false;
            }
            catch
            {
                outDateTime = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// 将日期转换为短日期格式，以方便进行日期比较
        /// 如2012-09-25 09:35:38转换为2012-09-25 00:00:00
        /// </summary>
        /// <param name="dt"></param>
        public static DateTime ToShortDt(DateTime dt)
        {
            return DateTime.Parse(dt.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 转换日期格式，以方便进行日期比较
        /// 如时间为2012-09-25 09:35:38，要求转换格式为yyyy-MM-dd，则转换为2012-09-25 00:00:00
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        public static DateTime Format(DateTime dt, string format)
        {
            string outDt;
            switch (format.ToLower())
            {
                case "yyyy-mm-dd hh:mm:ss":
                    outDt = dt.ToString(format);
                    break;
                case "yyyy-mm-dd hh:mm":
                    outDt = dt.ToString("yyyy-MM-dd HH:mm:00");
                    break;
                case "yyyy-mm-dd hh":
                    outDt = dt.ToString("yyyy-MM-dd HH:00:00");
                    break;
                case "yyyy-mm-dd":
                    outDt = dt.ToString("yyyy-MM-dd 00:00:00");
                    break;
                case "yyyy-mm":
                    outDt = dt.ToString("yyyy-MM-01 00:00:00");
                    break;
                case "yyyy":
                    outDt = dt.ToString("yyyy-01-01 00:00:00");
                    break;
                case "mm:ss":
                    outDt = dt.ToString("2000-01-01 00:mm:ss");
                    break;
                case "hh:mm:ss":
                    outDt = dt.ToString("2000-01-01 HH:mm:ss");
                    break;
                case "hh:mm":
                    outDt = dt.ToString("2000-01-01 HH:mm:00");
                    break;
                default:
                    outDt = dt.ToString(format);
                    break;
            }
            return DateTime.Parse(outDt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgTime"></param>
        /// <returns></returns>
        public static string FormatToLong(this string orgTime)
        {
            DateTime outTime;
            if (DateTime.TryParse(orgTime, out outTime))
            {
                return outTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return orgTime;
            }
        }

        /// <summary>
        /// 判断时间是否在指定的时间段内，包含开始结束时间，在时间段内则返回true，否则返回false
        /// </summary>
        /// <param name="dt">比较时间，要比较的时间值</param>
        /// <param name="beginDt">开始时间</param>
        /// <param name="endDt">结束时间</param>
        /// <param name="format">比较的格式为 yyyy-MM-dd则表示仅比较年月日</param>
        /// <returns></returns>
        public static bool Compare(DateTime dt, DateTime beginDt, DateTime endDt, string format)
        {
            //先格式化三个时间
            DateTime tmpDt = Format(dt, format);
            DateTime tmpBeginDt = Format(beginDt, format);
            DateTime tmpEndDt = Format(endDt, format);
            if (tmpBeginDt <= tmpDt && tmpDt <= tmpEndDt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断时间是否大于等于指定的时间值，比较时间大于等于被比较时间时，返回true
        /// </summary>
        /// <param name="dt">比较时间，前面的时间</param>
        /// <param name="beginDt">被比较时间，后面的时间</param>
        /// <param name="format">比较的日期格式 如yyyy-MM，表示只比较年月</param>
        /// <returns>比较时间小于被比较时间时false，返回，比较时间大于等于被比较时间时，返回true</returns>
        public static bool Compare(DateTime dt, DateTime beginDt, string format)
        {
            //先格式化三个时间
            DateTime tmpDt = Format(dt, format);
            DateTime tmpBeginDt = Format(beginDt, format);
            if (tmpBeginDt <= tmpDt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断比较时间和被比较时间的关系，满足关系时返回true，否则返回false
        /// </summary>
        /// <param name="dt">比较时间，前面的时间</param>
        /// <param name="beginDt">被比较时间，后面的时间</param>
        /// <param name="compareType">比较方式 > 或者 >= 等</param>
        /// <param name="format">比较的日期格式 如yyyy-MM，表示只比较年月</param>
        /// <returns>满足关系时返回true，否则返回false</returns>
        public static bool Compare(DateTime dt, DateTime beginDt, string compareType, string format)
        {
            //先格式化三个时间
            DateTime tmpDt = Format(dt, format);
            DateTime tmpBeginDt = Format(beginDt, format);
            switch (compareType.Trim())
            {
                case ">":
                    return tmpDt > tmpBeginDt;
                case ">=":
                    return tmpDt >= tmpBeginDt;
                case "<":
                    return tmpDt < tmpBeginDt;
                case "<=":
                    return tmpDt <= tmpBeginDt;
                case "=":
                    return tmpDt == tmpBeginDt;
                default:
                    throw new NotImplementedException(string.Format("不支持当前比较类型[{0}]", compareType));
            }
        }

        /// <summary>
        /// 根据星期值返回汉字星期几
        /// </summary>
        /// <param name="weekValue">星期值0~6</param>
        /// <returns>星期一~星期日</returns>
        public static string GetChinesWeek(int weekValue)
        {

            switch (weekValue)
            {
                case 0:
                    return "星期日";
                case 1:
                    return "星期一";
                case 2:
                    return "星期二";
                case 3:
                    return "星期三";
                case 4:
                    return "星期四";
                case 5:
                    return "星期五";
                case 6:
                    return "星期六";
                default:
                    return weekValue.ToString();
            }
        }


        /// <summary>
        /// datetime转换为unixtime
        /// 将时间类型转换为整形
        /// </summary>
        /// <returns></returns>
        public static int DateTimeToInt()
        {
            return DateTimeToInt(DateTime.Now);
        }

        /// <summary>
        /// datetime转换为unixtime
        /// 将时间类型转换为整形
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int DateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// datetime转换为unixtime
        /// 将时间类型转换为整形
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime IntToDateTime(int value)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfThisWeek()
        {
            DateTime dt = DateTime.Now;  //当前时间 
            return dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfThisMonth()
        {
            DateTime dt = DateTime.Now;  //当前时间 
            return dt.AddDays(1 - dt.Day);  //本月月初
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfThisQuarter()
        {
            DateTime dt = DateTime.Now;  //当前时间 
            return dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day);  //本季度初
        }

        #region 设置本地时间

        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref Systemtime time);

        [StructLayout(LayoutKind.Sequential)]
        private struct Systemtime
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        /// <summary>
        /// 设置本地时间
        /// </summary>
        /// <param name="dt"></param>
        public static void SetDate(DateTime dt)
        {
            Systemtime st;

            st.year = (short)dt.Year;
            st.month = (short)dt.Month;
            st.dayOfWeek = (short)dt.DayOfWeek;
            st.day = (short)dt.Day;
            st.hour = (short)dt.Hour;
            st.minute = (short)dt.Minute;
            st.second = (short)dt.Second;
            st.milliseconds = (short)dt.Millisecond;

            SetLocalTime(ref st);
        }

        #endregion
    }
}
