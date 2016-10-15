using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LuckyGitlabStatWebAPI.DAO
{
    public class ConnectLocalSQL
    {
        /// <summary>
        /// 连接本地Sql Server数据库
        /// </summary>
        /// <returns></returns>
        public SqlConnection ConnectDataBase()
        {
            //连接字符串用SQL Server身份验证的账户登录，不能用windows身份验证的用户
            SqlConnection conn = new SqlConnection("Data Source=202.196.96.79;Initial Catalog=LuckyGitlabStats;Persist Security Info=True;User ID=lucky;password=cuichongyang_13462420045");
            //SqlConnection conn = new SqlConnection("Data Source=DESKTOP-OB976GA\\SQLEXPRESS;Initial Catalog=LuckyGitLabStat;Persist Security Info=True;User ID=xiehongguang;password=xiehongguang_123");
            return conn;
        }
    }
}