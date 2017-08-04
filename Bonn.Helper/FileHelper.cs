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

using System;
using System.Collections.Generic;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 验证文件目录是否存在
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
        /// 验证文件目录是否存在，不存在则创建相应目录
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
        /// <param name="strMsg">文件消息内容</param>
        /// <param name="fileFullPath"></param>
        public static void WriteTxt2File(string strMsg, string fileFullPath)
        {
            try
            {
                //目录已存在，则先删除
                if (System.IO.File.Exists(fileFullPath) == true)
                {
                    System.IO.File.Move(fileFullPath, fileFullPath + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
                System.IO.StreamWriter objErrorText = System.IO.File.AppendText(fileFullPath);
                objErrorText.Write(strMsg);
                objErrorText.Flush();
                objErrorText.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 读取文本文件内容
        /// </summary>
        /// <param name="fileFullPath">文本文件全路径，包含文件名、后缀名。</param>
        /// <returns></returns>
        public static string ReadTxtFile(string fileFullPath)
        {
            try
            {
                
                if (System.IO.File.Exists(fileFullPath) == false)
                {
                    throw new Exception("文件不存在。");
                }
                return System.IO.File.ReadAllText(fileFullPath);
            }
            catch
            {
                throw;
            }
        }
    }
}
