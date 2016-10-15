using LuckyGitlabStatWebAPI.DAO;
using LuckyGitlabStatWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;


namespace LuckyGitlabStatWebAPI.Controllers
{
    public class CommonController : ApiController
    {
        //查询验证信息
        string checkInfo;
        //邮件发送信息
        string sendInfo;
        //用户密码
        string password;
        ConnectDB conncetdb = new ConnectDB();
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();
        bool flag;
        public CommonController()
        {
            /*Empty*/
        }
        #region MyRegion


        /// <summary>
        /// 检查用户是否已经注册/存在异常
        /// </summary>
        /// <param name="username">用户名（主键）</param>
        /// <returns>true  已注册</returns>
        /// <returns>false 未注册</returns>
        public bool GetCheckUser(string username)
        {
            Member member = new Member();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT * FROM Member where UserName=" + "'" + username + "'";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                member.username = singleInfoReader["UserName"].ToString().Trim();
                if (member.username.Equals(username))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            //try
            //{
            //    SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
            //    using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
            //    {
            //        using (SqlCommand command = conn.CreateCommand())
            //        {
            //            conn.Open();
            //            command.CommandText = "SELECT * FROM Member where UserName=" + "'" + username + "'";
            //            //ExecuteReader()：将 CommandText 发送到 Connection，并生成 SqlDataReader。
            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                // Loop over the results 
            //                while (reader.Read())//SqlDataReader到下一个记录
            //                {
            //                    member.username = reader["UserName"].ToString().Trim();
            //                    if (member.username.Equals(username))
            //                    {
            //                        flag = true;
            //                    }
            //                    else
            //                    {
            //                        flag = false;
            //                    }
            //                }
            //            }
            //            conn.Close();
            //        }
            //    }
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine("用户检查异常");
            //}
            return flag;
        }
        
        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>true/false</returns>
        public bool GetLoginCheck(string username, string password)
        {
            Member member = new Member();
            if (GetCheckUser(username))//检查是否已被注册
            {
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    //打开数据库
                    conn.Open();
                    //创建查询语句
                    SqlCommand querySingleInfo = conn.CreateCommand();
                    querySingleInfo.CommandText = "SELECT Password FROM Member where UserName=" + "'" + username + "'";
                    SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                    //有多行数据，用while循环
                    while (singleInfoReader.Read())
                    {
                        member.password = singleInfoReader["Password"].ToString().Trim();
                    }
                    if (member.password.Equals(password))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }

                    //关闭查询
                    singleInfoReader.Close();
                    //关闭数据库连接
                    conn.Close();
                }
                catch { flag = false; }
                //try
                //{
                //    SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
                //    using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                //    {
                //        using (SqlCommand command = conn.CreateCommand())
                //        {
                //            conn.Open();
                //            command.CommandText = "SELECT Password FROM Member where UserName=" + "'" + username + "'";
                //            using (SqlDataReader reader = command.ExecuteReader())
                //            {
                //                // 若是查询数据有多行，循环读取输出 
                //                while (reader.Read())
                //                {
                //                    member.password = reader["Password"].ToString().Trim();
                //                }
                //                if (member.password.Equals(password))
                //                {
                //                    flag = true;
                //                }
                //                else
                //                {
                //                    flag = false;
                //                }
                //            }
                //        }

                //    }
                //}
                //catch (SqlException e)
                //{
                //    Console.WriteLine("登陆异常/用户尚未注册");
                //    flag = false;
                //}
            }
            else
                flag = false;
            return flag;
        }

        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回管理员级别</returns>
        public int GetLoginCheckWithRank(string username, string password)
        {
            Member member = new Member();
            //表示无法登陆
            int returnNumber = 0;
            if (GetCheckUser(username))//检查是否已被注册
            {
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    //打开数据库
                    conn.Open();
                    //创建查询语句
                    SqlCommand querySingleInfo = conn.CreateCommand();
                    querySingleInfo.CommandText = "SELECT Rank, Password FROM Member where UserName=" + "'" + username + "'";
                    SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                    //有多行数据，用while循环
                    while (singleInfoReader.Read())
                    {
                        member.password = singleInfoReader["Password"].ToString().Trim();
                        member.rank = singleInfoReader["Rank"].ToString().Trim();
                    }
                    if (member.password.Equals(password))
                    {
                        returnNumber =int.Parse( member.rank);
                    }
                    else
                    {
                        returnNumber=0;
                    }

                    //关闭查询
                    singleInfoReader.Close();
                    //关闭数据库连接
                    conn.Close();
                }
                catch { returnNumber = 0; }
            }
            else
                returnNumber = 0;;
            return returnNumber;
        }
        
        /// <summary>
        /// 获取用户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>返回查询结果</returns>
        public string Getpassrord(string username)
        {
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT Password FROM Member where UserName=" + "'" + username + "'";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    password = singleInfoReader["Password"].ToString().Trim();
                }
                //关闭查询
                singleInfoReader.Close();
                //关闭数据库连接
                conn.Close();
            }
            catch(SqlException e)
            {
                password ="查询失败";
            }
            return password;
        }
       
        /// <summary>
        /// 发送密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">新密码</param>
        /// <returns></returns>
        public string GetSendpassrord(string username,string email)
        {
            Member member = new Member();
            if (GetCheckUser(username))//检查是否已被注册
            {
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    //打开数据库
                    conn.Open();
                    //创建查询语句
                    SqlCommand querySingleInfo = conn.CreateCommand();
                    querySingleInfo.CommandText = "SELECT Email FROM Member where UserName=" + "'" + username + "'";
                    SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                    //有多行数据，用while循环
                    while (singleInfoReader.Read())
                    {
                        member.email = singleInfoReader["Email"].ToString().Trim();
                    }
                    if (member.email.Equals(email))
                    {
                        string pwd = "你的密码是: ";
                        //获取密码
                        pwd += Getpassrord( username);
                        pwd += " , 请妥善保存";
                        //发送邮件
                        checkInfo = SecdEmail(email,pwd);
                    }
                    else
                    {
                        checkInfo = "邮箱错误";
                    }
                }
                catch(SqlException e)
                {
                    Console.WriteLine("查询错误");
                    checkInfo = "查询错误";
                }
            }
            else
                checkInfo = "用户名不存在";
            return checkInfo;
        }
       
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">邮件</param>
        /// <param name="password">授权码</param>
        /// <returns></returns>
        public string SecdEmail(string email,string message)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();

                smtpClient.EnableSsl = true;

                smtpClient.UseDefaultCredentials = true;

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式        

                smtpClient.Host = "smtp.qq.com";//指定SMTP服务器        

                smtpClient.Credentials = new System.Net.NetworkCredential("1312373957@qq.com", "ccfknzwurpfjjhic");//用户名和授权码

                // 发送邮件设置        

                MailMessage mailMessage = new MailMessage("1312373957@qq.com", email); // 发送人和收件人        

                mailMessage.Subject = "密码找回";//主题        

                mailMessage.Body = message;

                mailMessage.BodyEncoding = Encoding.UTF8;//正文编码        

                mailMessage.IsBodyHtml = true;//设置为HTML格式        

                mailMessage.Priority = MailPriority.High;//优先级
                smtpClient.Send(mailMessage);
                sendInfo="发送成功";
            }
            catch (Exception e)
            {
                sendInfo = "发送失败";
            }
            return sendInfo;
        }
        #endregion
        /// <summary>
        /// 获取程序版本号
        /// </summary>
        /// <returns></returns>
        public string GetProjectVersion()
        {
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return ver.Substring(0,5);
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">邮箱地址</param>
        /// <returns></returns>
        public string GetUpdateUserInfo(string username,string password,string email)
        {
            string returnInfo = null;
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "update Member set Password=" + "'" + password + "',Email=" + "'" + email + "' where username=" + "'" + username + "'";
                cmd.ExecuteNonQuery();
                flag = true;
                returnInfo += "更新成功";
            }
            catch (SqlException e)
            {
                flag = false;
                FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(e.ToString()); // 写入
                sw.Close();
                returnInfo += "更新异常";
            }
            return returnInfo;
        }
    }
}


