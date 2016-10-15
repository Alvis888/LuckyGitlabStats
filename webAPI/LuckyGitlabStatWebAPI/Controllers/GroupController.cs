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
    public class GroupController : ApiController
    {
        //临时储存读取数据
        string[] sArray;
        //姓名去重
        IList<string> distinctNameList;
        ConnectLocalSQL connectdb = new ConnectLocalSQL();
        //成员信息列表
        List<string> memberGroup = new List<string>();
        //编译信息列表
        List<MemberCommit> membercommitList = new List<MemberCommit>();
        //实例化编译对象
        MemberCommit membercommit = new MemberCommit();
        //组内提交总次数
        int pushtotal;
        //组内编译总次数
        int buildtotal;
        //组内编译成功总次数
        int buildSuccesstotal;
        //组内成员提交次数
        List<Dictionary<string, int>> membersPushNumber = new List<Dictionary<string, int>>();
        #region MyRegion
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public GroupController()
        {
            /*
             *Empty 
             */
        }
        /// <summary>
        /// 获取组信息
        /// </summary>
        /// <returns></returns>
        public List<Group> GetGroupInfo()
        {
            //小组列表信息
            List<Group> groupList = new List<Group>();
            //连接本地数据库
            SqlConnection conn = connectdb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select GroupName,GroupMonitor,GroupMembers,isDelete from memberGroup ";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                Group group = new Group();
                group.GroupName = singleInfoReader["GroupName"].ToString().Trim();
                group.GroupMonitor = singleInfoReader["GroupMonitor"].ToString().Trim();
                group.GroupMembers = singleInfoReader["GroupMembers"].ToString().Trim();
                group.isDelete =bool.Parse( singleInfoReader["isDelete"].ToString().Trim());
                //memberGroup.Add(group.GroupName);
                groupList.Add(group);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return groupList;
        }
        /// <summary>
        /// 删除小组
        /// </summary>
        /// <param name="groupname">组名</param>
        /// <returns></returns>
        public string GetDeleteGroup(string groupname)
        {
            string returnInfo;
            try
            {
                SqlConnection conn = connectdb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "update memberGroup  Set IsDelete=1 where groupname=" + "'" + groupname + "'";
                int result = querySingleInfo.ExecuteNonQuery();
                //关闭数据库连接
                conn.Close();
                returnInfo = "删除成功";
            }
            catch (SqlException e)
            {
                returnInfo = "删除失败";
            }
            return returnInfo;
        }
        /// <summary>
        /// 获取小组所有项目信息
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public IList<Project> GetProjectInfoGrouply(string groupname)
        {
            IList<Project> projectList = new List<Project>();
            List<Project> projectInfo = new List<Project>();
            SqlConnection conn = connectdb.ConnectDataBase();
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select * from MemberProject where groupname like '%" + groupname + "%'";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                Project project = new Project();
                project.projectName = getTotalCommitTimesReader["projectname"].ToString().Trim();
                project.projectMonitor = getTotalCommitTimesReader["projectMonitor"].ToString().Trim();
                project.projectMembers = getTotalCommitTimesReader["projectMembers"].ToString().Trim();
                projectInfo.Add(project);
            }
            projectList = projectInfo.Distinct().ToList();
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();
            return projectList;
        } 
        /// <summary>
        /// 获取小组成员名单
        /// </summary>
        /// <param name="name">小组名</param>
        /// <returns></returns>
        public IList<string> GetGroupMembersName(string groupname)
        {

            FileStream fs = new FileStream("c:\\text\\log.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("11111111111111111111"); // 写入
            List<string> nameList = new List<string>();
            try
            {
                SqlConnection conn = connectdb.ConnectDataBase();
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT  GroupMonitor,GroupMembers FROM MemberGroup where groupName=" + "'" + groupname + "' ";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    string members = getTotalCommitTimesReader["GroupMonitor"].ToString().Trim();
                    if (members != "")
                        members += ',';
                    members += getTotalCommitTimesReader["GroupMembers"].ToString().Trim();
                    sArray = members.Split(',');
                    //sw.WriteLine("sArray.Length=" + sArray.Length); // 写入
                    for (int i = 0; i < sArray.Length; i++)
                        nameList.Add(sArray[i]);
                    //sw.WriteLine("sArray.Length="); // 写入
                    //nameList.Add(members);
                }
                distinctNameList = nameList.Distinct().ToList();
                //关闭查询
                getTotalCommitTimesReader.Close();
                //关闭数据库连接
                conn.Close();
            }
            catch (SqlException e)
            {
                sw.WriteLine(e.ToString());
            }
            sw.Close();
            return distinctNameList;
        }
        /// <summary>
        /// 获取小组成员每人编译量
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字典树类型(用户名，次数)的列表</returns>
        public Dictionary<string, int> GetMembersBuildNumberByLongtime(string groupname, int queryday)
        {
            SingleController single = new SingleController();
            //成员姓名，次数
            Dictionary<string, int> membersNumber = new Dictionary<string, int>();
            //成员姓名列表
            IList<string> nameList = GetGroupMembersName(groupname);
            for (int i = 0; i < nameList.Count(); i++)
            {
                //调用Single方法，赋值给num
                int num = single.GetBuildTotal_personally_ByLongTime(nameList[i], queryday);
                //结果加入字典树
                string name = nameList[i];
                membersNumber.Add(name, num);
            }
            return membersNumber;
        }
        /// <summary>
        /// 获取小组成员每人编译成功量
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字典树类型(用户名，次数)的列表</returns>
        public Dictionary<string, int> GetMembersBuildSuccessNumberByDays(string groupname, int queryday)
        {
            SingleController single = new SingleController();
            //成员姓名，次数
            Dictionary<string, int> membersNumber = new Dictionary<string, int>();
            //成员姓名列表
            IList<string> nameList = GetGroupMembersName(groupname);
            for (int i = 0; i < nameList.Count(); i++)
            {
                //调用Single方法，赋值给num
                int num = single.GetSuccessTotal_Personally_ByLongTime(nameList[i], queryday);
                //结果加入字典树
                string name = nameList[i];
                membersNumber.Add(name, num);
            }
            return membersNumber;
        }
        /// <summary>
        /// 查找编译总次数
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回编译总次数</returns>
        public int GetBuildNumberGrouplyBylongTime(int queryday)
        {
            //以当前日期为第一天，因此 - 1
            queryday--;
            try
            {
                using (SqlConnection conn = connectdb.ConnectDataBase())
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT  COUNT(commitid) as result FROM MemberCommit where DateAdd(d,-" + queryday + ", CONVERT(varchar(12), getdate()+'8:00:00', 111)) <= CONVERT(varchar(12), CommitTime, 111)";
                        SqlDataReader reader = command.ExecuteReader();
                        //遍历查询结果
                        while (reader.Read())
                        {
                            //按照列解行读取
                            buildtotal = int.Parse(reader["result"].ToString());
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("查找异常");
            }
            return buildtotal;
        }
        /// <summary>
        /// 查找编译成功总次数
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回编译总次数</returns>
        public int GetBuildSuccessNumberGrouplyBylongTime(int queryday)
        {
            //以当前日期为第一天，因此 - 1
            queryday--;
            try
            {
                using (SqlConnection conn = connectdb.ConnectDataBase())
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT  COUNT(commitid) as result FROM MemberCommit where DateAdd(d,-" + queryday + ", CONVERT(varchar(12), getdate()+'8:00:00', 111)) <= CONVERT(varchar(12), CommitTime, 111) and result=1";
                        SqlDataReader reader = command.ExecuteReader();
                        //遍历查询结果
                        while (reader.Read())
                        {
                            //按照列解行读取
                            buildtotal = int.Parse(reader["result"].ToString());
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("查找异常");
            }
            return buildtotal;
        }
        /// <summary>
        /// 获取小组每天的编译量
        /// </summary>
        /// <param name="groupname">小组名</param>
        /// <param name="queryday">查询日期</param>
        /// <returns></returns>
        public Dictionary<long,int> GetMembersBuildByDays(string groupname, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> buildNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectdb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 111)) as queryTimes FROM MemberCommit,member  where Member.UserName=MemberCommit.username and Member.groupname like '%" + groupname + "%' and  DATEADD(d,-150, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)   group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
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
        /// 获取小组每天的编译成功量
        /// </summary>
        /// <param name="groupname">小组名</param>
        /// <param name="queryday">查询日期</param>
        /// <returns></returns>
        public Dictionary<long, int> GetMembersBuildSuccessByDays(string groupname, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> buildNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectdb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 111)) as queryTimes FROM MemberCommit,member  where Member.UserName=MemberCommit.username and Member.groupname like '%"+groupname+"%' and result=1 and  DATEADD(d,-150, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)   group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
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
        /// 获取小组成员每人提交量
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字典树类型(用户名，次数)的列表</returns>
        public Dictionary<string, int> GetMembersPushNumberByDays(string groupname, int queryday)
        {
            SingleController single = new SingleController();
            //成员姓名，次数
            Dictionary<string, int> membersNumber = new Dictionary<string, int>();
            //成员姓名列表
            IList<string> nameList = GetGroupMembersName(groupname);
            for (int i = 0; i < nameList.Count(); i++)
            {
                //调用Single方法，赋值给num
                int num = single.GetPushTotal_personally_ByLongTime(nameList[i], queryday);
                //结果加入字典树
                string name = nameList[i];
                membersNumber.Add(name, num);
            }
            return membersNumber;
        }
        /// <summary>
        /// 获取小组每天的提交量
        /// </summary>
        /// <param name="groupname">小组名</param>
        /// <param name="queryday">查询日期</param>
        /// <returns></returns>
        public Dictionary<long,int> GetMembersPushByDays(string groupname, int queryDays)
        {
            //以当前日作第一天，0代表今天
            queryDays--;
            Dictionary<long, int> commitNumber = new Dictionary<long, int>();
            //连接本地数据库
            SqlConnection conn = connectdb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 111)) as queryTimes FROM MemberCommit,member  where Member.UserName=MemberCommit.username and Member.groupname like '%" + groupname + "%' and  DATEADD(d,-150, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)   group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111)desc";
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
        /// 查找提交总次数
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回编译总次数</returns>
        public int GetPushNumberGrouplyBylongTime(int queryday)
        {
            //以当前日期为第一天，因此 - 1
            queryday--;
            try
            {
                using (SqlConnection conn = connectdb.ConnectDataBase())
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT  COUNT(commitid) as result FROM MemberCommitBeforeCompiling where DateAdd(d,-" + queryday + ", CONVERT(varchar(12), getdate()+'8:00:00', 111)) <= CONVERT(varchar(12), CommitTime, 111)";
                        SqlDataReader reader = command.ExecuteReader();
                        //遍历查询结果
                        while (reader.Read())
                        {
                            //按照列解行读取
                            buildtotal = int.Parse(reader["result"].ToString());
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("查找异常");
            }
            return buildtotal;
        }
        /// <summary>
        /// 获取小组做的所有项目
        /// </summary>
        /// <param name="grouname"></param>
        /// <returns></returns>
        public List<string> GetProjectOfGroup(string groupname)
        {
            List<string> projectList = new List<string>();
            SqlConnection conn = connectdb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select projectName from memberProject where groupname='"+groupname+"'";
            SqlDataReader singleInfoReader = cmd.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                string  projectName = singleInfoReader["projectName"].ToString().Trim();
                projectList.Add(projectName);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return projectList;
        }
        #endregion


    //    /// <summary>
    //    /// 指定项目自然时间总提交数量
    //    /// </summary>
    //    /// <param name="projectName"></param>
    //    /// <param name="queryDays"></param>
    //    /// <returns></returns>
    //    public int GetCommitToalOfProject_Weekly_Monthly_Yearly(string projectName, int queryDays)
    //    {

    //        {
    //            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
    //            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
    //            int addNumber = 0;
    //            SqlConnection conn = connectdb.ConnectDataBase();
    //            //打开数据库
    //            conn.Open();
    //            //创建查询语句
    //            SqlCommand querySingleInfo = conn.CreateCommand();
    //            //自然周查询
    //            if (queryDays == 7)
    //            {
    //                querySingleInfo.CommandText = " SET DATEFIRST 1  select  DATEPART(wk,committime) as week, count(DATEPART(wk, committime)) as times from MemberCommitBeforeCompiling where DATEPART(wk, committime) = (DATEPART(wk, getdate())) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by  DATEPART(wk, committime)   order by  DATEPART(wk, committime) desc";
    //                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //                //有多行数据，用while循环
    //                while (getTotalCommitTimesReader.Read())
    //                {
    //                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //                }
    //                //关闭查询
    //                getTotalCommitTimesReader.Close();
    //                conn.Close();
    //                return addNumber;
    //            }
    //            //自然月查询
    //            if (queryDays == 30)
    //            {
    //                querySingleInfo.CommandText = "   select top 2 DATEPART(m,committime) as month,count(DATEPART(m, committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) = (DATEPART(m, getdate())) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by DATEPART(m, committime)  order by DATEPART(m, committime) desc";
    //                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //                //有多行数据，用while循环
    //                while (getTotalCommitTimesReader.Read())
    //                {
    //                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //                }
    //                //关闭查询
    //                getTotalCommitTimesReader.Close();
    //                conn.Close();
    //                return addNumber;
    //            }
    //            //自然年查询
    //            else
    //            {
    //                querySingleInfo.CommandText = " select top 2 DATEPART(yy,committime) as month, count(DATEPART(yy, committime)) as times from MemberCommitBeforeCompiling where DATEPART(yy, committime) >= (DATEPART(yy, getdate()) - 2) group by DATEPART(yy, committime) order by DATEPART(yy, committime) desc";
    //                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //                //有多行数据，用while循环
    //                while (getTotalCommitTimesReader.Read())
    //                {
    //                    addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //                }
    //                //关闭查询
    //                getTotalCommitTimesReader.Close();
    //                conn.Close();
    //                return addNumber;
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 指定项目自然时间总编译数量
    //    /// </summary>
    //    /// <param name="projectName"></param>
    //    /// <param name="queryDays"></param>
    //    /// <returns></returns>
    //    public int GetBuildToalOfProject_Weekly_Monthly_Yearly(string projectName, int queryDays)
    //    {

    //        int addNumber = 0;
    //        SqlConnection conn = connectdb.ConnectDataBase();
    //        //打开数据库
    //        conn.Open();
    //        //创建查询语句
    //        SqlCommand querySingleInfo = conn.CreateCommand();
    //        //自然周查询
    //        if (queryDays == 7)
    //        {
    //            querySingleInfo.CommandText = "select  DATEPART(wk,committime) as week, count(DATEPART(wk, committime)) as times from MemberCommit where DATEPART(wk, committime) = (DATEPART(wk, getdate())) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by  DATEPART(wk, committime)   order by  DATEPART(wk, committime) desc";
    //            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //            //有多行数据，用while循环
    //            while (getTotalCommitTimesReader.Read())
    //            {
    //                addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //            }
    //            //关闭查询
    //            getTotalCommitTimesReader.Close();
    //            conn.Close();
    //            return addNumber;
    //        }
    //        //自然月查询
    //        if (queryDays == 30)
    //        {
    //            querySingleInfo.CommandText = "   select top 2 DATEPART(m,committime) as month,count(DATEPART(m, committime)) as times from MemberCommit where DATEPART(m, committime) = (DATEPART(m, getdate())) and DATEPART(yy, committime) = (DATEPART(yy, getdate()))  group by DATEPART(m, committime)  order by DATEPART(m, committime) desc";
    //            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //            //有多行数据，用while循环
    //            while (getTotalCommitTimesReader.Read())
    //            {
    //                addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //            }
    //            //关闭查询
    //            getTotalCommitTimesReader.Close();
    //            conn.Close();
    //            return addNumber;
    //        }
    //        //自然年查询
    //        else
    //        {
    //            querySingleInfo.CommandText = " select top 2 DATEPART(yy,committime) as month, count(DATEPART(yy, committime)) as times from MemberCommit where DATEPART(yy, committime) >= (DATEPART(yy, getdate()) - 2) group by DATEPART(yy, committime) order by DATEPART(yy, committime) desc";
    //            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
    //            //有多行数据，用while循环
    //            while (getTotalCommitTimesReader.Read())
    //            {
    //                addNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
    //            }
    //            //关闭查询
    //            getTotalCommitTimesReader.Close();
    //            conn.Close();
    //            return addNumber;
    //        }
    //    }
    }
}










//public Dictionary<string,int> GetGroupName(string groupname)
//{
//    //连接本地数据库
//    SqlConnection conn = connectLocaldb.ConnectDataBase();
//    //打开数据库
//    conn.Open();
//    //创建查询语句
//    SqlCommand querySingleInfo = conn.CreateCommand();
//    querySingleInfo.CommandText = "SELECT * FROM Member where UserName=" + "'" + username + "'"; ;
//    SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
//    //有多行数据，用while循环
//    while (singleInfoReader.Read())
//    {
//        member.username = singleInfoReader["UserName"].ToString().Trim();
//        member.password = singleInfoReader["Password"].ToString().Trim();
//        member.email = singleInfoReader["Email"].ToString().Trim();
//        member.sex = singleInfoReader["Sex"].ToString().Trim();
//        members.Add(member);
//    }
//    //关闭查询
//    singleInfoReader.Close();
//    //关闭数据库连接
//    conn.Close();

//    return member;
//}



/// <summary>
/// 获取小组成员每天编译量
/// </summary>
/// <param name = "groupname" > 组名 </ param >
/// < param name="queryday">查询日期</param>
/// <returns>返回字典树类型(用户名，次数)的列表</returns>
//public Dictionary<string, int> GetMembersBuildNumberByDays(string groupname, int queryday)
//{
//    //成员姓名，次数
//    Dictionary<string, int> membersNumber = new Dictionary<string, int>();
//    //成员姓名列表
//    List<string> nameList = GetMembersWhoHasCompiling(groupname);
//    for (int i = 0; i < nameList.Count(); i++)
//    {
//        //调用Single方法，获取成员编译量赋值给num
//        int num = single.GetBuildTotal_personally_ByLongTime(nameList[i], queryday);
//        //结果加入字典树
//        membersNumber.Add(nameList[i], num);
//    }
//    return membersNumber;
//}
/// <summary>
/// 获取小组成员每天编译成功量
/// </summary>
/// <param name="groupname">组名</param>
/// <param name="queryday">查询日期</param>
/// <returns>返回字典树类型(用户名，次数)的列表</returns>
//public Dictionary<string, int> GetMembersPushSuccessNumberByDays(string groupname, int queryday)
//{
//    //成员姓名，次数
//    Dictionary<string, int> membersNumber = new Dictionary<string, int>();
//    //成员姓名列表
//    List<string> nameList = GetMembersWhoHasCompiling(groupname);
//    for (int i = 0; i < nameList.Count(); i++)
//    {
//        //调用Single方法，获取成员编译成功量赋值给num
//        int num = single.GetBuildSuccessTotal_Personally_ByLongTime(nameList[i], queryday);
//        //结果加入字典树
//        membersNumber.Add(nameList[i], num);
//    }
//    return membersNumber;
//}


/// <summary>
/// 查找有编译记录的小组成员
/// </summary>
/// <param name = "groupname" > 组名 </ param >
/// < returns > 返回含有小组成员姓名的列表 </ returns >
//public List<string> GetMembersWhoHasCompiling(string groupname)
//{
//    //成员姓名列表
//    List<string> nameList = new List<string>();
//    string username;//成员姓名
//    try
//    {
//        using (SqlConnection conn = connectdb.ConnectDataBase())
//        {
//            conn.Open();
//            using (SqlCommand command = conn.CreateCommand())
//            {
//                command.CommandText = "select distinct username from membercommit where groupname=" + "'" + groupname + "'";
//                SqlDataReader reader = command.ExecuteReader();
//                //遍历查询结果
//                while (reader.Read())
//                {
//                    //按照列解行读取
//                    username = reader["username"].ToString().Trim();
//                    //将结果加入列表
//                    nameList.Add(username);
//                }
//            }
//            conn.Close();
//        }
//    }
//    catch (SqlException e)
//    {
//        Console.WriteLine("查找异常");
//    }
//    return nameList;
//}  


///// <summary>
///// 查询一段时间内组内提交总次数
///// </summary>
///// <param name = "groupname" > 组名 </ param >
///// < param name="queryday">查询日期</param>
///// <returns>返回提交总次数</returns>
//public int GetPushNumberGrouplyBylongTime(string groupname, int queryday)
//{
//    //以当前日期为第一天，因此 - 1
//    queryday--;
//    try
//    {
//        using (SqlConnection conn = connectdb.ConnectDataBase())
//        {
//            conn.Open();
//            using (SqlCommand command = conn.CreateCommand())
//            {
//                command.CommandText = "SELECT  COUNT(groupname) as result FROM MemberCommitBeforeCompiling where groupname=" + "'" + groupname + "'AND CONVERT(varchar(12), getdate()+'8:00:00' - " + queryday + ", 111) <= CONVERT(varchar(12), CommitTime, 111)";
//                SqlDataReader reader = command.ExecuteReader();
//                //遍历查询结果
//                while (reader.Read())
//                {
//                    //按照列解行读取
//                    pushtotal = int.Parse(reader["result"].ToString());
//                }
//            }
//            conn.Close();
//        }
//    }
//    catch (SqlException e)
//    {
//        Console.WriteLine("查找异常");
//    }
//    return pushtotal;
//}


/// <summary>
/// 查找组内编译总次数
/// </summary>
/// <param name = "groupname" > 组名 </ param >
/// < param name="queryday">查询日期</param>
/// <returns>返回编译总次数</returns>
//public int GetBuildNumberGrouplyBylongTime(string groupname, int queryday)
//{
//    //以当前日期为第一天，因此 - 1
//    queryday--;
//    try
//    {
//        using (SqlConnection conn = connectdb.ConnectDataBase())
//        {
//            conn.Open();
//            using (SqlCommand command = conn.CreateCommand())
//            {
//                command.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where groupName=" + "'" + groupname + "'AND CONVERT(varchar(12), getdate()+'8:00:00' - " + queryday + ", 111) <= CONVERT(varchar(12), CommitTime, 111)";
//                SqlDataReader reader = command.ExecuteReader();
//                //遍历查询结果
//                while (reader.Read())
//                {
//                    //按照列解行读取
//                    buildtotal = int.Parse(reader["result"].ToString());
//                }
//            }
//            conn.Close();
//        }
//    }
//    catch (SqlException e)
//    {
//        Console.WriteLine("查找异常");
//    }
//    return buildtotal;
//}

/// <summary>
/// 查找组内编译成功总次数
/// </summary>
/// <param name = "groupname" > 组名 </ param >
/// < param name="queryday">查询日期</param>
/// <returns>返回编译成功总次数</returns>
//public int GetBuildSuccessNumberGrouplyBylongTime(string groupname, int queryday)
//{
//    //以当前日期为第一天，因此 - 1
//    queryday--;
//    try
//    {
//        using (SqlConnection conn = connectdb.ConnectDataBase())
//        {
//            conn.Open();
//            using (SqlCommand command = conn.CreateCommand())
//            {
//                command.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where groupName=" + "'" + groupname + "'AND CONVERT(varchar(12), getdate()+'8:00:00' - " + queryday + ", 111) <= CONVERT(varchar(12), CommitTime, 111) AND Result=1";
//                SqlDataReader reader = command.ExecuteReader();
//                //遍历查询结果
//                while (reader.Read())
//                {
//                    //按照列解行读取
//                    buildSuccesstotal = int.Parse(reader["result"].ToString());
//                }
//            }
//            conn.Close();
//        }
//    }
//    catch (SqlException e)
//    {
//        Console.WriteLine("查找异常");
//    }
//    return buildSuccesstotal;
//}