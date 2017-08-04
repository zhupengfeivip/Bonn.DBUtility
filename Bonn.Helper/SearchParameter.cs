/*----------------------------------------------------------------
 * Copyright (C) 2014 
 * 版权所有。
 * 
 * 文件名称：SearchParameter.cs
 * 功能描述：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞
 * 修改描述：
 * 
 * ----------------------------------------------------------------*/

using System.Collections.Generic;
using System.Text;

namespace Bonn.Helper
{
    /// <summary>
    /// 查询条件单个参数类
    /// </summary>
    public class SearchParameter
    {
        /// <summary>
        /// 增加无参数的构造函数，以便于序列化
        /// </summary>
        public SearchParameter()
        {

        }

        /// <summary>
        /// 只初始化参数名和参数值，查询条件为等于(=)
        /// </summary>
        /// <param name="sCollName"></param>
        /// <param name="oCollValue"></param>
        public SearchParameter(string sCollName, object oCollValue)
        {
            //由列表组成参数，前加@，如果前缀没有的话，自动添加
            if (sCollName.StartsWith("@"))
                _collname = sCollName;
            else
                _collname = "@" + sCollName;

            //不指定条件时，默认条件为等号
            _condition = "=";
            _collvalue = oCollValue;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sCollName">列名，</param>
        /// <param name="sCondition">查询条件，如 = IN like等</param>
        /// <param name="oCollValue">值</param>
        public SearchParameter(string sCollName, string sCondition, object oCollValue)
        {
            //由列表组成参数，前加@，如果前缀没有的话，自动添加
            if (sCollName.StartsWith("@"))
                _collname = sCollName;
            else
                _collname = "@" + sCollName;

            _condition = sCondition;
            _collvalue = oCollValue;
        }

        private string _collname;

        /// <summary>
        /// 参数名称，如UserCode，为兼容以前，也可以是@UserCode
        /// </summary>
        public string CollName
        {
            get { return _collname; }
            set { _collname = value; }
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

        private object _collvalue;

        /// <summary>
        /// 参数值
        /// </summary>
        public object CollValue
        {
            get { return _collvalue; }
            set { _collvalue = value; }
        }
    }

    /// <summary>
    /// 查询条件参数集合类
    /// </summary>
    public class ListSearchParameter : List<SearchParameter>
    {
        /// <summary>
        /// 增加无参数的构造函数，以便于序列化
        /// </summary>
        public ListSearchParameter()
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
            this.Add(new SearchParameter(sCollName, sCondition, oCollValue));
        }

        /// <summary>
        /// 添加查询条件，查询条件为等于(=)
        /// </summary>
        /// <param name="sCollName">查询字段名称 格式如 @MeterId</param>
        /// <param name="oCollValue">值</param>
        public void Add(string sCollName, object oCollValue)
        {
            this.Add(new SearchParameter(sCollName, oCollValue));
        }

        /// <summary>
        /// 是否包含某参数
        /// </summary>
        /// <param name="para">参数对象</param>
        /// <param name="collName">查询字段名 格式如 @MeterId</param>
        /// <returns></returns>
        public static bool HaveParam(SearchParameter para, string collName)
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

        public SearchParameter this[string collName]
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

        ///// <summary>
        ///// 将参数对象序列化为json字符串
        ///// </summary>
        ///// <returns>格式：[{"DeptId-001":[{"0","地址X"},{"1","地址Y"},{"2","地址Z"}]}]</returns>
        //public string ToJson()
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    return jsSerializer.Serialize(this);
        //}

        ///// <summary>
        ///// 从json字符串得到参数对象
        ///// </summary>
        ///// <returns></returns>
        //public void GetFormJson(string json)
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    List<SearchParameter> lispParameters = jsSerializer.Deserialize<List<SearchParameter>>(json);
        //    foreach (SearchParameter searchParameter in lispParameters)
        //    {
        //        this.Add(searchParameter);
        //    }
        //}
    }
}
