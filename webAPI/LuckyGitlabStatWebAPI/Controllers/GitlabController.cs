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
    public class GitlabController : ApiController
    {
        //小组名
        string groupname;
        string username;
        ConnectLocalSQL connectLocaldb = new ConnectLocalSQL();
        public GitlabController()
        {
            /*Empty*/
        }
        /// <summary>
        /// 将push events的数据插入数据库
        /// </summary>
        /// <param name="push">接受到的json数据</param>
        /// <returns>是否插入成功</returns>
        public int PushEventInfo([FromBody]PushEvent push)
        {
            bool flag = true;
            ProjectController project = new ProjectController();
            List<string> projectName = new List<string>();
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                string sql = "INSERT INTO MemberCommitBeforeCompiling(Username,ProjectName,Version,GroupName,CommitTime,Branch) VALUES ('" + push.user_name + "','" + push.project.name + "','" + push.after + "','" + groupname + "',getdate(),'" + push.@ref + "') ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                //判断项目是否已存在
                IList<Project> namelist = project.GetAllProjectInfo();
                foreach(var i in namelist)
                {
                   //如果MemberProject表中已经存在该项目
                    if (i.projectName.Contains(push.project.name)|| push.project.name.Contains(i.projectName))
                        flag = false;
                }
                if (flag==true)
                {
                    sql = "INSERT INTO MemberProject(ProjectName,CommitTime,isdelete) VALUES ('" + push.project.name + "',getdate(),'0') ";
                    cmd = new SqlCommand(sql, conn);
                    result = cmd.ExecuteNonQuery();
                }
                //else
                //{
                //    SqlCommand querySingleInfo = conn.CreateCommand();
                //    querySingleInfo.CommandText = "SELECT projectMembers FROM MemberProject where projectName=" + "'" + push.project.name + "'";
                //    SqlDataReader singleInfoReader = querySingleInfo.ExecuteReader();
                //    //有多行数据，用while循环
                //    while (singleInfoReader.Read())
                //    {
                //       username = singleInfoReader["projectMembers"].ToString().Trim();
                //    }
                //    if(!username.Contains(push.user_name))
                //    {
                //        sql = "update MemberProject set ProjectMembers=" + "ProjectMembers+'," + push.user_name + "'";
                //        cmd = new SqlCommand(sql, conn);
                //        result = cmd.ExecuteNonQuery();
                //    }
                //    //关闭查询
                //    singleInfoReader.Close();
                //}
                conn.Close();
                return result;
            }
            catch (Exception e)
            {
                FileStream fs = new FileStream("c:\\test\\log.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(e.ToString()); // 写入
                sw.Close();
                return 0;
            }
            //bool flag = false;
            //try
            //{
            //    SqlConnectionStringBuilder connString2Builder = conncetdb.ConnectDataBase();
            //    using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
            //    {
            //        using (SqlCommand command = conn.CreateCommand())
            //        {
            //            conn.Open();
            //            command.CommandText = "INSERT INTO MemberCommitBeforeCompiling(Username,ProjectName,Version,GroupName,CommitTime,Branch,CommitMessage) VALUES ('" + push.user_name + "','" + push.project.name + "','" + push.after + "',null,getdate(),'" + push.project.default_branch + "','" + push.commits[1] + "') ";
            //            command.ExecuteNonQuery();
            //            conn.Close();
            //        }
            //    }
            //    flag = true;//return true;
            //}
            //catch (SqlException e)
            //{
            //    Console.WriteLine("注册异常");
            //    flag = false;
            //}

        }
        #region MyRegion

        /// <summary>
        /// 将Build events 的数据插入数据库
        /// </summary>
        /// <param name="post">接收到的接送数据</param>
        public int BuildEventInfo([FromBody]BuildEvent build)
        {
            if (build.build_status == "running" || build.build_status == "pending") { return 0; }
            else
            {
                try
                {
                    SqlConnection conn = connectLocaldb.ConnectDataBase();
                    //打开数据库
                    conn.Open();
                    bool buildResult = (build.build_status == "success" ? true : false);
                    string sql = "insert into MemberCommit(UserName,ProjectName,SpendTime,BeginTime,EndTime,Version,Result,CommitTime,Branch) Values('" + build.user.name + "','" + build.project_name + "','" + build.build_duration + "',DATEADD(hh,8,'" + (build.build_started_at).Substring(0, build.build_started_at.Length - 3) + "'),DATEADD(hh,8,'" + (build.build_finished_at).Substring(0, build.build_finished_at.Length - 3) + "'),'" + build.before_sha + "','" + buildResult + "',getdate(),'" + build.@ref + "')";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result;
                }
                catch (Exception e)
                {
                    FileStream fs1 = new FileStream("c:\\test\\log.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter sw1 = new StreamWriter(fs1); // 创建写入流
                    string s = (build.user.name);
                    sw1.WriteLine(e.ToString()); // 写入
                    sw1.Close();
                    return 0;
                }
            }
        }
        /// <summary>
        /// 将Issue Events 数据库插入数据库中
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        string sql; int result;
        public int IssueEventInfo([FromBody]IssueEvent issue)
        {
            try
            {
                SqlConnection conn = connectLocaldb.ConnectDataBase();
                conn.Open();
                sql = "insert into MemberIssue(assignee,projectname,starttime,updatetime,initiator,state,issue) Values('" + issue.assignee.name + "','" + issue.project.name + "',DATEADD(hh,8,'" + (issue.object_attributes.created_at).Substring(0, (issue.object_attributes.created_at).Length - 3) + "'),DATEADD(hh,8,'" + (issue.object_attributes.updated_at).Substring(0, (issue.object_attributes.updated_at).Length - 3) + "'),'" + issue.user.name + "','" + issue.object_attributes.state + "'," + issue.object_attributes.iid + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                FileStream fs = new FileStream("c:\\text\\log.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(e.ToString()); // 写入
                //sw.WriteLine("++"); // 写入
                //sw.WriteLine(issue.before); // 写入
                //sw.WriteLine("++"); // 写入
                sw.Close();
                result = 0;
            }
            return result;
        } 
        #endregion
    }
}