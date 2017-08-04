using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Bonn.DBUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlConnectionTest
    {
        /// <summary>
        /// 
        /// </summary>
        private string connectionString;
        
        /// <summary>
        /// 
        /// </summary>
        private int maxTestTime;

        /// <summary>
        /// 
        /// </summary>
        public event DBConnectionTestEventHandler ConnectionTestResult;

        /// <summary>
        /// 开始连接测试
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="maxTestTime">最大允许的测试时间，以秒为单位</param>
        public void BeginTest(string connectionString, int maxTestTime)
        {
            string newTimeout = "Connection Timeout=" + maxTestTime;

            if (connectionString.EndsWith(";"))
            {
                connectionString += newTimeout;
            }
            else
            {
                connectionString += (";" + newTimeout);
            }

            this.connectionString = connectionString + ";Pooling=false";
            this.maxTestTime = maxTestTime;

            Thread t = new Thread(new ThreadStart(TestThread));
            t.IsBackground = true;
            t.Name = "数据库连接测试线程";
            t.Start();
        }

        private void OnConnectionTest(bool isSuccessed)
        {
            if (ConnectionTestResult != null)
            {
                ConnectionTestResult(this, new DBConnectionTestEventArgs(isSuccessed));
            }
        }

        private void TestThread()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    if (cn.State == ConnectionState.Open)
                    {
                        OnConnectionTest(true);
                    }
                }
            }
            catch (Exception)
            {
                OnConnectionTest(false);
            }

        }
    }

    /// <summary>
    /// 委拖
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DBConnectionTestEventHandler(Object sender, DBConnectionTestEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class DBConnectionTestEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccessed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccessed"></param>
        public DBConnectionTestEventArgs(bool isSuccessed)
        {
            this.IsSuccessed = isSuccessed;
        }
    }
}
