using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
   public static class DecimalHelper
    {
       /// <summary>
       /// 将Decimal类型数据转换为用于显示的字符串
       /// 如1.123400转换为 1.1234
       /// </summary>
        /// <param name="decValue"></param>
       /// <returns></returns>
       public static string ToShowString(this decimal decValue)
       {
           return string.Format("{0:0.000000}", decValue).TrimEnd('0').TrimEnd('.');
       }
    }
}
