/*
 *  版权所有
 * 
 * 功能说明：语言类，用于加载和管理语言信息
 * 
 * 创建标识：朱鹏飞
 * 
 * 修改标识：
 * 修改说明：
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace Bonn.Helper
{
    /// <summary>
    /// 语言类
    /// </summary>
    public static class Language
    {
        /// <summary>
        /// 字典表
        /// </summary>
        private static Hashtable htLanguage = new Hashtable();

        /// <summary>
        /// 字典数量
        /// </summary>
        public static int Count
        {
            get
            {
                return Language.htLanguage.Count;
            }
        }

        /// <summary>
        /// 根据语言类型加载语言文件
        /// </summary>
        /// <param name="strXmlFilePath">
        /// 语言文件的绝对路径（不包含语言文件名）
        /// 文件夹中的语言文件命名必须为：CNMessages.xml或ENMessages.xml
        /// </param>
        /// <param name="strLanguage">语言类型 CN中文 EN英文</param>
        /// <returns>语言字典表</returns>
        public static void Init(string strXmlFilePath, string strLanguage)
        {
            string strFileName = strXmlFilePath;
            XmlDocument xDoc = new XmlDocument();
            if (!string.Empty.Equals(strLanguage))
            {
                strFileName += strLanguage + "Messages.xml";
                if (System.IO.File.Exists(strFileName) == false)
                {
                    throw new Exception("未找到语言文件。");
                }
                xDoc.Load(strFileName);
            }
            //先清空字典表
            htLanguage.Clear();

            if (xDoc.DocumentElement != null)
            {
                XmlNodeList messageNodeList = xDoc.DocumentElement.SelectNodes("Message");
                if (messageNodeList == null) return;
                foreach (XmlNode messageNode in messageNodeList)
                {
                    XmlNodeList xmlNodeList = messageNode.SelectNodes("ID");
                    if (xmlNodeList == null) continue;
                    string strMessageId = xmlNodeList[0].InnerText;

                    XmlNodeList selectNodes = messageNode.SelectNodes("Value");
                    if (selectNodes == null) continue;
                    string strMessageContent = selectNodes[0].InnerText;

                    if (htLanguage.Contains(strMessageId) == false)
                    {
                        htLanguage.Add(strMessageId, strMessageContent);
                    }
                }
            }
            //加载行业语言文件
            strFileName = strXmlFilePath;
            strFileName += strLanguage + "_IndustryMessages.xml";
            if (System.IO.File.Exists(strFileName) == false)
            {
                return;
            }
            xDoc.Load(strFileName);
            if (xDoc.DocumentElement != null)
            {
                XmlNodeList messageNodeList = xDoc.DocumentElement.SelectNodes("Message");
                if (messageNodeList == null) return;
                foreach (XmlNode messageNode in messageNodeList)
                {
                    XmlNodeList xmlNodeList = messageNode.SelectNodes("ID");
                    if (xmlNodeList == null) continue;
                    string strMessageId = xmlNodeList[0].InnerText;

                    XmlNodeList selectNodes = messageNode.SelectNodes("Value");
                    if (selectNodes == null) continue;
                    string strMessageContent = selectNodes[0].InnerText;

                    if (htLanguage.Contains(strMessageId) == false)
                    {
                        htLanguage.Add(strMessageId, strMessageContent);
                    }
                }
            }
        }

        /// <summary>
        /// 根据ID获取Value信息
        /// </summary>
        /// <param name="strID">字典ID</param>
        /// <returns></returns>
        public static string Get(string strID)
        {
            try
            {
                string value;
                if (htLanguage.ContainsKey(strID))
                {
                    value = htLanguage[strID].ToString();
                }
                else
                {
                    value = string.Format("未知代码:[{0}]", strID);
                }
                return value;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据ID获取Value信息
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <returns></returns>
        public static string Get(int id)
        {
            return Get(id.ToString());
        }

    }
}
