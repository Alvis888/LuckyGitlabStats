/**
 * Created by DELL on 2016/8/22.
 */
/**
 * Created by DELL on 2016/8/18.
 */
import React from 'react'
import List from 'material-ui/lib/lists/list';
import ListItem from 'material-ui/lib/lists/list-item';
import Avatar from 'material-ui/lib/avatar';
import {pinkA200, transparent,grey600,pink100,grey800} from 'material-ui/lib/styles/colors';
import Edit from 'material-ui/lib/svg-icons/editor/mode-edit';
import Delete from 'material-ui/lib/svg-icons/action/delete';
import Dialog from 'material-ui/lib/dialog';
import FlatButton from 'material-ui/lib/flat-button';
import UserDialog from './UserDialog'
import Select from 'react-select';
import Tooltip from 'react-bootstrap/lib/Tooltip';
import OverlayTrigger from 'react-bootstrap/lib/OverlayTrigger';
import NavLink from '../NavLink';
import TextField from 'material-ui/lib/text-field';


var resultAll = [];//用户信息
var resultSearch = [];//username选项数组
var resultUsername=[];//所有username
var rank;//编辑后的rank
var usernameEdit;//编辑后的username
var rankDefault;//编辑前的rank
var resultRank;//所有用户rank数组
var isSearch=0;//有无搜索
var deleteValue;//删除的username
var editValue;//编辑的username
var alertText;//弹窗内容


const iconStyles = {
    marginRight: 10,

};
const tooltipDelete = (
    <Tooltip id="tooltip">Delete</Tooltip>

);
const tooltipEdit = (
    <Tooltip id="tooltip">Edit</Tooltip>

);


const testStyle={
    fontSize: "17px",
    fontcolor:"#5c5c5c"

};

export default React.createClass({
    propTypes: {
        label: React.PropTypes.string,
    },
    //初始化组件
    getInitialState: function () {
        return {
            options:[], projectMembers: [],
            groupMonitor:[],groupMembers:[],
            projectMonitor:[],
            newProject: false, newGroup:false,
            newUser:false,projectname:null,
            member:null,
            deleteOpen:false,
            select:true,
            userEdit:null,
            admin:null,
            mon:null,
            mem:null,
            value:null
        };
    },
    //即将渲染组件
    componentWillMount: function () {
        this.userListGet();
        this.username();
        this.searchforeach();
        this.setState({select:true});
        this.setState({result:resultUsername});
        this.setState({options:resultSearch});
        this.setState({userName:""});

    },
    //更新组件
    componentWillUpdate:function(){
        /*
        * 若后台方法成功，则重新渲染组件*/
        if(sessionStorage.getItem('result')!="unknown"){
            console.log("result:"+sessionStorage.getItem('result'));
            this.componentWillMount();
            sessionStorage.setItem('result',"unknown");
        }
    },
    //打开user Edit Dialog
    handleOpenForUser :function (i){
        this.setState({newUser:true});
        var j=0;
        var editNum;
        if(isSearch==1){
        for(;j<resultUsername.length;j++){
            if(editValue==resultUsername[j]){
                this.setState({userEdit: resultUsername[j]});
                usernameEdit = resultUsername[j];
                rankDefault=resultRank[j];
                editNum=j;
                break;
            }
        }
        }
        if(isSearch==0){
           this.setState({userEdit: resultUsername[i]});
           usernameEdit = resultUsername[i];
           rankDefault=resultRank[i];
            editNum=i;
        }
        /**
         * 编辑前的rank radio的判断*/
        if(resultRank[editNum]==1){
            this.setState({admin:"checked"});
            this.setState({mon:""});
            this.setState({mem:""});
        }
        if(resultRank[editNum]==2){
            this.setState({admin:""});
            this.setState({mon:"checked"});
            this.setState({mem:""});
        }
        if(resultRank[editNum]==3){
            this.setState({admin:""});
            this.setState({mon:""});
            this.setState({mem:"checked"});
        }


    },
    //编辑后的rank radio选中判断
    radio:function () {
        var value="";
        var radio=document.getElementsByName("rank");
        for(var i=0;i<radio.length;i++){
            if(radio[i].checked){
                    value=radio[i].value;
                    if(value=="Root"){
                    this.setState({admin:"checked"});
                    this.setState({mon:""});
                    this.setState({mem:""});
                    rank=1;
                    break;
                }
                if(value=="Monitor"){
                    this.setState({admin:""});
                    this.setState({mon:"checked"});
                    this.setState({mem:""});
                    rank=2;
                    break;
                }
                if(value=="Member"){
                    this.setState({admin:""});
                    this.setState({mon:""});
                    this.setState({mem:"checked"});
                    rank=3;
                    break;

                }

            }
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
    //User Edit Dialog输入框onChange事件
    handleChangeEdit: function (event) {
        this.setState({username: event.target.value});
    },
    //实现search功能
    handleSelectUserMonitor :function(userName) {
        this.setState({ userName });
        if(userName.value){
            this.setState({result:userName.value});
            isSearch=1;
            deleteValue=userName.value;
            editValue=userName.value;
        }
        else{
            this.setState({result:resultUsername});
            isSearch=0;
        }
    },
 
    //打开Delete Dialog
    deleteOpen:function (i) {
        if(isSearch==0){
            deleteValue = resultUsername[i];
        }
        this.setState({deleteOpen:true});
    },
    //关闭Delete Dialog
    deleteClose:function () {
        this.setState({deleteOpen:false});
    },
    //获取所有用户信息
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
        isSearch=0;
    },
    //删除用户
    userDelete: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Root/GetDeleteUser',
            data:{username:deleteValue},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                if(data){
                    alert("Delete the user successfully!");
                    sessionStorage.setItem('result',"success");
                }
                else{
                    alert("Fail to delete the user successfully!");
                }
            },
            error : function() {
                alert("Error in program!");
            }
        });
        this.deleteClose();
    },
    //更新用户信息
    getUpdateUserInfo:function () {
        if(rank==null){
            rank=rankDefault;
        }
        $.ajax({
            url:"http://202.196.96.79:1500/api/Root/GetUpdateUserRank",
            data:{username:usernameEdit,rank:rank},
            contentType:"application/json",
            type:"get",
            cache:false,
            async:false,
            dataType:"json",
            success:function(data){
                if(data){
                    alert("Edit the user successfully!");
                    sessionStorage.setItem('result',"success");
                }
                else{
                    alert("Fail to edit the user!");
                }

            },
            error: function () {
                alert("Error in program!");
            }
        });
        this.handleClose();
    },
    //获取所有username,rank
    username:function () {
        var i=0;
        var j=0;
        resultUsername=[];
        resultRank=[];
         for(;i<resultAll.length;i++){
             if(resultAll[i].isdelete=="False"){
                 resultUsername[j]=resultAll[i].username;
                 resultRank[j]=resultAll[i].rank;
                 j++;
             }

         }
    },
    //获取username选项数组
    searchforeach:function () {
        var i = 0;
        for(;i<resultAll.length;i++){
            resultSearch[i] = {label:resultUsername[i],value:resultUsername[i]}
        }
    },
    //列表项的onClick事件，将查询的username，queryDays传给个人页面
    sessionStorage: function (name) {
        sessionStorage.setItem('username', name);
        sessionStorage.setItem('days', 7);
    },
    render() {
        var self=this;
        {/*user Edit Dialog按钮*/}
        const actionsEdit = [
            <NavLink to="/userlist">
                <FlatButton
                    label="Submit"
                    primary={true}
                    keyboardFocused={true}
                    onTouchTap={this. getUpdateUserInfo}
                />
            </NavLink>,
            <FlatButton
                label="Cancel"
                primary={true}
                onTouchTap={this.handleClose}
            />
        ];
        {/*user Delete Dialog按钮*/}
        const actions = [
            <FlatButton
                label="Yes"
                primary={true}
                onTouchTap={this.userDelete}
            />,
            <FlatButton
                label="No"
                primary={true}
                onTouchTap={this.deleteClose}
            />
        ];





        return(
            <div id="userList_user">
                <div id="userList_header">
                    <div id="userList_left">
                        {/* title*/}
                        <p className="userList_title">User List</p>
                    </div>
                    {/* 搜索框*/}
                    <div id="userList_right">
                        <div className="projectList_section">
                            <Select  value={this.state.userName} resetValue="reset" options={this.state.options} placeholder="Search" autoBlur={this.state.select} onChange={this.handleSelectUserMonitor} />
                        </div>

                    </div>
                    {/* New User Dialog*/}
                    <div id="userList_new">
                        <UserDialog/>
                    </div>
                </div>
                <div id="userList_right">


                </div>
                {/* User List*/}
                <div id="userList_list">

                    <List id="userList_realList">
                        {
                            React.Children.map(this.state.result, function (child,i) {
                                return<ListItem

                                    style={testStyle}
                                    primaryText={<NavLink to="/app" onClick={()=>{self.sessionStorage(child)}}>{child}</NavLink>}
                                    rightAvatar={<div><OverlayTrigger placement="bottom"  overlay={tooltipDelete} ><Delete style={iconStyles} color={grey600} className="delete" onClick={()=>{self.deleteOpen(i)}}/></OverlayTrigger></div>}
                                    leftAvatar={ <Avatar color={grey800} backgroundColor={pink100} style={{left: 8}}

                                    >{child.substring(0,1)}</Avatar>}
                                >
                                    <div><OverlayTrigger placement="bottom"  overlay={tooltipEdit} ><Edit style={iconStyles} color={grey600} className="edit" onClick={()=>{self.handleOpenForUser(i)}}/></OverlayTrigger></div>
                                </ListItem>;

                            })
                        }
                    </List>
                </div>
                <div >


                    {/* User Edit Dialog*/}
                    <Dialog
                        actions={actionsEdit}
                        modal={false}
                        open={this.state.newUser}
                        onRequestClose={this.handleClose} >
                        <div id="Dialog_Title">
                            <p id="title">Edit {usernameEdit}</p><br/>
                        </div>
                        <div id="textField">
                            username:<TextField id="textUsername"
                                                value={this.state.userEdit}
                                                hintText="UserName"
                                                onChange={this.handleChangeEdit}
                                                disabled={true}
                        /><br/><br/>
                            <div id="userDia_rank">
                                Rank:<input id="adminEdit" type="radio"  name="rank" value="Root" defaultChecked={this.state.admin} onChange={this.radio}/>Root
                                <input id="monEdit" type="radio" name="rank" value="Monitor" defaultChecked={this.state.mon} onChange={this.radio}/>Monitor
                                <input id="memEdit" type="radio" name="rank" value="Member" defaultChecked={this.state.mem} onChange={this.radio}/>Member
                            </div>
                        </div>
                    </Dialog>
                </div>

                {/* User Delete Dialog*/}
                <div>
                    <Dialog
                        actions={actions}
                        modal={false}
                        open={this.state.deleteOpen}
                        onRequestClose={this.handleClose}
                    >
                        Ready to delete  <strong>{deleteValue}</strong> ?
                    </Dialog>
                </div>

            </div>
            


        )
    }
})