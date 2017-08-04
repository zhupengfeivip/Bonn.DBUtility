/*
 *  版权所有
 * 
 * 功能说明：通用数据库转换助手
 * 
 * 创建标识：朱鹏飞
 * 
 * 修改标识：朱鹏飞
 * 修改说明：增加GetMdlByDr(),ToVale()方法
 * 
 * 
 * */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Bonn.Helper
{
    /// <summary>
    /// DataHelper 对齐并补全DataTable中数据的类
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// 对齐数据表中的数据，使其能以一个字段一一对齐
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="alignField">对齐字段</param>
        /// <param name="keyField">分组字段</param>
        /// <param name="fillField">缺省行需要填充的字段</param>
        /// <returns></returns>
        public static DataTable AlignData(DataTable dt, string alignField, string keyField, string fillField)
        {
            return AlignData(dt, alignField, keyField, new[] { fillField });
        }

        /// <summary>
        /// 对齐数据表中的数据，使其能以一个字段一一对齐
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="alignField">对齐字段</param>
        /// <param name="keyField">分组字段</param>
        /// <param name="fillField">缺省行需要填充的字段</param>
        /// <returns></returns>
        public static DataTable AlignData(DataTable dt, string alignField, string keyField, string[] fillField)
        {
            try
            {
                ArrayList alignFieldData = new ArrayList();
                ArrayList keyFieldData = new ArrayList();
                //循环添加对齐字段数据值和分组字段的数值
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    string tempData = dt.Rows[n][alignField].ToString();
                    if (!alignFieldData.Contains(tempData))
                    {
                        alignFieldData.Add(tempData);
                    }

                    tempData = dt.Rows[n][keyField].ToString();
                    if (!keyFieldData.Contains(tempData))
                    {
                        keyFieldData.Add(tempData);
                    }
                }
                //循环对比并补齐数据表中的数据
                for (int keyIndex = 0; keyIndex < keyFieldData.Count; keyIndex++)
                {
                    //筛选分组数据
                    DataRow[] drArray = dt.Select(keyField + "='" + keyFieldData[keyIndex] + "'", alignField + " asc");
                    //查询分组数据缺省值
                    ArrayList tempArray = (ArrayList)alignFieldData.Clone();
                    for (int j = 0; j < drArray.Length; j++)
                    {
                        string temp = drArray[j][alignField].ToString();
                        tempArray.Remove(temp);
                    }
                    //缺省值数量不为0时
                    if (tempArray.Count != 0)
                    {
                        //循环插入缺省值
                        for (int n = 0; n < tempArray.Count; n++)
                        {
                            //循环DataRow中的索引位置
                            int drIndex = 0;
                            for (; drIndex < drArray.Length; drIndex++)
                            {
                                string strAlignField = drArray[drIndex][alignField].ToString();

                                if (tempArray[n].ToString().CompareTo(strAlignField) < 0)
                                {
                                    int insertIndex = dt.Rows.IndexOf(drArray[drIndex]);
                                    dt.Rows.InsertAt(dt.NewRow(), insertIndex);
                                    dt.Rows[insertIndex][alignField] = tempArray[n];
                                    dt.Rows[insertIndex][keyField] = keyFieldData[keyIndex];
                                    foreach (string s in fillField)
                                    {
                                        dt.Rows[insertIndex][s] = 0;
                                    }
                                    break;
                                }
                            }
                            if (drIndex == drArray.Length)
                            {
                                int insertIndex = dt.Rows.IndexOf(drArray[drIndex - 1]);
                                insertIndex += 1;
                                dt.Rows.InsertAt(dt.NewRow(), insertIndex);
                                dt.Rows[insertIndex][alignField] = tempArray[n];
                                dt.Rows[insertIndex][keyField] = keyFieldData[keyIndex];
                                foreach (string s in fillField)
                                {
                                    dt.Rows[insertIndex][s] = 0;
                                }
                            }
                            drArray = dt.Select(keyField + "='" + keyFieldData[keyIndex] + "'", alignField + " asc");
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 对齐多个数据表中的数据，使其能以一个字段一一对齐
        /// </summary>
        /// <param name="tables">数据表</param>
        /// <param name="alignField">对齐字段</param>
        /// <param name="fillField">缺省行需要填充的字段</param>
        /// <returns></returns>
        public static DataTableCollection AlignData(DataTableCollection tables, string alignField, string fillField)
        {
            try
            {
                ArrayList alignFieldData = new ArrayList();
                int tablesCount = tables.Count;
                //循环添加对齐字段数据值
                for (int i = 0; i < tablesCount; i++)
                {
                    for (int n = 0; n < tables[i].Rows.Count; n++)
                    {
                        string tempData = tables[i].Rows[n][alignField].ToString();
                        if (!alignFieldData.Contains(tempData))
                        {
                            alignFieldData.Add(tempData);
                        }
                    }
                }
                //循环对比并补齐数据表中的数据
                for (int i = 0; i < tablesCount; i++)
                {
                    int tablesRowsCount = tables[i].Rows.Count;
                    ArrayList tempArray = (ArrayList)alignFieldData.Clone();
                    for (int j = 0; j < tablesRowsCount; j++)
                    {
                        string temp = tables[i].Rows[j][alignField].ToString();
                        tempArray.Remove(temp);
                    }
                    if (tempArray.Count != 0)
                    {
                        for (int n = 0; n < tempArray.Count; n++)
                        {
                            for (int c = 0; c < tablesRowsCount; c++)
                            {
                                string yearDate = tables[i].Rows[c][alignField].ToString();

                                string rowFirstDate = tables[i].Rows[0][alignField].ToString();
                                string rowLastDate = tables[i].Rows[tablesRowsCount - 1][alignField].ToString();
                                //小于开始日期
                                rowFirstDate = rowFirstDate.Length > 5 ? rowFirstDate.Substring(5) : rowFirstDate;
                                if (tempArray[n].ToString().CompareTo(rowFirstDate) < 0)
                                {
                                    tables[i].Rows.InsertAt(tables[i].NewRow(), 0);
                                    tables[i].Rows[0][alignField] = tempArray[n];
                                    tables[i].Rows[0][fillField] = 0;
                                    tablesRowsCount++;
                                    break;
                                }
                                //大于结束日期
                                else if (tempArray[n].ToString().CompareTo(rowLastDate) > 0)
                                {
                                    tables[i].Rows.Add(tables[i].NewRow());
                                    tables[i].Rows[tablesRowsCount][alignField] = tempArray[n];
                                    tables[i].Rows[tablesRowsCount][fillField] = 0;
                                    tablesRowsCount++;
                                    break;
                                }
                                else
                                {
                                    if (tempArray[n].ToString().CompareTo(yearDate) < 0)
                                    {
                                        tables[i].Rows.InsertAt(tables[i].NewRow(), c);
                                        tables[i].Rows[c][alignField] = tempArray[n];
                                        tables[i].Rows[c][fillField] = 0;
                                        tablesRowsCount++;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return tables;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 对齐多个数据表中的数据，使其能以一个字段一一对齐
        /// </summary>
        /// <param name="tables">数据表(表名是年份)</param>
        /// <param name="alignField">对齐字段(对齐字段需要是日期,格式：2012-09-19)</param>
        /// <param name="fillField">缺省行需要填充的字段</param>
        /// <param name="isMonth">月对比，不区分年</param>
        /// <returns></returns>
        public static DataTableCollection AlignData(DataTableCollection tables, string alignField, string fillField, bool isMonth)
        {
            try
            {
                if (!isMonth)
                {
                    return AlignData(tables, alignField, fillField);
                }
                ArrayList alignFieldData = new ArrayList();
                int tablesCount = tables.Count;
                //循环添加对齐字段数据值
                for (int i = 0; i < tablesCount; i++)
                {
                    for (int n = 0; n < tables[i].Rows.Count; n++)
                    {
                        string tempData = tables[i].Rows[n][alignField].ToString().Substring(5);
                        if (!alignFieldData.Contains(tempData))
                        {
                            alignFieldData.Add(tempData);
                        }
                    }
                }
                //循环对比并补齐数据表中的数据
                for (int i = 0; i < tablesCount; i++)
                {
                    int tablesRowsCount = tables[i].Rows.Count;
                    ArrayList tempArray = (ArrayList)alignFieldData.Clone();
                    for (int j = 0; j < tablesRowsCount; j++)
                    {
                        string temp = tables[i].Rows[j][alignField].ToString().Substring(5);
                        tempArray.Remove(temp);
                    }
                    if (tempArray.Count != 0)
                    {
                        for (int n = 0; n < tempArray.Count; n++)
                        {
                            for (int c = 0; c < tablesRowsCount; c++)
                            {
                                string yearDate = tables[i].Rows[c][alignField].ToString();
                                string date = yearDate.Substring(5);

                                string rowFirstDate = tables[i].Rows[0][alignField].ToString();
                                string rowLastDate = tables[i].Rows[tablesRowsCount - 1][alignField].ToString();
                                //大于开始日期，大于结束日期
                                if (tempArray[n].ToString().CompareTo(rowFirstDate.Substring(5)) > 0 &&
                                    tempArray[n].ToString().CompareTo(rowLastDate.Substring(5)) > 0)
                                {
                                    if (tempArray[n].ToString().CompareTo(date) < 0 || date.Substring(0, 2) == "01")
                                    {
                                        tempArray[n] = tables[i].Rows[c - 1][alignField].ToString().Substring(0, 5) + tempArray[n];

                                        tables[i].Rows.InsertAt(tables[i].NewRow(), c);
                                        tables[i].Rows[c][alignField] = tempArray[n];
                                        tables[i].Rows[c][fillField] = 0;
                                        tablesRowsCount++;
                                        break;
                                    }
                                }
                                //小于开始日期，小于结束日期
                                else if (tempArray[n].ToString().CompareTo(rowFirstDate.Substring(5)) < 0 &&
                                    tempArray[n].ToString().CompareTo(rowLastDate.Substring(5)) < 0 &&
                                         date.Substring(0, 2).CompareTo("07") < 0)
                                {
                                    if (tempArray[n].ToString().CompareTo(date) < 0)
                                    {
                                        tempArray[n] = tables[i].Rows[c - 1][alignField].ToString().Substring(0, 5) + tempArray[n];

                                        tables[i].Rows.InsertAt(tables[i].NewRow(), c);
                                        tables[i].Rows[c][alignField] = tempArray[n];
                                        tables[i].Rows[c][fillField] = 0;
                                        tablesRowsCount++;
                                        break;
                                    }
                                }
                                //小于开始日期，大于结束日期，大于七月份
                                else if (tempArray[n].ToString().CompareTo(rowFirstDate.Substring(5)) < 0 &&
                                         tempArray[n].ToString().CompareTo(rowLastDate.Substring(5)) > 0 &&
                                         tempArray[n].ToString().Substring(0, 2).CompareTo("07") > 0)
                                {
                                    tempArray[n] = rowFirstDate.Substring(0, 5) + tempArray[n];

                                    tables[i].Rows.InsertAt(tables[i].NewRow(), 0);
                                    tables[i].Rows[0][alignField] = tempArray[n];
                                    tables[i].Rows[0][fillField] = 0;
                                    tablesRowsCount++;
                                    break;
                                }
                                //小于开始日期，大于结束日期,小于等于七月份
                                else if (tempArray[n].ToString().CompareTo(rowFirstDate.Substring(5)) < 0 &&
                                         tempArray[n].ToString().CompareTo(rowLastDate.Substring(5)) > 0 &&
                                         tempArray[n].ToString().Substring(0, 2).CompareTo("07") <= 0)
                                {
                                    tempArray[n] = rowLastDate.Substring(0, 5) + tempArray[n];

                                    tables[i].Rows.Add(tables[i].NewRow());
                                    tables[i].Rows[tablesRowsCount][alignField] = tempArray[n];
                                    tables[i].Rows[tablesRowsCount][fillField] = 0;
                                    tablesRowsCount++;
                                    break;
                                }

                            }
                        }
                    }
                }
                return tables;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 插入排序(字符串排序)
        /// </summary>
        /// <param name="list">排序数组</param>
        /// <returns></returns>
        private static ArrayList sort(ArrayList list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                string t = list[i].ToString();
                int j = i;
                while ((j > 0) && (list[j - 1].ToString().CompareTo(t) > 0))
                {
                    list[j] = list[j - 1];
                    --j;
                }
                list[j] = t;
            }
            return list;
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static DataTable DataFormat(DataTable dataSource, string[] columnName, byte decimalPlaces)
        {
            //小数点长度为6时，直接返回
            if (decimalPlaces == 6) return dataSource;
            List<string> nameList = new List<string>();
            foreach (string name in columnName)
            {
                nameList.Add(name);
            }
            return DataFormat(dataSource, nameList, decimalPlaces);
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static DataTable DataFormat(DataTable dataSource, string[] columnName, object decimalPlaces)
        {
            byte declPlances = Convert.ToByte(decimalPlaces ?? 2);
            if (declPlances == 6) return dataSource;  //小数点长度为6时，直接返回

            List<string> nameList = new List<string>();
            foreach (string name in columnName)
                nameList.Add(name);

            return DataFormat(dataSource, nameList, declPlances);
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static DataSet DataFormat(DataSet dataSource, string[] columnName, object decimalPlaces)
        {
            byte declPlances = Convert.ToByte(decimalPlaces ?? 2);
            if (declPlances == 6) return dataSource;  //小数点长度为6时，直接返回
            List<string> nameList = new List<string>();
            foreach (string name in columnName)
            {
                nameList.Add(name);
            }
            DataTable dataTable = DataFormat(dataSource.Tables[0], nameList, declPlances);
            return dataTable.DataSet;
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static void DataFormat(ref DataSet dataSource, string[] columnName, object decimalPlaces)
        {
            byte declPlances = Convert.ToByte(decimalPlaces ?? 2);
            if (declPlances == 6) return;  //小数点长度为6时，直接返回
            List<string> nameList = new List<string>();
            foreach (string name in columnName)
            {
                nameList.Add(name);
            }
            DataTable dataTable = DataFormat(dataSource.Tables[0], nameList, declPlances);
            dataSource = dataTable.DataSet;
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static void DataFormat(ref DataTable dataSource, string[] columnName, object decimalPlaces)
        {
            byte declPlances = Convert.ToByte(decimalPlaces ?? 2);
            if (declPlances == 6) return;  //小数点长度为6时，直接返回
            List<string> nameList = new List<string>();
            foreach (string name in columnName)
            {
                nameList.Add(name);
            }
            dataSource = DataFormat(dataSource, nameList, declPlances);
        }

        /// <summary>
        /// 格式化数据表中的数据（处理小数位数）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明列表</param>
        /// <param name="decimalPlaces">小数点位数</param>
        /// <returns></returns>
        public static DataTable DataFormat(DataTable dataSource, List<string> columnName, byte decimalPlaces)
        {
            if (decimalPlaces == 6) return dataSource;  //小数点长度为6时，直接返回

            string formatString;
            if (decimalPlaces == 0)
                formatString = "{0:0}";
            else
                formatString = "{0:F" + decimalPlaces + "}";

            return DataFormat(dataSource, columnName, formatString);
        }

        /// <summary>
        /// 格式化数据表中的数据
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明数组</param>
        /// <param name="formatString">格式化字符串 如：{0:0.00}、{0:0.##}、{0:N2}、{0:F4}</param>
        /// <returns></returns>
        public static DataTable DataFormat(DataTable dataSource, string[] columnName, string formatString)
        {
            List<string> nameList = new List<string>();
            foreach (string name in columnName)
                nameList.Add(name);

            return DataFormat(dataSource, nameList, formatString);
        }

        /// <summary>
        /// 格式化数据表中的数据
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columnName">列明列表</param>
        /// <param name="formatString">格式化字符串 如：{0:0.00}、{0:0.##}、{0:N2}、{0:F4}</param>
        /// <returns></returns>
        public static DataTable DataFormat(DataTable dataSource, List<string> columnName, string formatString)
        {
            foreach (DataRow dr in dataSource.Rows)
            {
                foreach (string cName in columnName)
                {
                    try
                    {
                        decimal dNum;
                        bool isDecimal = decimal.TryParse(dr[cName] != DBNull.Value ? dr[cName].ToString() : "", out dNum);
                        if (isDecimal)
                        {
                            dr[cName] = string.Format(formatString, dNum);
                        }
                        else
                        {
                            if (dr[cName] != DBNull.Value)
                                dr[cName] = string.Format(formatString, dr[cName]);
                        }
                    }
                    catch (Exception ex)
                    {
                        //直接抛出异常，便于开发时发现错误
                        throw new Exception("格式化字符串不正确或数据表不包含该列，详细：" + ex);
                    }
                }
            }
            return dataSource;
        }

        /// <summary>
        /// 将数据库字段转换为相应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static string ToString(object obj, string defValue)
        {
            if (obj != DBNull.Value)
            {
                return obj.ToString();
            }
            return defValue;
        }

        /// <summary>
        /// 将数据库字段转换为相应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int ToInt(object obj, int defValue)
        {
            if (obj != DBNull.Value && string.IsNullOrWhiteSpace(obj.ToString()) == false)
            {
                return int.Parse(obj.ToString());
            }
            return defValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object obj, decimal defValue)
        {
            if (obj != DBNull.Value && string.IsNullOrWhiteSpace(obj.ToString()) == false)
            {
                return decimal.Parse(obj.ToString());
            }
            return defValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(object obj, DateTime defValue)
        {
            if (obj != DBNull.Value && string.IsNullOrWhiteSpace(obj.ToString()) == false)
            {
                return DateTime.Parse(obj.ToString());
            }
            return defValue;
        }

        /// <summary>
        /// 将数据库数据对象转换为实体值对象
        /// </summary>
        /// <typeparam name="T">要转换的类型 如int、string等</typeparam>
        /// <param name="dr">数据行</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static T ToValue<T>(DataRow dr, string columnName)
        {
            T result = default(T);
            if (dr.Table.Columns.Contains(columnName) == true)
            {
                if (dr[columnName] != DBNull.Value && string.IsNullOrWhiteSpace(dr[columnName].ToString()) == false)
                {
                    return (T)(Convert.ChangeType(dr[columnName], typeof(T)));
                }
            }
            return result;
        }

        /// <summary>
        /// 将数据库数据对象转换为实体值对象
        /// </summary>
        /// <typeparam name="T">要转换的类型 如int、string等</typeparam>
        /// <param name="dr">数据行</param>
        /// <param name="columnName">列名</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static T ToValue<T>(DataRow dr, string columnName, T defValue)
        {
            if (dr.Table.Columns.Contains(columnName) == true)
            {
                if (dr[columnName] != DBNull.Value && string.IsNullOrWhiteSpace(dr[columnName].ToString()) == false)
                {
                    return (T)(Convert.ChangeType(dr[columnName], typeof(T)));
                }
            }
            return defValue;
        }

        /// <summary>
        /// 将数据行转换为实体类型，要求列名和实体名必须一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mdl"></param>
        /// <param name="dr"></param>
        public static void GetMdlByDr<T>(ref T mdl, DataRow dr)
        {
            Type t = mdl.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                string name = pi.Name.ToLower();
                if (dr.Table.Columns.Contains(name) == false) continue;

                if (dr[name] != null && dr[name] != DBNull.Value && string.IsNullOrWhiteSpace(dr[name].ToString()) == false)
                {
                    if (pi.PropertyType == typeof(int?))
                    {
                        pi.SetValue(mdl, Convert.ChangeType(dr[name], typeof(int)), null);
                    }
                    else if (pi.PropertyType == typeof(decimal?))
                    {
                        pi.SetValue(mdl, Convert.ChangeType(dr[name], typeof(decimal)), null);
                    }
                    else if (pi.PropertyType == typeof(DateTime?))
                    {
                        pi.SetValue(mdl, Convert.ChangeType(dr[name], typeof(DateTime)), null);
                    }
                    else
                    {
                        pi.SetValue(mdl, Convert.ChangeType(dr[name], pi.PropertyType), null);
                    }
                }
            }
        }

        /// <summary>
        /// 将数据集的第一行转换为实体类型，忽略其他数据行，要求列名和实体名必须一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mdl"></param>
        /// <param name="dt"></param>
        public static void GetMdlByDt<T>(ref T mdl, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("无数据");

            GetMdlByDr(ref mdl, dt.Rows[0]);
        }

        /// <summary>
        /// 将数据集的第一行转换为实体类型，忽略其他数据行，要求列名和实体名必须一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mdl"></param>
        /// <param name="ds"></param>
        public static void GetMdlByDs<T>(ref T mdl, DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                throw new Exception("无数据");

            GetMdlByDr(ref mdl, ds.Tables[0].Rows[0]);
        }

        /// <summary>
        /// 将数据行转换为实体类型，要求列名和实体名必须一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ds"></param>
        public static List<T> GetMdlListByDs<T>(DataSet ds) where T : new()
        {
            if (ds == null || ds.Tables.Count <= 0)
                throw new Exception("数据集为空");

            return GetMdlListByDt<T>(ds.Tables[0]);
        }
        /// <summary>
        /// 将数据行转换为实体类型，要求列名和实体名必须一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        public static List<T> GetMdlListByDt<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T mdl = new T();
                Type t = mdl.GetType();
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    string name = pi.Name.ToLower();
                    if (dt.Columns.Contains(name) == false) continue;

                    if (dt.Rows[i][name] != null && dt.Rows[i][name] != DBNull.Value)
                    {
                        if (pi.PropertyType == typeof(int?))
                        {
                            if (dt.Rows[i][name] == null || dt.Rows[i][name] == DBNull.Value || string.IsNullOrWhiteSpace(dt.Rows[i][name].ToString()))
                                pi.SetValue(mdl, null, null);
                            else
                                pi.SetValue(mdl, Convert.ChangeType(dt.Rows[i][name], typeof(int)), null);
                        }
                        else if (pi.PropertyType == typeof(decimal?))
                        {
                            if (dt.Rows[i][name] == null || dt.Rows[i][name] == DBNull.Value || string.IsNullOrWhiteSpace(dt.Rows[i][name].ToString()))
                                pi.SetValue(mdl, null, null);
                            else
                                pi.SetValue(mdl, Convert.ChangeType(dt.Rows[i][name], typeof(decimal)), null);
                        }
                        else if (pi.PropertyType == typeof(DateTime?))
                        {
                            if (dt.Rows[i][name] == null || dt.Rows[i][name] == DBNull.Value || string.IsNullOrWhiteSpace(dt.Rows[i][name].ToString()))
                                pi.SetValue(mdl, null, null);
                            else
                                pi.SetValue(mdl, Convert.ChangeType(dt.Rows[i][name], typeof(DateTime)), null);
                        }
                        else
                        {
                            pi.SetValue(mdl, Convert.ChangeType(dt.Rows[i][name], pi.PropertyType), null);
                        }
                    }
                }
                list.Add(mdl);
            }
            return list;
        }

        /// <summary>
        /// 复制实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mdl"></param>
        /// <param name="newMdl"></param>
        public static void ModelClone<T>(T mdl, ref T newMdl)
        {
            Type t = mdl.GetType();
            Type newT = newMdl.GetType();

            foreach (PropertyInfo pi in t.GetProperties())
            {
                string name = pi.Name.ToLower();
                object value = pi.GetValue(name, null);
                foreach (PropertyInfo newPi in newT.GetProperties())
                {
                    if (name == newPi.Name.ToLower())
                        newPi.SetValue(newMdl, value, null);
                }

            }
        }

        /// <summary>
        /// 将实体对象转换为字典表，方便插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mdl"></param>
        /// <returns></returns>
        public static Dictionary<string, object> MdlToDict<T>(T mdl)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Type t = mdl.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                //如果属性为空，则不添加
                object value = pi.GetValue(mdl, null);
                if (value == null)
                {
                    continue;
                }
                dict.Add("@" + pi.Name, value);
            }
            return dict;
        }


        /// <summary>
        /// 水资源监控管理系统数据传输规约中指定数据帧的校验采用CRC校验，生成多项式为：
        /// X^7+X^6+X^5+X^2+1
        /// 基本传输顺序为低位在前，高位在后，所以需要采用反转CRC算法
        /// </summary>
        /// <param name="userData">控制域C、地址域、应用功能码、用户数据4部分</param>
        /// <returns></returns>
        public static byte GetVerifyCrc76520Byte(byte[] userData)
        {
            const byte encpyRev = 0xA7;//10100111
            int uiMaxBit;
            int uiRsb;
            byte crc_reg;

            int uiMesIndex;
            int uiLsb;
            int len1;
            byte i;
            byte crcreturn = 0;
            len1 = userData.Length;
            uiMaxBit = (len1 * 8) - 1;
            uiRsb = 0;
            i = userData[0];
            crc_reg = Convert.ToByte(userData[0] ^ encpyRev);
            while (uiRsb < uiMaxBit)
            {
                if (Convert.ToBoolean(crc_reg & 0x01))
                {
                    crc_reg = Convert.ToByte(crc_reg ^ encpyRev);
                }
                else
                {
                    crc_reg = Convert.ToByte(crc_reg >> 1);
                    uiMesIndex = uiRsb / 8 + 1;
                    uiLsb = uiRsb % 8;
                    if (uiMesIndex < userData.Length)
                    {
                        i = userData[uiMesIndex];
                        if (Convert.ToBoolean(userData[uiMesIndex] & (0x01 << uiLsb)))
                        {
                            crc_reg = Convert.ToByte(crc_reg | 0x80);
                        }
                    }
                    uiRsb++;
                }
            }
            //将crc从发送顺序转变为正常数据表示顺序，即最高位和最低位依次调换
            for (i = 0; i < 7; i++)
            {
                if (Convert.ToBoolean(crc_reg & Convert.ToByte(0x01 << i)))
                {
                    crcreturn = Convert.ToByte(crcreturn | (0x01 << (7 - i)));
                }
            }
            return crcreturn;
        }





    }
}
