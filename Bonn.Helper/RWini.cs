/* 
 * 版    权：
 * 版    本：v1.0
 * 功能说明：实现对INI文件的读写操作。
 * 
 * 创建标识：朱鹏飞
 * 
 * 修改标识：朱鹏飞 
 * 修改说明：
 * 
 * 
 * */


using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Bonn.Helper
{
    /// <summary>
    /// 读写INI文件
    /// </summary>
    public class RWini
    {
        /// <summary>
        /// INI文件路径，全路径包含文件名
        /// </summary>
        public string path = string.Empty;

        /// <summary>
        /// 排他锁，防止并发写入
        /// </summary>
        private readonly object _lockObj = new object();

        public RWini(string iniPath)
        {
            path = iniPath;
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteValue(string section, string key, string value)
        {
            //添加排他锁，解决并发写入的问题
            Monitor.Enter(_lockObj);
            try
            {
                WritePrivateProfileString(section, key, value, path);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteValue(string section, string key, int value)
        {
            //添加排他锁，解决并发写入的问题
            Monitor.Enter(_lockObj);
            try
            {
                WritePrivateProfileString(section, key, value.ToString(), path);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteValue(string section, string key, object value)
        {
            //添加排他锁，解决并发写入的问题
            Monitor.Enter(_lockObj);
            try
            {
                WritePrivateProfileString(section, key, value.ToString(), path);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }

        /// <summary>
        /// 读取INI文件指定
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, path);

            return temp.ToString();
        }

        /// <summary>
        /// 读取INI文件指定
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public T ReadValue<T>(string section, string key, T defValue)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, path);

            //如果值为空或者空字符串时，返回默认值
            if (temp.Length == 0 || string.IsNullOrWhiteSpace(temp.ToString()))
                return defValue;

            return (T)(Convert.ChangeType(temp.ToString(), typeof(T)));
        }

        /// <summary>
        /// 读取INI文件指定
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadValue<T>(string section, string key, T defValue, out T value)
        {
            try
            {
                value = ReadValue(section, key, defValue);
                return true;
            }
            catch (Exception)
            {
                value = defValue;
                return false;
            }
        }

        #region API函数声明

        ////声明读写INI文件的API函数 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #endregion  API函数声明
    }
}
