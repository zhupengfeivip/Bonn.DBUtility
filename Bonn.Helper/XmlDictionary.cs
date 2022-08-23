using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace Bonn.Helper
{
    /// <summary>  
    /// 标题：支持 XML 序列化的 Dictionary  
    /// </summary>  
    /// <typeparam name="TKey"></typeparam>  
    /// <typeparam name="TValue"></typeparam>  
    [XmlRoot("XmlDictionary")]
    [Serializable]
    public class XmlDictionary<TKey, TValue>
      : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region 构造函数


        public XmlDictionary()
            : base()
        {
        }


        public XmlDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }


        public XmlDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }


        public XmlDictionary(int capacity)
            : base(capacity)
        {
        }


        public XmlDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }





        #endregion 构造函数


        #region IXmlSerializable Members


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }


        /// <summary>  
        /// 从对象的 XML 表示形式生成该对象  
        /// </summary>  
        /// <param name="reader"></param>  
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }


        /**/


        /// <summary>  
        /// 将对象转换为其 XML 表示形式  
        /// </summary>  
        /// <param name="writer"></param>  
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }


        #endregion IXmlSerializable Members
    }
}