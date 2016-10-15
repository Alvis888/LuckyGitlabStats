/**
 * Created by Xin on 2016/8/18.
 */
import React from 'react';
import DropDownMenu from 'material-ui/lib/DropDownMenu';
import MenuItem from 'material-ui/lib/menus/menu-item';
import NavLink from '../NavLink'
import CommitChart from './MainPageCharts/CommitChart'
import BuildChart from './MainPageCharts/BuildChart'
import IssueChart from './MainPageCharts/IssueChart'

var timeFrame;
var projectNumber = 0, projectChange;
var groupNumber = 0, groupChange;
var userNumber = 0, userChange;
var commitNowTotal, commitChange;
var buildNowTotal, buildChange;
var issueOpenedNow, issueOpenedChange, issueClosedChange;

const style1 = {
    width: 130,
    fontSize: 18
};

export default React.createClass({
    getInitialState: function() {
        this.proGroUserTotal();                            //获取当前时间段内项目、小组、成员的数量
        timeFrame = sessionStorage.getItem('timeFrame');   //获取默认时间段：week
        return {value: 1};                                 //设置DropDownMenu控件默认显示week
    },
    //当点击时间段变化时改变DropDownMenu控件显示的时间段
    timeFrameChange: function (event, index, value) {
        this.setState({value});
    },
    componentWillMount: function () {
        //获取当前时间段内项目、小组、成员的数量相对前一个时间段的变化量
        this.proGroUserChange();
        //获取当前时间段内提交、编译、issue的总量，及相对前一个时间段的变化量
        this.ComBuiIsschange();
        //获取当前时间段内成员的提交量
        this.userRanking();
    },
    componentWillUpdate:function(){
        //判断时间段是否改变，若改变则重新赋值,且重新渲染
        if(sessionStorage.getItem('timeFrame') != timeFrame){
            timeFrame = sessionStorage.getItem('timeFrame');
            this.componentWillMount();
        }
    },
    proGroUserTotal: function () {
        //获取项目总数
        $.ajax({
            url:'http://202.196.96.79:1500/api/project/GetAllProjectInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                for(var i = 0; i < data.length; i++){
                    if(data[i].isDelete == false){
                        projectNumber++;
                    }
                }
            },
            error : function() {
                alert("failed to get project number in MainPage!");
            }
        });
        //获取小组总数
        $.ajax({
            url:'http://202.196.96.79:1500/api/Group/GetGroupInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                for(var i = 0; i < data.length; i++){
                    if(data[i].isDelete == false){
                        groupNumber++;
                    }
                }
            },
            error : function() {
                alert("failed to get group number in MainPage!");
            }
        });
        //获取成员总数
        $.ajax({
            url:'http://202.196.96.79:1500/api/Root/GetAllUserInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                for(var i = 0; i < data.length; i++){
                    if(data[i].isdelete == 'False'){
                        userNumber++;
                    }
                }
                console.log('userNumber: '+userNumber);
            },
            error : function() {
                alert("failed to get user number in MainPage!");
            }
        });
    },
    proGroUserChange: function () {
        //获取一段时间内项目的变化量
        $.ajax({
            url:'http://202.196.96.79:1500/api/root/GetProjectAdd_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                projectChange = 0;
                if(data > 0){
                    projectChange = "+" + data;
                }
                else{
                    projectChange = data;
                }
            },
            error : function() {
                alert("failed to get project data in MainPage!");
            }
        });
        //获取一段时间内小组的变化量
        $.ajax({
            url:'http://202.196.96.79:1500/api/root/GetGroupAdd_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                groupChange = 0;
                if(data > 0){
                    groupChange = "+" + data;
                }
                else{
                    groupChange = data;
                }
            },
            error : function() {
                alert("failed to get group data in MainPage!");
            }
        });
        //获取一段时间内成员的变化量
        $.ajax({
            url:'http://202.196.96.79:1500/api/root/GetUserAdd_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                userNumber = 0;
                if(data > 0){
                    userChange = "+" + data;
                }
                else{
                    userChange = data;
                }
            },
            error : function() {
                alert("failed to get user data in MainPage!");
            }
         });
    },
    ComBuiIsschange: function () {
        //获取一个时间段内的总提交情况
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberCommit_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                commitNowTotal = 0; 
                commitChange = 0;
                var commitTotal = 0, commitLastTotal = 0;
                for (var i in data.m_Item1) {
                    commitNowTotal += data.m_Item1[i];
                }
                for (var j in data.m_Item2) {
                    commitTotal += data.m_Item2[j];
                }
                commitLastTotal = commitTotal - commitNowTotal;
                if(commitNowTotal - commitLastTotal > 0){
                    commitChange = "+" + (commitNowTotal - commitLastTotal);
                }
                else{
                    commitChange = commitNowTotal - commitLastTotal;
                }
            },
            error : function() {
                alert("failed to get commit data in MainPage!");
            }
        });
        //获取一个时间段内的总编译情况
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberBuild_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                buildNowTotal = 0;
                buildChange = 0;
                var buildTotal = 0, buildLastTotal = 0;
                for (var i in data.m_Item1) {
                    buildNowTotal += data.m_Item1[i];
                }
                for (var j in data.m_Item2) {
                    buildTotal += data.m_Item2[j];
                }
                buildLastTotal = buildTotal - buildNowTotal;
                if(buildNowTotal - buildLastTotal > 0){
                    buildChange = "+" + (buildNowTotal - buildLastTotal);
                }
                else{
                    buildChange = buildNowTotal - buildLastTotal;
                }
            },
            error : function() {
                alert("failed to get commit data in MainPage!");
            }
        });
        //获取一个时间段内未关闭的issue的情况
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberIssueOpened_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                issueOpenedNow = 0;
                issueOpenedChange = 0;
                issueClosedChange = 0;
                var issueOpenedTotal = 0, issueOpenedLast = 0;
                for (var i in data.m_Item1) {
                    issueOpenedNow += data.m_Item1[i];
                }
                for (var j in data.m_Item2) {
                    issueOpenedTotal += data.m_Item2[j];
                }
                issueOpenedLast = issueOpenedTotal - issueOpenedNow;
                if(issueOpenedNow - issueOpenedLast > 0){
                    issueOpenedChange = "+" + (issueOpenedNow - issueOpenedLast);
                }
                else{
                    issueOpenedChange = issueOpenedNow - issueOpenedLast;
                }
            },
            error : function() {
                alert("failed to get opened issue data in MainPage!");
            }
        });
        //获取一个时间段内已关闭的issue的数量
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberIssueClosed_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                issueOpenedChange = 0;
                issueClosedChange = 0;
                var issueClosedTotal = 0, issueClosedLast = 0, issueClosedNow = 0;
                for (var i in data.m_Item1) {
                    issueClosedNow += data.m_Item1[i];
                }
                for (var j in data.m_Item2) {
                    issueClosedTotal += data.m_Item2[j];
                }
                issueClosedLast = issueClosedTotal - issueClosedNow;
                if(issueClosedNow - issueClosedLast > 0){
                    issueClosedChange = "+" + (issueClosedNow - issueClosedLast);
                }
                else{
                    issueClosedChange = issueClosedNow - issueClosedLast;
                }
            },
            error : function() {
                alert("failed to get closed issue data in MainPage!");
            }
        });
    },
    userRanking: function () {
        //获取一个时间段内每个成员的提交量
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMembersPushNumberByDays',
            data:{queryday: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                
            },
            error : function() {
                alert("failed to get user's committing times in MainPage!");
            }
        });
    },
    timeFrame7: function () {
        sessionStorage.setItem('timeFrame', 7);
    },
    timeFrame30: function () {
        sessionStorage.setItem('timeFrame', 30);
    },
    timeFrame365: function () {
        sessionStorage.setItem('timeFrame', 365);
    },
    render(){
        return(
            <div>
                <div id="mainPage_left">
                    <p className="mainPage_title">Admin Area</p>
                </div>
                <div id="mainPage_time">
                    <DropDownMenu style={style1} value={this.state.value} onChange={this.timeFrameChange}>
                        <MenuItem value={1} primaryText="week" onClick={this.timeFrame7}><NavLink to="/mainpage"/></MenuItem>
                        <MenuItem value={2} primaryText="month" onClick={this.timeFrame30}><NavLink to="/mainpage"/></MenuItem>
                        <MenuItem value={3} primaryText="year" onClick={this.timeFrame365}><NavLink to="/mainpage"/></MenuItem>
                    </DropDownMenu>
                </div>
                <div className="mainPage_part">
                    <div className="mainPage_partOnePaper">
                        <div className="mainPage_type">Project</div>
                        <div className="mainPage_change">{projectChange}</div>
                        <div className="mainPage_number">{projectNumber}</div>
                        <NavLink to="/projectlist" className="mainPage_nav" ><div className="mainPage_button">Project List</div></NavLink>
                    </div>
                    <div className="mainPage_partOnePaper">
                        <div className="mainPage_type">Group</div>
                        <div className="mainPage_change">{groupChange}</div>
                        <div className="mainPage_number">{groupNumber}</div>
                        <NavLink to="/grouplist" className="mainPage_nav" ><div className="mainPage_button">Group List</div></NavLink>
                    </div>
                    <div className="mainPage_partOnePaper">
                        <div className="mainPage_type">User</div>
                        <div className="mainPage_change">{userChange}</div>
                        <div className="mainPage_number">{userNumber}</div>
                        <NavLink to="/userlist" className="mainPage_nav" ><div className="mainPage_button">User List</div></NavLink>
                    </div>
                </div>
                <div className="mainPage_divide1"></div>
                <div className="mainPage_part">
                    <div className="mainPage_partTwoPaper">
                        <div className="mainPage_type">Commit</div>
                        <div className="mainPage_change">{commitChange}</div>
                        <div className="mainPage_partTwoNumber">{commitNowTotal}</div>
                    </div>
                    <div className="mainPage_barChart"><CommitChart/></div>
                </div>
                <div className="mainPage_part">
                    <div className="mainPage_partTwoPaper">
                        <div className="mainPage_type">Build</div>
                        <div className="mainPage_change">{buildChange}</div>
                        <div className="mainPage_partTwoNumber">{buildNowTotal}</div>
                    </div>
                    <div className="mainPage_barChart"><BuildChart/></div>
                </div>
                <div className="mainPage_part">
                    <div className="mainPage_partTwoPaper">
                        <div className="mainPage_type">Issue</div>
                        <div className="mainPage_issueOpenedChange">opened:{issueOpenedChange}</div><br/>
                        <div className="mainPage_issueClosedChange">closed:{issueClosedChange}</div>
                        <div className="mainPage_partTwoNumber">{issueOpenedNow}</div>
                    </div>
                    <div className="mainPage_barChart"><IssueChart/></div>
                </div>
                <div className="mainPage_divide"></div>
                <div className="mainPage_horizontalBarChart"></div>
            </div>
        )
    }
})



                                                                                                                                                                                     