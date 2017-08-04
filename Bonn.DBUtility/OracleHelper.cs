/*
 * Copyright (C) 
 * 
 * 文件名称：OracleHelper.cs
 * 功能说明：
 * 
 * 创建标识：朱鹏飞 2013-10-08 16:18:33
 * 
 * 修改标识：msg 2016-02-01
 * 修改说明：添加有关第三方数据库连接字符串方法
 * 
 * */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using Bonn.Helper;

namespace Bonn.DBUtility
{
    /// <summary>
    /// Oracle数据访问抽象基础类
    /// </summary>
    public class OracleHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString = string.Empty;

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetConnString()
        {
            return ConnectionString;
        }

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。 值 0 指示无限制，在 CommandTimeout 中应避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public static int CmdTimeOut = 30;

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。 值 0 指示无限制，在 CommandTimeout 中应避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public static int DB_CMDTIMEOUT
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
        /// Execute a database query which does not include a select
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="commandType">Command type either stored procedure or SQL</param>
        /// <param name="commandText">Acutall SQL Command</param>
        /// <param name="commandParameters">Parameters to bind to the command</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            // Create a new Oracle command
            OracleCommand cmd = new OracleCommand();

            //Create a connection
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {

                //Prepare the command
                PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);

                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against an existing database transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing database transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleTransaction trans, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = trans.Connection.CreateCommand();
            PrepareCommand(cmd, trans.Connection, trans, commandType, commandText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="connection">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {

            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL语句操作数据库
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, List<OracleParameter> commandParameters = null)
        {

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(commandText, conn);
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    if (commandParameters != null)
                    {
                        foreach (OracleParameter parm in commandParameters)
                            cmd.Parameters.Add(parm);
                    }
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    conn.Close();
                    return val;
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    cmd.Dispose();
                }
            }


        }

        /// <summary>
        /// 执行SQL语句操作数据库
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params OracleParameter[] commandParameters)
        {

            using (OracleConnection conn = new OracleConnection(GetConnString()))
            {
                OracleCommand cmd = new OracleCommand();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    cmd = BuildCommand(conn, commandText, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    conn.Close();
                    return val;
                }
                catch (OracleException ex)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    cmd.Dispose();
                    conn.Dispose();
                }
            }


        }

        /// <summary>
        /// 执行SQL语句操作数据库
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string cmdText)
        {

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                StringBuilder commandText = new StringBuilder(cmdText);
                OracleCommand cmd = new OracleCommand(commandText.ToString(), conn);
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    int val = cmd.ExecuteNonQuery();
                    return val;
                }
                catch (Exception err)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

        }

        #region ExecuteReader

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {

            //Create the command and connection
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, commandType, commandText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行Reader.提供从数据源读取数据行的只进流的方法
        /// </summary>
        /// <param name="commandText">SQL文本命令</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string commandText)
        {

            //Create the command and connection
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, CommandType.Text, commandText, null);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return rdr;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        #endregion


        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// </summary>
        /// <param name="connectionString">命令类型</param>
        /// <param name="strSql">命令文本</param>
        /// <returns></returns>
        public static object ExecuteScalar(string strSql, string connectionString = null)
        {
            OracleCommand cmd = new OracleCommand();
            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, strSql, null);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <returns></returns>
        public static object ExecuteScalar(CommandType commandType, string commandText)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, commandType, commandText, null);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static Object ExecuteScalar(string commandText, List<OracleParameter> commandParameters = null)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, commandText, null);
                if (commandParameters != null)
                {
                    cmd.Parameters.Clear();
                    foreach (OracleParameter parm in commandParameters)
                        cmd.Parameters.Add(parm);
                }
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// Execute an OracleCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connectionString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText,
            params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, commandType, commandText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// Execute an OracleCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(conn, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="connection">一个现有的数据库连接 </param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        ///	<summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        ///	Execute	a OracleCommand (that returns a 1x1 resultset)	against	the	specified SqlTransaction
        ///	using the provided parameters.
        ///	</summary>
        ///	<param name="transaction">A	valid SqlTransaction</param>
        ///	<param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        ///	<param name="commandText">The stored procedure name	or PL/SQL command</param>
        ///	<param name="commandParameters">An array of	OracleParamters used to execute the command</param>
        ///	<returns>An	object containing the value	in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked	or commited, please	provide	an open	transaction.", "transaction");

            // Create a	command	and	prepare	it for execution
            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            // Execute the command & return	the	results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters	from the command object, so	they can be	used again
            cmd.Parameters.Clear();
            return retval;
        }

        #endregion

        /// <summary>
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="commandType">Command type, e.g. stored procedure</param>
        /// <param name="commandText">Command test</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {

            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (OracleParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }

        ///// <summary>
        ///// Converter to use boolean data type with Oracle
        ///// </summary>
        ///// <param name="value">Value to convert</param>
        ///// <returns></returns>
        //public static string OraBit(bool value)
        //{
        //    if (value)
        //        return "Y";
        //    else
        //        return "N";
        //}

        ///// <summary>
        ///// Converter to use boolean data type with Oracle
        ///// </summary>
        ///// <param name="value">Value to convert</param>
        ///// <returns></returns>
        //public static bool OraBool(string value)
        //{
        //    if (value.Equals("Y"))
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// 执行存储过程  (使用该方法切记要手工关闭SqlDataReader和连接)
        ///// </summary>
        ///// <param name="storedProcName">存储过程名</param>
        ///// <param name="parameters">存储过程参数</param>
        ///// <returns>SqlDataReader</returns>
        //public static OracleDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        //{
        //    OracleConnection connection = new OracleConnection(ConnectionString);
        //    OracleDataReader returnReader;
        //    connection.Open();
        //    OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
        //    command.CommandType = CommandType.StoredProcedure;
        //    returnReader = command.ExecuteReader();
        //    //Connection.Close(); 不能在此关闭，否则，返回的对象将无法使用            
        //    return returnReader;
        //}

        #region RunProcedure

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, OracleParameter[] parameters, string tableName)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter oracleDA = new OracleDataAdapter();
                oracleDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                oracleDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_result">int型输出值</param>
        /// <param name="out_msg">输出信息</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref string out_msg, ref int out_result)
        {

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, parameters);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["out_result"].Value != DBNull.Value)
                        out_result = int.Parse(cmd.Parameters["out_result"].Value.ToString());
                    if (cmd.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)cmd.Parameters["out_msg"].Value;
                    connection.Close();

                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }

            }

        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, OracleParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter oracleDA = new OracleDataAdapter();
                oracleDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                oracleDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程（一个decimal型和一个字符型输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_result">decimal型输出数值</param>
        /// <param name="out_msg">输出信息</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref decimal out_result, ref string out_msg)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, parameters);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["out_result"].Value != DBNull.Value)
                        out_result = decimal.Parse(cmd.Parameters["out_result"].Value.ToString());
                    if (cmd.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)cmd.Parameters["out_msg"].Value;
                    connection.Close();
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行存储过程（一个数值型输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_result">输出信息</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref decimal out_result)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, parameters);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["out_result"].Value != DBNull.Value)
                        out_result = decimal.Parse(cmd.Parameters["out_result"].Value.ToString());
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行存储过程（一个字符型输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_msg">输出信息</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref string out_msg)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, parameters);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)cmd.Parameters["out_msg"].Value;
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行存储过程（一个decimal型、一个字符型、一个游标输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_result">decimal型输出数值</param>
        /// <param name="out_msg">输出信息</param>
        /// <param name="io_allrec">游标</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref decimal out_result, ref string out_msg, ref DataSet io_allrec)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    DataSet dataSet = new DataSet();
                    connection.Open();
                    OracleDataAdapter oracleDA = new OracleDataAdapter();
                    oracleDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                    oracleDA.Fill(dataSet);
                    io_allrec = dataSet;
                    if (oracleDA.SelectCommand.Parameters["out_result"].Value != DBNull.Value)
                        out_result = decimal.Parse(oracleDA.SelectCommand.Parameters["out_result"].Value.ToString());
                    if (oracleDA.SelectCommand.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)oracleDA.SelectCommand.Parameters["out_msg"].Value;
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行存储过程（一个string型、一个int型、一个游标输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_msg">输出信息</param>
        /// <param name="out_result">输入标记</param>
        /// <param name="io_allrec">游标</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref string out_msg, ref int out_result, ref DataSet io_allrec)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    DataSet dataSet = new DataSet();
                    connection.Open();
                    OracleDataAdapter oracleDA = new OracleDataAdapter();
                    oracleDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                    oracleDA.Fill(dataSet);
                    io_allrec = dataSet;
                    if (oracleDA.SelectCommand.Parameters["out_result"].Value != DBNull.Value)
                        out_result = int.Parse(oracleDA.SelectCommand.Parameters["out_result"].Value.ToString());
                    if (oracleDA.SelectCommand.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)oracleDA.SelectCommand.Parameters["out_msg"].Value;
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行存储过程（一个字符型、一个游标输出参数）
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数集</param>
        /// <param name="out_msg">输出信息</param>
        /// <param name="io_allrec">游标</param>
        public static void RunProcedure(string storedProcName, OracleParameter[] parameters, ref string out_msg, ref DataSet io_allrec)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    DataSet dataSet = new DataSet();
                    connection.Open();
                    OracleDataAdapter oracleDA = new OracleDataAdapter();
                    oracleDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                    oracleDA.Fill(dataSet);
                    io_allrec = dataSet;
                    if (oracleDA.SelectCommand.Parameters["out_msg"].Value != DBNull.Value)
                        out_msg = (String)oracleDA.SelectCommand.Parameters["out_msg"].Value;
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    out_msg = ex.ToString();
                    throw new Exception(ex.Message);
                }
            }
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="oracleParameter">参数集</param>
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
            IEnumerable<OracleParameter> oracleParameter, out Dictionary<string, object> outDict, string connectionString = null)
        {
            string sqlConnString = connectionString;
            if (string.IsNullOrWhiteSpace(sqlConnString))
                sqlConnString = GetConnString();

            using (OracleConnection connection = new OracleConnection(sqlConnString))
            {
                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, oracleParameter);
                    int affectRows = cmd.ExecuteNonQuery();

                    outDict = new Dictionary<string, object>();
                    foreach (OracleParameter parameter in oracleParameter)
                    {
                        if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
                            outDict.Add(parameter.ParameterName, parameter.Value);
                    }
                    return affectRows;
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

        #endregion

        /// <summary>
        /// 获得记录总数（仅在存储过程分页中使用）
        /// </summary>
        /// <param name="sqlCount">查询语句,含排序部分</param>
        /// <returns></returns>
        public static int GetPagedRecordsCount(string sqlCount)
        {
            string storedProcName = "pkg_page.sp_getrecordcount";	//分页存储过程名称
            int recordsCount = 0;
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleParameter[] para = new OracleParameter[2];
                OracleParameter p1 = new OracleParameter();
                p1.Direction = ParameterDirection.Input;
                p1.OracleType = OracleType.VarChar;
                p1.ParameterName = "p_sqlcount";
                p1.Value = sqlCount;
                p1.Size = 1024;

                OracleParameter p2 = new OracleParameter();
                p2.Direction = ParameterDirection.Output;
                p2.OracleType = OracleType.Number;
                p2.ParameterName = "p_outrecordcount";
                //p2.Value = 0;

                para[0] = p1;
                para[1] = p2;

                try
                {
                    connection.Open();
                    OracleCommand cmd = BuildQueryCommand(connection, storedProcName, para);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["p_outrecordcount"].Value != DBNull.Value)
                    {
                        recordsCount = int.Parse(cmd.Parameters["p_outrecordcount"].Value.ToString());
                    }
                    connection.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return recordsCount;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string sqlString)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(sqlString, connection);
                    command.Fill(ds);
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 返回dataset和datatable名
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="sTableName"></param>
        /// <returns></returns>
        public static DataSet Query(DataSet dsName, string sqlString, string sTableName)
        {
            using (OracleConnection con = new OracleConnection(ConnectionString))
            {
                //DataSet dsName = new DataSet();
                try
                {
                    con.Open();
                    OracleDataAdapter dap = new OracleDataAdapter(sqlString, con);
                    dap.Fill(dsName, sTableName);
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dsName;
            }

        }
        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="sqlStr">sql语句</param>
        /// <returns></returns>
        public static DataSet Query(string connectionString, string sqlStr)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                DataSet dsName = new DataSet();
                try
                {
                    con.Open();
                    OracleDataAdapter dap = new OracleDataAdapter(sqlStr, connectionString);
                    dap.Fill(dsName);
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dsName;
            }
        }

        /// <summary>
        /// 执行查询语句,返回DataTable
        /// </summary>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public static DataTable Query(CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataTable tempTable = new DataTable();
                connection.Open();
                OracleDataAdapter oracleDA = new OracleDataAdapter();
                if (commandType == CommandType.Text)
                {
                    oracleDA.SelectCommand = BuildCommand(connection, commandText, commandParameters);
                }
                else
                {
                    oracleDA.SelectCommand = BuildQueryCommand(connection, commandText, commandParameters);
                }
                oracleDA.Fill(tempTable);
                connection.Close();
                return tempTable;
            }
        }


        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, OracleParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
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
            return command;
        }

        /// <summary>
        /// 连接串的存储过程重载
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IEnumerable<OracleParameter> parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);

            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (OracleParameter parameter in parameters)
                {
                    if (parameter == null) break;
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
        /// 构建 OracleCommand 对象
        /// </summary>
        /// <param name="connection">OracleConnection对象</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameters">OracleParameter参数对象</param>
        /// <returns>OracleCommand对象</returns>		
        private static OracleCommand BuildCommand(OracleConnection connection, string sqlStr, OracleParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(sqlStr, connection);
            command.CommandType = CommandType.Text;
            if (parameters != null)
            {
                foreach (OracleParameter parameter in parameters)
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
        /// 通过命令类型、命令和参数组执行费查询操作
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            // Create a new Oracle command
            OracleCommand cmd = new OracleCommand();
            //Create a connection
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                //Prepare the command
                PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oracleType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleParameter MakeParams(string name, OracleType oracleType, int size, ParameterDirection direction, object value)
        {
            OracleParameter para = new OracleParameter();
            para.Direction = direction;
            para.OracleType = oracleType;
            para.ParameterName = name;
            para.Size = size;
            para.Value = value;
            return para;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oracleType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleParameter MakeParams(string name, OracleType oracleType, ParameterDirection direction, object value)
        {
            OracleParameter para = new OracleParameter();
            para.OracleType = oracleType;
            para.ParameterName = name;
            para.Value = value;
            return para;
        }

        /// <summary>
        /// 根据条件查询已存在的重复数据行数,返回查询记录数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParam">查询条件参数</param>
        /// <example>
        /// <code>
        /// Bonn.Helper.ListSearchParameter listParameter = new ListSearchParameter();
        /// 
        /// listParameter.Clear();
        /// listParameter.Add("AREA_ID", "001");
        /// int rowsCount = OracleHelper.GetExistedRows("T_YKT_MID_AREA", listParameter);
        /// </code>
        /// </example>
        /// <returns>返回查询记录数</returns>
        /// <remarks>
        /// 2015-03-11 朱鹏飞 发现此方法使用参数在某些情况下无法正常返回记录数，原因为参数类型没有指定
        /// </remarks>
        public static int GetExistedRows(string strTableName, Bonn.Helper.ListSearchParameter listParam)
        {
            string strSql = " SELECT COUNT(*) FROM " + strTableName + " WHERE 1=1 ";

            int i = 0;
            List<OracleParameter> listPara = new List<OracleParameter>();
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    continue;
                }
                else if (sParam.Condition.Trim().ToUpper() == "NOT IN")
                {
                    strSql += string.Format(" AND {0} NOT IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    continue;
                }
                else
                {
                    strSql += string.Format("AND {0} {1} :Para{2} ", sParam.CollName.Replace("@", ""), sParam.Condition, i);
                    OracleParameter oracleParameter = new OracleParameter();
                    oracleParameter.Direction = ParameterDirection.Input;
                    oracleParameter.ParameterName = "Para" + i;
                    oracleParameter.Value = sParam.CollValue;
                    listPara.Add(oracleParameter);
                    i++;
                }
            }
            return Convert.ToInt32(ExecuteScalar(strSql, listPara));
        }

        /// <summary>
        /// 根据条件查询已存在的重复数据行数,返回查询记录数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParam">查询条件参数</param>
        /// <example>
        /// <code>
        /// Bonn.Helper.ListSearchParameter listParameter = new ListSearchParameter();
        /// 
        /// listParameter.Clear();
        /// listParameter.Add("AREA_ID", "001");
        /// int rowsCount = OracleHelper.GetExistedRows("T_YKT_MID_AREA", listParameter);
        /// </code>
        /// </example>
        /// <returns>返回查询记录数</returns>
        /// <remarks>
        /// 2015-03-11 朱鹏飞 发现此方法使用参数在某些情况下无法正常返回记录数，原因为参数类型没有指定
        /// </remarks>
        public static int GetExistedRows(string strTableName, List<OracleExtParameter> listParam)
        {
            string strSql = " SELECT COUNT(*) FROM " + strTableName + " WHERE 1=1 ";

            int i = 0;
            List<OracleParameter> listPara = new List<OracleParameter>();
            foreach (OracleExtParameter sParam in listParam)
            {
                strSql += string.Format("AND {0} {1} :{2} ", sParam.CollName, sParam.Condition, sParam.OracleParameter.ParameterName);
                listPara.Add(sParam.OracleParameter);
                i++;
            }
            return Convert.ToInt32(ExecuteScalar(strSql, listPara));
        }

        #region Insert

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="seqName"></param>
        /// <param name="seqValue"></param>
        /// <example>
        /// <code>
        /// //声明一个数据字典
        /// Dictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// 
        /// //向字典中添加数据
        /// dict.Add("buildingId", 0);
        /// dict.Add("boilerId", 0);
        /// 
        /// //向数据库插入数据
        /// OracleHelper.Insert("TableName", dict);
        /// </code>
        /// </example>
        /// <remarks>
        /// 2015-03-06 北控三星同步时发现有可能出现未将对象引用到实例的错误，怀疑可能是参数未指定数值类型和长度引起，暂未解决
        /// </remarks>
        /// <returns></returns>
        [Obsolete("此方法存在一定的问题，尽量使用新的方法")]
        public static int Insert(string strTableName, Dictionary<string, object> dict, string seqName = null,
            string seqValue = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append("INSERT INTO " + strTableName + " ( ");
            strParameter.Append(" VALUES ( ");

            OracleParameter[] parameter = new OracleParameter[dict.Count];
            if (!string.IsNullOrWhiteSpace(seqName) && !string.IsNullOrWhiteSpace(seqValue))
            {
                //序列名称不为空时，添加序列名称和值
                strSql.AppendFormat(" {0},", seqName);
                strParameter.AppendFormat("{0},", seqValue);
            }
            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                strSql.AppendFormat(" {0},", kpr.Key);
                strParameter.AppendFormat(":{0},", kpr.Key);
                parameter[i] = new OracleParameter(kpr.Key, kpr.Value);
                i++;
            }
            //去掉最后的,
            strSql.Remove(strSql.Length - 1, 1);
            strSql.Append(")");
            strParameter.Remove(strParameter.Length - 1, 1);
            strParameter.Append(")");
            strSql.Append(strParameter);
            return ExecuteNonQuery(strSql.ToString(), parameter);
        }

        /// <summary>
        /// 通用新增数据函数，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="listParameter">插入字段数据字典</param>
        /// <param name="seqName"></param>
        /// <param name="seqValue"></param>
        /// <example>
        /// <code>
        /// //声明一个数据字典
        /// Dictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// 
        /// //向字典中添加数据
        /// dict.Add("buildingId", 0);
        /// dict.Add("boilerId", 0);
        /// 
        /// //向数据库插入数据
        /// OracleHelper.Insert("TableName", dict);
        /// </code>
        /// </example>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static int Insert(string strTableName, ListOracleExtParameter listParameter, string seqName = null, string seqValue = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append("INSERT INTO " + strTableName + " ( ");
            strParameter.Append(" VALUES ( ");

            OracleParameter[] parameter = new OracleParameter[listParameter.Count];
            if (!string.IsNullOrWhiteSpace(seqName) && !string.IsNullOrWhiteSpace(seqValue))
            {
                //序列名称不为空时，添加序列名称和值
                strSql.AppendFormat(" {0},", seqName);
                strParameter.AppendFormat("{0},", seqValue);
            }
            int i = 0;
            foreach (OracleExtParameter oracleExtParameter in listParameter)
            {
                strSql.AppendFormat(" {0},", oracleExtParameter.CollName);
                strParameter.AppendFormat(":{0},", oracleExtParameter.CollName);
                parameter[i] = oracleExtParameter.OracleParameter;
                i++;
            }
            //去掉最后的,
            strSql.Remove(strSql.Length - 1, 1);
            strSql.Append(")");
            strParameter.Remove(strParameter.Length - 1, 1);
            strParameter.Append(")");
            strSql.Append(strParameter);
            return ExecuteNonQuery(strSql.ToString(), parameter);
        }

        /// <summary>
        /// 通用新增数据函数(不使用参数，直接使用SQL语句)，返回受影响的行数
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="seqName"></param>
        /// <param name="seqValue"></param>
        /// <example>
        /// <code>
        /// //声明一个数据字典
        /// Dictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// 
        /// //向字典中添加数据
        /// dict.Add("buildingId", 0);
        /// dict.Add("boilerId", 0);
        /// 
        /// //向数据库插入数据
        /// OracleHelper.Insert("TableName", dict);
        /// </code>
        /// </example>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static int InsertWithSql(string strTableName, Dictionary<string, object> dict, string seqName = null, string seqValue = null)
        {
            string strSql;
            return InsertWithSql(strTableName, dict, out strSql, seqName, seqValue);
        }

        /// <summary>
        /// 通用新增数据函数(不使用参数，直接使用SQL语句)，返回受影响的行数，输出SQL语句，供测试与分析使用
        /// </summary>
        /// <param name="strTableName">数据表名</param>
        /// <param name="dict">插入字段数据字典</param>
        /// <param name="strSql"></param>
        /// <param name="seqName"></param>
        /// <param name="seqValue"></param>
        /// <example>
        /// <code>
        /// //声明一个数据字典
        /// Dictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// 
        /// //向字典中添加数据
        /// dict.Add("buildingId", 0);
        /// dict.Add("boilerId", 0);
        /// 
        /// //向数据库插入数据
        /// OracleHelper.Insert("TableName", dict);
        /// </code>
        /// </example>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static int InsertWithSql(string strTableName, Dictionary<string, object> dict, out string strSql, string seqName = null, string seqValue = null)
        {
            StringBuilder strSqlColName = new StringBuilder();
            StringBuilder strSqlColValue = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(seqName) && !string.IsNullOrWhiteSpace(seqValue))
            {
                //序列名称不为空时，添加序列名称和值
                strSqlColName.AppendFormat(" {0},", seqName);
                strSqlColValue.AppendFormat("{0},", seqValue);
            }
            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                string keyFalg = "";
                if (kpr.Key.Contains('|'))
                {
                    string[] temp = kpr.Key.Split('|');
                    keyFalg = temp[0].ToUpper();
                    strSqlColName.AppendFormat(" {0},", temp[1]);
                }
                else
                    strSqlColName.AppendFormat(" {0},", kpr.Key);

                if (kpr.Value is string)
                    strSqlColValue.AppendFormat("'{0}',", kpr.Value);
                else if (kpr.Value is int)
                    strSqlColValue.AppendFormat("{0},", kpr.Value);
                else if (kpr.Value is decimal)
                    strSqlColValue.AppendFormat("{0},", kpr.Value);
                else if (kpr.Value is double)
                    strSqlColValue.AppendFormat("{0},", kpr.Value);
                else if (kpr.Value is DateTime)
                {
                    if (!string.IsNullOrEmpty(keyFalg) && keyFalg.Equals("TIMESTAMP"))
                        strSqlColValue.AppendFormat("{0},", "TO_TIMESTAMP('" + ((DateTime)kpr.Value).ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd HH24:mi:ss')");
                    else
                        strSqlColValue.AppendFormat("{0},", "TO_DATE('" + ((DateTime)kpr.Value).ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd HH24:mi:ss')");
                }
                else
                    strSqlColValue.AppendFormat("{0},", kpr.Value);
                i++;

            }
            //去掉最后的,
            strSqlColName.Remove(strSqlColName.Length - 1, 1);
            strSqlColValue.Remove(strSqlColValue.Length - 1, 1);
            strSql = string.Format("INSERT INTO {0}({1}) VALUES({2})", strTableName, strSqlColName, strSqlColValue);

            //throw new Exception("ORA-031131");//测试通信通道结尾问题
            //throw new Exception("ORA-12170");//测试通信超时问题

            return ExecuteNonQuery(strSql);
        }

        #endregion

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dict"></param>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public static int Update(string strTableName, Dictionary<string, object> dict, Bonn.Helper.ListSearchParameter listParam)
        {
            string strSql = "UPDATE " + strTableName + " SET ";

            OracleParameter[] parameter = new OracleParameter[dict.Count + listParam.Count];

            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                strSql += string.Format(" {0}=:{1},", kpr.Key, kpr.Key);
                parameter[i] = new OracleParameter(kpr.Key, kpr.Value);
                i++;
            }
            //去掉最后多出的逗号
            strSql = strSql.Substring(0, strSql.Length - 1);
            strSql = strSql + " WHERE 1=1 ";
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                }
                else
                {
                    strSql += string.Format(" AND {0} {1}:{0} ", sParam.CollName.Replace("@", ""), sParam.Condition);
                    parameter[i] = new OracleParameter(sParam.CollName.Replace("@", ""), sParam.CollValue);
                    i++;
                }
            }

            return ExecuteNonQuery(strSql, parameter);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dict"></param>
        /// <param name="listParam"></param>
        /// <param name="strConn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Update(string strTableName, Dictionary<string, object> dict, Bonn.Helper.ListSearchParameter listParam = null,
            string strConn = null, OracleTransaction trans = null)
        {
            string strSql = "";
            StringBuilder strWhere = new StringBuilder();
            strSql += "UPDATE " + strTableName + " SET ";
            strWhere.Append(" WHERE 1=1 ");

            OracleParameter[] parameter;
            if (listParam == null)
                parameter = new OracleParameter[dict.Count];
            else
                parameter = new OracleParameter[dict.Count + listParam.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> kpr in dict)
            {
                string keyFalg = "";
                if (kpr.Key.Contains('|'))
                {
                    string[] temp = kpr.Key.Split('|');
                    keyFalg = temp[0].ToUpper();
                    strSql += string.Format(" {0}=:{1},", temp[1], temp[1]);
                    parameter[i] = new OracleParameter(temp[1], kpr.Value);
                }
                else
                {
                    strSql += string.Format(" {0}=:{1},", kpr.Key, kpr.Key);
                    parameter[i] = new OracleParameter(kpr.Key, kpr.Value);
                }
                i++;
            }
            //去掉最后多出的逗号
            strSql = strSql.Substring(0, strSql.Length - 1);
            if (listParam != null)
            {
                strSql = strSql + " WHERE 1 = 1 ";
                foreach (Bonn.Helper.SearchParameter sParam in listParam)
                {
                    if (sParam.Condition.Trim().ToUpper() == "IN")
                    {
                        strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    }
                    else
                    {
                        strSql += string.Format(" AND {0} {1}:{0} ", sParam.CollName.Replace("@", ""), sParam.Condition);
                        parameter[i] = new OracleParameter(sParam.CollName.Replace("@", ""), sParam.CollValue);
                        i++;
                    }
                }
            }
            if (trans == null && string.IsNullOrWhiteSpace(strConn))
                return ExecuteNonQuery(strSql, parameter);

            if (trans != null)
                return ExecuteNonQuery(trans, strSql, parameter);
            else
                return ExecuteNonQuery(strConn, strSql, parameter);
        }

        ///// <summary>
        ///// 删除数据
        ///// </summary>
        ///// <param name="strTableName"></param>
        ///// <param name="listParam"></param>
        ///// <returns></returns>
        //public static int Delete(string strTableName, Bonn.Helper.ListSearchParameter listParam)
        //{
        //    string strSql = "DELETE " + strTableName;

        //    OracleParameter[] parameter = new OracleParameter[listParam.Count];
        //    int i = 0;
        //    strSql = strSql + " WHERE 1=1 ";
        //    foreach (Bonn.Helper.SearchParameter sParam in listParam)
        //    {
        //        if (sParam.Condition.Trim().ToUpper() == "IN")
        //        {
        //            strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
        //        }
        //        else
        //        {
        //            strSql += string.Format(" AND {0} {1} :{0} ", sParam.CollName.Replace("@", ""), sParam.Condition);
        //            parameter[i] = new OracleParameter(sParam.CollName.Replace("@", ""), sParam.CollValue);
        //            i++;
        //        }
        //    }

        //    return ExecuteNonQuery(strSql, parameter);
        //}

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="listParam"></param>
        /// <param name="strConn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Delete(string strTableName, ListSearchParameter listParam = null,
            string strConn = null, OracleTransaction trans = null)
        {
            string strSql = "DELETE FROM " + strTableName;
            strSql += " WHERE 1=1 ";

            OracleParameter[] parameter;
            if (listParam == null)
            {
                parameter = new OracleParameter[0];
            }
            else
            {
                parameter = new OracleParameter[listParam.Count];
                int i = 0;
                foreach (Bonn.Helper.SearchParameter sParam in listParam)
                {
                    if (sParam.Condition.Trim().ToUpper() == "IN")
                    {
                        strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    }
                    else
                    {
                        strSql += string.Format(" AND {0} {1}:{0} ", sParam.CollName.Replace("@", ""), sParam.Condition);
                        parameter[i] = new OracleParameter(sParam.CollName.Replace("@", ""), sParam.CollValue);
                        i++;
                    }
                }
            }

            if (trans == null && string.IsNullOrWhiteSpace(strConn))
                return ExecuteNonQuery(strSql, parameter);

            if (trans != null)
                return ExecuteNonQuery(trans, strSql, parameter);

            return ExecuteNonQuery(strConn, strSql, parameter);
        }

        /// <summary>
        /// 对连接执行  语句并返回受影响的行数。带事务操作
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="str"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OracleTransaction trans, string str, IEnumerable<OracleParameter> sqlParams)
        {
            try
            {
                if (trans.Connection.State == ConnectionState.Closed)
                {
                    trans.Connection.Open();
                }
                OracleCommand command = new OracleCommand(str, trans.Connection);
                command.CommandTimeout = DB_CMDTIMEOUT;
                command.Transaction = trans;
                foreach (OracleParameter parameter in sqlParams)
                {
                    command.Parameters.Add(parameter);
                }
                int val = command.ExecuteNonQuery();
                command.Parameters.Clear();
                return val;
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
        public static int ExecuteNonQuery(string connStr, string str, IEnumerable<OracleParameter> sqlParams)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    try
                    {
                        OracleCommand command = new OracleCommand(str, connection);
                        command.CommandTimeout = DB_CMDTIMEOUT;
                        foreach (OracleParameter sqlPara in sqlParams)
                        {
                            command.Parameters.Add(sqlPara);
                        }
                        int val = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        return val;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="listParam"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int GetExistedRows(string strTableName, ListSearchParameter listParam, string strConn = null)
        {
            string connString = strConn;
            if (string.IsNullOrWhiteSpace(connString))
                connString = GetConnString();

            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        return GetExistedRows(trans, strTableName, listParam);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="strTableName"></param>
        /// <param name="listParam"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static int GetExistedRows(OracleTransaction trans, string strTableName, Bonn.Helper.ListSearchParameter listParam, string strConn = null)
        {
            string strSql = " SELECT COUNT(*) FROM " + strTableName + " WHERE 1=1 ";
            int i = 0;
            List<OracleParameter> listPara = new List<OracleParameter>();
            foreach (Bonn.Helper.SearchParameter sParam in listParam)
            {
                if (sParam.Condition.Trim().ToUpper() == "IN")
                {
                    strSql += string.Format(" AND {0} IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    continue;
                }
                else if (sParam.Condition.Trim().ToUpper() == "NOT IN")
                {
                    strSql += string.Format(" AND {0} NOT IN {1} ", sParam.CollName.Replace("@", ""), sParam.CollValue);
                    continue;
                }
                else
                {
                    strSql += string.Format("AND {0} {1}:Para{2} ", sParam.CollName.Replace("@", ""), sParam.Condition, i);
                    OracleParameter oracleParameter = new OracleParameter();
                    oracleParameter.Direction = ParameterDirection.Input;
                    oracleParameter.ParameterName = "Para" + i;
                    oracleParameter.Value = sParam.CollValue;
                    listPara.Add(oracleParameter);
                    i++;
                }
            }
            if (string.IsNullOrWhiteSpace(strConn))
                return Convert.ToInt32(ExecuteScalar(trans, strSql, listPara));
            else
                return Convert.ToInt32(ExecuteScalar(strConn, trans, strSql, listPara));

        }
        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(OracleTransaction trans, string cmdText, IEnumerable<OracleParameter> cmdParms)
        {
            OracleCommand cmd = new OracleCommand(cmdText, trans.Connection, trans);
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            object val = cmd.ExecuteScalar();
            return val;
        }

        /// <summary>
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为.Net的数据类型返回。忽略额外的列或行。
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connStr, OracleTransaction trans, string cmdText, IEnumerable<OracleParameter> cmdParms)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.Transaction = trans;
                cmd.CommandTimeout = DB_CMDTIMEOUT;
                if (cmdParms != null)
                {
                    foreach (OracleParameter parm in cmdParms)
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
    }
}
