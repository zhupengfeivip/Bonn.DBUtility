using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonn.SqliteUtility
{
    public class SqliteHelper
    {
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetConnString(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("未设置数据库连接字符串");

            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
        }
    }
}
