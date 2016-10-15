using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyGitlabStatWebAPI.Models
{
    public class Project
    {
        public Project()
        {
            projectName = "";
            projectMonitor = "";
            projectMembers = "";
            groupName = "";
            isDelete = false;
        }
        public bool isDelete { set; get; }
        /// <summary>
        /// 项目名
        /// </summary>
        public string projectName { set; get; }
        /// <summary>
        /// 项目组长
        /// </summary>
        public string projectMonitor { set; get; }
        /// <summary>
        /// 项目成员
        /// </summary>
        public string projectMembers { set; get; }
        /// <summary>
        /// 小组名
        /// </summary>
        public string groupName { set; get; }
    }
}