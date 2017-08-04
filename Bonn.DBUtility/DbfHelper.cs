using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace Bonn.DBUtility
{
    /// <summary>
    /// DBF文件操作类
    /// </summary>
    public class DbfHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static string oleDbConn;

        /// <summary>
        /// 
        /// </summary>
        public static string sourceDbPath;

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string strSql)
        {
            try
            {
                using (OdbcConnection con = new OdbcConnection(oleDbConn))
                {
                    con.Open();
                    OdbcCommand com = new OdbcCommand(strSql, con);
                    return com.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="strSql"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string dbPath, string strSql)
        {
            try
            {
                string oledbstr = @"Driver={Microsoft Visual FoxPro Driver};UID=;SourceDB=" + dbPath +
                                  ";SourceType=DBF;Exclusive=No;BackgroundFetch=Yes;Collate=Machine;Null=Yes;Deleted=Yes;";
                using (OdbcConnection con = new OdbcConnection(oledbstr))
                {
                    con.Open();
                    OdbcCommand com = new OdbcCommand(strSql, con);
                    return com.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="strSql"></param>
        /// <param name="odbcParams"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string dbPath, string strSql, IEnumerable<OdbcParameter> odbcParams)
        {
            try
            {
                string oledbstr = @"Driver={Microsoft Visual FoxPro Driver};UID=;SourceDB=" + dbPath +
                        ";SourceType=DBF;Exclusive=No;BackgroundFetch=Yes;Collate=Machine;Null=Yes;Deleted=Yes;";

                using (OdbcConnection con = new OdbcConnection(oledbstr))
                {
                    con.Open();
                    OdbcCommand com = new OdbcCommand(strSql, con);
                    foreach (OdbcParameter sqlPara in odbcParams)
                    {
                        com.Parameters.Add(sqlPara);
                    }
                    return com.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="strTableName"></param>
        /// <param name="colNames"></param>
        /// <returns></returns>
        public static int CreateTable(string dbPath, string strTableName, List<string> colNames)
        {
            string strSql = "CREATE TABLE" + strTableName + " (";
            foreach (string colName in colNames)
            {
                strSql += colName + ", ";
            }
            strSql = strSql.TrimEnd(',') + ")";
            return ExecuteNonQuery(dbPath, strSql);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="strTableName"></param>
        /// <param name="createSql"></param>
        /// <returns></returns>
        public static int CreateTable(string dbPath, string strTableName, string createSql)
        {
            return ExecuteNonQuery(dbPath, createSql);
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数。
        /// </summary>
        /// <param name="dbPath">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <returns>返回受影响的行数。</returns>
        public static int Insert(string dbPath, string strTableName, Dictionary<string, object> dict)
        {
            string strSql = "INSERT INTO " + strTableName + " (";
            string strValue = (" VALUES(");
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                strSql += kpr.Key.Substring(1) + ",";
                if (kpr.Value == null)
                    strValue += string.Format("'{0}',", string.Empty);
                else if (kpr.Value is string && kpr.Value.ToString().Contains('^') == false)
                    strValue += string.Format("'{0}',", kpr.Value);
                else
                    strValue += string.Format("{0},", kpr.Value);

            }
            strSql = strSql.TrimEnd(',') + ")";
            strValue = strValue.TrimEnd(',') + ")";
            strSql += strValue;
            return ExecuteNonQuery(dbPath, strSql);
        }


        /// <summary>
        /// 通用新增数据函数，返回受影响的行数。
        /// </summary>
        /// <param name="dbPath">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="odbcParams">插入字段数据字典</param>
        /// <returns>返回受影响的行数。</returns>
        public static int Insert(string dbPath, string strTableName, IEnumerable<OdbcParameter> odbcParams)
        {
            string strSql = "INSERT INTO " + strTableName + " (";
            string strValue = (" VALUES(");
            IEnumerable<OdbcParameter> odbcParameters = odbcParams as IList<OdbcParameter> ?? odbcParams.ToList();
            foreach (OdbcParameter odbcParameter in odbcParameters)
            {
                strSql += odbcParameter.ParameterName.Substring(1) + ",";

                strValue += string.Format("?,");
            }
            strSql = strSql.TrimEnd(',') + ")";
            strValue = strValue.TrimEnd(',') + ")";
            strSql += strValue;
            return ExecuteNonQuery(dbPath, strSql, odbcParameters);
        }

        /// <summary>
        /// 返回DataSet,不带参数
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns></returns>
        public static DataSet QueryDs(string sql)
        {
            try
            {
                string oledbStr = @"Driver={Microsoft Visual FoxPro Driver};UID=;SourceDB=" + sourceDbPath +
                        ";SourceType=DBF;Exclusive=No;BackgroundFetch=Yes;Collate=Machine;Null=Yes;Deleted=Yes;";

                using (OdbcConnection con = new OdbcConnection(oledbStr))
                {
                    con.Open();
                    OdbcCommand sqlCommand = new OdbcCommand(sql, con);
                    OdbcDataAdapter sqlDataAdapter = new OdbcDataAdapter(sqlCommand);
                    DataSet datatable = new DataSet();
                    sqlDataAdapter.Fill(datatable);
                    sqlCommand.Dispose();
                    con.Close();
                    con.Dispose();
                    return datatable;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 返回DataSet,不带参数
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns></returns>
        public static DataTable QueryDt(string sql)
        {
            try
            {
                string oledbStr = @"Driver={Microsoft Visual FoxPro Driver};UID=;SourceDB=" + sourceDbPath +
                    ";SourceType=DBF;Exclusive=No;BackgroundFetch=Yes;Collate=Machine;Null=Yes;Deleted=Yes;";

                using (OdbcConnection con = new OdbcConnection(oledbStr))
                {
                    con.Open();
                    OdbcCommand sqlCommand = new OdbcCommand(sql, con);
                    OdbcDataAdapter sqlDataAdapter = new OdbcDataAdapter(sqlCommand);
                    DataTable dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    sqlCommand.Dispose();
                    con.Close();
                    con.Dispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
