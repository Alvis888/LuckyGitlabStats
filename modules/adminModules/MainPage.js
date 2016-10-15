/**
 * Created by Xin on 2016/8/18.
 */
import React from 'react';
//import MobileTearSheet from '../../../MobileTearSheet';
import List from 'material-ui/lib/lists/list';
import ListItem from 'material-ui/lib/lists/list-item';
import ActionGrade from 'material-ui/lib/svg-icons/action/grade';
import ActionInfo from 'material-ui/lib/svg-icons/action/info';
import ContentInbox from 'material-ui/lib/svg-icons/content/inbox';
import ContentDrafts from 'material-ui/lib/svg-icons/content/drafts';
import ContentSend from 'material-ui/lib/svg-icons/content/send';
import Divider from 'material-ui/lib/divider';
import NavLink from '../NavLink'
import User from './UserDialog'
import Layer from './layer/layer'
import FlatButton from 'material-ui/lib/flat-button';
import AppBar from 'material-ui/lib/app-bar';

var groupAll = [], userAll=[], userCommit = [];
var resultProject = [], resultGroup = [], resultUsername = [];
var order=[];
var projectAll = [];
var proNumber = 0, groNumber = 0, userNumber = 0;

export default React.createClass({
    getInitialState: function() {
        return {project: null, group: null, user: null};
    },
  /*  projectMouseOver: function (i) {
     //   this.projectMember(resultProject[i]);
       // this.projectMembersCommit(resultProject[i]);
        var htmlProject = "";
      /*  htmlProject = "<div class='mainPage_tips'>"+"<div class='mainPage_first'>"+resultProject[i]+"</div>"
            +"Monitor:"+projectAll[i].projectMonitor+"<br/>"
            +"Members:"+projectAll[i].projectMembers+"<br/>"
            +"</div>";*/
  /*      $('#project_'+i).mouseover(function()
        {
            layer.tips(resultProject[i], '#project_'+i, {time: 900000});
        });
    },
    groupMouseOver: function (i) {
     //   this.groupProject(resultGroup[i]);
        var htmlGroup = "";
        htmlGroup = "<div class='mainPage_tips'><div class='mainPage_first'>"+resultGroup[i]+"</div>"
            +"Monitor:"+groupAll[i].GroupMonitor+"<br/>"
            +"Members:"+groupAll[i].GroupMembers+"<br/>"
            +"Project:"+"babycare&nbsp;..."+"</div>";
        $('#group_'+i).mouseover(function()
        {
            layer.tips(htmlGroup, '#group_'+i, {time: 900000});
        });
    },
    userMouseOver: function (i) {
        //this.userCommit(resultUsername[i]);
        var htmlUser = "";
        var sex = "";
        if(userAll[i].sex == "False") {
            sex = 'female';
        }
        else
            sex = 'male';
        htmlUser = "<div class='mainPage_tips'><div class='mainPage_first'>"+resultUsername[i]+"</div>"
            +"sex:"+sex+"<br/>"
            +"group:"+userAll[i].groupName+"<br/>"
            +"project:"+"fundbook&nbsp;12"+"<br/>"+"<div class='mainPage_project'>"+"LuckyGitlabStats&nbsp;17"+"</div>"+"</div>";
        /*  setTimeout((i)=>{this.output(i)},2000);
           function output(i) {
            $('#user_'+i).mouseover(function()
            {
                layer.tips(html, '#user_'+i, {time: 900000});
            });
        }*/
    /*    $('#user_'+i).mouseover(function()
        {
            layer.tips(htmlUser, '#user_'+i, {time: 900000});
        });
    },
    output: function (i) {
        $('#user_'+i).mouseover(function()
        {
            layer.tips(html, '#user_'+i, {time: 900000});
        });
    },
    mouseLeave: function (i) {
        $('#project_'+i).mouseleave(function()
        {
            layer.tips('', '#project_'+i);
        });
        $('#group_'+i).mouseleave(function()
        {
            layer.tips('', '#group_'+i);
        });
        $('#user_'+i).mouseleave(function()
        {
            layer.tips('', '#user_'+i);
        });
    },*/
    componentWillMount: function () {
        this.listGet();
        this.set();
    },
    set: function () {
        this.setState({project: resultProject});
        this.setState({group: resultGroup});
        this.setState({user: resultUsername});
    },
    listGet: function () {
        //get project list
        $.ajax({
            url:'http://202.196.96.79:1500/api/project/GetAllProjectInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                proNumber = 0;
                for(var i = 0, j = 0; i < data.length; i++){
                    if(data[i].isDelete == false){
                        proNumber++;
                        if(j < 5){
                            projectAll[i] = data[i];
                            resultProject[j] = data[i].projectName;
                            j++;
                        }
                    }
                    else{
                        continue;
                    }
                }
            },
            error : function() {
                alert("failed to get number of project!");
            }
        });
        //get group list
        $.ajax({
            url:'http://202.196.96.79:1500/api/Group/GetGroupInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                groNumber = 0;
                for(var i = 0, j = 0; i < data.length; i++){
                    if(data[i].isDelete == false){
                        groNumber++;
                        if(j < 5){
                            groupAll[i] = data[i];
                            resultGroup[j] = data[i].GroupName;
                            j++;
                        }
                    }
                    else{
                        continue;
                    }
                }
            },
            error : function() {
                alert("failed to get number of group!");
            }
        });
        //get user list
        $.ajax({
            url:'http://202.196.96.79:1500/api/Root/GetAllUserInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                userNumber = 0;
                for(var i = 0, j = 0; i < data.length; i++){
                    if(data[i].isdelete == 'False'){
                        userNumber++;
                        if(j < 5){
                            userAll[i] = data[i];
                            resultUsername[j] = data[i].username;
                            j++;
                        }
                    }
                    else{
                        continue;
                    }
                }
            },
            error : function() {
                alert("failed to get number of user!");
            }
        });
    },
   /* projectMember: function (project) {
        $.ajax({
            url:'http://202.196.96.79:1500/api/project/GetProjectMembersName',
            type:"get",
            data:{projectname: project},
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                console.log(data);
                projectMember = data;
            },
            error : function() {
                console.log("ProjectMembers failed to get data!");
            }
        });
    },
    projectMembersCommit: function (project) {
        $.ajax({
            url:'http://202.196.96.79:1500/api/project/GetMembersPushNumberOfProject',
            type:"get",
            data:{projectname: project, querydays: 7},
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                projectMembersCommit = data;
                console.log('projectMembersCommit');
                console.log(data);
            },
            error : function() {
                alert("ProjectMembersCommit failed to get data!");
            }
        });
    },
    groupProject: function (group) {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Project/GetProjectNameGrouply',
            type:"get",
            data:{groupname: group},
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                order = data;
                console.log('groupproject');
                console.log(data);
            },
            error: function() {
                alert("groupProject failed to get data!");
            }
        });
    },
    userCommit: function (user) {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Single/GetMembersProjectsPushNumber',
            type:"get",
            data:{username: user, queryDays: 7},
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                userCommit = data;
                console.log('userCommit');
                console.log(data);
            },
            error : function() {
                alert("userCommit failed to get data!");
            }
        });
    },*/
    render(){
        var self = this;
        var projectList = React.Children.map(this.state.project, function (child,i) {
            return <li id={'project_'+i}>{child}</li>;
        });
        var groupList = React.Children.map(this.state.group, function (child,i) {
            return <li id={'group_'+i}>{child}</li>;
        });
        var userList = React.Children.map(this.state.user, function (child,i) {
            return <li id={'user_'+i}>{child}</li>;
        });
        return(
            <div>
                <div id="mainPage_left">
                    <p className="mainPage_title">Admin Area</p>
                </div>
                <div id="mainPage_main">
                    <div className="mainPage_paper">
                        <div className="mainPage_type">Projects</div>
                        <div className="mainPage_number"><NavLink to="/projectlist" className="MainPage_navLink">{proNumber}</NavLink></div>
                        <div className="mainPage_ul">
                            <ul id="ul">
                                {projectList}
                            </ul>
                        </div>
                    </div>
                    <div className="mainPage_paper">
                        <div className="mainPage_type">Groups</div>
                        <div className="mainPage_number"><NavLink to="/grouplist" className="MainPage_navLink">{groNumber}</NavLink></div>
                        <div className="mainPage_ul">
                            <ul id="ul1">
                                {groupList}
                            </ul>
                        </div>
                    </div>
                    <div className="mainPage_paper">
                        <div className="mainPage_type">Users</div>
                        <div className="mainPage_number"><NavLink to="/userlist" className="MainPage_navLink">{userNumber}</NavLink></div>
                        <div className="mainPage_ul">
                            <ul>
                                {userList}
                            </ul>
                        </div>
                     </div>
                </div>
            </div>
        )
    }
})



                                                                                                                                                                                     