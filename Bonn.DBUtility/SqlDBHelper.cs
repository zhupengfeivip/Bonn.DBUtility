/*
 *  版权所有
 * 
 * 功能说明：通用SQL SERVER数据库操作类
 * 
 * 创建标识：朱鹏飞 2017-8-4
 * 创建标识：调用Bonn.Helper对数据库连接字符串进行des加密
 * 
 * 修改标识：
 * 修改说明：
 * 
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Bonn.Helper;

namespace Bonn.DBUtility
{
    /// <summary>
    /// SQL Server数据库处理助手
    /// </summary>
    public static class SqlDbHelper
    {
        private static string _dbConnString = string.Empty;

        /// <summary>
        /// 连接数据库字符串
        /// </summary>
        public static string Db
        {
            get
            {
                //数据库连接信息为空时获取数据库连接字符串
                if (string.IsNullOrWhiteSpace(_dbConnString) == true)
                {
                    //判断配置文件中是否有conn数据库连接字符串
                    if (ConfigurationManager.AppSettings["conn"] == null)
                    {
                        throw new Exception("没有配置也无法获取数据库连接信息。");
                    }
                    try
                    {
                        if (ConfigurationManager.AppSettings["IsDecryptedOfDbConn"] != null &&
                            ConfigurationManager.AppSettings["IsDecryptedOfDbConn"].Trim().ToLower() == "false")
                        {
                            //如果存在配置IsDecryptedOfDbConn，且为false，表示不加密数据库连接字符串
                            _dbConnString = ConfigurationManager.AppSettings["conn"];
                        }
                        else
                        {
                            if (DecryptDbConnString == false)
                            {
                                _dbConnString = ConfigurationManager.AppSettings["conn"];
                            }
                            else
                            {
                                _dbConnString = Bonn.Helper.Des3.Decrypt(ConfigurationManager.AppSettings["conn"]);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("解密数据库连接字符串错误," + ex.Message);
                    }

                }

                return _dbConnString;
            }
            set
            {
                _dbConnString = value;
            }
        }

        /// <summary>
        /// 拼接数据库连接字符串
        /// </summary>
        /// <param name="server"></param>
        /// <param name="db"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="port"></param>
        /// <param name="connType">0 windows身份验证 1 sqlserver身份验证 </param>
        /// <returns></returns>
        public static string BuildConnString(string server, string db, string user, string pwd, short port = 1433, byte connType = 1)
        {
            if (connType == 0)
            {
                throw new Exception("暂未实现");
            }
            else
            {
                //示例：Data Source=127.0.0.1,1433;Initial Catalog=MCFL;Persist Security Info=True;User ID=sa;Password=shujt
                return $"Data Source={server},{port};Initial Catalog={db};Persist Security Info=True;User ID={user};Password={pwd}";
            }
           
        }


        /// <summary>
        /// 是否加密数据库连接字符串
        /// </summary>
        private static bool _decryptDbConnString = true;

        /// <summary>
        /// 是否加密数据库连接字符串，默认为加密
        /// </summary>
        public static bool DecryptDbConnString
        {
            get { return _decryptDbConnString; }
            set { _decryptDbConnString = value; }
        }

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。 值 0 指示无限制，在 CommandTimeout 中应避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public static int CmdTimeOut = 30;

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。 值 0 指示无限制，在 CommandTimeout 中应避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public static int DbCmdtimeout
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ConfigurationSettings.AppSettings["DBCmdTimeout"]) == false)
                    {
                        CmdTimeOut = int.Parse(ConfigurationSettings.AppSettings["DBCmdTimeout"]);
                    }
                }
                catch
                {
                    CmdTimeOut = 30;
                }
                return CmdTimeOut;
            }
            set
            {
                CmdTimeOut = value;
            }
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDbConnString()
        {
            return Db;
        }

        /// <summary>
        /// SQL最小时间
        /// </summary>
        public static DateTime MinDateTime => new DateTime(1753, 1, 1, 12, 0, 0);

        /// <summary>
        /// SQL最大时间
        /// </summary>
        public static DateTime MaxDateTime => new DateTime(9999, 12, 31, 23, 59, 59);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsSql2000()
        {
            string strSql = "SELECT SERVERPROPERTY('productversion')";
            string strVer = ExecuteScalar(strSql).ToString();
            //SQL2000 8.00.2039
            return strVer.StartsWith("8");
        }


        #region 查询

        /// <summary>
        /// 返回DataTable,不带参数
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static DataTable QueryDt(SqlCommand comm)
        {
            try
            {
                SqlCommand sqlCommand = comm;
                sqlCommand.CommandTimeout = DbCmdtimeout;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable datatable = new DataTable();
                sqlDataAdapter.Fill(datatable);
                comm.Dispose();
                return datatable;
            }
            catch (SqlException ex)
            {
                throw ex;
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
        public static DataSet QueryDs(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Db))
                {
                    con.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(sql, con);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet datatable = new DataSet();
                        sqlDataAdapter.Fill(datatable);
                        sqlCommand.Dispose();
                        con.Close();
                        con.Dispose();
                        return datatable;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 带事务的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataSet QueryDs(SqlTransaction trans, string strSql)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                    trans.Connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(strSql, trans.Connection);
                    command.Transaction = trans;
                    command.CommandTimeout = DbCmdtimeout;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = command;
                    DataSet datatable = new DataSet();
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 传入数据库连接字符串，返回DataSet,不带参数
        /// </summary>
        /// <param name="strConn">数据库连接字符串</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns></returns>
        public static DataSet QueryDs(string strConn, string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    con.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(sql, con);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet datatable = new DataSet();
                        sqlDataAdapter.Fill(datatable);
                        sqlCommand.Dispose();
                        con.Close();
                        con.Dispose();
                        return datatable;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataSet QueryDs(string strSql, SqlParameter[] sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    sqlCommand.Parameters.AddRange(sqlParameter);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataSet datatable = new DataSet();
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataSet QueryDs(string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    foreach (SqlParameter parameter in sqlParameter)
                    {
                        sqlCommand.Parameters.Add(parameter);
                    }
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataSet datatable = new DataSet();
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 带事务和参数的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataSet QueryDs(SqlTransaction trans, string strSql, SqlParameter[] sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                    trans.Connection.Open();
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                command.Parameters.AddRange(sqlParameter);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataSet datatable = new DataSet();
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 带事务和参数的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataSet QueryDs(SqlTransaction trans, string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                foreach (SqlParameter parameter in sqlParameter)
                {
                    command.Parameters.Add(parameter);
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataSet datatable = new DataSet();
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 带事务的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tbName"></param>
        /// <param name="listParam"></param>
        /// <param name="orderCol"></param>
        /// <returns></returns>
        public static DataSet QueryDsByTbname(SqlTransaction trans, string tbName, Bonn.Helper.ListSearchParameter listParam = null, string orderCol = null)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                StringBuilder strSql = new StringBuilder();

                strSql.AppendFormat("SELECT * FROM {0} WHERE 1 = 1 ", tbName);

                int i = 0;
                List<SqlParameter> listPara = new List<SqlParameter>();
                if (listParam != null)
                {
                    foreach (Bonn.Helper.SearchParameter sParam in listParam)
                    {
                        if (sParam.Condition.Trim().ToUpper() == "IN")
                        {
                            strSql.AppendFormat(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                        }
                        else
                        {
                            strSql.AppendFormat(" AND {0} {1} @Para{2}", sParam.CollName.Substring(1), sParam.Condition, i);
                            listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                            i++;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(orderCol) == false)
                    strSql.AppendFormat(" ORDER BY {0} ", orderCol);

                return QueryDs(trans, strSql.ToString(), listPara);
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// 带事务的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tbName"></param>
        /// <param name="listParam"></param>
        /// <param name="orderCol"></param>
        /// <returns></returns>
        public static int GetValueByTbname(SqlTransaction trans, string tbName, Bonn.Helper.ListSearchParameter listParam = null, string orderCol = null)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                StringBuilder strSql = new StringBuilder();
                StringBuilder strParameter = new StringBuilder();

                strSql.AppendFormat("SELECT * FROM {0} WHERE 1 = 1 ", tbName);

                int i = 0;
                List<SqlParameter> listPara = new List<SqlParameter>();
                if (listParam != null)
                {
                    foreach (Bonn.Helper.SearchParameter sParam in listParam)
                    {
                        if (sParam.Condition.Trim().ToUpper() == "IN")
                        {
                            strSql.AppendFormat(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                        }
                        else
                        {
                            strSql.AppendFormat(" AND {0} {1} @Para{2}", sParam.CollName.Substring(1), sParam.Condition, i);
                            listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                            i++;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(orderCol) == false)
                    strSql.AppendFormat(" ORDER BY {0} ", orderCol);

                DataSet ds = QueryDs(trans, strSql.ToString(), listPara);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables.Count == 0)
                    return -1;

                if (listParam == null) return -2;

                foreach (SearchParameter parameter in listParam)
                {
                    if (ds.Tables[0].Rows[0][parameter.CollName] == DBNull.Value)
                        continue;

                    parameter.CollValue = ds.Tables[0].Rows[0][parameter.CollName];
                }

                return 1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// 带事务的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QueryDt(SqlTransaction trans, string strSql)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 带事务和参数的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QueryDt(SqlTransaction trans, string strSql, SqlParameter[] sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.Transaction = trans;
                command.CommandTimeout = DbCmdtimeout;
                command.Parameters.AddRange(sqlParameter);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 带事务和参数的数据库
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QueryDt(SqlTransaction trans, string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.Transaction = trans;
                command.CommandTimeout = DbCmdtimeout;
                foreach (SqlParameter parameter in sqlParameter)
                {
                    command.Parameters.Add(parameter);
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 返回数据集DataTable
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QueryDt(StringBuilder strSql)
        {
            return QueryDt(strSql.ToString());
        }

        /// <summary>
        /// 返回数据集DataTable
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql)
        {
            return QueryDt(Db, strSql);
        }

        /// <summary>
        /// 返回数据集DataTable
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string connStr, string strSql)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = connStr;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    datatable.TableName = "dataTable1";
                    sqlDataAdapter.Fill(datatable);
                    sqlCommand.Parameters.Clear();
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql, SqlParameter[] sqlParameter, string strConn = null)
        {
            string connString;
            if (string.IsNullOrWhiteSpace(strConn))
                connString = GetDbConnString();
            else
                connString = strConn;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    sqlCommand.Parameters.AddRange(sqlParameter);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable("datatable");
                    sqlDataAdapter.Fill(datatable);
                    sqlCommand.Parameters.Clear();
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    foreach (SqlParameter parameter in sqlParameter)
                    {
                        sqlCommand.Parameters.Add(parameter);
                    }
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql">查询语句，要求查询语句必须有条件，无查询条件时需要添加WHERE 1 = 1</param>
        /// <param name="listParam"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql, Bonn.Helper.ListSearchParameter listParam, SqlTransaction trans = null)
        {
            List<SqlParameter> listPara = new List<SqlParameter>();
            BuildCondition(listParam, ref strSql, ref listPara);

            return trans == null ? QueryDt(strSql, listPara) : QueryDt(trans, strSql, listPara);
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="listParam"></param>
        /// <param name="ascString"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static DataTable QueryDt(string strSql, Bonn.Helper.ListSearchParameter listParam, string ascString, SqlTransaction trans = null)
        {
            List<SqlParameter> listPara = new List<SqlParameter>();
            BuildCondition(listParam, ref strSql, ref listPara);

            if (!string.IsNullOrWhiteSpace(ascString))
                strSql += " ORDER BY " + ascString;

            return trans == null ? QueryDt(strSql, listPara) : QueryDt(trans, strSql, listPara);
        }




        /// <summary>
        /// 带事务的数据库  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(SqlTransaction trans, string strSql)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 带事务和参数的数据库  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(SqlTransaction trans, string strSql, SqlParameter[] sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.Transaction = trans;
                command.CommandTimeout = DbCmdtimeout;
                command.Parameters.AddRange(sqlParameter);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 带事务和参数的数据库  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(SqlTransaction trans, string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.Transaction = trans;
                command.CommandTimeout = DbCmdtimeout;
                foreach (SqlParameter parameter in sqlParameter)
                {
                    command.Parameters.Add(parameter);
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable datatable = new DataTable();
                sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                sqlDataAdapter.Fill(datatable);
                return datatable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 返回数据集DataTable  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(StringBuilder strSql)
        {
            return QuerySchemaDt(strSql.ToString());
        }

        /// <summary>
        /// 返回数据集DataTable  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string strSql)
        {
            return QuerySchemaDt(Db, strSql);
        }

        /// <summary>
        /// 返回数据集DataTable  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string connStr, string strSql)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = connStr;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据集DataTable  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataSet QuerySchemaDs(string strSql, string connStr = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                if (connStr == null)
                    sqlConnection.ConnectionString = Db;
                else
                    sqlConnection.ConnectionString = connStr;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataSet dataSet = new DataSet();
                    dataSet.EnforceConstraints = false;//不实施约束规则
                    sqlDataAdapter.FillSchema(dataSet, SchemaType.Mapped);
                    sqlDataAdapter.Fill(dataSet);
                    return dataSet;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string strSql, SqlParameter[] sqlParameter, string strConn = null)
        {
            string connString;
            if (string.IsNullOrWhiteSpace(strConn))
                connString = GetDbConnString();
            else
                connString = strConn;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    sqlCommand.Parameters.AddRange(sqlParameter);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string strSql, IEnumerable<SqlParameter> sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    foreach (SqlParameter parameter in sqlParameter)
                    {
                        sqlCommand.Parameters.Add(parameter);
                    }
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    sqlDataAdapter.FillSchema(datatable, SchemaType.Mapped);
                    sqlDataAdapter.Fill(datatable);
                    return datatable;
                }
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql">查询语句，要求查询语句必须有条件，无查询条件时需要添加WHERE 1 = 1</param>
        /// <param name="listParam"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string strSql, Bonn.Helper.ListSearchParameter listParam, SqlTransaction trans = null)
        {
            List<SqlParameter> listPara = new List<SqlParameter>();
            BuildCondition(listParam, ref strSql, ref listPara);

            return trans == null ? QuerySchemaDt(strSql, listPara) : QuerySchemaDt(trans, strSql, listPara);
        }

        /// <summary>
        /// 返回数据库DataTable带参数数组  返回的DataTable包含数据库表结构
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="listParam"></param>
        /// <param name="ascString"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static DataTable QuerySchemaDt(string strSql, Bonn.Helper.ListSearchParameter listParam, string ascString, SqlTransaction trans = null)
        {
            List<SqlParameter> listPara = new List<SqlParameter>();
            BuildCondition(listParam, ref strSql, ref listPara);

            if (!string.IsNullOrWhiteSpace(ascString))
                strSql += " ORDER BY " + ascString;

            return trans == null ? QuerySchemaDt(strSql, listPara) : QuerySchemaDt(trans, strSql, listPara);
        }

        #endregion


        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="str">Transact-SQL 语句</param>
        /// <returns>受影响的行数。</returns>
        public static int ExecuteNonQuery(string str)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = Db;
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(str, sqlConnection);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
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
        /// <param name="strSqlGroup">Transact-SQL 语句</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string[] strSqlGroup)
        {
            using (SqlConnection conn = new SqlConnection(SqlDbHelper.Db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < strSqlGroup.Length; i++)
                        {
                            if (string.IsNullOrEmpty(strSqlGroup[i])) continue;

                            SqlDbHelper.ExecuteNonQuery(trans, strSqlGroup[i]);
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sqlParams"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string str, SqlParameter[] sqlParams)
        {
            return ExecuteNonQuery(Db, str, sqlParams);
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句ArrayList、带事物并返回是否成功。
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(ArrayList arrListSqlGroup)
        {
            using (SqlConnection conn = new SqlConnection(SqlDbHelper.Db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < arrListSqlGroup.Count; i++)
                        {
                            if (string.IsNullOrEmpty(arrListSqlGroup[i].ToString())) continue;

                            ExecuteNonQuery(trans, arrListSqlGroup[i].ToString());
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句ArrayList、带参数和事物并返回是否成功。
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(ArrayList arrListSqlGroup, ArrayList arrListParameter)
        {
            using (SqlConnection conn = new SqlConnection(SqlDbHelper.Db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < arrListSqlGroup.Count; i++)
                        {
                            if (string.IsNullOrEmpty(arrListSqlGroup[i].ToString())) continue;

                            ExecuteNonQuery(trans, arrListSqlGroup[i].ToString(), (SqlParameter[])arrListParameter[i]);
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句ArrayList、带事物并返回是否成功。
        /// </summary>
        /// <param name="arrListSqlGroup">Transact-SQL 语句</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(List<string> arrListSqlGroup)
        {
            using (SqlConnection conn = new SqlConnection(SqlDbHelper.Db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < arrListSqlGroup.Count; i++)
                        {
                            if (string.IsNullOrEmpty(arrListSqlGroup[i].ToString())) continue;

                            SqlDbHelper.ExecuteNonQuery(trans, arrListSqlGroup[i].ToString());
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句List、带参数和事物并返回是否成功。
        /// </summary>
        /// <param name="arrListSqlGroup">Transact-SQL 语句 列表</param>
        /// <param name="arrListParameter">SqlParameter参数数组的列表</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(List<string> arrListSqlGroup, List<SqlParameter[]> arrListParameter)
        {
            using (SqlConnection conn = new SqlConnection(SqlDbHelper.Db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < arrListSqlGroup.Count; i++)
                        {
                            if (string.IsNullOrEmpty(arrListSqlGroup[i].ToString())) continue;

                            SqlDbHelper.ExecuteNonQuery(trans, arrListSqlGroup[i].ToString(), (SqlParameter[])arrListParameter[i]);
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sqlParams"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string str, IEnumerable<SqlParameter> sqlParams)
        {
            return ExecuteNonQuery(Db, str, sqlParams);
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。,带事务操作
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string strSql)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(strSql, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。带事务操作
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="str"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string str, SqlParameter[] sql)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(str, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                if (sql != null)
                {
                    command.Parameters.AddRange(sql);
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。带事务操作
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="str"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string str, IEnumerable<SqlParameter> sqlParams)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                SqlCommand command = new SqlCommand(str, trans.Connection);
                command.CommandTimeout = DbCmdtimeout;
                command.Transaction = trans;
                foreach (SqlParameter sqlParameter in sqlParams)
                {
                    command.Parameters.Add(sqlParameter);
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="str">Transact-SQL 语句</param>
        /// <returns>受影响的行数。</returns>
        public static int ExecuteNonQuery(string connStr, string str)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = connStr;
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(str, sqlConnection);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="str"></param>
        /// <param name="sql"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string connStr, string str, SqlParameter[] sql)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = connStr;
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(str, sqlConnection);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        sqlCommand.Parameters.AddRange(sql);
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="str"></param>
        /// <param name="sqlParams"></param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string connStr, string str, IEnumerable<SqlParameter> sqlParams)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = connStr;
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(str, sqlConnection);
                        sqlCommand.CommandTimeout = DbCmdtimeout;
                        foreach (SqlParameter sqlPara in sqlParams)
                        {
                            sqlCommand.Parameters.Add(sqlPara);
                        }
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询方法 带数组参数数组(请使用QueryDs方法)
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string strSql, SqlParameter[] sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    sqlCommand.Parameters.AddRange(sqlParameter);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet, "s");
                    return dataSet;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

        }

        /// <summary>
        /// 读数据 无数组参数数组
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        /// 此处代码不能显示关闭连接，关闭连接后DataReader就读不到信息了
        public static SqlDataReader ExecuteReader(string cmdText)
        {
            SqlConnection conn = new SqlConnection(Db);
            try
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandTimeout = DbCmdtimeout;
                SqlDataReader sdr = null;
                conn.Open();
                //关闭DataReader时，自动关闭连接
                sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return sdr;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 读数据 带数组参数数组
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// 此处代码不能显示关闭连接，关闭连接后DataReader就读不到信息了
        public static SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] cmdParms)
        {
            SqlConnection conn = new SqlConnection(Db);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandTimeout = DbCmdtimeout;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            SqlDataReader sdr = null;
            conn.Open();
            try
            {
                //关闭DataReader时，自动关闭连接
                sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
            }
            catch (Exception)
            {
                conn.Close();
                throw;
            }
            return sdr;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。带事务重载
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="cmdText"></param>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用（在 Visual Basic 中为 Nothing）。</returns>
        public static object ExecuteScalar(string connStr, string cmdText)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    return sqlCommand.ExecuteScalar();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction trans, string cmdText)
        {
            if (trans == null)
                return ExecuteScalar(cmdText, new SqlParameter[] { });
            else
                return ExecuteScalar(trans, cmdText, new SqlParameter[] { });
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand(cmdText, trans.Connection);
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            cmd.Transaction = trans;

            object val = cmd.ExecuteScalar();

            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(cmdText, new SqlParameter[] { });
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, SqlParameter[] cmdParms)
        {
            if (cmdParms == null) throw new ArgumentNullException("cmdParms");
            return ExecuteScalar(Db, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。带参数
        /// </summary>
        /// <param name="cmdText">Sql命令</param>
        /// <param name="cmdParms">命令参数 可为null</param>
        /// <returns>int类型 执行结果为null时返回0</returns>
        public static int ExecuteScalarToInt(string cmdText, SqlParameter[] cmdParms = null)
        {
            object obj = ExecuteScalar(Db, cmdText, cmdParms);
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。带参数
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmdText">Sql命令</param>
        /// <param name="cmdParms">命令参数 可为null</param>
        /// <returns>int类型 执行结果为null时返回0</returns>
        public static int ExecuteScalarToInt(SqlTransaction trans, string cmdText, SqlParameter[] cmdParms = null)
        {
            object obj = ExecuteScalar(trans, cmdText, cmdParms);
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, IEnumerable<SqlParameter> cmdParms)
        {
            return ExecuteScalar(Db, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction trans, string cmdText, IEnumerable<SqlParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand(cmdText, trans.Connection, trans);
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            object val = cmd.ExecuteScalar();
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connStr, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(cmdText, con);

                    if (cmdParms != null)
                    {
                        foreach (SqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    con.Open();
                    cmd.CommandTimeout = DbCmdtimeout;
                    object val = cmd.ExecuteScalar();
                    return val;
                }
                catch
                {
                    con.Close();
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connStr, string cmdText, IEnumerable<SqlParameter> cmdParms)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.CommandTimeout = DbCmdtimeout;
                    if (cmdParms != null)
                    {
                        foreach (SqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    con.Open();
                    object val = cmd.ExecuteScalar();
                    return val;
                }
                catch
                {
                    con.Close();
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connStr, SqlTransaction trans, string cmdText, IEnumerable<SqlParameter> cmdParms)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Transaction = trans;
                cmd.CommandTimeout = DbCmdtimeout;
                if (cmdParms != null)
                {
                    foreach (SqlParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
                try
                {
                    conn.Open();
                    object val = cmd.ExecuteScalar();
                    return val;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 运行存储过程返回DataTable
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="sqlParameter">存储过程参数</param>
        /// <returns></returns>
        public static DataTable StoredDataTable(string storedProcName, SqlParameter[] sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = Db;
                sqlConnection.Open();
                try
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandTimeout = DbCmdtimeout;
                    sqlCommand.CommandText = storedProcName;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.Parameters.AddRange(sqlParameter);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    DataTable datatable = new DataTable();
                    sqlDataAdapter.Fill(datatable);
                    sqlCommand.Parameters.Clear();
                    return datatable;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

        }

        #region 分页查询 SQL2000以上

        /// <summary>
        /// 分页函数
        /// </summary>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="currentPageIndex">当前是第几页</param>
        /// <param name="columns">查询结果需要得到的字段</param>
        /// <param name="tableName">需要查询的表 </param>
        /// <param name="condition">查询条件, 不用加where关键字</param>
        /// <param name="ascColumn">排序的字段名 (即 order by column asc/desc)</param>
        /// <param name="bitOrderType">排序的类型 (0为升序,1为降序)</param>
        /// <param name="pkColumn">主键名称</param>
        /// <param name="count">总记录数</param>
        /// <returns>返回数据集Datatable</returns>
        public static DataTable GetPagedDataset(int pageSize, int currentPageIndex, string columns, string tableName,
            string condition, string ascColumn, int bitOrderType, string pkColumn, out int count)
        {
            count = 0;
            DataTable result = new DataTable();
            if (pageSize <= 0 || currentPageIndex < 0)
            {
                return result;
            }
            try
            {
                string strTemp;
                string strSql;
                string strOrderType;
                string CountSql;
                string TabTemp = ascColumn;
                if (!string.IsNullOrEmpty(TabTemp))
                {
                    if (TabTemp.Contains("."))
                        TabTemp = TabTemp.Split('.')[0];
                }
                CountSql = "SELECT COUNT(*) FROM " + tableName + " WHERE 1=1 and " + condition + " ";//获取总记录数
                if (bitOrderType == 1) //降序
                {
                    strOrderType = " ORDER BY " + ascColumn + " DESC ";
                    //strTemp = "<(SELECT min";
                }
                else //升序
                {
                    strOrderType = " ORDER BY " + ascColumn + " ASC ";
                    //strTemp = ">(SELECT max";
                }

                strTemp = " not in ";
                if (currentPageIndex == 1) //第一页
                {
                    if (condition != "")
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " WHERE 1=1 and " + condition + " " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " + strOrderType + " ";
                }
                else //其它页
                {
                    int pindex = (currentPageIndex - 1) * pageSize;
                    if (condition != "")
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                  " WHERE 1=1 and " + condition + " AND " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " +
                                  " " + pkColumn + " FROM " + tableName + "  where 1=1 and " + condition + " " + strOrderType + ") " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                  " WHERE 1=1 and " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " + pkColumn + " " +
                                  " FROM " + tableName + " " + strOrderType + ") " + strOrderType + " ";
                }

                count = Convert.ToInt32(QueryDt(CountSql).Rows[0][0].ToString());
                result = QueryDt(strSql);

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 分页函数
        /// </summary>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="currentPageIndex">当前是第几页</param>
        /// <param name="strQuerySql">SQL查询语句</param>
        /// <param name="condition">查询条件, 不用加where关键字</param>
        /// <param name="ascColumn">排序的字段名 (即 order by column asc/desc)</param>
        /// <param name="bitOrderType">排序的类型 (0为升序,1为降序)</param>
        /// <param name="pkColumn">主键名称</param>
        /// <param name="count">总记录数</param>
        /// <returns>返回数据集Datatable</returns>
        public static DataTable GetPagedDataset(int pageSize, int currentPageIndex, string strQuerySql, string condition,
            string ascColumn, int bitOrderType, string pkColumn, out int count)
        {
            count = 0;
            DataTable result = new DataTable();
            if (pageSize <= 0 || currentPageIndex < 0)
            {
                return result;
            }
            try
            {
                string columns;
                string tableName;
                string strTemp;
                string strSql;
                string strOrderType;
                string CountSql;
                //分解查询语句得到列信息和表信息
                SplitSql(strQuerySql, out tableName, out columns);
                CountSql = "SELECT COUNT(*) FROM " + tableName + " WHERE 1=1 and " + condition + " ";//获取总记录数
                if (bitOrderType == 1) //降序
                {
                    strOrderType = " ORDER BY " + ascColumn + " DESC ";
                }
                else //升序
                {
                    strOrderType = " ORDER BY " + ascColumn + " ASC ";
                }

                strTemp = " not in ";
                if (currentPageIndex == 1) //第一页
                {
                    if (condition != "")
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " WHERE 1=1 and " + condition + " " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " + strOrderType + " ";
                }
                else //其它页
                {
                    int pindex = (currentPageIndex - 1) * pageSize;
                    if (condition != "")
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                 " WHERE 1=1 and " + condition + " AND " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " +
                                 " " + pkColumn + " FROM " + tableName + "  where 1=1 and " + condition + " " + strOrderType + ") " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                 " WHERE 1=1 and " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " + pkColumn + " " +
                                 " FROM " + tableName + " " + strOrderType + ") " + strOrderType + " ";
                }

                count = Convert.ToInt32(QueryDt(CountSql).Rows[0][0].ToString());
                result = QueryDt(strSql);

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 分页函数
        /// </summary>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="currentPageIndex">当前是第几页</param>
        /// <param name="strQuerySql">SQL查询语句</param>
        /// <param name="condition">查询条件, 不用加where关键字，格式为" 1 = 1 AND 2 = 2 "</param>
        /// <param name="parameters"></param>
        /// <param name="ascColumn">排序的字段名 (即 order by column asc/desc)</param>
        /// <param name="bitOrderType">排序的类型 (0为升序,1为降序)</param>
        /// <param name="pkColumn">主键名称</param>
        /// <param name="count">总记录数</param>
        /// <returns>返回数据集Datatable</returns>
        public static DataTable GetPagedDataset(int pageSize, int currentPageIndex, string strQuerySql, string condition,
            IEnumerable<SqlParameter> parameters, string ascColumn, int bitOrderType, string pkColumn, out int count)
        {
            DataTable totalData;
            DataTable outDataTable = GetPagedDataset(pageSize, currentPageIndex, strQuerySql, condition,
                parameters, ascColumn, bitOrderType, pkColumn, string.Empty, out totalData);
            count = Convert.ToInt32(totalData.Rows[0][0]);
            return outDataTable;
        }

        /// <summary>
        /// 分页函数
        /// </summary>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="currentPageIndex">当前是第几页</param>
        /// <param name="strQuerySql">SQL查询语句</param>
        /// <param name="condition">查询条件, 不用加where关键字，格式为" 1 = 1 AND 2 = 2 "</param>
        /// <param name="parameters"></param>
        /// <param name="ascColumn">排序的字段名 (即 order by column asc/desc)</param>
        /// <param name="bitOrderType">排序的类型 (0为升序,1为降序)</param>
        /// <param name="pkColumn">主键名称</param>
        /// <param name="totalSqlString"></param>
        /// <param name="totalData">总记录数</param>
        /// <returns>返回数据集Datatable</returns>
        public static DataTable GetPagedDataset(int pageSize, int currentPageIndex, string strQuerySql, string condition,
            IEnumerable<SqlParameter> parameters, string ascColumn, int bitOrderType, string pkColumn,
            string totalSqlString, out DataTable totalData)
        {
            totalData = new DataTable();
            DataTable outDataTable = new DataTable();
            if (pageSize <= 0 || currentPageIndex < 0)
                return outDataTable;
            try
            {
                string columns;
                string tableName;
                string strTemp;
                string strSql;
                string strOrderType;
                string countSql;
                //分解查询语句得到列信息和表信息
                SplitSql(strQuerySql, out tableName, out columns);
                if (!string.IsNullOrWhiteSpace(condition))
                    countSql = string.Format("SELECT COUNT(*) {2} FROM {0} WHERE 1=1 AND {1}", tableName, condition, totalSqlString);//获取总记录数
                else
                    countSql = string.Format("SELECT COUNT(*) {1} FROM {0}", tableName, totalSqlString);//获取总记录数

                if (bitOrderType == 1)
                    strOrderType = " ORDER BY " + ascColumn + " DESC "; //降序
                else
                    strOrderType = " ORDER BY " + ascColumn + " ASC ";  //升序

                strTemp = " not in ";
                if (currentPageIndex == 1) //第一页
                {
                    if (!string.IsNullOrWhiteSpace(condition))
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " WHERE 1=1 AND " + condition + " " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " + strOrderType + " ";
                }
                else //其它页
                {
                    int pindex = (currentPageIndex - 1) * pageSize;
                    if (!string.IsNullOrWhiteSpace(condition))
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                 " WHERE 1 = 1 AND " + condition + " AND " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " +
                                 " " + pkColumn + " FROM " + tableName + "  WHERE 1=1 AND " + condition + " " + strOrderType + ") " + strOrderType + " ";
                    else
                        strSql = "SELECT TOP " + pageSize + " " + columns + " FROM " + tableName + " " +
                                 " WHERE 1 = 1 AND " + pkColumn + " " + strTemp + "(SELECT TOP " + pindex + " " + pkColumn + " " +
                                 " FROM " + tableName + " " + strOrderType + ") " + strOrderType + " ";
                }

                SqlParameter[] sqlParameters = parameters as SqlParameter[] ?? parameters.ToArray();
                totalData = QueryDt(countSql, sqlParameters);
                outDataTable = QueryDt(strSql, sqlParameters);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return outDataTable;
        }

        #endregion

        #region 分页查询 SQL2005以上

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
            IEnumerable<SqlParameter> parameters, string ascColumn, int bitOrderType,
            string totalSqlString, out DataTable totalData)
        {
            DataTable result;
            if (pageSize <= 0 || currentPageIndex <= 0)
                throw new Exception("每页记录数或者当前页数不正确");

            try
            {
                StringBuilder strSql = new StringBuilder();
                StringBuilder countSql = new StringBuilder();

                IEnumerable<SqlParameter> sqlParameters = parameters as SqlParameter[] ?? parameters.ToArray();
                IEnumerable<SqlParameter> sqlParams1 = CopySqlParameter(sqlParameters);
                IEnumerable<SqlParameter> sqlParams2 = CopySqlParameter(sqlParameters);

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

        #endregion

        /// <summary>
        /// 获取表中最大编号
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="fldname"></param>
        /// <returns></returns>
        public static int GetMaxId(string tbname, string fldname)
        {
            using (SqlConnection con = new SqlConnection(Db))
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    //由于没有事务，所以不需要添加锁
                    SqlCommand cmd = new SqlCommand("SELECT MAX(CAST(" + fldname + " AS INT)) FROM " + tbname, con);
                    cmd.CommandTimeout = DbCmdtimeout;
                    string result = cmd.ExecuteScalar().ToString();
                    if (result == "")
                        return 0;
                    else
                        return Convert.ToInt32(result);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 获取表中最大编号
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="fldname"></param>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public static int GetMaxId(string tbname, string fldname, ListSearchParameter listParam)
        {
            using (SqlConnection con = new SqlConnection(Db))
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    //由于没有事务，所以不需要添加锁
                    string strSql = string.Format("SELECT MAX(CAST({0} AS int)) FROM {1} WHERE 1=1 ", fldname, tbname);
                    foreach (SearchParameter searchParameter in listParam)
                    {
                        string colName = searchParameter.CollName;
                        if (colName.StartsWith("@")) colName = colName.Substring(1);
                        strSql += string.Format("AND {0}{1}{2}", colName, searchParameter.Condition, searchParameter.CollValue);
                    }
                    SqlCommand cmd = new SqlCommand(strSql, con);
                    cmd.CommandTimeout = DbCmdtimeout;
                    string result = cmd.ExecuteScalar().ToString();
                    if (result == "")
                        return 0;
                    else
                        return Convert.ToInt32(result);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 获取表中最大编号
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="fldname"></param>
        /// <param name="listParam"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int GetMaxId(string tbname, string fldname, ListSearchParameter listParam, SqlTransaction trans)
        {
            //增加更新锁防止并发 事务未结束前，其他查询处于等待状态，直到事务结束
            string strSql = string.Format("SELECT MAX(CAST({0} AS int)) FROM {1} WITH (UPDLOCK) WHERE 1=1 ", fldname, tbname);
            foreach (SearchParameter searchParameter in listParam)
            {
                string colName = searchParameter.CollName;
                if (colName.StartsWith("@")) colName = colName.Substring(1);
                strSql += string.Format("AND {0}{1}{2}", colName, searchParameter.Condition, searchParameter.CollValue);
            }
            SqlCommand cmd = new SqlCommand(strSql, trans.Connection, trans);
            cmd.CommandTimeout = DbCmdtimeout;
            string result = cmd.ExecuteScalar().ToString();
            if (result == "")
                return 0;
            else
                return Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取表中最大编号,带事务
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tbname"></param>
        /// <param name="fldname"></param>
        /// <returns></returns>
        public static int GetMaxId(SqlTransaction trans, string tbname, string fldname)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                //增加更新锁防止并发 事务未结束前，其他查询处于等待状态，直到事务结束
                string strSql = string.Format("SELECT MAX(CAST({0} AS INT)) FROM {1} WITH (UPDLOCK)", fldname, tbname);
                SqlCommand cmd = new SqlCommand(strSql, trans.Connection);
                cmd.Transaction = trans;
                cmd.CommandTimeout = DbCmdtimeout;
                string result = cmd.ExecuteScalar().ToString();
                if (result == "")
                    return 0;
                else
                    return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据条件查询表中最大编号
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tbname"></param>
        /// <param name="fldname"></param>
        /// <param name="dict">查询条件数据字典</param>
        /// <returns></returns>
        public static int GetMaxId(SqlTransaction trans, string tbname, string fldname, Dictionary<string, string> dict)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                //增加更新锁防止并发 事务未结束前，其他查询处于等待状态，直到事务结束
                string strSql = string.Format("SELECT MAX(CAST({0} AS INT)) FROM {1} WITH (UPDLOCK) WHERE 1=1 ", fldname, tbname);
                foreach (KeyValuePair<string, string> kpr in dict)
                {
                    strSql = strSql + " AND " + kpr.Key + "='" + kpr.Value + "' ";
                }

                SqlCommand cmd = new SqlCommand(strSql, trans.Connection);
                cmd.Transaction = trans;
                cmd.CommandTimeout = DbCmdtimeout;
                string s = cmd.ExecuteScalar().ToString();
                if (s == "")
                    return 0;
                else
                    return Convert.ToInt32(s);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 组合参数
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static SqlParameter[] BuildParameter(Dictionary<string, object> dict)
        {
            SqlParameter[] para = new SqlParameter[dict.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                para[i] = new SqlParameter(kpr.Key, kpr.Value);
                i++;
            }
            return para;
        }

        /// <summary>
        /// 根据条件查询已存在的重复数据行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">查询条件数据字典</param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int GetExistedRows(string strTableName, Dictionary<string, object> dict, string strConn = null)
        {
            string connString = strConn;
            if (string.IsNullOrWhiteSpace(connString))
                connString = GetDbConnString();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        return GetExistedRows(trans, strTableName, dict);
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
            }
        }

        /// <summary>
        /// 根据条件查询已存在的重复数据行数
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">查询条件数据字典</param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int GetExistedRows(SqlTransaction trans, string strTableName, Dictionary<string, object> dict, string strConn = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT COUNT(*) FROM " + strTableName + " WHERE 1=1 ");

            SqlParameter[] parameter = new SqlParameter[dict.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                {
                    strSql.Append(" AND " + kpr.Key.Substring(1) + " = " + kpr.Key);
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value);
                }
                else
                {
                    strSql.Append(" AND " + kpr.Key + " = @" + kpr.Key);
                    parameter[i] = new SqlParameter("@" + kpr.Key, kpr.Value);
                }

                i++;
            }
            if (!string.IsNullOrWhiteSpace(strConn))
                return Convert.ToInt32(ExecuteScalar(strConn, trans, strSql.ToString(), parameter));
            else
                return Convert.ToInt32(ExecuteScalar(trans, strSql.ToString(), parameter));
        }

        /// <summary>
        /// 根据条件查询已存在的重复数据行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParam">查询条件参数</param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int GetExistedRows(string strTableName, ListSearchParameter listParam, string strConn = null)
        {
            string connString = strConn;
            if (string.IsNullOrWhiteSpace(connString))
                connString = GetDbConnString();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        return GetExistedRows(trans, strTableName, listParam);
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
            }
        }


        /// <summary>
        /// 根据条件查询已存在的重复数据行数,返回查询记录数
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParam">查询条件参数</param>
        /// <param name="strConn"></param>
        /// <returns>返回查询记录数</returns>
        public static int GetExistedRows(SqlTransaction trans, string strTableName, ListSearchParameter listParam, string strConn = null)
        {
            string strSql = " SELECT COUNT(*) FROM " + strTableName + " WHERE 1=1 ";

            int i = 0;
            List<SqlParameter> listPara = new List<SqlParameter>();
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else
                {
                    strSql += string.Format(" AND {0} {1} @Para{2}", sParam.CollName.TrimStart('@'), sParam.Condition, i);
                    listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                    i++;
                }
            }
            if (string.IsNullOrWhiteSpace(strConn))
                return Convert.ToInt32(ExecuteScalar(trans, strSql, listPara));
            else
                return Convert.ToInt32(ExecuteScalar(strConn, trans, strSql, listPara));
        }


        /// <summary>
        /// 根据条件查询已存在的重复数据行数,返回查询记录数
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="strWhere">查询条件参数，直接写And a = b，无条件时为空</param>
        /// <returns>返回查询记录数</returns>
        public static int GetExistedRows(SqlTransaction trans, string strTableName, string strWhere = null)
        {
            string strSql = $" SELECT COUNT(*) FROM {strTableName} WHERE 1=1 {strWhere}";
            return Convert.ToInt32(ExecuteScalar(trans, strSql));
        }

        #region Insert

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict, string strConn = null)
        {
            StringBuilder strColumn = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();

            SqlParameter[] parameter = new SqlParameter[dict.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                    strColumn.Append(kpr.Key.Substring(1));
                else
                    strColumn.Append(kpr.Key);
                if (kpr.Key.StartsWith("@"))
                    strParameter.Append(kpr.Key);
                else
                    strParameter.Append("@" + kpr.Key);
                if (i < dict.Count - 1)
                {
                    strColumn.Append(", ");
                    strParameter.Append(", ");
                }
                if (kpr.Key.StartsWith("@"))
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value);
                else
                    parameter[i] = new SqlParameter("@" + kpr.Key, kpr.Value ?? DBNull.Value);

                i++;
            }
            string strSql = string.Format("INSERT INTO {0}({1}) VALUES ({2})", strTableName, strColumn, strParameter);

            if (string.IsNullOrWhiteSpace(strConn))
                return ExecuteNonQuery(strSql, parameter);
            else
                return ExecuteNonQuery(strConn, strSql, parameter);
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParameters">插入字段数据字典</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Insert(string strTableName, List<SqlParameter> listParameters, SqlTransaction trans = null)
        {
            StringBuilder sbColumnName = new StringBuilder();
            StringBuilder sbParameter = new StringBuilder();

            foreach (SqlParameter parameter in listParameters)
            {
                sbColumnName.Append(parameter.ParameterName.Substring(1) + ", ");
                sbParameter.Append(parameter.ParameterName + ", ");
            }
            //去掉最后的, 
            sbColumnName.Remove(sbColumnName.Length - 2, 2);
            sbParameter.Remove(sbParameter.Length - 2, 2);
            string strSql = string.Format("INSERT INTO {0}({1}) VALUES({2})", strTableName, sbColumnName, sbParameter);
            if (trans == null)
                return ExecuteNonQuery(strSql, listParameters);
            else
                return ExecuteNonQuery(trans, strSql, listParameters);
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数。
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <returns>返回受影响的行数。</returns>
        public static int Insert(SqlTransaction trans, string strTableName, Dictionary<string, object> dict)
        {
            if (string.IsNullOrWhiteSpace(strTableName))
            {
                throw new Exception("更新的表名不能为空");
            }
            if (dict.Count == 0)
            {
                throw new Exception("更新的字段不能为空");
            }
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append("INSERT INTO " + strTableName + " ( ");
            strParameter.Append(" VALUES ( ");

            SqlParameter[] parameter = new SqlParameter[dict.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                {
                    if (i < dict.Count - 1)
                    {
                        strSql.Append(kpr.Key.Substring(1) + " , ");
                        strParameter.Append(kpr.Key + " , ");
                    }
                    else
                    {
                        strSql.Append(kpr.Key.Substring(1) + " ) ");
                        strParameter.Append(kpr.Key + " ) ");
                    }
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value);
                    i++;
                }
                else
                {
                    //参数不是以@开头
                    strSql.Append(kpr.Key + " ,");
                    strParameter.Append("@" + kpr.Key + " ,");
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value);
                    if (i == dict.Count - 1)
                    {
                        strSql.Remove(strSql.Length - 1, 1);
                        strParameter.Remove(strParameter.Length - 1, 1);
                        strSql.Append(" ) ");
                        strParameter.Append(" ) ");
                    }
                    i++;
                }
            }
            strSql.Append(strParameter);
            return ExecuteNonQuery(trans, strSql.ToString(), parameter);
        }

        /// <summary>
        /// 通用新增数据函数，执行成功返回1，同时返回自增长标识列当前插入值。
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="id">输出自动增长序列的ID</param>
        /// <returns>返回受影响的行数。</returns>
        public static int Insert(string strTableName, Dictionary<string, object> dict, ref int id)
        {
            using (SqlConnection conn = new SqlConnection(GetDbConnString()))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int dbResult = Insert(trans, strTableName, dict, ref id);
                        trans.Commit();
                        return dbResult;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 通用新增数据函数，执行成功返回1，同时返回自增长标识列当前插入值。
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="id">输出自动增长序列的ID</param>
        /// <returns>返回受影响的行数。</returns>
        public static int Insert(SqlTransaction trans, string strTableName, Dictionary<string, object> dict, ref int id)
        {
            if (string.IsNullOrWhiteSpace(strTableName))
                throw new Exception("更新的表名不能为空");
            if (dict.Count == 0)
                throw new Exception("更新的字段不能为空");
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append("INSERT INTO " + strTableName + " (");
            strParameter.Append(" VALUES (");

            SqlParameter[] parameter = new SqlParameter[dict.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                {
                    if (i < dict.Count - 1)
                    {
                        strSql.Append(kpr.Key.Substring(1) + " , ");
                        strParameter.Append(kpr.Key + " , ");
                    }
                    else
                    {
                        strSql.Append(kpr.Key.Substring(1) + " ) ");
                        strParameter.Append(kpr.Key + " ) ");
                    }
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value);
                    i++;
                }
                else
                {
                    //参数不是以@开头
                    strSql.Append(kpr.Key + " ,");
                    strParameter.Append("@" + kpr.Key + " ,");
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value);
                    if (i == dict.Count - 1)
                    {
                        strSql.Remove(strSql.Length - 1, 1);
                        strParameter.Remove(strParameter.Length - 1, 1);
                        strSql.Append(" ) ");
                        strParameter.Append(" ) ");
                    }
                    i++;
                }
            }
            strSql.Append(strParameter);
            strSql.Append(";SELECT SCOPE_IDENTITY()");
            object obj = ExecuteScalar(trans, strSql.ToString(), parameter);
            if (obj == DBNull.Value)
            {
                return -1;
            }
            id = Convert.ToInt32(obj);
            return 1;
        }

        #endregion

        /// <summary>
        /// 通用更新数据函数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">要更新的字段数据字典</param>
        /// <param name="listParam">更新条件参数集</param>
        /// <returns></returns>
        public static int Update(string strTableName, Dictionary<string, object> dict, ListSearchParameter listParam)
        {
            return Update(null, strTableName, dict, listParam);
        }

        /// <summary>
        /// 通用更新数据函数
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">要更新的字段数据字典</param>
        /// <param name="listParam">更新条件参数集</param>
        /// <returns></returns>
        public static int Update(SqlTransaction trans, string strTableName, Dictionary<string, object> dict, Bonn.Helper.ListSearchParameter listParam)
        {
            string strSql = string.Format("UPDATE {0} SET ", strTableName);
            List<SqlParameter> listPara = new List<SqlParameter>();
            foreach (KeyValuePair<String, Object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                    strSql += kpr.Key.Substring(1) + " = " + kpr.Key + " ,";
                else
                    strSql += kpr.Key + " = @" + kpr.Key + " ,";
                listPara.Add(new SqlParameter(kpr.Key, kpr.Value ?? DBNull.Value));
            }
            strSql = strSql.TrimEnd(',');
            strSql += "WHERE 1=1 ";

            BuildCondition(listParam, ref strSql, ref listPara);

            return trans == null ? ExecuteNonQuery(strSql, listPara) : ExecuteNonQuery(trans, strSql, listPara);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dict"></param>
        /// <param name="listParam"></param>
        /// <param name="trans"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int Update(string strTableName, Dictionary<string, object> dict, Bonn.Helper.ListSearchParameter listParam = null,
            string strConn = null, SqlTransaction trans = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            strSql.Append("UPDATE " + strTableName + " SET ");
            strWhere.Append(" WHERE 1=1 ");

            List<SqlParameter> listPara = new List<SqlParameter>();
            int i = 0;
            foreach (KeyValuePair<String, Object> kpr in dict)
            {
                strSql.AppendFormat("{0} = @Para{1},", kpr.Key.Substring(1), i);
                listPara.Add(new SqlParameter("@Para" + i, kpr.Value ?? DBNull.Value));
                i++;
            }
            strSql.Remove(strSql.Length - 1, 1);    //去除最后一个,
            if (listParam != null)
            {
                foreach (Bonn.Helper.SearchParameter sParam in listParam)
                {
                    strWhere.Append(@" AND " + sParam.CollName.Substring(1) + " " +
                                    (string.IsNullOrWhiteSpace(sParam.Condition) ? "=" : sParam.Condition) + " @Para" + i);
                    listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                    i++;
                }
            }

            strSql.Append(strWhere);

            //不可能trans和strConn都为空
            if (trans == null && string.IsNullOrWhiteSpace(strConn))
                return ExecuteNonQuery(strSql.ToString(), listPara);

            if (trans != null)
                return ExecuteNonQuery(trans, strSql.ToString(), listPara);
            else
                return ExecuteNonQuery(strConn, strSql.ToString(), listPara);
        }

        /// <summary>
        /// 追加更新记录，适用于数值型字段
        /// 
        /// 例子：
        /// dict.Add("+PayNo", 1);
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dict">字段名前第一个字符表示操作类型，如+FactMoney表示FactMoney追加value</param>
        /// <param name="listParam">列名不需要以@开头</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int AppendUpdate(string strTableName, Dictionary<string, object> dict, ListSearchParameter listParam = null,
            SqlTransaction trans = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            strSql.Append("UPDATE " + strTableName + " SET ");
            strWhere.Append(" WHERE 1=1 ");

            List<SqlParameter> listPara = new List<SqlParameter>();
            int i = 0;
            foreach (KeyValuePair<String, Object> kpr in dict)
            {
                string colName = kpr.Key.Substring(1);
                string operType = kpr.Key.Substring(0, 1);//第一个字符为运算符号，比如 + - * 等
                if (kpr.Key.Substring(0, 1) == "=")
                    strSql.AppendFormat($"{colName} = @Para{i},");
                else
                    strSql.AppendFormat($"{colName} = {colName} {kpr.Key.Substring(0, 1)} @Para{i},");
                listPara.Add(new SqlParameter("@Para" + i, kpr.Value ?? DBNull.Value));
                i++;
            }
            strSql.Remove(strSql.Length - 1, 1);    //去除最后一个,
            if (listParam != null)
            {
                foreach (Bonn.Helper.SearchParameter sParam in listParam)
                {
                    strWhere.Append(@" AND " + sParam.CollName.Substring(1) + " " +
                                    (string.IsNullOrWhiteSpace(sParam.Condition) ? "=" : sParam.Condition) + " @Para" + i);
                    listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                    i++;
                }
            }

            strSql.Append(strWhere);

            //不可能trans和strConn都为空
            if (trans == null)
                return ExecuteNonQuery(strSql.ToString(), listPara);
            else
                return ExecuteNonQuery(trans, strSql.ToString(), listPara);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static int Delete(string strTableName, Dictionary<string, object> dict)
        {
            return Delete(null, strTableName, dict);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strTableName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static int Delete(SqlTransaction trans, string strTableName, Dictionary<string, object> dict)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("DELETE FROM " + strTableName);
            strSql.Append(" WHERE 1=1 ");

            SqlParameter[] parameter = new SqlParameter[dict.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                if (kpr.Key.StartsWith("@"))
                {
                    strSql.AppendFormat("AND {0} = {1} ", kpr.Key.Substring(1), kpr.Key);
                    parameter[i] = new SqlParameter(kpr.Key, kpr.Value);
                }
                else
                {
                    strSql.AppendFormat("AND {0} = @Para{1} ", kpr.Key, i);
                    parameter[i] = new SqlParameter("@Para" + i, kpr.Value);
                }
                i++;
            }

            if (trans == null)
                return ExecuteNonQuery(strSql.ToString(), parameter);
            else
                return ExecuteNonQuery(trans, strSql.ToString(), parameter);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strTableName"></param>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public static int Delete(SqlTransaction trans, string strTableName, Bonn.Helper.ListSearchParameter listParam)
        {
            string strSql = string.Empty;
            strSql += "DELETE FROM " + strTableName;
            strSql += " WHERE 1=1 ";

            int i = 0;
            List<SqlParameter> listPara = new List<SqlParameter>();
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else
                {
                    strSql += " AND " + sParam.CollName.Substring(1) + " " + sParam.Condition + " @Para" + i;
                    listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                    i++;
                }
            }
            return ExecuteNonQuery(trans, strSql, listPara);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="listParam"></param>
        /// <param name="trans"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int Delete(string strTableName, Bonn.Helper.ListSearchParameter listParam = null,
            string strConn = null, SqlTransaction trans = null)
        {
            string strSql = string.Empty;
            strSql += "DELETE FROM " + strTableName;
            strSql += " WHERE 1=1 ";

            List<SqlParameter> listPara = new List<SqlParameter>();
            if (listParam != null)
            {
                int i = 0;
                foreach (Bonn.Helper.SearchParameter sParam in listParam)
                {
                    if (sParam.Condition.Trim().ToUpper() == "IN")
                    {
                        strSql += string.Format(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                    }
                    else
                    {
                        strSql += " AND " + sParam.CollName.Substring(1) + " " + sParam.Condition + " @Para" + i;
                        listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                        i++;
                    }
                }
            }

            //不可能trans和strConn都为空
            if (trans == null && string.IsNullOrWhiteSpace(strConn))
                return ExecuteNonQuery(strSql, listPara);

            if (trans != null)
                return ExecuteNonQuery(trans, strSql, listPara);
            else
                return ExecuteNonQuery(strConn, strSql, listPara);
        }

        /// <summary>
        /// 组装条件语句
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="strSqlCondition">输出的条件语句，以AND开头格式如"AND 1 = 1 AND 2 = 2"</param>
        /// <param name="listPara"></param>
        public static void BuildCondition(Bonn.Helper.ListSearchParameter listParam, ref string strSqlCondition,
            ref List<SqlParameter> listPara)
        {
            if (listParam == null) return;
            int i = 0;
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                //由于IN条件后的值不能是字符串，所以需要特殊处理
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSqlCondition += string.Format(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else if (sParam.Condition.Trim().ToUpper() == "NOT IN")
                {
                    strSqlCondition += string.Format(" AND {0} NOT IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else
                {
                    strSqlCondition += " AND " + sParam.CollName.Substring(1) + " " + sParam.Condition + " @Para" + i;
                    listPara.Add(new SqlParameter("@Para" + i, sParam.CollValue));
                    i++;
                }
            }
        }

        /// <summary>
        /// 组装条件语句
        /// </summary>
        /// <param name="listParam"></param>
        public static string BuildCondition(Bonn.Helper.ListSearchParameter listParam)
        {
            string strSql = string.Empty;
            if (listParam == null) return strSql;
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else if (sParam.Condition.Trim().ToUpper() == "NOT IN")
                {
                    strSql += string.Format(" AND {0} NOT IN {1}", sParam.CollName.Substring(1), sParam.CollValue);
                }
                else
                {
                    strSql += string.Format(" AND {0} {1} '{2}' ", sParam.CollName.Substring(1), sParam.Condition, sParam.CollValue);
                }
            }
            return strSql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sqlWhere">格式 如 WHERE 1 = 1 </param>
        /// <param name="trans"></param>
        /// <param name="ascString">格式 order by id desc</param>
        /// <returns></returns>
        public static DataTable GetRecords(string tableName, string sqlWhere = null, SqlTransaction trans = null, string ascString = null)
        {
            string sql = $"SELECT * FROM {tableName} {sqlWhere} {ascString}";
            return trans == null ? QueryDt(sql) : QueryDt(trans, sql);
        }

        #region RunProcedure

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(GetDbConnString()))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDataAdapter.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="sqlParameter">参数集</param>
        /// <param name="outDict">输出信息</param>
        /// <example>
        /// <code>
        /// List&lt;SqlParameter&lt; parameters = new List&lt;SqlParameter&lt;();
        /// parameters.Add(new SqlParameter("@heatSiteId", heatSiteId));
        /// parameters.Add(new SqlParameter { ParameterName = "@replenishWater", SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Output });
        /// //声明一个返回参数字典
        /// Dictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// //调用存储过程
        /// int affectRows = SqlDBHelper.RunProcedure("pkg_GetHeatSiteNum_DaXingSync", parameters, out outDict);
        /// //循环字典，得到输出值
        /// foreach (KeyValuePair&lt; string, object&lt;  keyValuePair in outDict)
        /// {
        ///     switch (keyValuePair.Key)
        ///     {
        ///        case "@boilerPressure":
        ///             //锅炉压力
        ///             dict.Add("boilerPressure", keyValuePair.Value);
        ///             break;     
        ///        case "@replenishWater":
        ///             //补水量
        ///             dict.Add("replenishWater", keyValuePair.Value);
        ///             break;
        ///     }
        /// }
        /// </code>
        /// </example>
        public static int RunProcedure(string storedProcName,
            IEnumerable<SqlParameter> sqlParameter, out Dictionary<string, object> outDict, string connectionString = null)
        {
            string sqlConnString = connectionString;
            if (string.IsNullOrWhiteSpace(sqlConnString))
                sqlConnString = GetDbConnString();

            using (SqlConnection connection = new SqlConnection(sqlConnString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = BuildQueryCommand(connection, storedProcName, sqlParameter);
                    cmd.ExecuteNonQuery();

                    outDict = new Dictionary<string, object>();
                    foreach (SqlParameter parameter in sqlParameter)
                    {
                        if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
                            outDict.Add(parameter.ParameterName, parameter.Value);
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回执行结果
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IEnumerable<SqlParameter> sqlParameter = null, string connectionString = null)
        {
            string sqlConnString = connectionString;
            if (string.IsNullOrWhiteSpace(sqlConnString))
                sqlConnString = GetDbConnString();

            using (SqlConnection connection = new SqlConnection(sqlConnString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = BuildQueryCommand(connection, storedProcName, sqlParameter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回执行结果，包含事务
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///  执行计算用户分捧存储过程，计算附加信息
        ///  List&lt;SqlParameter&lt; listParas = new List&lt;SqlParameter&lt;();
        ///  SqlParameter sqlParameter;
        ///  
        ///  sqlParameter = new SqlParameter();
        ///  sqlParameter.ParameterName = "@groupId";
        ///  sqlParameter.SqlDbType = SqlDbType.Int;
        ///  sqlParameter.Direction = ParameterDirection.Input;
        ///  sqlParameter.Value = groupId;
        ///  listParas.Add(sqlParameter);
        ///  sqlParameter = new SqlParameter();
        ///  sqlParameter.ParameterName = "@freezeDay";
        ///  sqlParameter.SqlDbType = SqlDbType.VarChar;
        ///  sqlParameter.Size = 10;
        ///  sqlParameter.Direction = ParameterDirection.Input;
        ///  sqlParameter.Value = freezeDate.ToString("yyyy-MM-dd");
        ///  listParas.Add(sqlParameter);
        ///  affactRows = SqlDBHelper.RunProcedure(trans, "pkg_AccountUserHeatData2012", listParas);
        ///  if (affactRows &lt;= 0)
        ///  {
        ///      return -1;
        ///  }
        /// </code>
        /// </example>
        public static int RunProcedure(SqlTransaction trans, string storedProcName, List<SqlParameter> sqlParameter)
        {
            int outReturn = -1;
            sqlParameter.Add(new SqlParameter() { ParameterName = "@OutReturn", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });
            SqlCommand cmd = BuildQueryCommand(trans.Connection, storedProcName, sqlParameter);
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            foreach (SqlParameter parameter in sqlParameter)
            {
                if (parameter.Direction == ParameterDirection.ReturnValue)
                {
                    if (parameter.ParameterName == "@OutReturn")
                        outReturn = Convert.ToInt32(parameter.Value);
                }
            }
            return outReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static int ExecProcedure(string storedProcName, List<SqlParameter> sqlParameter)
        {
            using (SqlConnection conn = new SqlConnection(GetDbConnString()))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    int outReturn = -1;
                    sqlParameter.Add(new SqlParameter() { ParameterName = "@OutReturn", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });
                    SqlCommand cmd = BuildQueryCommand(trans.Connection, storedProcName, sqlParameter);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    foreach (SqlParameter parameter in sqlParameter)
                    {
                        if (parameter.Direction == ParameterDirection.ReturnValue)
                        {
                            if (parameter.ParameterName == "@OutReturn")
                                outReturn = Convert.ToInt32(parameter.Value);
                        }
                    }
                    trans.Commit();
                    return outReturn;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int RunProcedure(SqlTransaction trans, string storedProcName, List<SqlParameter> sqlParameter, out int outResult, out string outMsg)
        {
            outResult = -1;
            outMsg = string.Empty;
            Dictionary<string, object> outDict = new Dictionary<string, object>();
            sqlParameter.Add(new SqlParameter() { ParameterName = "@OutResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });
            sqlParameter.Add(new SqlParameter() { ParameterName = "@OutReturn", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });
            sqlParameter.Add(new SqlParameter() { ParameterName = "@outMsg", SqlDbType = SqlDbType.VarChar, Size = 1000, Direction = ParameterDirection.Output });
            SqlCommand cmd = BuildQueryCommand(trans.Connection, storedProcName, sqlParameter);
            cmd.Transaction = trans;

            int affectRows = cmd.ExecuteNonQuery();

            foreach (SqlParameter parameter in sqlParameter)
            {
                if (parameter.Direction == ParameterDirection.Output ||
                    parameter.Direction == ParameterDirection.InputOutput)
                {
                    if (parameter.ParameterName == "@OutResult")
                        outResult = Convert.ToInt32(parameter.Value);
                    else if (parameter.ParameterName == "@outMsg")
                        outMsg = Convert.ToString(parameter.Value);
                }
            }
            return affectRows;
        }

        #endregion

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IEnumerable<SqlParameter> parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandTimeout = DbCmdtimeout;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    //2010-07-03 朱鹏飞 如果参数为空，则认为参数传递结束，跳出循环，提高执行性能
                    if (parameter == null) break;

                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        /// <summary>
        /// 拆分SQL得到表信息和字段信息，用于分页查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="tableInfo"></param>
        /// <param name="columnInfo"></param>
        public static void SplitSql(string strSql, out string tableInfo, out string columnInfo)
        {
            //去掉前后空格
            //去掉开始的SELECT
            string strTemp = strSql.Trim().Remove(0, 6).Trim();
            //转换为大写，检索第一个SELECT和FROM的位置，取中间部分为columnInfo，其他部分为tableInfo
            //转换大写只为检索使用，检索出位置即可。输出时仍然按原字符串内容输出，防止改变字段串内容
            //找到第一个FROM的位置，后面的即为表名
            int indexOfFrom = strTemp.ToUpper().Trim().IndexOf("FROM", StringComparison.Ordinal);
            //得到列
            columnInfo = strTemp.Substring(0, indexOfFrom).Trim();
            //去掉列
            strTemp = strTemp.Remove(0, indexOfFrom).Trim();
            //去掉FROM
            strTemp = strTemp.Remove(0, 4).Trim();
            tableInfo = strTemp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static SqlParameter CopySqlParameter(SqlParameter parameter)
        {
            SqlParameter outParameter = new SqlParameter();
            outParameter.ParameterName = parameter.ParameterName;
            outParameter.DbType = parameter.DbType;
            outParameter.SqlDbType = parameter.SqlDbType;
            outParameter.Size = parameter.Size;
            outParameter.Value = parameter.Value;
            outParameter.SqlValue = parameter.SqlValue;
            outParameter.Direction = parameter.Direction;
            return outParameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static IEnumerable<SqlParameter> CopySqlParameter(IEnumerable<SqlParameter> sqlParams)
        {
            List<SqlParameter> outSqlParameters = new List<SqlParameter>();
            foreach (SqlParameter sqlParameter in sqlParams)
            {
                SqlParameter outParameter = CopySqlParameter(sqlParameter);
                outSqlParameters.Add(outParameter);
            }
            return outSqlParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static void AddSqlPara(this List<SqlParameter> sqlParams, string parameterName, SqlDbType sqlDbType, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter outParameter = new SqlParameter();
            outParameter.ParameterName = parameterName;
            outParameter.SqlDbType = sqlDbType;
            outParameter.Value = value;
            outParameter.Direction = direction;
            sqlParams.Add(outParameter);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static void AddSqlPara(this List<SqlParameter> sqlParams, string parameterName, SqlDbType sqlDbType, int size, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter outParameter = new SqlParameter();
            outParameter.ParameterName = parameterName;
            outParameter.SqlDbType = sqlDbType;
            outParameter.Size = size;
            outParameter.Value = value;
            outParameter.Direction = direction;
            sqlParams.Add(outParameter);
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int GetTotalRows(string strSql)
        {
            DataTable dataTable = QueryDt(strSql);
            if (dataTable == null)
                return 0;

            return dataTable.Rows.Count;
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="querySql"></param>
        /// <param name="listCol"></param>
        /// <returns></returns>
        public static int GetTotalRows(string querySql, ref Dictionary<string, object> listCol)
        {
            string strCalc = "COUNT(*)";
            foreach (KeyValuePair<string, object> keyValuePair in listCol)
            {
                strCalc += $", {keyValuePair.Key}";
            }
            string newSql = $"SELECT {strCalc} FROM ({querySql}) t";
            DataTable dataTable = QueryDt(newSql);
            if (dataTable == null || dataTable.Rows.Count == 0)
                return 0;

            return dataTable.Rows[0][0].ToString().ToInt();
        }
    }
}
