using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LuckyGitlabStatWebAPI.DAO;
using LuckyGitlabStatWebAPI.Models;
using System.Data.SqlClient;

namespace LuckyGitlabStatWebAPI.Controllers
{
    public class AllMemberController : ApiController
    {
        //所有成员的数据
        string returndata;
        //成员提交总量
        int pushNumber = 0;
        //成员编译总量
        int buildNumber;
        //编译成功总量
        int buildSuccess;
        //编译成功率
        double successRate = 0.00;
        //连接本地数据库
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();
        public AllMemberController()
        {
            /*Empty*/
        }
        #region MyRegion
        /// <summary>
        /// 查询所有人一段时间内总提交次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetPushTotal_AllMember_ByLongTime(int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommitBeforeCompiling where  DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  ";
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
        /// 查询所有人一段时间内总编译次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetBuildTotal_AllMember_ByLongTime(int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  ";
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
        /// 查询所有人一段时间内总编译成功次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>总次数</returns>
        public int GetBuildSuccess_AllMember_ByLongTime(int queryDays)
        {
            queryDays--;
            int total = 0;//提交总次数
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Commitid) as result FROM MemberCommit where DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111) and result=1";
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
        /// 返回提交数据分析
        /// </summary>
        /// <returns></returns>
        public string GetAllData()
        {
            //成员提交量
            pushNumber = GetPushTotal_AllMember_ByLongTime(7);
            //成员编译量
            buildNumber = GetBuildTotal_AllMember_ByLongTime(7);
            //成员编译成功量
            buildSuccess = GetBuildSuccess_AllMember_ByLongTime(7);
            //编译成功率
            if (pushNumber == 0 || buildNumber == 0 || buildSuccess == 0)
                successRate = 0.00;
            else
            {
                successRate = Convert.ToDouble(buildSuccess) / Convert.ToDouble(buildNumber);
                //保留4位小数
                successRate = Math.Round(successRate, 4) * 100;
            }
            returndata = "一周内,共提交" + pushNumber + "次、编译" + buildNumber + "次、成功" + buildSuccess + "次，成功率为" + successRate + "%。";
            return returndata;
        }

        /// <summary>
        /// 获取所有人员一段时间每天提交的次数
        /// </summary>
        /// <param name="queryDays">查询天数</param>
        /// <returns>每天提交次数,以日期为天数，日期从小到大排列</returns>
        public Dictionary<long, int> GetPersonsCommitTotalByDays(int queryDays)
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
            querySingleInfo.CommandText = "SELECT CONVERT(varchar(12), CommitTime, 111) as querydate, COUNT(CONVERT(varchar(12), CommitTime, 111)) as queryTimes FROM MemberCommitBeforeCompiling  where  DATEADD(d,-"+queryDays+", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)   group by CONVERT(varchar(12), CommitTime, 111) order by CONVERT(varchar(12), CommitTime, 111) desc";
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
        /// 获取所有成员每人提交量
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字典树类型(用户名，次数)的列表</returns>
        public Dictionary<string, int> GetMembersPushNumberByDays(int queryday)
        {
            SingleController single = new SingleController();
            RootController root = new RootController();
            //成员姓名，次数
            Dictionary<string, int> membersNumber = new Dictionary<string, int>();
            //成员姓名列表
            List<Member> memberList = root.GetAllUserInfo();
            for (int i = 0; i < memberList.Count(); i++)
            {
                //调用Single方法，赋值给num
                int num = single.GetPushTotal_personally_ByLongTime(memberList[i].username, queryday);
                //结果加入字典树
                string name = memberList[i].username;
                membersNumber.Add(name, num);
            }
            return membersNumber;
        }

        /// <summary>
        /// 获取小组成员每人编译量
        /// </summary>
        /// <param name = "groupname" > 组名 </ param >
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字典树类型(用户名，次数)的列表</returns>
        public Dictionary<string, int> GetMembersBuildNumberByLongtime(int queryday)
        {
            SingleController single = new SingleController();
            RootController root = new RootController();
            //成员姓名，次数
            Dictionary<string, int> membersNumber = new Dictionary<string, int>();
            //成员姓名列表
            List<Member> memberList = root.GetAllUserInfo();
            for (int i = 0; i < memberList.Count(); i++)
            {
                //调用Single方法，赋值给num
                int num = single.GetBuildTotal_personally_ByLongTime(memberList[i].username, queryday);
                //结果加入字典树
                string name = memberList[i].username;
                membersNumber.Add(name, num);
            }
            return membersNumber;
        }
        #endregion
        #region 增加量

        /// <summary>
        /// 获取Issue相对于昨天的增加量
        /// </summary>
        /// <returns></returns>
        public int GetIssueAddThanYesday()
        {
            List<int> issueNumber = new List<int>();
            int[] number = new int[2];
            number[1] = number[0] = 0;
            int i = 0;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), StartTime, 111), COUNT( CONVERT(varchar(12), StartTime, 111)) as times from memberissue where DATEADD(d,-1, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), StartTime, 111) and State='%open%' group by CONVERT(varchar(12), StartTime, 111) order by CONVERT(varchar(12), StartTime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                number[i++] = int.Parse(singleInfoReader["times"].ToString().Trim());
                //issueNumber.Add( int .Parse( singleInfoReader["times"].ToString().Trim()));

            }
            int addNum = number[0] - number[1];
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return addNum;
        }
        /// <summary>
        /// 获取提交次数相对于昨天的增加量
        /// </summary>
        /// <returns></returns>
        public int GetCommitAddThanYesday()
        {
            int[] number = new int[2];
            number[1] = number[0] = 0;
            int i = 0;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111), COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommitbeforecompiling where DATEADD(d,-1, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                number[i++] = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            int addNum = number[0] - number[1];
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return addNum;
        }
        /// <summary>
        /// 获取编译次数相对于昨天的增加量
        /// </summary>
        /// <returns></returns>
        public int GetBuildAddThanYesday()
        {
            int[] number = new int[2];
            number[1] = number[0] = 0;
            int i = 0;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111), COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommit where DATEADD(d,-1, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                number[i++] = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            int addNum = number[0] - number[1];
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return addNum;
        }
        /// <summary>
        /// 获取编译成功次数相对于昨天的增加量
        /// </summary>
        /// <returns></returns>
        public int GetBuildSuccessAddThanYesday()
        {
            int[] number = new int[2];
            number[1] = number[0] = 0;
            int i = 0;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select CONVERT(varchar(12), Committime, 111), COUNT( CONVERT(varchar(12), Committime, 111)) as times from membercommit where DATEADD(d,-1, CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), Committime, 111) and result=1 group by CONVERT(varchar(12), Committime, 111) order by CONVERT(varchar(12), Committime, 111) desc";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                number[i++] = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            int addNum = number[0] - number[1];
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return addNum;
        }
        /// <summary>
        /// 获取当天issue关闭量
        /// </summary>
        /// <returns></returns>
        public int GetIssueCloseNumberToday()
        {
            int number = 0;
            //连接本地数据库
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "select  count(CONVERT(varchar(12), UpdateTime, 111)) as times from memberIssue where CONVERT(varchar(12), getdate(), 111) = CONVERT(varchar(12), UpdateTime, 111)and State='closed'";
            SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (singleInfoReader.Read())
            {
                number = int.Parse(singleInfoReader["times"].ToString().Trim());
            }
            //关闭查询
            singleInfoReader.Close();
            //关闭数据库连接
            conn.Close();
            return number;
        }
        #endregion

        #region 自然日期

        /// <summary>
        /// 获取自然周、自然月、自然年的提交次数
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetMemberCommit_Weekly_Monthly_Yearly(int queryDays)
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
                querySingleInfo.CommandText = " select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommitBeforeCompiling where DATEPART(wk, committime) = DATEPART(wk, getdate()) group by DATEPART(w, committime)";
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

                querySingleInfo.CommandText = " select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommitBeforeCompiling where DATEPART(wk, committime) >= (DATEPART(wk, getdate())-1) group by DATEPART(w, committime)";
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
                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) = DATEPART(m, getdate()) group by DATEPART(d, committime)order by day asc";
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

                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) >= (DATEPART(m, getdate())-1) group by DATEPART(d, committime)order by day asc";
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
                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommitBeforeCompiling where DATEPART(yy, committime) = DATEPART(yy, getdate()) group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
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

                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommitBeforeCompiling where DATEPART(yy, committime) >= (DATEPART(yy, getdate())-1) group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
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
        /// 获取小组成员每人自然周、自然月、自然年提交量
        /// </summary>
        /// < param name="queryday">查询日期</param>
        /// <returns>返回字Tuple 类型 List 列表</returns>
        public List<Tuple<string, Dictionary<int, int>>> GetMembersCommitNumberByDays(int queryday)
        {
            SingleController single = new SingleController();
            RootController root = new RootController();
            //成员姓名，次数
            List<Tuple<string, Dictionary<int, int>>> membersNumber = new List<Tuple<string, Dictionary<int, int>>>();
            //成员姓名列表
            List<Member> memberList = root.GetAllUserInfo();
            for (int i = 0; i < memberList.Count(); i++)
            {
                //调用Single方法，赋值给num
                Tuple<string, Dictionary<int, int>> tupleNumber = single.GetPersionCommit_Weekly_Monthly_Yearly(memberList[i].username, queryday);
                //结果加入字典树
                //string name = memberList[i].username;
                membersNumber.Add(tupleNumber);
            }
            return membersNumber;
        }
        /// <summary>
        /// 获取自然周、自然月、自然年的编译次数
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetMemberBuild_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberbuildOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberbuildTwice = new Dictionary<int, int>();
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
                querySingleInfo.CommandText = " select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommit where DATEPART(wk, committime) = DATEPART(wk, getdate()) group by DATEPART(w, committime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = " select DATEPART(w,committime) as weekday,count(DATEPART(w,committime)) as times from MemberCommit where DATEPART(wk, committime) >= (DATEPART(wk, getdate())-1) group by DATEPART(w, committime)";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberbuildOnce, memberbuildTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommit where DATEPART(m, committime) = DATEPART(m, getdate()) group by DATEPART(d, committime)order by day asc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(d,committime) as day,count(DATEPART(d,committime)) as times from MemberCommitBeforeCompiling where DATEPART(m, committime) >= (DATEPART(m, getdate())-1) group by DATEPART(d, committime)order by day asc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberbuildOnce, memberbuildTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommit where DATEPART(yy, committime) = DATEPART(yy, getdate()) group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(m,committime) as month,count(DATEPART(m,committime)) as times from MemberCommit where DATEPART(yy, committime) >= (DATEPART(yy, getdate())-1) group by DATEPART(m, committime) order by DATEPART(m, committime)desc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberbuildTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberbuildOnce, memberbuildTwice);
                conn.Close();
                return tupleCommit;
            }
        }
        /// <summary>
        /// 获取自然周、自然月、自然年的Issue新建次数
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetMemberIssueOpened_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberIssueOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberIssueTwice = new Dictionary<int, int>();
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
                querySingleInfo.CommandText = " select DATEPART(w,starttime) as weekday,count(DATEPART(w,starttime)) as times from MemberIssue where DATEPART(wk, starttime) = DATEPART(wk, getdate()) and state='opened' group by DATEPART(w, starttime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                querySingleInfo.CommandText = " select DATEPART(w,starttime) as weekday,count(DATEPART(w,starttime)) as times from MemberIssue where DATEPART(wk, starttime) >= (DATEPART(wk, getdate())-1)  and state = 'opened'  group by DATEPART(w, starttime)";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = " select DATEPART(d,starttime) as day,count(DATEPART(d,starttime)) as times from MemberIssue where DATEPART(m, starttime) = DATEPART(m, getdate()) and state='opened' group by DATEPART(d, starttime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(d,starttime) as day,count(DATEPART(d,starttime)) as times from MemberIssue where DATEPART(m, starttime) >= (DATEPART(m, getdate())-1) and state='opened'  group by DATEPART(d, starttime)order by day asc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,starttime) as month,count(DATEPART(m,starttime)) as times from MemberIssue where DATEPART(yy, starttime) = DATEPART(yy, getdate())  and state='opened' group by DATEPART(m, starttime)   order by DATEPART(m, starttime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(m,starttime) as month,count(DATEPART(m,starttime)) as times from MemberIssue where DATEPART(yy, starttime) >= (DATEPART(yy, getdate())-1)  and state='opened' group by DATEPART(m, starttime) order by DATEPART(m, starttime)desc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
        }
        /// <summary>
        /// 获取自然周、自然月、自然年的Issue新建次数
        /// </summary>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public Tuple<Dictionary<int, int>, Dictionary<int, int>> GetMemberIssueClosed_Weekly_Monthly_Yearly(int queryDays)
        {
            Dictionary<int, int> memberIssueOnce = new Dictionary<int, int>();
            Dictionary<int, int> memberIssueTwice = new Dictionary<int, int>();
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
                querySingleInfo.CommandText = " select DATEPART(w,starttime) as weekday,count(DATEPART(w,starttime)) as times from MemberIssue where DATEPART(wk, starttime) = DATEPART(wk, getdate()) and state='opened' group by DATEPART(w, starttime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                querySingleInfo.CommandText = " select DATEPART(w,starttime) as weekday,count(DATEPART(w,starttime)) as times from MemberIssue where DATEPART(wk, starttime) >= (DATEPART(wk, getdate())-1)  and state = 'opened'  group by DATEPART(w, starttime)";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["weekday"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然月查询
            if (queryDays == 30)
            {
                querySingleInfo.CommandText = " select DATEPART(d,starttime) as day,count(DATEPART(d,starttime)) as times from MemberIssue where DATEPART(m, starttime) = DATEPART(m, getdate()) and state='opened' group by DATEPART(d, starttime)";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(d,updatetime) as day,count(DATEPART(d,updatetime)) as times from MemberIssue where DATEPART(m, updatetime) >= (DATEPART(m, getdate())-1) and state='closed'  group by DATEPART(d, updatetime) order by day asc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["day"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
            //自然年查询
            else
            {
                querySingleInfo.CommandText = "  select DATEPART(m,updatetime) as month,count(DATEPART(m,updatetime)) as times from MemberIssue where DATEPART(yy, updatetime) = DATEPART(yy, getdate())  and state='closed' group by DATEPART(m, updatetime) order by DATEPART(m, updatetime)desc";
                SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueOnce.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();

                querySingleInfo.CommandText = "  select DATEPART(m,updatetime) as month,count(DATEPART(m,updatetime)) as times from MemberIssue where DATEPART(yy, updatetime) >= (DATEPART(yy, getdate())-1)  and state='closed' group by DATEPART(m, updatetime) order by DATEPART(m, updatetime)desc";
                getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
                //有多行数据，用while循环
                while (getTotalCommitTimesReader.Read())
                {
                    queryKey = int.Parse(getTotalCommitTimesReader["month"].ToString().Trim());
                    commitNumber = int.Parse(getTotalCommitTimesReader["times"].ToString().Trim());
                    memberIssueTwice.Add(queryKey, commitNumber);
                }
                //关闭查询
                getTotalCommitTimesReader.Close();
                Tuple<Dictionary<int, int>, Dictionary<int, int>> tupleCommit = new Tuple<Dictionary<int, int>, Dictionary<int, int>>(memberIssueOnce, memberIssueTwice);
                conn.Close();
                return tupleCommit;
            }
        }

        #endregion
    }
}

/*
 
        /// <summary>
        /// 一段时间内提交总次数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="queryDays"></param>
        /// <returns></returns>
        public int GetCommitTimes_ByLongTime(int queryDays)
        {
            queryDays--;
            int commitTimes = 0;
            MemberCommit memberCommit = new MemberCommit();
            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Version) as result FROM MemberCommitBeforeCompiling where  DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                memberCommit.projectTotal = getTotalCommitTimesReader["result"].ToString().Trim();
                commitTimes = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();
            return commitTimes;
        }
         /// <summary>
        /// 查询个人一段时间内编译总次数
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="queryDate">查询天数</param>
        /// <returns>memberCommit</returns> 
        public int BuildTimes_ByLongTime(string username, int queryDays)
        {
            int BuildTimes = 0;
            MemberCommit memberCommit = new MemberCommit();

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
                BuildTimes = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();

            return BuildTimes;
        }
       
        /// <summary>
        /// 查询个人一段时间内编译成功总次数
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="queryDate">查询天数</param>
        /// <returns>memberCommit</returns>
        public int BuildSuccessTimes_ByLongTime(string username, int queryDays)
        {

            int buildSuccessTimes = 0;
            MemberCommit memberCommit = new MemberCommit();

            SqlConnection conn = connectLocaldb.ConnectDataBase();
            //打开数据库
            conn.Open();
            //创建查询语句
            SqlCommand querySingleInfo = conn.CreateCommand();
            querySingleInfo.CommandText = "SELECT  COUNT(Result) as result FROM MemberCommit where UserName=" + "'" + username + "'AND AND DATEADD(d,-" + queryDays + ", CONVERT(varchar(12), getdate(), 111)) <= CONVERT(varchar(12), CommitTime, 111)  AND  Result=1 ";
            SqlDataReader getTotalCommitTimesReader = querySingleInfo.ExecuteReader();
            //有多行数据，用while循环
            while (getTotalCommitTimesReader.Read())
            {
                memberCommit.projectTotal = getTotalCommitTimesReader["result"].ToString().Trim();
                buildSuccessTimes = int.Parse(memberCommit.projectTotal);
            }
            //关闭查询
            getTotalCommitTimesReader.Close();
            //关闭数据库连接
            conn.Close();

            return buildSuccessTimes;
        }
   
     */
