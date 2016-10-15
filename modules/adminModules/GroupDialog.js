/**
 * Created by Alvis on 2016/8/18.
 */

import React from 'react';
import Dialog from 'material-ui/lib/Dialog';
import TextField from 'material-ui/lib/text-field';
import Select from 'react-select';
import FlatButton from 'material-ui/lib/flat-button';
import NavLink from '../NavLink'

var resultAll = [];//所有的user信息
var resultSearch = [];//userName选项数组
var monitorname = [];//选中的组长
var membername=[];//选中的组员
var resultUsername=[];//所有的username
var result="false";
var alertText;



export default React.createClass({


    propTypes: {
        label: React.PropTypes.string,
    },
    //初始化组件
    getInitialState: function () {
        return {
            options: [], projectMembers: [],
            groupMonitor:[],groupMembers:[],
            projectMonitor:[],
            newProject: false, newGroup:false,
            newUser:false,projectname:null,
            monitor:[],
            member:[],
            groupname:null,
            test_groupName:null,
        };
    },
    //渲染组件的准备
    componentWillMount: function () {
        this.userListGet();
        this.username();
        this.searchforeach();
        this.setState({options:resultSearch});
    },
   
    //打开New Group Dialog
    handleOpenForGroup :function (){
        this.setState({newGroup:true});
    },
    //关闭New Group Dialog
    handleClose :function (){
        /*if判断*/
        if(this.state.newGroup==true)
        {
            this.setState({newGroup:false});
            this.setState({groupMonitor:null});
            this.setState({groupMembers:null});
        }

    },
    //输入框onChange事件
    handleChange: function (event) {
        this.setState({groupname: event.target.value});
        /*
        * 如果输入的有值，则不提示*/
        if(this.state.groupname){
            this.setState({test_groupName:""});
        }
    },
    //groupMonitor select框onChange事件
    handleSelectGroupMonitor :function(groupMonitor) {
        this.setState({ groupMonitor });
        monitorname=groupMonitor;

    },
    //groupMembers select框onChange事件
    handleSelectGroupMembers :function(groupMembers) {
        this.setState({ groupMembers });
        membername=groupMembers;
    },
    //获取所有user信息
    userListGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Root/GetAllUserInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                resultAll = data;
            },
            error : function() {
                alert("Error in program!");
            }
        });
    },
    //获取所有username
    username:function () {
        var i=0;
        var j=0;
        for(;i<resultAll.length;i++){
            if(resultAll[i].isdelete=="False"){
            resultUsername[j]=resultAll[i].username;
                j++;
            }
        }
    },
    //username选项数组
    searchforeach:function () {
        var i = 0;
        for(;i<resultUsername.length;i++){
            resultSearch[i] = {label:resultUsername[i],value:resultUsername[i]};
        }
    },
    //新建组
    postGroupInfo:function () {
        var group={
            GroupName:this.state.groupname,
            GroupMonitor:monitorname,
            GroupMembers:membername
        };
        /*
        * GroupName为空时，提示为空且不与后台交互*/
        if(!this.state.groupname){
            this.setState({test_groupName:"*Not null"});
        }
        if(this.state.groupname){
        $.ajax({
            url:"http://202.196.96.79:1500/api/Root/PostNewGroup",
            data:JSON.stringify(group),
            contentType:"application/json",
            type:"post",
            cache:false,
            async:false,
            dataType:"json",
            success:function(data){
                if(data=="成功"){
                alert("Add a new group successfully!");
                sessionStorage.setItem('result',"success");
                }
                if(data=="组名已存在"){
                    alert("Group name already exists!");
                }
            },
            error: function () {
                alert("Error in program!");
            }
        });
        this.handleClose();
        }

    },
    render() {
        {/*new group buttons*/}
        const actions = [
           <NavLink to="grouplist">
               <FlatButton
                label="Submit"
                primary={true}
                onTouchTap={this.postGroupInfo}
            />
           </NavLink>,
            <FlatButton
                label="Cancel"
                primary={true}
                onTouchTap={this.handleClose}
            />
        ];
        return (
            <div >
                {/*New Group Button*/}
                <a id="Dialog_btn" onClick={this.handleOpenForGroup} >New Group</a>
                {/*new Group Dialog*/}
                <Dialog
                    actions={actions}
                    modal={false}
                    open={this.state.newGroup}
                    onRequestClose={this.handleClose} >
                    <div id="Dialog_Title">
                        <h2 id="title">Add a new group</h2><br/>

                    </div>
                    <div id="textField">
                        <TextField id="textFieldProNew"
                                   hintText="GroupName"
                                   onChange={this.handleChange}
                        />
                        <div id="groupDia_groupName" dangerouslySetInnerHTML={{__html: this.state.test_groupName}}></div>
                        <br/><br/>
                        <div className="section">
                            <h3 className="section-heading">{this.props.label}</h3>
                            <Select multi simpleValue  value={this.state.groupMonitor}  options={this.state.options} placeholder="Select Monitor" onChange={this.handleSelectGroupMonitor} />
                        </div>
                        <div className="section">
                            <h3 className="section-heading">{this.props.label}</h3>
                            <Select multi simpleValue  value={this.state.groupMembers}  options={this.state.options} placeholder="Select Members"  onChange={this.handleSelectGroupMembers} />
                        </div>
                    </div>
                </Dialog>
            </div>
        );
    }
})
{/*   propTypes: {
 React.PropTypes 提供很多验证器 (validator) 来验证传入数据的有效性。
 当向 props 传入无效数据时，JavaScript 控制台会抛出警告。
 注意为了性能考虑，只在开发环境验证 propTypes*/}