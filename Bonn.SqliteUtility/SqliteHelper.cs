using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonn.SqliteUtility
{
    public static class SqliteHelper
    {

        /// <summary>
        /// 数据库路径
        /// </summary>
        public static string DbPath = "db.sqlite";

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetConnString(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("未设置数据库连接字符串");

            return $"Data Source={filePath}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SQLiteConnection GetConn(string filePath)
        {
            string connstr = GetConnString(filePath);
            SQLiteConnection tempconn = new SQLiteConnection(connstr);
            return tempconn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection GetConn()
        {
            return GetConn(DbPath);
        }

        /// <summary>
        /// 创建数据库，数据库文件不存在时可直接创建
        /// </summary>
        public static void CreateDB()
        {
            using (SQLiteConnection conn = GetConn())
            {
                conn.Open();
                conn.Close();
            }
        }

        ///// <summary>
        ///// 创建表
        ///// </summary>
        //public static void CreateTable()
        //{
        //    using (SQLiteConnection conn = GetConn())
        //    {
        //        if (conn.State != System.Data.ConnectionState.Open)
        //        {
        //            conn.Open();
        //            SQLiteCommand cmd = new SQLiteCommand();
        //            cmd.Connection = conn;
        //            cmd.CommandText = "CREATE TABLE t1(id varchar(4),score int)";
        //            cmd.ExecuteNonQuery();
        //        }
        //        conn.Close();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strSql, SQLiteTransaction trans = null)
        {
            SQLiteConnection conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                conn.Open();
                //定义command对象，并执行相应的SQL语句
                SQLiteCommand myCommand = new SQLiteCommand(strSql, conn);
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
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeInsert(string strTableName, Dictionary<string, object> dict, SQLiteTransaction trans = null)
        {
            SQLiteConnection dbconn;
            if (trans != null)
                dbconn = trans.Connection;
            else
                dbconn = GetConn();

            if (dbconn.State != System.Data.ConnectionState.Open) dbconn.Open();
            StringBuilder strColumnSql = new StringBuilder();
            strColumnSql.AppendFormat("INSERT INTO {0}(", strTableName);

            SQLiteCommand dbCommand = new SQLiteCommand();
            if (trans != null)
                dbCommand.Transaction = trans;

            StringBuilder strParameter = new StringBuilder();
            int paraIndex = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                SQLiteParameter parameter = DictValueToParam(kpr.Key, kpr.Value);
                if (parameter == null) continue;
                dbCommand.Parameters.Add(parameter);

                strColumnSql.Append($"{kpr.Key},");

                //strParameter.Append("@Para" + paraIndex + " ,");
                strParameter.Append($"@{kpr.Key},");
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
        public static int SafeInsert(string strTableName, Dictionary<string, object> dict, out int id, SQLiteTransaction trans = null)
        {
            SQLiteConnection dbconn;
            if (trans != null)
                dbconn = trans.Connection;
            else
                dbconn = GetConn();

            if (dbconn.State != ConnectionState.Open) dbconn.Open();
            StringBuilder strColumnSql = new StringBuilder();
            strColumnSql.AppendFormat("INSERT INTO {0}(", strTableName);

            SQLiteCommand dbCommand = new SQLiteCommand();
            if (trans != null)
                dbCommand.Transaction = trans;

            StringBuilder strParameter = new StringBuilder();
            int paraIndex = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                SQLiteParameter parameter = DictValueToParam(kpr.Key, kpr.Value);
                if (parameter == null) continue;
                dbCommand.Parameters.Add(parameter);

                strColumnSql.Append($"{kpr.Key},");

                strParameter.Append($"@{kpr.Key} ,");
            }
            strParameter.Remove(strParameter.Length - 1, 1);
            strParameter.Append(" ) ");

            strColumnSql.Remove(strColumnSql.Length - 1, 1);
            strColumnSql.Append(") VALUES(");

            dbCommand.Connection = dbconn;
            dbCommand.CommandText = strColumnSql.ToString() + strParameter;
            int dbResult = dbCommand.ExecuteNonQuery();

            dbCommand.CommandText = "select last_insert_rowid();";
            dbCommand.Parameters.Clear();
            id = Convert.ToInt32(dbCommand.ExecuteScalar());

            if (trans == null) dbconn.Close();

            return dbResult;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeUpdate(string tableName, Dictionary<string, object> dict, Dictionary<string, object> para = null, SQLiteTransaction trans = null)
        {
            SQLiteConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                SQLiteCommand dbCommand = new SQLiteCommand();
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
                        strSql += $"[{colName}] = [{colName}] + @{colName}, ";
                    else if (op == "*=" || op == "=*")
                        strSql += $"[{colName}] = [{colName}] * @{colName}, ";
                    else if (op == "-=")
                        strSql += $"[{colName}] = @{colName} - [{colName}], ";
                    else if (op == "=-")
                        strSql += $"[{colName}] = [{colName}] - @{colName}, ";
                    else
                        strSql += $"[{colName}] {op} @{colName}, ";
                    SQLiteParameter parameter = DictValueToParam(colName, kpr.Value);
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
                        strSql += $"AND [{colName}] {op} @{colName} ";
                        SQLiteParameter parameter = DictValueToParam(colName, kpr.Value);
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int SafeDelete(string tableName, Dictionary<string, object> para = null, SQLiteTransaction trans = null)
        {
            SQLiteConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                SQLiteCommand dbCommand = new SQLiteCommand();
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

                        string colName;
                        string op;
                        GetColAndOp(kpr.Key, out colName, out op);
                        strSql += $"AND [{colName}] {op} @{colName} ";
                        SQLiteParameter parameter = DictValueToParam(colName, kpr.Value);
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
        /// 根据条件查询已存在的重复数据行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="para"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int GetExistedRows(string tableName, Dictionary<string, object> para = null, SQLiteTransaction trans = null)
        {
            SQLiteConnection conn;
            if (trans != null)
                conn = trans.Connection;
            else
                conn = GetConn(); //getConn():得到连接对象OleDbConnection conn = getConn(); //getConn():得到连接对象
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                SQLiteCommand dbCommand = new SQLiteCommand();
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
                        strSql += $"AND [{colName}] {op} @{colName} ";
                        SQLiteParameter parameter = DictValueToParam(colName, kpr.Value);
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet Query(string strSql, IEnumerable<SQLiteParameter> parameters = null, SQLiteTransaction trans = null)
        {
            DataSet mydataset; //定义DataSet
            try
            {
                SQLiteConnection conn = GetConn(); //getConn():得到连接对象
                SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                mydataset = new DataSet();
                adapter.SelectCommand = new SQLiteCommand(strSql, conn);
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
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql, IEnumerable<SQLiteParameter> sqlParameter = null, SQLiteTransaction trans = null)
        {
            return Query(strSql,sqlParameter,trans).Tables[0];
        }


        /// <summary>
        /// 分页函数
        /// </summary>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="currentPageIndex">当前是第几页</param>
        /// <param name="columns">SQL查询语句列名</param>
        /// <param name="tableName">SQL查询语句表名，不能包含条件</param>
        /// <param name="condition">查询条件, 不用加where关键字，格式为" AND 1 = 1 "</param>
        /// <param name="parameters">查询语句相关参数</param>
        /// <param name="ascColumn">排序的字段名，传入字段名即可</param>
        /// <param name="bitOrderType">排序的类型 (0为升序,1为降序)</param>
        /// <param name="totalSqlString"></param>
        /// <param name="totalData">总记录数，汇总字段</param>
        /// <returns>返回数据集Datatable</returns>
        public static DataTable Get2005PagedDataTable(int pageSize, int currentPageIndex, string columns, string tableName, string condition,
            IEnumerable<SQLiteParameter> parameters, string ascColumn, int bitOrderType,
            string totalSqlString, out DataTable totalData)
        {
            DataTable result;
            if (pageSize <= 0 || currentPageIndex <= 0)
                throw new Exception("每页记录数或者当前页数不正确");

            try
            {
                StringBuilder strSql = new StringBuilder();
                StringBuilder countSql = new StringBuilder();

                IEnumerable<SQLiteParameter> sqlParameters = parameters as SQLiteParameter[] ?? parameters.ToArray();
                IEnumerable<SQLiteParameter> sqlParams1 = CopySqlParameter(sqlParameters);
                IEnumerable<SQLiteParameter> sqlParams2 = CopySqlParameter(sqlParameters);

                //获取汇总数据
                countSql.AppendFormat($"SELECT COUNT(*) {totalSqlString} FROM {tableName} WHERE 1 = 1 {condition} ");

                int beginNo = pageSize * (currentPageIndex - 1) + 1;
                int endNo = pageSize * (currentPageIndex);
                string orderType = bitOrderType == 1 ? " DESC" : "";
                strSql.AppendFormat($@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {ascColumn}{orderType}) AS NO, {columns} 
                    FROM {tableName} WHERE 1 = 1 {condition}) t WHERE  t.NO BETWEEN {beginNo} AND {endNo}");

                totalData = QueryDt(countSql.ToString(), sqlParams1);
                result = QueryDt(strSql.ToString(), sqlParams2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static IEnumerable<SQLiteParameter> CopySqlParameter(IEnumerable<SQLiteParameter> sqlParams)
        {
            List<SQLiteParameter> outSqlParameters = new List<SQLiteParameter>();
            foreach (SQLiteParameter sqlParameter in sqlParams)
            {
                SQLiteParameter outParameter = CopySqlParameter(sqlParameter);
                outSqlParameters.Add(outParameter);
            }
            return outSqlParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static SQLiteParameter CopySqlParameter(SQLiteParameter parameter)
        {
            SQLiteParameter outParameter = new SQLiteParameter();
            outParameter.ParameterName = parameter.ParameterName;
            outParameter.DbType = parameter.DbType;
            outParameter.Size = parameter.Size;
            outParameter.Value = parameter.Value;
            outParameter.Direction = parameter.Direction;
            return outParameter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="parameters"></param>
        /// <param name="sql">sql必须是where 1=1 结尾，或者必须是有where条件的</param>
        public static void BuildWhereSqlByDict(Dictionary<string, object> para, ref StringBuilder sql, out List<SQLiteParameter> parameters)
        {
            int paraIndex = 0;
            parameters = new List<SQLiteParameter>();
            if (para == null) return;
            sql.Append(" ");
            foreach (KeyValuePair<string, object> kpr in para)
            {
                if (kpr.Value == null) continue;
                if (kpr.Value == DBNull.Value) continue;

                string colName;
                string op;
                GetColAndOp(kpr.Key, out colName, out op);

                sql.Append($"AND [{colName}] {op} @{colName} ");

                SQLiteParameter parameter = DictValueToParam(colName, kpr.Value);
                parameters.Add(parameter);
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
        public static SQLiteParameter DictValueToParam(string key, object value)
        {
            if (value == null) return null;
            if (value == DBNull.Value) return null;

            SQLiteParameter parameter = new SQLiteParameter();
            parameter.ParameterName = "@" + key;
            if (value is int)
            {
                parameter.DbType = DbType.Int32;
            }
            else if (value is string)
            {
                parameter.DbType = DbType.String;
                parameter.Size = value.ToString().Length;
            }
            else if (value is DateTime)
            {
                parameter.DbType = DbType.DateTime;
            }
            else if (value is decimal)
            {
                parameter.DbType = DbType.Decimal;
            }
            else if (value is bool)
            {
                parameter.DbType = DbType.Boolean;
            }
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kpr"></param>
        /// <param name="paraIndex"></param>
        /// <returns></returns>
        //public static SQLiteParameter DictValueToParam(KeyValuePair<string, object> kpr, ref int paraIndex)
        //{
        //    SQLiteParameter para = DictValueToParam(kpr.Key, kpr.Value);
        //    para.ParameterName = "@" + kpr.Key;
        //    paraIndex++;
        //    return para;
        //}

        //public static SQLiteParameter DictValueToParam(string key,object value)
        //{
        //    SQLiteParameter para = DictValueToParam(key,value);
        //    para.ParameterName = "@" + key;
        //    return para;
        //}






    }
}
