using LuckyGitlabStatWebAPI.DAO;
using LuckyGitlabStatWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LuckyGitlabStatWebAPI.Controllers
{
    public class RootController : ApiController
    {
        bool flag;
        //返回信息
        string returnInfo = "异常";
        //连接本地数据库
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();
        Member member = new Member();
        List<Member> members = new List<Member>();
        public RootController() { /*Empty*/}
        #region MyRegion

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        public List<Member> GetAllUserInfo()
        {
            //FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs); // 创建写入流
            //sw.WriteLine("11111111111111111111"); // 写入
            //sw.WriteLine("++"); // 写入
            //sw.Close();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT * FROM Member ";
            SqlDataReader userInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (userInfoReader.Read())
            {
                Member member = new Member();
                member.username = userInfoReader["UserName"].ToString().Trim();
                member.password = userInfoReader["Password"].ToString().Trim();
                member.email = userInfoReader["Email"].ToString().Trim();
                member.sex = userInfoReader["Sex"].ToString().Trim();
                member.rank = userInfoReader["Rank"].ToString().Trim();
                member.groupName = userInfoReader["Groupname"].ToString().Trim();
                member.isdelete = userInfoReader["IsDelete"].ToString().Trim();
                members.Add(member);
            }
            //关闭查询
            userInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return members;
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="member">注册人信息的对象</param>
        /// <returns>true/false</returns>
        public string PostRegister([FromBody]Member member)
        {
            CommonController common = new CommonController();
            int rankNumber = 3;
            string returnInfo="";
            //判断用户名是否为空
            if (member.username == null || member.username == "")
            {
                returnInfo="Username is null. ";
            }
            if (common.GetCheckUser(member.username) == false)
            {
                member.sex = (member.sex == "male" ? "true" : "false");
                if (member.rank == "Root")
                    rankNumber = 1;
                else
                     if (member.rank == "Monitor")
                    rankNumber = 2;
                else
                    rankNumber = 3;
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    conn.Open();
                    string sql = "INSERT INTO Member(username,password,sex,rank,groupname,isdelete,buildTime) VALUES ('" + member.username + "','" + member.username + "'+'_123','" + member.sex + "'," + rankNumber + ",'" + member.groupName + "',0,getdate())";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    returnInfo = "Success";
                }
                catch (SqlException e)
                {
                    returnInfo = "Error";
                    FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs); // 创建写入流
                    sw.WriteLine(e.ToString()); // 写入
                    sw.Close();
                }
            }
            else
                returnInfo = "Username is already existed. "; ;
            return returnInfo;
        }
        /// <summary>
        /// 更新用户级别
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool GetUpdateUserRank(string username, int rank)
        {
            bool flag = false;
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "Update Member set Rank=" + "'" + rank + "'where username=" + "'" + username + "' ";
                SqlDataReader userRankInfo = querySingleInfo.ExecuteReader();
                //关闭查询
                userRankInfo.Close();
                //关闭数据库连接
                conn.Close();
                flag = true;
            }
            catch (SqlException e)
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public bool GetDeleteUser(string username)
        {
            bool flag;
            FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("删除成员"); // 写入
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                //删除Member表信息
                querySingleInfo.CommandText = "update Member set isDelete=1 where UserName=" + "'" + username + "'";
                int memberResult = querySingleInfo.ExecuteNonQuery();

                //关闭数据库连接
                conn.Close();
                flag = true;
            }
            catch (SqlException e)
            {
                sw.WriteLine(e.ToString());
                flag = false;
            }

            sw.Close();
            return flag;
        }
        /// <summary>
        /// 检查组名是否存在
        /// </summary>
        /// <param name="username">用户名（主键）</param>
        /// <returns>true  已注册</returns>
        /// <returns>false 未注册</returns>
        public bool GetCheckGroup(string groupname)
        {

            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT * FROM MemberGroup where GroupName=" + "'" + groupname + "'";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            string CurrentGroupName;
            while (singleInfoReader.Read())
            {
                CurrentGroupName = singleInfoReader["GroupName"].ToString().Trim();
                if (CurrentGroupName.Equals(groupname))
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
            return flag;
        }
        #region 小组的创建于删除
        /// <summary>
        /// 新建小组
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public string PostNewGroup([FromBody] Group group)
        {
            FileStream fs = new FileStream("c:\\text\\log.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("11111111111111111111"); // 写入
            sw.WriteLine(group.GroupName); // 写入
            if (group.GroupName == null || group.GroupName == "")
            {
                returnInfo = "组名不能为空";
                sw.Close();
                return returnInfo;
            }
            if (!GetCheckGroup(group.GroupName))
            {
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO MemberGroup (GroupName,GroupMonitor,GroupMembers,IsDelete,BuildTime) VALUES ('" + group.GroupName + "','" + group.GroupMonitor + "','" + group.GroupMembers + "',0,getdate())";
                    int result = cmd.ExecuteNonQuery();
                    returnInfo = "成功";
                    conn.Close();
                }
                catch (Exception e)
                {
                    returnInfo = "异常";
                    sw.WriteLine(e.ToString()); // 写入
                }
            }
            else
            {
                sw.WriteLine("2222222222222222"); // 写入
                returnInfo = "组名已存在";
            }
            sw.Close();
            return returnInfo;
        }
        /// <summary>
        /// 删除小组
        /// </summary>
        /// <param name="group">组名</param>
        /// <returns></returns>
        public bool GetDeleteGroup(string groupName)
        {
            bool flag = false;
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "delete MemberGroup where GroupName=" + "'" + groupName + "";
                int result = cmd.ExecuteNonQuery();
                flag = true;
                conn.Close();
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
        /// <summary>
        /// 修改组信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="groupMonitor">组长</param>
        /// <param name="groupMembers">成员</param>
        /// <returns></returns>
        public bool GetUpdateGroup(string groupName, string groupMonitor, string groupMembers)
        {
            FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("111111111111"); // 写入

            bool flag = false;
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand cmd = conn.CreateCommand();
                //Update Member set Rank = " + "'" + rank + "'where username = " + "'" + username + "' ";
                cmd.CommandText = "Update MemberGroup set GroupMembers=" + "'" + groupMembers + "', GroupMonitor=" + "'" + groupMonitor + "' where GroupName=" + "'" + groupName + "'";
                int result = cmd.ExecuteNonQuery();
                string[] monitor = groupMonitor.Split(',');
                foreach (var i in monitor)
                {
                    cmd.CommandText = " update member set groupname=" + "'" + groupName + "'where username=" + "'" + i + "'";
                    result = cmd.ExecuteNonQuery();
                }
                string[] member = groupMembers.Split(',');
                foreach (var i in member)
                {
                    cmd.CommandText = " update member set groupname=" + "'" + groupName + "'where username=" + "'" + i + "'";
                    result = cmd.ExecuteNonQuery();
                }
                //关闭数据库连接
                conn.Close();
                flag = true;
            }
            catch (SqlException e)
            {
                flag = false;
                sw.WriteLine(e.ToString()); // 写入
            }
            sw.Close();
            return flag;
        }

        #endregion
        #region 增加量 

        /// <summary>
        /// 获取自然周、自然月、自然年的项目增量 
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public int GetProjectAdd_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
            int addNumber = 0;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            //自然周查询
            if (queryDays == 7)
            {
                querySingleInfo.CommandText = " SET DATEFIRST 1  select top 2 DATEPART(wk,committime) as month, count(DATEPART(wk, committime)) as times from Memberproject where DATEPART(wk, committime) >= (DATEPART(m, getdate()) - 2) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by DATEPART(wk, committime)   order by DATEPART(wk, committime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = "   select top 2 DATEPART(m,committime) as month,count(DATEPART(m, committime)) as times from Memberproject where DATEPART(m, committime) >= (DATEPART(m, getdate()) - 2) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by DATEPART(m, committime)  order by DATEPART(m, committime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = " SET  select top 2 DATEPART(yy,committime) as month, count(DATEPART(yy, committime)) as times from Memberproject where DATEPART(yy, committime) >= (DATEPART(yy, getdate()) - 2) group by DATEPART(yy, committime) order by DATEPART(yy, committime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
        }
        /// <summary>
        /// 获取自然周、自然月、自然年的小组增量 
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public int GetGroupAdd_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
            int addNumber = 0;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            //自然周查询
            if (queryDays == 7)
            {
                querySingleInfo.CommandText = " SET DATEFIRST 1  select top 2 DATEPART(wk, buildtime) as month,	count(DATEPART(wk, buildtime)) as times from Membergroup where DATEPART(wk, buildtime) >= (DATEPART(wk, getdate()) - 2)   and DATEPART(yy, buildtime) = (DATEPART(yy, getdate())) group by DATEPART(wk, buildtime) order by DATEPART(wk, buildtime) desc ";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = " SET   select top 2  DATEPART(m, buildtime) as month, 	count(DATEPART(m, buildtime)) as times from Membergroup where DATEPART(m, buildtime) >= (DATEPART(m, getdate()) - 2)    and DATEPART(yy, buildtime) = (DATEPART(yy, getdate())) group by DATEPART(m, buildtime) order by DATEPART(m, buildtime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = " select top 2 DATEPART(yy, buildtime) as month, 	count(DATEPART(yy, buildtime)) as times from Membergroup where DATEPART(yy, buildtime) >= (DATEPART(yy, getdate()) - 2) group by DATEPART(yy, buildtime) order by DATEPART(yy, buildtime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
        }
        /// <summary>
        /// 获取自然周、自然月、自然年的成员增量 
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public int GetUserAdd_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
            int addNumber = 0;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            //自然周查询
            if (queryDays == 7)
            {
                querySingleInfo.CommandText = " SET DATEFIRST 1  select top 2 DATEPART(wk, buildtime) as weekly,	count(DATEPART(wk, buildtime)) as times from Member where DATEPART(wk, buildtime) >= (DATEPART(wk, getdate()) - 2)   and DATEPART(yy, buildtime) = (DATEPART(yy, getdate())) group by DATEPART(wk, buildtime) order by DATEPART(wk, buildtime) desc ";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = " select top 2  DATEPART(m, buildtime) as month, 	count(DATEPART(m, buildtime)) as times from Member where DATEPART(m, buildtime) >= (DATEPART(m, getdate()) - 2)    and DATEPART(yy, buildtime) = (DATEPART(yy, getdate())) group by DATEPART(m, buildtime) order by DATEPART(m, buildtime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = " select top 2 DATEPART(yy, buildtime) as yearly, 	count(DATEPART(yy, buildtime)) as times from Member where DATEPART(yy, buildtime) >= (DATEPART(yy, getdate()) - 2) group by DATEPART(yy, buildtime) order by DATEPART(yy, buildtime) desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim()) - addNumber;
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                conn.Close();
                return addNumber;
            }
        } 
        #endregion
    }
}


#region MyRegion
/// <summary>
/// 删除成员所有信息
/// </summary>
/// <param name="username">用户名</param>
/// <returns></returns>
//public bool GetDeleteUser(string username)
//{
//    bool flag;
//    FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
//    StreamWriter sw = new StreamWriter(fs); // 创建写入流
//    sw.WriteLine("删除成员"); // 写入

//    try
//    {
//        //连接本地数据库
//        SqlConnection conn = connectLocaldb.ConnectDataBase();
//        //打开数据库
//        conn.Open();
//        //创建查询语句
//        SqlCommand querySingleInfo = conn.CreateCommand();

//        //删除编译表信息
//        querySingleInfo.CommandText = "delete MemberCommit where UserName=" + "'" + username + "'";
//        int memberCommitResult = querySingleInfo.ExecuteNonQuery();

//        //删除提交表信息
//        querySingleInfo.CommandText = "delete MemberCommitBeforeCompiling where UserName=" + "'" + username + "'";
//        int memberPushResult = querySingleInfo.ExecuteNonQuery();

//        //删除issue表信息
//        querySingleInfo.CommandText = "delete MemberIssue where Assignee=" + "'" + username + "'";
//        int memberIssueResult = querySingleInfo.ExecuteNonQuery();

//        //删除Member表信息
//        querySingleInfo.CommandText = "delete Member where UserName=" + "'" + username + "'";
//        int memberResult = querySingleInfo.ExecuteNonQuery();

//        //关闭数据库连接
//        conn.Close();
//        flag = true;
//    }
//    catch (SqlException e)
//    {
//        sw.WriteLine(e.ToString());
//        flag = false;
//    }

//    sw.Close();
//    return flag;
//} 
#endregion