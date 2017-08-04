/*----------------------------------------------------------------
 * Copyright (C) 2014 
 * 版权所有。
 * 
 * 文件名称：SearchParameter.cs
 * 功能描述：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞 2014-09-29
 * 修改描述：增加无参数的构造函数，以便于序列化
 * 
 * ----------------------------------------------------------------*/

using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;

namespace Bonn.DBUtility
{
    /// <summary>
    /// 查询条件单个参数类
    /// </summary>
    public class OracleExtParameter
    {
        /// <summary>
        /// 增加无参数的构造函数，以便于序列化
        /// </summary>
        public OracleExtParameter()
        {

        }

        /// <summary>
        /// 只初始化参数名和参数值，查询条件为等于(=)
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="collValue"></param>
        public OracleExtParameter(string collName, object collValue)
        {
            //不指定条件时，默认条件为等号
            _collname = collName;
            _condition = "=";
            _collvalue = collValue;

            OracleParameter.ParameterName = collName;
            OracleParameter.Value = collValue;
        }

        /// <summary>
        /// 只初始化参数名和参数值，查询条件为等于(=)
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="collValue"></param>
        /// <param name="oracleType"></param>
        public OracleExtParameter(string collName, object collValue, OracleType oracleType)
        {
            //不指定条件时，默认条件为等号
            _collname = collName;
            _condition = "=";
            _collvalue = collValue;

            OracleParameter.ParameterName = collName;
            OracleParameter.Value = collValue;
            OracleParameter.OracleType = oracleType;
        }

        /// <summary>
        /// 只初始化参数名和参数值，查询条件为等于(=)
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="collValue"></param>
        /// <param name="oracleType"></param>
        /// <param name="size"></param>
        public OracleExtParameter(string collName, object collValue, OracleType oracleType, int size)
        {
            //不指定条件时，默认条件为等号
            _collname = collName;
            _condition = "=";
            _collvalue = collValue;

            OracleParameter.ParameterName = collName;
            OracleParameter.Value = collValue;
            OracleParameter.OracleType = oracleType;
            OracleParameter.Size = size;
        }

        /// <summary>
        /// 只初始化参数名和参数值，查询条件为等于(=)
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="collValue"></param>
        /// <param name="oracleType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public OracleExtParameter(string collName, object collValue, OracleType oracleType, int size, ParameterDirection direction)
        {
            //不指定条件时，默认条件为等号
            _collname = collName;
            _condition = "=";
            _collvalue = collValue;

            OracleParameter.ParameterName = collName;
            OracleParameter.Value = collValue;
            OracleParameter.OracleType = oracleType;
            OracleParameter.Size = size;
            OracleParameter.Direction = direction;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collName">列名，</param>
        /// <param name="condition">查询条件，如 = IN like等</param>
        /// <param name="collValue">值</param>
        public OracleExtParameter(string collName, string condition, object collValue)
        {
            _collname = collName;
            _condition = condition;
            _collvalue = collValue;

            OracleParameter.ParameterName = collName;
            OracleParameter.Value = collValue;
        }

        private string _collname;
        /// <summary>
        /// 参数名称，如@UserCode
        /// </summary>
        public string CollName
        {
            get
            {
                return _collname;
            }
            set
            {
                _collname = value;
                OracleParameter.ParameterName = CollName;
            }
        }
        private string _condition;

        /// <summary>
        /// 查询条件，包含大于，小于，等于，in
        /// </summary>
        public string Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        private OracleParameter _oracleParameter = new OracleParameter();
        /// <summary>
        /// OracleParameter
        /// </summary>
        public OracleParameter OracleParameter
        {
            get { return _oracleParameter; }
            set { _oracleParameter = value; }
        }




        private object _collvalue;

        /// <summary>
        /// 参数值
        /// </summary>
        public object CollValue
        {
            get
            {
                return _collvalue;
            }
            set
            {
                _collvalue = value;
                OracleParameter.Value = value;
            }
        }

    }

    /// <summary>
    /// 查询条件参数集合类
    /// </summary>
    public class ListOracleExtParameter : List<OracleExtParameter>
    {
        /// <summary>
        /// 增加无参数的构造函数，以便于序列化
        /// </summary>
        public ListOracleExtParameter()
        {

        }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="sCollName">查询字段名称 格式如 @MeterId</param>
        /// <param name="sCondition">查询条件 > = </param>
        /// <param name="oCollValue">值</param>
        public void Add(string sCollName, string sCondition, object oCollValue)
        {
            this.Add(new OracleExtParameter(sCollName, sCondition, oCollValue));
        }

        /// <summary>
        /// 添加查询条件，查询条件为等于(=)
        /// </summary>
        /// <param name="sCollName">查询字段名称 格式如 @MeterId</param>
        /// <param name="oCollValue">值</param>
        public void Add(string sCollName, object oCollValue)
        {
            this.Add(new OracleExtParameter(sCollName, oCollValue));
        }

        /// <summary>
        /// 是否包含某参数
        /// </summary>
        /// <param name="para">参数对象</param>
        /// <param name="collName">查询字段名 格式如 @MeterId</param>
        /// <returns></returns>
        public static bool HaveParam(OracleExtParameter para, string collName)
        {
            return para.CollName.Contains(collName);
        }

        /// <summary>
        /// 获取指定列的索引
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public int IndexOf(string collName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].CollName == collName)
                {
                    return i;
                }
            }
            return -1;
        }

        public OracleExtParameter this[string collName]
        {
            get
            {
                return this[IndexOf(collName)];
            }
            set
            {
                this[IndexOf(collName)] = value;
            }
        }
    }
}
