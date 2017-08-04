/*
 * 
 * 
 * 功能说明：正则表达式验证类
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
using System.Text.RegularExpressions;

namespace Bonn.Helper
{
    public class RegexHelper
    {

        #region 正则表达式

        /// <summary>    
        /// 电子邮件正则表达式    
        /// </summary>    
        public static readonly string EmailRegex = @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$";
        // @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样     

        /// <summary>    
        /// 检测是否有中文字符正则表达式    
        /// </summary>    
        public static readonly string CHZNRegex = "[\u4e00-\u9fa5]";

        /// <summary>    
        /// 检测用户名格式是否有效(只能是汉字、字母、下划线、数字)    
        /// </summary>    
        public static readonly string UserNameRegex = @"^([\u4e00-\u9fa5A-Za-z_0-9]{0,})$";

        /// <summary>    
        /// 密码有效性正则表达式(仅包含字符数字下划线）6~16位    
        /// </summary>    
        public static readonly string PasswordCharNumberRegex = @"^[A-Za-z_0-9]{6,16}$";

        /// <summary>    
        /// 密码有效性正则表达式（纯数字或者纯字母，不通过） 6~16位    
        /// </summary>    
        public static readonly string PasswordRegex = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,16}$";

        /// <summary>    
        /// INT类型数字正则表达式    
        /// </summary>    
        public static readonly string ValidIntRegex = @"^[1-9]\d*\.?[0]*$";

        /// <summary>    
        /// 是否数字正则表达式    
        /// </summary>    
        public static readonly string NumericRegex = @"^[-]?\d+[.]?\d*$";

        /// <summary>    
        /// 是否整数字正则表达式    
        /// </summary>    
        public static readonly string NumberRegex = @"^[0-9]+$";

        /// <summary>    
        /// 是否整数正则表达式（可带带正负号）    
        /// </summary>    
        public static readonly string NumberSignRegex = @"^[+-]?[0-9]+$";

        /// <summary>    
        /// 是否是浮点数正则表达式    
        /// </summary>    
        public static readonly string DecimalRegex = "^[0-9]+[.]?[0-9]+$";

        /// <summary>    
        /// 是否是浮点数正则表达式(可带正负号)    
        /// </summary>    
        public static readonly string DecimalSignRegex = "^[+-]?[0-9]+[.]?[0-9]+$";//等价于^[+-]?\d+[.]?\d+$    

        /// <summary>    
        /// 固定电话正则表达式    
        /// </summary>    
        public static readonly string PhoneRegex = @"^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$";

        /// <summary>    
        /// 移动电话正则表达式   
        /// </summary>    
        public static readonly string MobileRegex = @"^(13|14|15|18)\d{9}$";

        /// <summary>    
        /// 固定电话、移动电话正则表达式    
        /// </summary>    
        public static readonly string PhoneMobileRegex = @"^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$|^(13|15|18)\d{9}$";

        /// <summary>    
        /// 身份证15位正则表达式    
        /// </summary>    
        public static readonly string ID15Regex = @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$";

        /// <summary>    
        /// 身份证18位正则表达式    
        /// </summary>    
        public static readonly string ID18Regex = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[A-Z])$";

        /// <summary>    
        /// URL正则表达式    
        /// </summary>    
        public static readonly string UrlRegex = @"\b(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]*[-A-Za-z0-9+&@#/%=~_|]";

        /// <summary>    
        /// IP正则表达式    
        /// </summary>    
        public static readonly string IPRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        /// <summary>    
        /// Base64编码正则表达式。    
        /// 大小写字母各26个，加上10个数字，和加号“+”，斜杠“/”，一共64个字符，等号“=”用来作为后缀用途    
        /// </summary>    
        public static readonly string Base64Regex = @"[A-Za-z0-9\+\/\=]";

        /// <summary>    
        /// 16进制编码正则表达式。    
        /// 大小写字母A~F、a~f、10个数字 
        /// </summary>    
        public static readonly string HexRegex = @"^[A-Fa-f0-9]+$";

        /// <summary>    
        /// 是否为纯字符的正则表达式    
        /// </summary>    
        public static readonly string LetterRegex = @"^[A-Za-z]+$";

        /// <summary>    
        /// GUID正则表达式    
        /// </summary>    
        public static readonly string GuidRegex = "[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}|[A-F0-9]{32}";

        #endregion


        /// <summary>
        /// 验证是否是IP地址，正确的格式为xxx.xxx.xxx.xxx
        /// </summary>
        /// <param name="strIp"></param>
        /// <returns></returns>
        public static bool isIp(string strIp)
        {
            try
            {
                if (string.IsNullOrEmpty(strIp) == true)
                {
                    return false;
                }
                string[] temp = strIp.Split('.');
                if (temp.Length != 4)
                {
                    return false;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (string.IsNullOrEmpty(temp[i])) return false;

                    if (i == 0)
                    {
                        //IP地址第一个数字不能为0。
                        if (int.Parse(temp[i]) == 0) return false;
                    }

                    if (int.Parse(temp[i]) > 255)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 正则表达式

        /// <summary>
        /// 正整数或零
        /// </summary>
        /// <param name="newParamValue">需要验证的参数值</param>
        /// <param name="strError">错误信息</param>      
        public static bool IsZeroOrInt(string newParamValue)
        {
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(newParamValue) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="newParamValue">需要验证的参数值</param>
        /// <param name="strError">错误信息</param>   
        public static bool IsInt(string newParamValue)
        {
            Regex regex = new Regex(@"^[0-9]*[1-9][0-9]*$");
            if (regex.IsMatch(newParamValue) == false)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 数字
        /// </summary>
        /// <param name="newParamValue">需要验证的参数值</param>
        /// <param name="strError">错误信息</param>   
        public static bool IsNumber(string newParamValue)
        {
            //Regex regex = new Regex(@"^[0-9]*$");
            Regex regex = new Regex(@"^[-+]?[0-9]+$");
            if (regex.IsMatch(newParamValue) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// IP地址
        /// </summary>
        /// <param name="newParamValue">需要验证的参数值</param>
        /// <param name="strError">错误信息</param> 
        public static bool IsIpAddress(string newParamValue)
        {
            //Regex regex = new Regex(@"/(\d+)\.(\d+)\.(\d+)\.(\d+)/g");
            Regex regex = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (regex.IsMatch(newParamValue) == false)
            {
                return false;
            }
            return true;
        }


        /// <summary>    
        /// 手机有效性    
        /// </summary>    
        public static bool IsValidMobile(string mobile)
        {
            Regex regex = new Regex(MobileRegex);
            return regex.IsMatch(mobile);
        }

        #endregion

    }
}
