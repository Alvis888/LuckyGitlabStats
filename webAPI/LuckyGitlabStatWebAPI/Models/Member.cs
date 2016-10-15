using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyGitlabStatWebAPI.Models
{
    public class Member
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { set; get; }
        /// <summary>
        /// 登陆密码
        /// </summary>
        public string password { set; get; }
        /// <summary>
        /// outlook邮箱
        /// </summary>`
        public string email { set; get; }
        /// <summary>
        /// 用户性别
        /// </summary>
        public string sex { set; get; }
        /// <summary>
        /// 用户级别
        /// </summary>
        public string rank { set; get; }
        /// <summary>
        /// 组名
        /// </summary>
        public string groupName { set; get; }
        /// <summary>
        /// 是否已经能够被删除
        /// </summary>
        public string isdelete { set; get; }
    }
}