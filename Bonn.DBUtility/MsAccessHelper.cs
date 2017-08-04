/*----------------------------------------------------------------
 * Copyright (C) 2014 
 * 版权所有。
 * 
 * 文件名称：MsAccessHelper.cs
 * 功能描述：
 * 
 * 创建标识：朱鹏飞 2014-07-12 11:14:50
 * 
 * 修改标识：
 * 修改描述：
 * 
 * ----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Security;
using System.Text;

namespace Bonn.DBUtility
{
    public static class MsAccessHelper
    {
        /// <summary>
        /// 将.NET中的数据类型转换为access中的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string ConvertDbType(Type type)
        {
            if (type == typeof(String))
            {
                return "varchar";
            }
            if (type == typeof(System.Int16))
            {
                return "NUMBER";
            }
            if (type == typeof(System.Int32))
            {
                return "NUMBER";
            }
            if (type == typeof(System.Int64))
            {
                return "NUMBER";
            }
            if (type == typeof(System.DateTime))
            {
                return "DateTime";
            }
            return type.Name;
        }

        public static string dbPath;



        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetConnString(string filePath)
        {
            return $"Provider=Microsoft.Jet.OLEDB.4.0 ;Data Source={filePath}";
        }

        public static OleDbConnection getConn(string filePath)
        {
            string connstr = $"Provider=Microsoft.Jet.OLEDB.4.0 ;Data Source={filePath}";
            OleDbConnection tempconn = new OleDbConnection(connstr);
            return tempconn;
        }

        public static OleDbConnection getConn()
        {
            return getConn(dbPath);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dataTable"></param>
        public static void CreateTable(string filePath, DataTable dataTable)
        {
            //创建表
            using (OleDbConnection dbconn = new OleDbConnection(GetConnString(filePath)))
            {
                dbconn.Open();
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("CREATE TABLE {0}(", string.IsNullOrWhiteSpace(dataTable.TableName) ? "MdbTable" : dataTable.TableName);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    strSql.Append(string.Format("{0} {1},", dataTable.Columns[i].ColumnName, ConvertDbType(dataTable.Columns[i].DataType)));
                }
                strSql.Remove(strSql.Length - 1, 1);
                strSql.Append(")");
                OleDbCommand dbCommand = new OleDbCommand(strSql.ToString(), dbconn);
                dbCommand.ExecuteNonQuery();
                dbconn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strSql)
        {
            OleDbConnection conn = getConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();
                //定义command对象，并执行相应的SQL语句
                OleDbCommand myCommand = new OleDbCommand(strSql, conn);
                return myCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw (new Exception("数据库出错:" + e.Message));
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataSet Query(string strSql)
        {
            DataSet mydataset; //定义DataSet
            try
            {
                OleDbConnection conn = getConn(); //getConn():得到连接对象
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                mydataset = new DataSet();
                adapter.SelectCommand = new OleDbCommand(strSql, conn);
                adapter.Fill(mydataset, "notes");
                conn.Close();
            }
            catch (Exception e)
            {
                throw (new Exception("数据库出错:" + e.Message));
            }
            return mydataset;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dataTable"></param>
        public static void InsertData(string filePath, DataTable dataTable)
        {
            //添加数据
            using (OleDbConnection dbconn = new OleDbConnection(GetConnString(filePath)))
            {
                dbconn.Open();
                StringBuilder strColumnSql = new StringBuilder();
                strColumnSql.AppendFormat("INSERT INTO {0}(", string.IsNullOrWhiteSpace(dataTable.TableName) ? "MdbTable" : dataTable.TableName);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    strColumnSql.Append(string.Format("[{0}],", dataTable.Columns[i].ColumnName));
                }
                strColumnSql.Remove(strColumnSql.Length - 1, 1);
                strColumnSql.Append(") VALUES(");

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    StringBuilder strValueSql = new StringBuilder();
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        strValueSql.Append(string.Format("'{0}',", dataTable.Rows[i][j]));
                    }
                    strValueSql.Remove(strValueSql.Length - 1, 1);
                    strValueSql.Append(")");

                    string strSql = strColumnSql.ToString() + strValueSql;
                    OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                    dbCommand.ExecuteNonQuery();
                }

                dbconn.Close();
            }
        }


        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <returns></returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict)
        {
            using (OleDbConnection dbconn = getConn())
            {
                dbconn.Open();
                StringBuilder strColumnSql = new StringBuilder();
                strColumnSql.AppendFormat("INSERT INTO {0}(", strTableName);
                foreach (KeyValuePair<string, object> kpr in dict)
                {
                    strColumnSql.Append($"[{kpr.Key}],");
                }
                strColumnSql.Remove(strColumnSql.Length - 1, 1);
                strColumnSql.Append(") VALUES(");

                foreach (KeyValuePair<string, object> kpr in dict)
                {
                    StringBuilder strValueSql = new StringBuilder();
                    strValueSql.Append($"'{kpr.Value.ToString().ClearErrorCode()}',");
                    strValueSql.Remove(strValueSql.Length - 1, 1);
                    strValueSql.Append(")");

                    string strSql = strColumnSql.ToString() + strValueSql;
                    OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                    return dbCommand.ExecuteNonQuery();
                }
                dbconn.Close();
            }
            return 0;
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict, out int id)
        {
            id = -1;
            int dbResult = 0;
            using (OleDbConnection dbconn = getConn())
            {
                dbconn.Open();
                StringBuilder strColumnSql = new StringBuilder();
                strColumnSql.AppendFormat("INSERT INTO [{0}](", strTableName);
                foreach (KeyValuePair<string, object> kpr in dict)
                {
                    strColumnSql.Append($"[{kpr.Key}],");
                }
                strColumnSql.Remove(strColumnSql.Length - 1, 1);
                strColumnSql.Append(") VALUES(");

                StringBuilder strValueSql = new StringBuilder();
                foreach (KeyValuePair<string, object> kpr in dict)
                {
                    strValueSql.Append($"'{kpr.Value.ToString().ClearErrorCode()}',");
                }
                strValueSql.Remove(strValueSql.Length - 1, 1);
                strValueSql.Append(")");

                string strSql = strColumnSql.ToString() + strValueSql;
                OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                dbResult = dbCommand.ExecuteNonQuery();
                if (dbResult <= 0)
                {
                    return -1;
                }

                dbCommand.CommandText = "select @@identity as id";
                dbCommand.Parameters.Clear();
                id = Convert.ToInt32(dbCommand.ExecuteScalar());

                dbconn.Close();
            }
            return dbResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strWhere"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static int Update(string tableName, string strWhere, Dictionary<string, object> dict)
        {
            string strSql = $"UPDATE {tableName} SET ";
            foreach (KeyValuePair<string, object> keyValuePair in dict)
            {
                if (keyValuePair.Value is int)
                    strSql += $"[{keyValuePair.Key}] = {keyValuePair.Value}, ";
                else if (keyValuePair.Value is string)
                    strSql += $"[{keyValuePair.Key}] = '{keyValuePair.Value.ToString().ClearErrorCode()}', ";
                else
                    strSql += $"[{keyValuePair.Key}] = '{keyValuePair.Value.ToString().ClearErrorCode()}', ";
            }
            strSql = strSql.Trim();
            strSql = strSql.Remove(strSql.Length - 1, 1);
            strSql += strWhere;
            return ExecuteNonQuery(strSql);
        }

        /// <summary>
        /// 创建表并添加数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dataTable"></param>
        public static void CreateInsertData(string filePath, DataTable dataTable)
        {
            //创建表
            CreateTable(filePath, dataTable);

            //添加数据
            InsertData(filePath, dataTable);
        }

        private static string ClearErrorCode(this string strSql)
        {
            strSql = strSql.Replace("'", " ");
            strSql = strSql.Replace("''", " ");
            strSql = strSql.Replace("\"", " ");
            return strSql;
        }
    }
}
