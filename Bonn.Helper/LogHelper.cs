/*
 * 
 * 
 * 功能说明：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞 2012-03-08
 * 修改说明：
 * 
 * 修改标识：朱鹏飞 2012-07-17
 * 修改说明：增加日志文件名称，是否显示日志等参数，以支持不同的需求，但仍然兼容以前版本。
 * 
 * 修改标识：朱鹏飞 2013-07-16
 * 修改说明：将日志路径等参数设置为公开属性，以增加灵活性
 * 
 * */
using System;
using System.IO;
using System.Threading;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogHelper
    {

        private static string _logPath = Environment.CurrentDirectory + "\\Log";

        /// <summary>
        /// 日志文件路径，不包含日志文件名
        /// </summary>
        public static string logPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(System.Configuration.ConfigurationSettings.AppSettings["LogPath"]) == false)
                {
                    return System.Configuration.ConfigurationSettings.AppSettings["LogPath"];
                }

                return _logPath;
            }
            set
            {
                _logPath = value;
            }
        }


        /// <summary>
        /// 日志文件名称。
        /// </summary>
        public static string LogFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

        /// <summary>
        /// 日志记录中是否包含日期，如果包含，则日志内容前会有年月日时分秒，默认为显示时间
        /// <para>显示时间样式为：2012-07-17 11:44:28   日志内容</para>
        /// </summary>
        public static bool LogTime = true;

        /// <summary>
        /// 最大文件字节数
        /// </summary>
        public static long MaxFileLength = 2 * 1024 * 1024;

        /// <summary>
        /// 排他锁，防止并发写入
        /// </summary>
        public static object lockObj = new object();

        /// <summary>
        /// 验证日志文件目录是否存在
        /// </summary>
        /// <returns>存在返回true，不存在或者异常时，返回false</returns>
        public static bool CheckDir(string path)
        {
            try
            {
                return System.IO.Directory.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证日志文件目录是否存在
        /// </summary>
        /// <returns>存在返回true，不存在或者异常时，返回false</returns>
        public static bool CheckAndCreateDir(string path)
        {
            try
            {

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 记录文本信息到日志文件中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strMsg">文件消息内容</param>
        /// <param name="path"></param>
        /// <param name="logTime"></param>
        public static void WriteTxt(string path, string fileName, bool logTime, string strMsg)
        {
            //添加排他锁，解决并发写入的问题
            Monitor.Enter(lockObj);
            try
            {
                //目录不存在或者出现异常，则返回
                if (!CheckAndCreateDir(path))
                {
                    return;
                }
                //如果日志文件大小超过了指定最大值，则转存为新的文件
                FileInfo fi = new FileInfo(path + "\\" + fileName);
                if (fi.Exists == true && fi.Length >= MaxFileLength)
                {
                    fi.MoveTo(path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_long_" + fileName);
                }

                string logContent = strMsg;
                if (logTime == true)
                {
                    logContent = string.Format("{0}   {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strMsg);
                }

                using (StreamWriter sw = File.AppendText(path + "\\" + fileName))
                {
                    sw.WriteLine(logContent);
                }
            }
            catch
            {
                return;
            }
            finally
            {
                Monitor.Exit(lockObj);
            }
        }

        /// <summary>
        /// 记录文本信息到日志文件中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strMsg">文件消息内容</param>
        /// <param name="path"></param>
        public static void WriteTxt(string path, string fileName, string strMsg)
        {
            WriteTxt(path, fileName, LogTime, strMsg);
        }

        /// <summary>
        /// 记录文本信息到日志文件中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strMsg">文件消息内容</param>
        public static void WriteTxt(string fileName, string strMsg)
        {
            WriteTxt(logPath, fileName, LogTime, strMsg);
        }

        /// <summary>
        /// 记录文本信息到日志文件中
        /// </summary>
        /// <param name="strMsg">文件消息内容</param>
        public static void WriteTxt(string strMsg)
        {
            WriteTxt(logPath, LogFileName, LogTime, strMsg);
        }

        /// <summary>
        /// 记录异常消息内容到文件日志
        /// </summary>
        /// <param name="ex">异常消息内容</param>
        public static void WriteTxt(System.Exception ex)
        {
            WriteTxt(ex.ToString());
        }

        /// <summary>
        /// 记录异常消息内容到文件日志
        /// </summary>
        /// <param name="ex">异常消息内容</param>
        public static void WriteErrorTxt(System.Exception ex)
        {
            WriteTxt(ex.ToString());
        }
    }
}
