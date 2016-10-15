/**
 * Created by DELL on 2016/8/22.
 */
/**
 * Created by Alvis on 2016/8/18.
 */

import React from 'react';
import Dialog from 'material-ui/lib/Dialog';
import TextField from 'material-ui/lib/text-field';
import Select from 'react-select';
import FlatButton from 'material-ui/lib/flat-button';
import NavLink from '../NavLink';


var resultAllGroup;//所有组信息
var resultSearchGroup;//所有组选项
var selectGroup;//选中的组
var selectSex;//选中的性别
var selectRank;//选中的级别

export default React.createClass({

    propTypes: {
        label: React.PropTypes.string,
    },
    //初始化组件
    getInitialState: function () {
        return {
            projectMembers: [],
            groupMonitor:[],groupMembers:[],
            projectMonitor:[],
            newProject: false, newGroup:false,
            newUser:false,projectname:null,
            member:null,
            username:null,
            options:[],
            test_groupName:null,
            test_userName:null,
            test_sex:null,
            test_rank:null
        };
    },
    //渲染组件前的准备
    componentWillMount: function () {
        this.groupListGet();
        this.searchforGroup();
        this.setState({options:resultSearchGroup});

    },
    //打开New User Dialog
    handleOpenForUser :function (){
        this.setState({newUser:true});
        this.setState({groupName:""});
    },
    //groupName select框的onChange事件
    handleSelectGroupName:function (groupName) {
        this.setState({groupName});
        selectGroup=groupName;
        if(selectGroup){
            this.setState({test_groupName:""});
        }
    },
    
    handleClose :function (){
        /*if判断*/
        if(this.state.newProject==true)
        {
            this.setState({newProject:false});
            this.setState({projectMonitor:null});
            this.setState({projectMembers:null});
        }
        if(this.state.newGroup==true)
        {
            this.setState({newGroup:false});
            this.setState({groupMonitor:null});
            this.setState({groupMembers:null});
        }
        if(this.state.newUser==true)
        {
            this.setState({newUser:false});
            this.setState({value:null});
        }
    },
    //username输入框onChange事件
    handleChange: function (event) {
        this.setState({username: event.target.value});
        if (this.state.username!="") {
            this.setState({test_userName:""});
        }
    },
    //获取选中的sex
    handleSex:function () {
        var value="";
        var radio=document.getElementsByName("sex");
        for(var i=0;i<radio.length;i++){
            if(radio[i].checked){
                value=radio[i].value;
                break;
            }
        }
        return value;

    },
    //Sex radioButton有选中情况时，不提示
    checkSex:function () {
        this.setState({test_sex:""});
    },
    //Rank radioButton有选中情况时，不提示
    checkRank:function () {
        this.setState({test_rank:""});

    },
    //获取选中的Rank
    handleRank:function () {
        var value="";
        var radio=document.getElementsByName("rank");
        for(var i=0;i<radio.length;i++){
            if(radio[i].checked){
                value=radio[i].value;
                break;
            }
        }
        return value;

    },
    //New User
    handlePost:function(){
        selectRank=this.handleRank();
        selectSex=this.handleSex();
        var user={
            username:this.state.username,
            group: selectGroup,
            sex:selectSex,
            rank:selectRank
        };
        /*
        * 如果username,sex,rank有一项为空,则提示不为空
        * 且无法向后台传值*/
        if(!this.state.username){
            this.setState({test_userName:"*Not null"});
        }
        if(!selectGroup){
            this.setState({test_groupName:"*Not null"});
        }
        if(selectSex==""){
            this.setState({test_sex:"*Choose one"});
        }
        if(selectRank==""){
            this.setState({test_rank:"*Choose one"});
        }
        if(this.state.username&&selectGroup&&selectSex!=""&&selectRank!=""){
        $.ajax({
            url:"http://202.196.96.79:1500/api/Root/PostRegister",
            data: JSON.stringify(user),
            contentType:"application/json",
            type:"post",
            async:false,
            dataType:"json",
            success:function(data){
                if(data=="Success"){
                    alert("Add a user successfully!");
                    sessionStorage.setItem('result',"success");
                }
                else{
                    alert("username already exists!");
                }
            },
            error: function () {
                alert("Error in program!");
            }
        });
        this.setState({username:null});
        selectGroup="";
        selectSex="";
        selectRank="";
        this.handleClose();
        }

    },
    //获取所有group信息
    groupListGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Group/GetGroupInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                resultAllGroup = data;
            },
            error : function() {
                alert("Error in program!");
            }
        });
    },
    //获取组选项数组
    searchforGroup:function () {
        var i = 0;
        var j=0;
        resultSearchGroup=[];
        for(;i<resultAllGroup.length;i++){
            if(resultAllGroup[i].isDelete==false){
                resultSearchGroup[j] = {label:resultAllGroup[i].GroupName,value:resultAllGroup[i].GroupName};
                j++;
            }
        }
    },

    render() {
        {/*New User Dialog Buttons*/}
        const actions = [
            <NavLink to="/userlist">
            <FlatButton
                label="Submit"
                primary={true}
                keyboardFocused={false}
                onClick={this.handlePost}
            />
            </NavLink>,
            <FlatButton
                label="Cancel"
                primary={true}
                onClick={this.handleClose}
            />
        ];
        return (
            <div >
                {/*open new User Dialog Button*/}
                <a id="Dialog_btn" onClick={()=>{this.handleOpenForUser();}} >New User</a>
                {/*new User Dialog*/}
                <Dialog
                    actions={actions}
                    modal={false}
                    open={this.state.newUser}
                    onRequestClose={this.handleClose} >
                    <div id="Dialog_Title">
                        <p id="title">Add a new user</p><br/>
                    </div>
                    <div id="textField">
                        <TextField id="textFieldNew"
                                   hintText="UserName"
                                   onChange={this.handleChange}
                        />
                        <div id="userDia_userName" dangerouslySetInnerHTML={{__html: this.state.test_userName}}></div>
                        <br/><br/>
                        <div id="userDia_text">
                            <Select multi simpleValue value={this.state.groupName} options={this.state.options} placeholder="Select Group" autoBlur={this.state.select} onChange={this.handleSelectGroupName} />
                            <div id="userDia_groupName" dangerouslySetInnerHTML={{__html: this.state.test_groupName}}></div>
                         </div>
                        <div id="userDia_sex">
                            Sex:  <input id="male" type="radio"  name="sex" value="male" onClick={this.checkSex}/>Male
                            <input id="female" type="radio" name="sex" value="female" onClick={this.checkSex}/>Female
                            <div id="userDia_testSex" dangerouslySetInnerHTML={{__html: this.state.test_sex}}></div>
                        </div>
                        <div id="userDia_rank">
                            Rank:  <input id="admin" type="radio"  name="rank" value="Root"  onClick={this.checkRank}/>Root
                            <input id="mon" type="radio" name="rank" value="Monitor"  onClick={this.checkRank}/>Monitor
                            <input id="mem" type="radio" name="rank" value="Member"  onClick={this.checkRank}/>Member
                            <div id="userDia_testRank" dangerouslySetInnerHTML={{__html: this.state.test_rank}}></div>
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