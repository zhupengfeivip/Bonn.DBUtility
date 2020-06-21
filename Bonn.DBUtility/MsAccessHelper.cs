/*----------------------------------------------------------------
* Copyright (C) 2014 
* 版权所有。
* 
* 文件名称：MsAccessHelper.cs
* 功能描述：
* 
* 创建标识：朱鹏飞 2017-8-7 10:00:00
* 
* 修改标识：
* 修改描述：
* 
* ----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net.Sockets;
using System.Text;

namespace Bonn.DBUtility
{
    /// <summary>
    /// 
    /// </summary>
    public static class MsAccessHelper
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static int DbType = 2;


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

        /// <summary>
        /// 数据库名称
        /// </summary>
        public static string DbPath = "mydb.accdb";

        /// <summary>
        /// 是否加密
        /// </summary>
        public static bool Encryp = false;


        /// <summary>
        /// 
        /// </summary>
        public static string Pwd = "111";


        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetConnString(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("未设置数据库连接字符串");

            if (DbType == 1)
            {
                return $"Provider=Microsoft.Jet.OLEDB.4.0 ;Data Source={filePath}";
            }

            if (Encryp)
            {
                return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Jet OLEDB:Database Password=111";//Persist Security Info=False
                //return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Jet OLEDB:Database Password={Pwd};";
            }
            else
            {
                return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static OleDbConnection GetConn(string filePath)
        {
            string connstr = GetConnString(filePath);
            OleDbConnection tempconn = new OleDbConnection(connstr);
            return tempconn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static OleDbConnection GetConn()
        {
            return GetConn(DbPath);
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
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strSql, OleDbTransaction trans = null)
        {
            OleDbConnection conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();
                //定义command对象，并执行相应的SQL语句
                OleDbCommand myCommand = new OleDbCommand(strSql, conn);
                if (trans != null)
                    myCommand.Transaction = trans;
                return myCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        ///   执行查询，并返回由查询返回的结果集中的第一行的第一列。
        ///    其他列或行将被忽略。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static object GetSingle(string strSql)
        {
            return ExecuteScalar(strSql);
        }

        /// <summary>
        ///   执行查询，并返回由查询返回的结果集中的第一行的第一列。
        ///    其他列或行将被忽略。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="trans"></param>
        /// <returns>在结果中的第一行的第一列设置，或为 null 引用的结果集是否为空。</returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///   无法从最初在其中登记连接上下文执行不同的事务上下文中的命令。
        /// </exception>
        public static object ExecuteScalar(string strSql, OleDbTransaction trans = null)
        {
            OleDbConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                //定义command对象，并执行相应的SQL语句
                OleDbCommand myCommand = new OleDbCommand(strSql, conn);
                if (trans != null)
                    myCommand.Transaction = trans;
                return myCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (trans == null) conn.Close();
            }
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int GetCount(string strSql, OleDbTransaction trans = null)
        {
            return Convert.ToInt32(ExecuteScalar(strSql, trans));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idName"></param>
        /// <param name="tableName"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int GetMaxId(string idName, string tableName, OleDbTransaction trans = null)
        {
            OleDbConnection conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();
                string strSql = $"SELECT MAX({idName}) FROM {tableName}";
                //定义command对象，并执行相应的SQL语句
                OleDbCommand myCommand = new OleDbCommand(strSql, conn);
                if (trans != null)
                    myCommand.Transaction = trans;
                Object obj = myCommand.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToInt32(obj);

                return 1;
            }
            catch (Exception)
            {
                throw;
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
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, OleDbParameter[] parameters = null, OleDbTransaction trans = null)
        {
            OleDbConnection conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();
                //定义command对象，并执行相应的SQL语句
                OleDbCommand myCommand = new OleDbCommand(strSql, conn);
                if (parameters != null)
                    myCommand.Parameters.AddRange(parameters);
                if (trans != null)
                    myCommand.Transaction = trans;
                Object obj = myCommand.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToInt32(obj) > 0;

                return false;
            }
            catch (Exception)
            {
                throw;
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
        public static DataSet Query(StringBuilder strSql)
        {
            return Query(strSql.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static DataSet Query(string strSql, OleDbParameter[] parameters, OleDbTransaction trans = null)
        {
            DataSet mydataset; //定义DataSet
            try
            {
                OleDbConnection conn = GetConn(); //getConn():得到连接对象
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                mydataset = new DataSet();
                adapter.SelectCommand = new OleDbCommand(strSql, conn);
                if (parameters != null)
                    adapter.SelectCommand.Parameters.AddRange(parameters);
                if (trans != null)
                    adapter.SelectCommand.Transaction = trans;
                adapter.Fill(mydataset, "mydata");
                adapter.Dispose();
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return mydataset;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet Query(string strSql, List<OleDbParameter> parameters = null, OleDbTransaction trans = null)
        {
            DataSet mydataset; //定义DataSet
            try
            {
                OleDbConnection conn = GetConn(); //getConn():得到连接对象
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                mydataset = new DataSet();
                adapter.SelectCommand = new OleDbCommand(strSql, conn);
                if (parameters != null)
                    adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());
                if (trans != null)
                    adapter.SelectCommand.Transaction = trans;
                adapter.Fill(mydataset, "mydata");
                adapter.SelectCommand.Dispose();
                adapter.Dispose();
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return mydataset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        public static int ExecuteSql(string strSql, OleDbTransaction trans = null)
        {
            //添加数据
            using (OleDbConnection dbconn = GetConn())
            {
                dbconn.Open();
                OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                if (trans != null)
                    dbCommand.Transaction = trans;
                int rows = dbCommand.ExecuteNonQuery();

                dbconn.Close();
                return rows;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        public static int ExecuteSql(StringBuilder strSql, OleDbTransaction trans = null)
        {
            return ExecuteSql(strSql.ToString(), trans);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="trans"></param>
        public static int ExecuteSql(string strSql, OleDbParameter[] parameters, OleDbTransaction trans = null)
        {
            //添加数据
            using (OleDbConnection dbconn = GetConn())
            {
                dbconn.Open();
                OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                if (trans != null)
                    dbCommand.Transaction = trans;
                dbCommand.Parameters.AddRange(parameters);
                int rows = dbCommand.ExecuteNonQuery();

                dbconn.Close();
                return rows;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="trans"></param>
        public static int ExecuteSql(string strSql, List<OleDbParameter> parameters, OleDbTransaction trans = null)
        {
            //添加数据
            using (OleDbConnection dbconn = GetConn())
            {
                dbconn.Open();
                OleDbCommand dbCommand = new OleDbCommand(strSql, dbconn);
                if (trans != null)
                    dbCommand.Transaction = trans;
                dbCommand.Parameters.AddRange(parameters.ToArray());
                int rows = dbCommand.ExecuteNonQuery();

                dbconn.Close();
                return rows;
            }
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeInsert(string strTableName, Dictionary<string, object> dict, OleDbTransaction trans = null)
        {
            OleDbConnection dbconn;
            if (trans != null)
                dbconn = trans.Connection;
            else
                dbconn = GetConn();

            if (dbconn.State != ConnectionState.Open) dbconn.Open();
            StringBuilder strColumnSql = new StringBuilder();
            strColumnSql.AppendFormat("INSERT INTO {0}(", strTableName);

            OleDbCommand dbCommand = new OleDbCommand();
            if (trans != null)
                dbCommand.Transaction = trans;

            StringBuilder strParameter = new StringBuilder();
            int paraIndex = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                if (parameter == null) continue;
                dbCommand.Parameters.Add(parameter);

                strColumnSql.Append($"[{kpr.Key}],");

                strParameter.Append("@Para" + paraIndex + " ,");
            }
            strParameter.Remove(strParameter.Length - 1, 1);
            strParameter.Append(" ) ");

            strColumnSql.Remove(strColumnSql.Length - 1, 1);
            strColumnSql.Append(") VALUES(");

            dbCommand.Connection = dbconn;
            dbCommand.CommandText = strColumnSql.ToString() + strParameter;
            int rows = dbCommand.ExecuteNonQuery();
            if (trans == null) dbconn.Close();
            return rows;
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeInsert(string strTableName, Dictionary<string, object> dict, out int id, OleDbTransaction trans = null)
        {
            OleDbConnection dbconn;
            if (trans != null)
                dbconn = trans.Connection;
            else
                dbconn = GetConn();

            if (dbconn.State != ConnectionState.Open) dbconn.Open();
            StringBuilder strColumnSql = new StringBuilder();
            strColumnSql.AppendFormat("INSERT INTO {0}(", strTableName);

            OleDbCommand dbCommand = new OleDbCommand();
            if (trans != null)
                dbCommand.Transaction = trans;

            StringBuilder strParameter = new StringBuilder();
            int paraIndex = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                if (parameter == null) continue;
                dbCommand.Parameters.Add(parameter);

                strColumnSql.Append($"[{kpr.Key}],");

                strParameter.Append("@Para" + paraIndex + " ,");
            }
            strParameter.Remove(strParameter.Length - 1, 1);
            strParameter.Append(" ) ");

            strColumnSql.Remove(strColumnSql.Length - 1, 1);
            strColumnSql.Append(") VALUES(");

            dbCommand.Connection = dbconn;
            dbCommand.CommandText = strColumnSql.ToString() + strParameter;
            int dbResult = dbCommand.ExecuteNonQuery();

            dbCommand.CommandText = "SELECT @@identity AS id";
            dbCommand.Parameters.Clear();
            id = Convert.ToInt32(dbCommand.ExecuteScalar());

            if (trans == null) dbconn.Close();

            return dbResult;
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict, OleDbTransaction trans = null)
        {
            using (OleDbConnection dbconn = GetConn())
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
                    if (trans != null)
                        dbCommand.Transaction = trans;
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
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict, out int id, OleDbTransaction trans = null)
        {
            id = -1;
            int dbResult = 0;
            using (OleDbConnection dbconn = GetConn())
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
                if (trans != null)
                    dbCommand.Transaction = trans;
                dbResult = dbCommand.ExecuteNonQuery();
                if (dbResult <= 0)
                {
                    return -1;
                }

                dbCommand.CommandText = "SELECT @@identity AS id";
                dbCommand.Parameters.Clear();
                id = Convert.ToInt32(dbCommand.ExecuteScalar());

                if (trans == null) dbconn.Close();
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeUpdate(string tableName, Dictionary<string, object> dict, Dictionary<string, object> para = null, OleDbTransaction trans = null)
        {
            OleDbConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                OleDbCommand dbCommand = new OleDbCommand();
                dbCommand.Connection = conn;
                if (trans != null)
                    dbCommand.Transaction = trans;

                string strSql = $"UPDATE {tableName} SET ";
                int paraIndex = 0;
                foreach (KeyValuePair<string, object> kpr in dict)
                {
                    if (kpr.Value == null) continue;
                    if (kpr.Value == DBNull.Value) continue;

                    string colName;
                    string op;
                    GetColAndOp(kpr.Key, out colName, out op);
                    if (op == "+=" || op == "=+")
                        strSql += $"[{colName}] = [{colName}] + @Para{paraIndex}, ";
                    else if (op == "*=" || op == "=*")
                        strSql += $"[{colName}] = [{colName}] * @Para{paraIndex}, ";
                    else if (op == "-=")
                        strSql += $"[{colName}] = @Para{paraIndex} - [{colName}], ";
                    else if (op == "=-")
                        strSql += $"[{colName}] = [{colName}] - @Para{paraIndex}, ";
                    else
                        strSql += $"[{colName}] {op} @Para{paraIndex}, ";
                    OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                    dbCommand.Parameters.Add(parameter);

                    paraIndex++;
                }
                strSql = strSql.Trim();
                strSql = strSql.Remove(strSql.Length - 1, 1);

                if (para != null)
                {
                    strSql += " WHERE 1 = 1 ";
                    foreach (KeyValuePair<string, object> kpr in para)
                    {
                        if (kpr.Value == null) continue;
                        if (kpr.Value == DBNull.Value) continue;

                        string colName;
                        string op;
                        GetColAndOp(kpr.Key, out colName, out op);
                        strSql += $"AND [{colName}] {op} @Para{paraIndex} ";
                        OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                        dbCommand.Parameters.Add(parameter);

                        paraIndex++;
                    }
                }

                dbCommand.CommandText = strSql;
                return dbCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (trans == null) conn.Close();
            }
        }

        /// <summary>
        /// 根据字符串获取查询条件信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="colName"></param>
        /// <param name="op"></param>
        public static void GetColAndOp(string key, out string colName, out string op)
        {
            op = "=";
            colName = key;
            if (key.StartsWith(">=") ||
                    key.StartsWith("<=") ||
                    key.StartsWith("+=") ||
                    key.StartsWith("=+") ||
                    key.StartsWith("-=") ||
                    key.StartsWith("=-") ||
                    key.StartsWith("*=") ||
                    key.StartsWith("=*") ||
                    key.StartsWith("=/") ||
                    key.StartsWith("/="))
            {
                op = key.Substring(0, 2);
                colName = key.Substring(2, key.Length - 2);
            }
            else if (key.StartsWith("=") || key.StartsWith(">") || key.StartsWith("<"))
            {
                op = key.Substring(0, 1);
                colName = key.Substring(1, key.Length - 1);
            }
            else if (key.StartsWith("?"))
            {
                op = "LIKE";
                colName = key.Substring(1, key.Length - 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="parameters"></param>
        /// <param name="sql">sql必须是where 1=1 结尾，或者必须是有where条件的</param>
        public static void BuildWhereSqlByDict(Dictionary<string, object> para, ref StringBuilder sql, out List<OleDbParameter> parameters)
        {
            int paraIndex = 0;
            parameters = new List<OleDbParameter>();
            if (para == null) return;
            sql.Append(" ");
            foreach (KeyValuePair<string, object> kpr in para)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                string colName;
                string op;
                GetColAndOp(kpr.Key, out colName, out op);

                sql.Append($"AND [{colName}] {op} @Para{paraIndex} ");

                OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                parameters.Add(parameter);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeDelete(string tableName, Dictionary<string, object> para = null, OleDbTransaction trans = null)
        {
            OleDbConnection conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();

                OleDbCommand dbCommand = new OleDbCommand();
                dbCommand.Connection = conn;
                if (trans != null)
                    dbCommand.Transaction = trans;

                string strSql = $"DELETE FROM {tableName} ";
                int paraIndex = 0;
                //strSql = strSql.Trim();
                //strSql = strSql.Remove(strSql.Length - 1, 1);

                if (para != null)
                {
                    strSql += "WHERE 1 = 1 ";
                    foreach (KeyValuePair<string, object> kpr in para)
                    {
                        if (kpr.Value == null) continue;
                        if (kpr.Value == DBNull.Value) continue;

                        strSql += $"AND [{kpr.Key}] = @Para{paraIndex} ";
                        OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                        dbCommand.Parameters.Add(parameter);

                        paraIndex++;
                    }
                }

                dbCommand.CommandText = strSql;
                return dbCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
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
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private static string ClearErrorCode(this string strSql)
        {
            strSql = strSql.Replace("'", " ");
            strSql = strSql.Replace("''", " ");
            strSql = strSql.Replace("\"", " ");
            return strSql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kpr"></param>
        /// <returns></returns>
        public static OleDbParameter DictValueToParam(KeyValuePair<string, object> kpr)
        {
            if (kpr.Value == null) return null;
            if (kpr.Value == DBNull.Value) return null;

            OleDbParameter parameter = new OleDbParameter();
            parameter.ParameterName = "@" + kpr.Key;
            if (kpr.Value is int)
            {
                parameter.OleDbType = OleDbType.Integer;
            }
            else if (kpr.Value is string)
            {
                parameter.OleDbType = OleDbType.VarChar;
                parameter.Size = kpr.Value.ToString().Length;
            }
            else if (kpr.Value is DateTime)
            {
                parameter.OleDbType = OleDbType.Date;
            }
            else if (kpr.Value is decimal)
            {
                parameter.OleDbType = OleDbType.Decimal;
            }
            else if (kpr.Value is bool)
            {
                parameter.OleDbType = OleDbType.Boolean;
            }
            parameter.Value = kpr.Value;
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kpr"></param>
        /// <param name="paraIndex"></param>
        /// <returns></returns>
        public static OleDbParameter DictValueToParam(KeyValuePair<string, object> kpr, ref int paraIndex)
        {
            OleDbParameter para = DictValueToParam(kpr);
            para.ParameterName = "@Para" + paraIndex;
            paraIndex++;
            return para;
        }


        /// <summary>
        /// 根据条件查询已存在的重复数据行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int GetExistedRows(string tableName, Dictionary<string, object> para = null, OleDbTransaction trans = null)
        {
            OleDbConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                OleDbCommand dbCommand = new OleDbCommand();
                dbCommand.Connection = conn;
                if (trans != null)
                    dbCommand.Transaction = trans;

                string strSql = $"SELECT COUNT(*) FROM {tableName} ";
                int paraIndex = 0;
                if (para != null)
                {
                    strSql += " WHERE 1 = 1 ";
                    foreach (KeyValuePair<string, object> kpr in para)
                    {
                        if (kpr.Value == null) continue;
                        if (kpr.Value == DBNull.Value) continue;

                        string colName;
                        string op;
                        GetColAndOp(kpr.Key, out colName, out op);
                        strSql += $"AND [{colName}] {op} @Para{paraIndex} ";
                        OleDbParameter parameter = DictValueToParam(kpr, ref paraIndex);
                        dbCommand.Parameters.Add(parameter);

                        paraIndex++;
                    }
                }

                dbCommand.CommandText = strSql;
                return Convert.ToInt32(dbCommand.ExecuteScalar().ToString());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (trans == null) conn.Close();
            }
        }
    }
}
