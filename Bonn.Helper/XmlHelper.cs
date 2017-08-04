/*----------------------------------------------------------------
 * Copyright (C) 
 * 版权所有。
 * 
 * 文件名称：XmlHelper.cs
 * 功能描述：xml文件处理助手
 * 
 * 创建标识：朱鹏飞
 * 
 * 
 * ----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace Bonn.Helper
{
    /// <summary>
    /// xml文件处理助手
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 写文件锁
        /// </summary>
        private static object _lockObj = new object();


        #region 序列化

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件，使用默认的UTF8编码格式
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        public static void XmlSerializeToFile(object o, string path)
        {
            XmlSerializeToFile(o, path, Encoding.UTF8);
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Monitor.Enter(_lockObj);//添加排他锁，解决并发写入的问题
            try
            {
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializeInternal(file, o, encoding);
                }
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
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="o"></param>
        /// <param name="encoding"></param>
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string strXml) where T : new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strXml))
                    return new T();

                using (StringReader sr = new StringReader(strXml))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    T testClass = (T)xmlSerializer.Deserialize(sr);
                    return testClass;
                }
            }
            catch (Exception)
            {
                return new T();
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。，使用默认的UTF8编码格式
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path)
        {
            return XmlDeserializeFromFile<T>(path, Encoding.UTF8);
        }

        #endregion 反序列化
    }

}
