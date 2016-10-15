using LuckyGitlabStatWebAPI.DAO;
using LuckyGitlabStatWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LuckyGitlabStatWebAPI.Controllers
{
    public class SingleController : ApiController
    {
        //连接azure数据库
        ConnectDB conncetdb = new ConnectDB();
        //连接本地数据库
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();

        MemberCommit memberCommit = new MemberCommit();
        Member member = new Member();
        List<Member> members = new List<Member>();

        //项目提交总量
        int pushTotalOfProject;
        int[] num = new int[181];
        int total;//提交总次数
        double rate;//正确率
        #region MyRegion
        public SingleController()
        {
            /* Empty */
        }
        /// <summary>
        /// 查询获取个人信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public Member GetSingleInfo(string username)
        {
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
                member.password = singleInfoReader["Password"].ToString().Trim();
                member.email = singleInfoReader["Email"].ToString().Trim();
                member.sex = singleInfoReader["Sex"].ToString().Trim();
                member.groupName = singleInfoReader["Groupname"].ToString().Trim();
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
            //            //查询语句
            //            command.CommandText = "SELECT * FROM Member where UserName=" + "'" + username + "'";
            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                // 若是查询数据有多行，循环读取输出 
            //                while (reader.Read())
            //                {
            //                    member.username = reader["UserName"].ToString().Trim();
            //                    member.password = reader["Password"].ToString().Trim();
            //                    member.email = reader["Email"].ToString().Trim();
            //                    member.sex = bool.Parse(reader["Sex"].ToString().Trim());
            //                    members.Add(member);
            //                }
            //            }
            //            conn.Close();
            //        }
            //    }
            //    return member;
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine("查询异常");
            //    // MessageBox.Show("查询异常");
            //}
            return member;
        }
        /// <summary>
        /// 查询个人一段时间内编译成功总次数
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="queryDate">查询天数</param>
        /// <returns>memberCommit</returns>
        public int GetSuccessTotal_Personally_ByLongTime(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;

            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  AND  Result=1 ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                memberCommit.projectTotal = getTotalCommitTimesReader["result"].ToString().Trim();
                total = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();
            //SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
            //using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
            //{
            //    using (SqlCommand command = conn.CreateCommand())
            //    {
            //        conn.Open();
            //        command.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND getdate()-" + queryDays + "<=CommitTime AND  Result=1 ";
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            // Loop over the results 
            //            while (reader.Read())
            //            {
            //                memberCommit.projectTotal = reader["result"].ToString().Trim();
            //                total = int.Parse(memberCommit.projectTotal);
            //            }
            //        }
            //        conn.Close();
            //    }
            //}
            return total;
        }
        /// <summary>
        /// 查询个人一段时间内编译总次数
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="queryDate">查询天数</param>
        /// <returns>memberCommit</returns>
        public int GetBuildTotal_personally_ByLongTime(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111) ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                memberCommit.projectTotal = getTotalCommitTimesReader["result"].ToString().Trim();
                total = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();
            //SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
            //using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
            //{
            //    using (SqlCommand command = conn.CreateCommand())
            //    {
            //        conn.Open();
            //        command.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND getdate()-" + queryDays + "<=CommitTime AND  Result=1 ";
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            // Loop over the results 
            //            while (reader.Read())
            //            {
            //                memberCommit.projectTotal = reader["result"].ToString().Trim();
            //                total = int.Parse(memberCommit.projectTotal);
            //            }
            //        }
            //        conn.Close();
            //    }
            //}
            return total;
        }
        /// <summary>
        /// 查询个人一段时间内总提交次数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetPushTotal_personally_ByLongTime(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Version) as result FROM MemberCommitBeforeCompiling where UserName=" + "'" + username + "'AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                memberCommit.projectTotal = getTotalCommitTimesReader["result"].ToString().Trim();
                total = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
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
            //            command.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND getdate()-" + queryDays + "<=CommitTime ";

            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                // Loop over the results 
            //                while (reader.Read())
            //                {
            //                    //string s = reader.ToString();
            //                    total = int.Parse(memberCommit.projectTotal = reader["Result"].ToString().Trim());
            //                }
            //            }
            //            conn.Close();
            //        }
            //    }
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine("查询异常");
            //}
            return total;
        }
        /// <summary>
        /// 个人每天提交次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Dictionary<long, int> GetPersonCommitByDays(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> commitNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 112)) as queryTimes FROM MemberCommitBeforeCompiling  where UserName='" + username + "' and DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 112)) <= CONVERT(varchar(12), CommitTime, 112)  group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                DateTime querydate = DateTime.Parse(singleInfoReader["querydate"].ToString().Trim());
                int queryTimes = int.Parse(singleInfoReader["queryTimes"].ToString().Trim());

                long intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                intResult = long.Parse((querydate.Date - startTime).TotalSeconds.ToString());
                commitNumber.Add(intResult * 1000, queryTimes);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return commitNumber;
        }
        /// <summary>
        /// 个人每天编译次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Dictionary<long, int> GetPersonBuildByDays(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> buildNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            try
            {

                querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 112)) as queryTimes FROM MemberCommit  where UserName=" + "'" + username + "' and DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 112)) <= CONVERT(varchar(12), CommitTime, 112)  group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    DateTime querydate = DateTime.Parse(singleInfoReader["querydate"].ToString().Trim());
                    int queryTimes = int.Parse(singleInfoReader["queryTimes"].ToString().Trim());

                    long intResult = 0;
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    intResult = long.Parse((querydate.Date - startTime).TotalSeconds.ToString());
                    buildNumber.Add(intResult * 1000, queryTimes);
                }
                //关闭查询
                singleInfoReader.Close();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            //关闭数据库连接
            conn.Close();
            return buildNumber;
        }
        /// <summary>
        /// 个人每天编译成功
        /// </summary>
        /// <param name="username"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Dictionary<long, int> GetPersonBuildSuccessByDays(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> buildNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 112)) as queryTimes FROM MemberCommit  where UserName='" + username + "' and DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 112)) <= CONVERT(varchar(12), CommitTime, 112) and result=1  group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                DateTime querydate = DateTime.Parse(singleInfoReader["querydate"].ToString().Trim());
                int queryTimes = int.Parse(singleInfoReader["queryTimes"].ToString().Trim());

                long intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                intResult = long.Parse((querydate.Date - startTime).TotalSeconds.ToString());
                buildNumber.Add(intResult * 1000, queryTimes);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return buildNumber;
        }
        /// <summary>
        /// 获取一段时间内编译数据详细信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="queryDays">查询日期</param>
        /// <returns>返回版本号，项目名，编译时间，编译结果，编译时间</returns>
        public List<MemberCommit> GetDataForTable(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            List<MemberCommit> memberCommits = new List<MemberCommit>();
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT Version, ProjectName, committime, Result, spendtime,branch FROM MemberCommit  where UserName=" + "'" + username + "'AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  order by committime desc";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    MemberCommit memberCommitInfo = new MemberCommit();
                    memberCommitInfo.version = singleInfoReader["Version"].ToString().Trim();
                    memberCommitInfo.projectname = singleInfoReader["ProjectName"].ToString().Trim();
                    memberCommitInfo.committime = singleInfoReader["committime"].ToString().Trim();
                    memberCommitInfo.projectResult = singleInfoReader["Result"].ToString().Trim();
                    memberCommitInfo.spendtime = singleInfoReader["spendtime"].ToString().Trim();
                    memberCommitInfo.branch = singleInfoReader["spendtime"].ToString().Trim();
                    memberCommitInfo.branch = singleInfoReader["branch"].ToString().Trim();
                    memberCommits.Add(memberCommitInfo);
                }
                //关闭查询
                singleInfoReader.Close();
                //关闭数据库连接
                conn.Close();
            }
            catch { return memberCommits; }
            //try
            //{
            //    SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
            //    using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
            //    {
            //        using (SqlCommand command = conn.CreateCommand())
            //        {
            //            conn.Open();
            //            command.CommandText = "SELECT Version, ProjectName, committime, Result, spendtime FROM MemberCommit  where UserName=" + "'" + username + "'AND getdate()+'8:00:00'-" + queryDays + "<=CommitTime order by committime desc";
            //            //ExecuteReader()：将 CommandText 发送到 Connection，并生成 SqlDataReader。
            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                // Loop over the results 
            //                while (reader.Read())//SqlDataReader到下一个记录
            //                {
            //                    MemberCommit memberCommitInfo = new MemberCommit();
            //                    memberCommitInfo.version = reader["Version"].ToString().Trim();
            //                    memberCommitInfo.projectname = reader["ProjectName"].ToString().Trim();
            //                    memberCommitInfo.committime = reader["committime"].ToString().Trim();
            //                    memberCommitInfo.projectResult = reader["Result"].ToString().Trim();
            //                    memberCommitInfo.spendtime = reader["spendtime"].ToString().Trim();
            //                    memberCommits.Add(memberCommitInfo);
            //                }
            //            }
            //            conn.Close();
            //        }
            //    }
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine("查找异常");
            //}
            return memberCommits;
        }
        /// <summary>
        /// 获取个人issue信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public List<IssueEvent> getDataForIssueTable(string username, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            List<IssueEvent> memberIssues = new List<IssueEvent>();
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT issueid, Issue, ProjectName, Initiator, Assignee, Starttime,Updatetime,state FROM MemberIssue  where Assignee=" + "'" + username + "'AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), starttime, 111)  group by  issueid, Issue, ProjectName, Initiator, Assignee, Starttime,Updatetime,state order by  Issue desc";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    IssueEvent memberIssue = new IssueEvent();
                    memberIssue.issueid = singleInfoReader["issueid"].ToString().Trim();
                    memberIssue.object_attributes.assignee_id = singleInfoReader["Issue"].ToString().Trim();
                    memberIssue.project.name = singleInfoReader["ProjectName"].ToString().Trim();
                    memberIssue.object_attributes.created_at = singleInfoReader["Starttime"].ToString().Trim();
                    memberIssue.object_attributes.updated_at = singleInfoReader["Updatetime"].ToString().Trim();
                    memberIssue.user.username = singleInfoReader["Initiator"].ToString().Trim();
                    memberIssue.assignee.username = singleInfoReader["Assignee"].ToString().Trim();
                    memberIssue.object_attributes.state = singleInfoReader["state"].ToString().Trim();

                    memberIssues.Add(memberIssue);
                }
                //关闭查询
                singleInfoReader.Close();
                //关闭数据库连接
                conn.Close();
            }
            catch (SqlException e)
            {
                return memberIssues;
            }
            return memberIssues;
        }
        /// <summary>
        /// 获取个人指定项目提交总量
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="projectname">项目名</param>
        /// <param name="queryDays">查询日期</param>
        /// <returns></returns>
        public int GetPushNumberOfProjectByLongTime(string username, string projectname, int queryDays)
        {
            queryDays--;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT count(commitid) as times  FROM MemberCommitBeforeCompiling,member where MemberCommitBeforeCompiling.UserName=" + "'" + username + "'  and MemberCommitBeforeCompiling.ProjectName=" + "'" + projectname + "' and MemberCommitBeforeCompiling.username=Member.Username  AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <=CONVERT(varchar(12), CommitTime, 111)";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                pushTotalOfProject = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return pushTotalOfProject;
        }
        /// <summary>
        /// 获取个人指定项目编译总量
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="projectname">项目名</param>
        /// <param name="queryDays">查询日期</param>
        /// <returns></returns>
        public int GetBuildNumberOfProjectByLongTime(string username, string projectname, int queryDays)
        {
            queryDays--;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT count(commitid) as times  FROM MemberCommit,member where MemberCommit.UserName=" + "'" + username + "'  and MemberCommit.ProjectName like " + "'%" + projectname + "%' and MemberCommit.username=Member.Username  AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <=CONVERT(varchar(12), CommitTime, 111)";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                pushTotalOfProject = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return pushTotalOfProject;
        }
        /// <summary>
        /// 获取个人指定项目编译成功总量
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="projectname">项目名</param>
        /// <param name="queryDays">查询日期</param>
        /// <returns></returns>
        public int GetBuildSuccessNumberOfProjectByLongTime(string username, string projectname, int queryDays)
        {
            queryDays--;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT count(commitid) as times  FROM MemberCommit,member where MemberCommit.UserName=" + "'" + username + "'  and MemberCommit.ProjectName like " + "'%" + projectname + "%' and MemberCommit.username=Member.Username  AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <=CONVERT(varchar(12), CommitTime, 111) and result=1";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                pushTotalOfProject = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return pushTotalOfProject;
        }
        /// <summary>
        /// 获取个人参与的所有项目
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public IList<string> GetMembersProject(string username)
        {
            List<string> memberProject = new List<string>();
            IList<string> memberproject = new List<string>();
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT Projectname FROM MemberProject where ProjectMonitor like" + "'%" + username + "%' or ProjectMembers like" + "'%" + username + "%'";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    string projectname = singleInfoReader["Projectname"].ToString().Trim();
                    memberProject.Add(projectname);
                }
                memberproject = memberProject.Distinct().ToList();
                //关闭查询
                singleInfoReader.Close();
                //关闭数据库连接
                conn.Close();
            }
            catch (SqlException e)
            {
                FileStream fs = new FileStream("c:\\text\\log.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(e.ToString()); // 写入
                sw.Close();
            }

            return memberproject;
        }
        /// <summary>
        /// 获取个人参与的每个项目提交次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetMembersProjectsPushNumber(string username, int queryDays)
        {
            int number;
            Dictionary<string, int> pushNumber = new Dictionary<string, int>();
            IList<string> projectname = GetMembersProject(username);
            foreach (var i in projectname)
            {
                number = GetPushNumberOfProjectByLongTime(username, i, queryDays);
                pushNumber.Add(i, number);
            }
            return pushNumber;
        }
        /// <summary>
        /// 获取个人参与的每个项目编译次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetMembersProjectsBuildNumber(string username, int queryDays)
        {
            int number;
            Dictionary<string, int> pushNumber = new Dictionary<string, int>();
            IList<string> projectname = GetMembersProject(username);
            foreach (var i in projectname)
            {
                number = GetBuildNumberOfProjectByLongTime(username, i, queryDays);
                pushNumber.Add(i, number);
            }
            return pushNumber;
        }
        /// <summary>
        /// 获取个人参与的每个项目编译成功次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetMembersProjectsBuildSuccessNumber(string username, int queryDays)
        {
            int number;
            Dictionary<string, int> pushNumber = new Dictionary<string, int>();
            IList<string> projectname = GetMembersProject(username);
            foreach (var i in projectname)
            {
                number = GetBuildSuccessNumberOfProjectByLongTime(username, i, queryDays);
                pushNumber.Add(i, number);
            }
            return pushNumber;
        }
        #endregion
        /// <summary>
        /// 获取个人自然周、自然月、自然年的提交次数
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<string, Dictionary<int, int>> GetPersionCommit_Weekly_Monthly_Yearly(string username, int queryDays)
        {

            Dictionary<int, int> memberCommit = new Dictionary<int, int>();
            //Tuple<string, Dictionary<string, int>> tupleMemberCommit = new Tuple<string, Dictionary<string, int>>();
            int queryKey;
            int commitNumber;
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            //自然周查询
            if (queryDays == 7)
            {
                querySingleInfo.CommandText = " SET DATEFIRST 1  select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommitBeforeCompiling where username=" + "'"+username+"'and DATEPART(wk, committime) = DATEPART(wk, getdate()) group by DATEPART(w, committime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommit.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<string, Dictionary<int, int>> tupleCommit =new Tuple<string, Dictionary<int, int>>(username,memberCommit);
                conn.Close();
                return  tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = "   select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where  username=" + "'" + username + "'and DATEPART(m, committime) = DATEPART(m, getdate()) group by DATEPART(d, committime)order by day asc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommit.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<string, Dictionary<int, int>> tupleCommit = new Tuple<string, Dictionary<int, int>>(username, memberCommit);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommitBeforeCompiling where  username=" + "'" + username + "'and DATEPART(yy, committime) = DATEPART(yy, getdate()) group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommit.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<string, Dictionary<int, int>> tupleCommit = new Tuple<string, Dictionary<int, int>>(username, memberCommit);
                conn.Close();
                return tupleCommit;
            }
        }
    }
}
        