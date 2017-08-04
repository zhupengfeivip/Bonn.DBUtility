/*
 * 
 * 
 * 功能说明：
 * 
 * 创建标识：
 * 
 * 修改标识：朱鹏飞
 * 修改说明：
 * 
 * */
using System;
using System.Data;
using System.Data.OleDb;

namespace Bonn.Helper
{
    /// <summary>
    /// Excle文件处理助手
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 读取Excel文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable ImportExcelToDataTable(string fileName)
        {
            //连接定义
            string xlsDriver = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";
            OleDbConnection cn = new OleDbConnection(string.Format(xlsDriver, fileName));
            cn.Open();

            try
            {
                DataTable schema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                //取得第一个表名    
                string tableName = schema.Rows[0]["TABLE_NAME"].ToString();
                //读取数据
                OleDbDataAdapter da = new OleDbDataAdapter("select * from [" + tableName + "] ", cn);
                DataTable dtExcel = new DataTable();
                //数据放入到ds中
                da.Fill(dtExcel);
                da.Dispose();
                cn.Dispose();
                //返回
                return dtExcel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn != null)
                {
                    cn.Dispose();
                }
            }
        }
    }
}
