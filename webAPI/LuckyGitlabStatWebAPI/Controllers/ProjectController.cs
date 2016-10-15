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
    public class ProjectController : ApiController
    {
        //临时储存读取数据
        string[] sArray;
        //姓名去重
        IList<string> distinctNameList;
        /// <summary>
        /// 用户级别，默认最高
        /// </summary>
        int rank = 1;
        bool flag = false;
        //用于项目名查重
        IList<Project> projectList = new List<Project>();
        //项目成员提交量
        Dictionary<string, int> MembersPushMember = new Dictionary<string, int>();
        //连接本地数据库
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();
        public ProjectController()
        {
            /*
             Empty
             */
        }

        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public IList<Project> GetAllProjectInfo()
        {
            //小组项目名
            List<Project> projectName = new List<Project>();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select distinct projectname,projectmonitor,projectMembers , Committime,groupname,isdelete from MemberProject   order by CommitTime desc ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                Project project = new Project();
                project.projectName = getTotalCommitTimesReader["projectname"].ToString().Trim();
                project.projectMonitor = getTotalCommitTimesReader["projectmonitor"].ToString().Trim();
                project.projectMembers = getTotalCommitTimesReader["projectMembers"].ToString().Trim();
                project.groupName = getTotalCommitTimesReader["groupname"].ToString().Trim();
                project.isDelete = bool.Parse(getTotalCommitTimesReader["isdelete"].ToString().Trim());
                projectName.Add(project);
            }
            projectList = projectName.Distinct().ToList();
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();
            return projectList;
        }
        /// <summary>
        /// 获取项目的任务人
        /// </summary>
        /// <param name="projectname">项目名</param>
        /// <returns></returns>
        public IList<string> GetProjectMembersName(string projectname)
        {
            FileStream fs = new FileStream("c:\\text\\log.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("11111111111111111111"); // 写入
            List<string> nameList = new List<string>();
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT ProjectMonitor,ProjectMembers FROM MemberProject where ProjectName=" + "'" + projectname + "'";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    string monitor = getTotalCommitTimesReader["ProjectMonitor"].ToString().Trim();
                    string member = getTotalCommitTimesReader["ProjectMembers"].ToString().Trim();
                    if (monitor != "")
                        monitor += ',';
                    if (member.StartsWith(","))
                        member = member.Substring(1);
                    member = monitor + member;
                    sArray = member.Split(',');
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
            //SqlConnection conn = connectLocaldb.ConnectDataBase();
            //conn.Open();
            //SqlCommand querySingleInfo = conn.CreateCommand();
            //querySingleInfo.CommandText = "select distinct  UserName from MemberCommitBeforeCompiling where  ProjectName="+"'"+ projectname + "'";
            //SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            ////有多行数据，用while循环
            //while (getTotalCommitTimesReader.Read())
            //{
            //    projectMember.Add(getTotalCommitTimesReader["UserName"].ToString().Trim());
            //}
            ////关闭查询
            //getTotalCommitTimesReader.Close();
            //if(projectMember.Count>0)
            //{
            //    string name = ",";
            //    foreach (var i in projectMember)
            //    {
            //        name += i;
            //    }
            //    name = name.Substring(1);
            //    string sql = "update MemberProject set ProjectMembers="+"'" + name+ "' where ProjectName="+"'"+ projectname + "'";
            //    SqlCommand cmd = new SqlCommand(sql, conn);
            //    int result = cmd.ExecuteNonQuery();
            //    conn.Close();
            //    return projectMember;
            //}
            //else
            //{
            //    //关闭数据库连接
            //    conn.Close();
            //    return projectMember;
            //}

        }
        /// <summary>
        /// 获取项目成员提交数量
        /// </summary>
        /// <param name="projectname"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<string, int>, string> GetMembersPushNumberOfProject(string projectname, int querydays)
        {
            string returnInfo = "";
            SingleController single = new SingleController();
            IList<string> projectMember = new List<string>();
            projectMember = GetProjectMembersName(projectname);
            if (projectMember != null)
            {
                for (int i = 0; i < projectMember.Count(); i++)
                {
                    MembersPushMember.Add(projectMember[i], single.GetPushNumberOfProjectByLongTime(projectMember[i], projectname, querydays));
                }
            }
            else
                returnInfo = "请手动添加项目成员";
            Tuple<Dictionary<string, int>, string> tuple = new Tuple<Dictionary<string, int>, string>(MembersPushMember, returnInfo);
            return tuple;
        }
        /// <summary>
        /// 获取项目成员编译数量
        /// </summary>
        /// <param name="projectname"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetMembersBuildNumberOfProject(string projectname, int querydays)
        {
            SingleController single = new SingleController();
            IList<string> projectMember = new List<string>();
            projectMember = GetProjectMembersName(projectname);
            for (int i = 0; i < projectMember.Count(); i++)
            {
                MembersPushMember.Add(projectMember[i], single.GetBuildNumberOfProjectByLongTime(projectMember[i], projectname, querydays));
            }
            return MembersPushMember;
        }
        /// <summary>
        /// 获取项目成员编译成功数量
        /// </summary>
        /// <param name="projectname"></param>
        /// <param name="querydays"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetMembersBuildSuccessNumberOfProject(string projectname, int querydays)
        {
            SingleController single = new SingleController();
            IList<string> projectMember = new List<string>();
            projectMember = GetProjectMembersName(projectname);
            for (int i = 0; i < projectMember.Count(); i++)
            {
                MembersPushMember.Add(projectMember[i], single.GetBuildSuccessNumberOfProjectByLongTime(projectMember[i], projectname, querydays));
            }
            return MembersPushMember;
        }
        /// <summary>
        /// 获取项目管理级别
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public int GetProjectRank(string username, string projectname)
        {
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select monitor,projectMembers from Project where projectName=" + "'" + projectname + "'";
            SqlDataReader read = cmd.ExecuteReader();
            while (read.Read())
            {
                string monitor = read["monitor"].ToString().Trim();
                if (monitor.Contains(username))
                    rank = 2;
                string projectMember = read["projectMembers"].ToString().Trim();
                if (projectMember.Contains(username))
                    rank = 3;
            }
            conn.Close();
            return rank;
        }
        /// <summary>
        /// 更新项目管理员（若无记录，则写入）
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public string GetUpdateProject(string projectName, string projectMonitor, string projectMembers, string groupName)
        {
            string info = "";
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "update Memberproject set projectMonitor=" + "'" + projectMonitor + "',projectmembers=" + "'" + projectMembers + "',groupname=" + "'" + groupName + "' where projectname=" + "'" + projectName + "'";
                cmd.ExecuteNonQuery();
                flag = true;
                info += "更新成功";
            }
            catch (SqlException e)
            {
                flag = false;
                FileStream fs = new FileStream("c:\\text\\log2.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(e.ToString()); // 写入
                sw.Close();
                info += "更新异常";
            }

            return info;
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="groupname">项目名</param>
        /// <returns></returns>
        public string GetDeleteProject(string projectname)
        {
            string returnInfo;
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "update memberproject  Set IsDelete=1 where projectname=" + "'" + projectname + "'";
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
        /// 查询指定项目一段时间内总提交次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetPushTotalOfProject_ByLongTime(string projectname, int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommitBeforeCompiling where  DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111) and projectname=" + "'" + projectname + "' ";
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

            return total;
        }
        /// <summary>
        /// 查询指定项目一段时间内总编译次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetBuildTotalOfProject_ByLongTime(string projectname, int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  and projectname like " + "'%" + projectname + "%'";
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

            return total;
        }
        /// <summary>
        /// 查询指定项目一段时间内总编译成功次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetBuildSuccessOfProject_ByLongTime(string projectname, int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)and projectname like " + "'%" + projectname + "%' and result=1";
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

            return total;
        }
        /// <summary>
        ///  项目issue
        /// </summary>
        /// <param name="projectname">项目名字</param>
        /// <returns></returns>
        public List<IssueEvent> getDataForIssueTable(string projectname)
        {
            List<IssueEvent> memberIssues = new List<IssueEvent>();
            try
            {
                //连接本地数据库
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                //打开数据库
                conn.Open();
                //创建查询语句
                SqlCommand querySingleInfo = conn.CreateCommand();
                querySingleInfo.CommandText = "SELECT issueid, Issue, Initiator, Assignee, Starttime,Updatetime,state FROM MemberIssue  where projectname=" + "'" + projectname + "' group by  issueid, Issue,  Initiator, Assignee, Starttime,Updatetime,state order by  Issue desc";
                SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (singleInfoReader.Read())
                {
                    IssueEvent memberIssue = new IssueEvent();
                    memberIssue.issueid = singleInfoReader["issueid"].ToString().Trim();
                    memberIssue.object_attributes.assignee_id = singleInfoReader["Issue"].ToString().Trim();
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
        /// 获取每天提交次数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetCommitNumber_bylongtime(string projectname, int queryDays)
        {
            queryDays--;
            Dictionary<string, int> commitList = new Dictionary<string, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111) as date, COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommitbeforecompiling where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) and projectname=" + "'" + projectname + "' group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                string date = singleInfoReader["date"].ToString().Trim();
                int number = int.Parse(singleInfoReader["times"].ToString().Trim());
                commitList.Add(date, number);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return commitList;
        }
        /// <summary>
        /// 获取每天编译次数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetBuildNumber_bylongtime(string projectname, int queryDays)
        {
            Dictionary<string, int> buildList = new Dictionary<string, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111) as date, COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) and projectname like " + "'%" + projectname + "%' group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                string date = singleInfoReader["date"].ToString().Trim();
                int number = int.Parse(singleInfoReader["times"].ToString().Trim());
                buildList.Add(date, number);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return buildList;
        }
        /// <summary>
        /// 获取每天编译成功次数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetBuildSuccessNumber_bylongtime(string projectname, int queryDays)
        {
            Dictionary<string, int> buildSuccessList = new Dictionary<string, int>();
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111) as date, COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) and  projectname like " + "'%" + projectname + "%' and result=1 group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                string date = singleInfoReader["date"].ToString().Trim();
                int number = int.Parse(singleInfoReader["times"].ToString().Trim());
                buildSuccessList.Add(date, number);
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return buildSuccessList;
        }



        /// <summary>
        /// 获取自然周、自然月、自然年的提交次数(以及上周期)
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetProjectCommit_Weekly_Monthly_Yearly(string projectName,int queryDays)
        {
            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
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
                querySingleInfo.CommandText = "SET DATEFIRST 1 select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommitBeforeCompiling where DATEPART(wk, committime) = DATEPART(wk, getdate()) and projectName like'%" + projectName + "%' group by DATEPART(w, committime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "SET DATEFIRST 1 select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommitBeforeCompiling where DATEPART(wk, committime) = (DATEPART(wk, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(w, committime)";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = "select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) = (DATEPART(m, getdate())) and projectName like'%" + projectName + "%' group by DATEPART(d, committime)order by day asc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) = (DATEPART(m, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(d, committime)order by day asc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommitBeforeCompiling where DATEPART(yy, committime) = DATEPART(yy, getdate()) and projectName like'%" + projectName + "%' group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommitBeforeCompiling where DATEPART(yy, committime) = (DATEPART(yy, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
        }

        /// <summary>
        /// 获取自然周、自然月、自然年的编译次数(以及上周期)
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetProjectBuild_Weekly_Monthly_Yearly(string projectName, int queryDays)
        {
            Dictionary<int, int> memberCommitOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberCommitTwice = new Dictionary<int, int>();
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
                querySingleInfo.CommandText = "SET DATEFIRST 1 select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommit where DATEPART(wk, committime) = DATEPART(wk, getdate()) and projectName like'%" + projectName + "%' group by DATEPART(w, committime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "SET DATEFIRST 1 select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommit where DATEPART(wk, committime) = (DATEPART(wk, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(w, committime)";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = "select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommit where DATEPART(m, committime) = (DATEPART(m, getdate())) and projectName like'%" + projectName + "%' group by DATEPART(d, committime)order by day asc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommit where DATEPART(m, committime) = (DATEPART(m, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(d, committime)order by day asc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommit where DATEPART(yy, committime) = DATEPART(yy, getdate()) and projectName like'%" + projectName + "%' group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommit where DATEPART(yy, committime) = (DATEPART(yy, getdate())-1) and projectName like'%" + projectName + "%' group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberCommitTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberCommitOnce, memberCommitTwice);
                conn.Close();
                return tupleCommit;
            }
        }









       
    }
}

