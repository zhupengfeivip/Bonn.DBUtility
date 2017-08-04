/*
 * 
 * 
 * 功能说明：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞
 * 修改说明：
 * 
 * */
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Management;
namespace Bonn.Helper
{
    /// <summary>
    /// 安全相关函数
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">未加密的原始字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string MD5_old(string str)
        {
            if (str == null) return "";
            string ret = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            return ret;
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">未加密的原始字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string MD5(string str)
        {
            if (str == null) return "";
            return MD5_old(str).Substring(0, 20);
        }

        /// <summary>
        /// 加解密算法-异或取反
        /// <para>加解密为同一方法，再次加密即为解密</para>
        /// </summary>
        /// <param name="orgString">要加密的字符串，明码</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Xor(string orgString, string key)
        {
            string result = string.Empty;
            for (int i = 0; i < orgString.Length / 2; i++)
            {
                byte bValue = Convert.ToByte(orgString.Substring(i * 2, 2), 16);
                //原始字符串的每个字节与密钥所有字节异或
                for (int j = 0; j < key.Length / 2; j++)
                {
                    //按位异或
                    bValue ^= Convert.ToByte(key.Substring(j * 2, 2), 16);
                    //求反
                    bValue ^= 0xFF;
                }
                result += Convert.ToString(bValue, 16).ToUpper().PadLeft(2, '0');
            }
            return result;
        }

        /// <summary>
        /// 获取客户端请求的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                {
                    return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
            }
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            string cpuInfo = "";
            ManagementClass cimobject = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo.Properties["ProcessorId"].Value != null && string.IsNullOrEmpty(mo.Properties["ProcessorId"].Value.ToString()) == false)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        public static string GetDiskID()
        {
            string diskID = "";
            ManagementClass cimobject = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (var o in moc)
            {
                var mo = o as ManagementObject;
                if (mo != null && (mo.Properties["Model"].Value != null && string.IsNullOrEmpty(mo.Properties["Model"].Value.ToString()) == false))
                {
                    diskID = mo.Properties["Model"].Value.ToString();
                    break;
                }
            }
            return diskID;
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        public static string GetDiskNumber()
        {
            string diskID = "";
            ManagementClass cimobject = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (var o in moc)
            {
                var mo = o as ManagementObject;
                if (mo != null && (mo.Properties["SerialNumber"].Value != null && string.IsNullOrEmpty(mo.Properties["SerialNumber"].Value.ToString()) == false))
                {
                    diskID = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
            }
            return diskID;
        }

        /// <summary>
        /// 获取主板序列号
        /// </summary>
        /// <returns></returns>
        public static string GetBaseBoardNumber()
        {
            string baseBoardNumber = "";
            ManagementClass cimobject = new ManagementClass("Win32_BaseBoard");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (var o in moc)
            {
                var mo = o as ManagementObject;
                if (mo != null && (mo.Properties["SerialNumber"].Value != null && string.IsNullOrEmpty(mo.Properties["SerialNumber"].Value.ToString()) == false))
                {
                    baseBoardNumber = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
            }
            return baseBoardNumber;
        }

        /// <summary>
        /// 获取网卡MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress()
        {
            string macAddress = "";
            ManagementClass cimobject = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (var o in moc)
            {
                var mo = o as ManagementObject;
                if (mo == null) continue;
                if ((bool)mo["IPEnabled"] == true)
                {
                    if (mo.Properties["MacAddress"].Value != null && string.IsNullOrEmpty(mo.Properties["MacAddress"].Value.ToString()) == false)
                    {
                        macAddress = mo.Properties["MacAddress"].Value.ToString();
                        return macAddress;
                    }
                }
            }
            return macAddress;
        }

        /// <summary>
        /// 获取BIOS编号。
        /// 注意并非所有的主板都有BIOS编号，也并非所有的主板都可以获取到编号。
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSid()
        {
            string bioSid = "";
            ManagementClass cimobject = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (var o in moc)
            {
                var mo = o as ManagementObject;
                if (mo != null && (mo.Properties["SerialNumber"].Value != null && string.IsNullOrEmpty(mo.Properties["SerialNumber"].Value.ToString()) == false))
                {
                    bioSid = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
            }
            return bioSid;
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <returns></returns>
        public static int GetRound()
        {
            System.Random rdm = new System.Random();
            int temp = rdm.Next(int.MaxValue);
            return temp;
        }

        /// <summary>
        /// 获取主机名
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }
    }
}
