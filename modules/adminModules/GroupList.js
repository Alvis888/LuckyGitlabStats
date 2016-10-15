import React from 'react'
import List from 'material-ui/lib/lists/list';
import ListItem from 'material-ui/lib/lists/list-item';
import Avatar from 'material-ui/lib/avatar';
import {pinkA200, transparent,grey600,pink100,grey800} from 'material-ui/lib/styles/colors';
import Edit from 'material-ui/lib/svg-icons/editor/mode-edit';
import Delete from 'material-ui/lib/svg-icons/action/delete';
import Dialog from 'material-ui/lib/dialog';
import FlatButton from 'material-ui/lib/flat-button';
import Tooltip from 'react-bootstrap/lib/Tooltip';
import OverlayTrigger from 'react-bootstrap/lib/OverlayTrigger';
import Select from 'react-select';
import GroupDialog from './GroupDialog'
import TextField from 'material-ui/lib/text-field';
import NavLink from '../NavLink'


var resultAll = [];//所有user信息
var resultSearch = [];//组名选项数组
var resultNew = [];//所有组名
var resultNum;//删除的列表项的下标
var resultSearchUser=[];//Edit Dialog选项数组
var monitorname = [];//Edit Dialog选中的Monitor
var membername=[];//Edit Dialog选中的Members
var resultUsername=[];//所有username
var groupNum;//选中的列表项的下标
var resultMon=[];//Edit Dialog编辑前的Monitor
var resultMem=[];//Edit Dialog编辑前的Members
var searchValue;//搜索的groupName
var isSearch=0;//是否已搜索
var resultGroup=[];//所有group信息
const iconStyles = {
    marginRight: 10,
};
const testStyle={
    fontSize: "17px",
    fontcolor:"#5c5c5c"

};
const tooltipDelete = (
    <Tooltip id="tooltip">Delete</Tooltip>

);
const tooltipEdit = (
    <Tooltip id="tooltip">Edit</Tooltip>

);



export default React.createClass({
    propTypes: {
        label: React.PropTypes.string,
    },
    //初始化组件
    getInitialState: function () {

        return {
            options: [], projectMembers: [],
            groupMonitor:null,groupMembers:null,
            projectMonitor:[],
            optionEdit:[],
            newProject: false, editGroup:false,
            newUser:false,projectname:null,
            member:null,
            select:true,
            deleteOpen:false,
            groupEdit:null,
            idDelete:false,
            test_groupName:null,
        };
    },
    //渲染组件的准备
    componentWillMount: function () {
        this.userListGet();
        this.usernameGet();
        this.searchuser();
        this.setState({optionEdit:resultSearchUser});
        this.groupListGet();
        this.searchforeach();
        this.setState({select:true});
        this.setState({result:resultNew});
        this.setState({options:resultSearch});
        this.setState({selectGroup:""});
    },
    
    //判定后台是否获取成功，刷新页面
    componentWillUpdate:function(){
        if(sessionStorage.getItem('result')!="unknown"){
            this.componentWillMount();
            sessionStorage.setItem('result',"unknown");
        }
    },
    //打开Edit Dialog
    handleOpenForGroup :function (i){
        this.setState({editGroup:true});
        this.setState({test_groupName:""});
        var j;
        if(isSearch==1){
            for(j=0;j<resultNew.length;j++){
                if(resultNew[j]==searchValue){
                    groupNum=j;
                    break;
                }
            }
        }
        if(isSearch==0){
            groupNum=i;
        }
        this.setState({groupEdit: resultNew[groupNum]});
        this.searchMem();
        this.setState({groupMonitor:resultMon});
        this.setState({groupMembers:resultMem});

    },
    //关闭Edit Dialog
    handleCloseForGroup:function () {
        this.setState({editGroup:false});
    },
    handleClose :function (){
        /*if判断*/
        if(this.state.newProject==true)
        {
            this.setState({newProject:false});
            this.setState({projectMonitor:null});
            this.setState({projectMembers:null});
        }
        if(this.state.editGroup==true)
        {
            this.setState({editGroup:false});
            this.setState({groupMonitor:null});
            this.setState({groupMembers:null});
        }
        if(this.state.newUser==true)
        {
            this.setState({newUser:false});

        }
    },

    //实现search功能
    handleSearchGroup :function(selectGroup) {
        this.setState({ selectGroup });
        
        if(selectGroup.value) {
            this.setState({result:selectGroup.value});
            searchValue=selectGroup.value;
            isSearch=1;
        }
        else{
            this.setState({result:resultNew});
            isSearch=0;

        }

    },
    //Edit Dialog输入框，判断有值时不提示
    handleChangeEdit: function (event) {
        this.setState({groupname: event.target.value});
        if(this.state.groupname!=""){
        this.setState({test_groupName:""});
        }
        
    },
    //Edit Dialog Monitor选择框onChange事件
    handleEditGroupMonitor :function(groupMonitor) {
        this.setState({ groupMonitor });
        monitorname=groupMonitor;
    },
    //Edit Dialog Members选择框onChange事件
    handleSelectGroupMembers :function(groupMembers) {
        this.setState({ groupMembers });
        membername=groupMembers;

    },
    //获取userInfo
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
    //获取所有username
    usernameGet:function () {
        var i=0;
        var j=0;
        for(;i<resultAll.length;i++){
            if(resultAll[i].isdelete=="False"){
            resultUsername[j]=resultAll[i].username;
                j++;
            }
        }
    },
    //获取Edit Dialog选项数组
    searchuser:function () {
        var i = 0;
        for(;i<resultUsername.length;i++){
            resultSearchUser[i] = {label:resultUsername[i],value:resultUsername[i]};
        }
    },

    //打开Delete Dialog
    deleteOpen:function (i) {
        var j;
        if(isSearch==1){
            for(j=0;j<resultNew.length;j++){
                if(resultNew[j]==searchValue){
                    resultNum=j;
                    break;
                }
            }
        }
        if(isSearch==0){
            resultNum=i;
        }

        this.setState({deleteOpen:true});
    },
    //关闭Delete Dialog
    deleteClose:function () {
        this.setState({deleteOpen:false});
    },
    //获取所有GroupInfo
    groupListGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Group/GetGroupInfo',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                resultGroup = data;
            },
            error : function() {
                alert("Error in program!");
            }
        });
    },
    //删除组
    groupDelete: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Group/GetDeleteGroup',
            data:{groupname:resultNew[resultNum]},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function () {
                alert("Delete the group successfully!");
                sessionStorage.setItem('result',"success");
            },
            error : function() {
                alert("Error in program!");
            }
        });
        this.deleteClose();
    },
    //Edit group
    postUpdateGroupInfo:function () {
        /*
        * 判断groupname是否为空
        * 若为空，进行提示且不进行后续代码*/
       if(this.state.groupname==""){
           this.setState({test_groupName:"*Not null"});

       }
           /*
           * groupname不为空，判断
           * monitorname,membername，goupname是否为空
           * 若为空，将编辑前的值赋给它*/
       else{
        if(this.state.groupEdit==null){
            this.state.groupEdit=resultNew[groupNum];
        }
        if(monitorname.length==0){
            monitorname=resultMon;
        }
        if(membername.length==0){
            membername=resultMem;
        }
        if(!this.state.groupname){
            this.state.groupname=resultNew[groupNum];
        }
        $.ajax({
            url:"http://202.196.96.79:1500/api/Root/GetUpdateGroup",
            data: {groupName:this.state.groupEdit,groupMonitor:monitorname,groupMembers:membername},
            contentType:"application/json",
            type:"get",
            cache:false,
            async:false,
            dataType:"json",
            success:function(data){
                if(data){
                    alert("Edit the group successfully!");
                    sessionStorage.setItem('result',"success");
                }
                else{
                    alert("Fail to edit the group successfully!");
                }

            },
            error: function () {
                alert("Error in program!");
            }
        });
           this.handleCloseForGroup();

       }
    },
    //获取所有GroupName,和组名选项数组
    searchforeach:function () {
        resultNew=[];
        var j=0;
        for(var i = 0;i<resultGroup.length;i++){
            if(resultGroup[i].isDelete==false){
                resultNew[j]=resultGroup[i].GroupName;
                resultSearch[j] = {label:resultGroup[i].GroupName,value:resultGroup[i].GroupName};
                j++;
            }
        }
    },
    //获取Edit Dialog编辑前的Monitor,Members
    searchMem:function () {
        var i=0;
        for(;i<resultGroup.length;i++){
            if(resultGroup[i].GroupName==resultNew[groupNum]){
                resultMon=resultGroup[i].GroupMonitor;
                resultMem=resultGroup[i].GroupMembers;
                break;
            }
        }

    },
    render() {
        var self=this;
        {/*Edit Dialog Button*/}
        const actionsEdit = [
            <NavLink to="/grouplist">
                <FlatButton
                    label="Submit"
                    primary={true}
                    keyboardFocused={true}
                    onTouchTap={this.postUpdateGroupInfo}
                />
            </NavLink>,
            <FlatButton
                label="Cancel"
                primary={true}
                onTouchTap={this.handleClose}
            />
        ];
        {/*Delete Dialog Button*/}
        const actions = [
            <NavLink to="/grouplist">
                <FlatButton
                    label="Yes"
                    primary={true}
                    onTouchTap={this.groupDelete}
                />
            </NavLink>,
            <FlatButton
                label="No"
                primary={true}
                onTouchTap={this.deleteClose}
            />
        ];
        return(
            <div id="groupList_group">
                <div id="groupList_header">
                    {/*title*/}
                    <div id="groupList_left">
                        <p className="groupList_title">Group List</p>
                    </div>
                    {/*搜索框*/}
                    <div id="groupList_right">
                        <div className="groupList_section">
                            <Select value={this.state.selectGroup}  resetValue="reset" options={this.state.options} autoBlur={this.state.select} placeholder="Search"  onChange={this.handleSearchGroup} />
                        </div>

                    </div>
                    {/*New Group Dialog*/}
                    <div id="groupList_new">
                        <GroupDialog/>
                    </div>
                    {/*Edit Group Dialog*/}
                    <div>
                        <Dialog
                            actions={actionsEdit}
                            modal={false}
                            open={this.state.editGroup}
                            onRequestClose={this.handleClose} >
                            <div id="Dialog_Title">
                                <h2 id="title">Edit {this.state.groupEdit}</h2><br/>

                            </div>
                            <div id="textField">
                                <div id="textFieldOne">GroupName:  <TextField
                                                       hintText="GroupName"
                                                       disabled={true}
                                                       defaultValue={this.state.groupEdit}
                                                       onChange={this.handleChangeEdit}/>
                                    <div id="GroupList_groupName" dangerouslySetInnerHTML={{__html: this.state.test_groupName}}></div>
                                </div>
                                <div className="section">
                                    <Select multi simpleValue  value={this.state.groupMonitor}  options={this.state.optionEdit} placeholder="Select Monitor" onChange={this.handleEditGroupMonitor} valueArray={this.state.result}/>
                                </div>
                                <div className="section">
                                    <Select multi simpleValue  value={this.state.groupMembers}  options={this.state.optionEdit} placeholder="Select Members"  onChange={this.handleSelectGroupMembers} />
                                </div>
                            </div>
                        </Dialog>
                    </div>

                </div>
                {/*Group List*/}
                <div id="groupList_list">
                    <List id="groupList_realList">
                        {
                            React.Children.map(this.state.result, function (child,i) {
                                return <ListItem
                                    id="group_listItem"
                                    style={testStyle}
                                    primaryText={<NavLink to="/group">{child}</NavLink>}
                                    rightAvatar={ <div ><OverlayTrigger placement="bottom" overlay={tooltipDelete} ><Delete style={iconStyles} color={grey600} className="delete"onClick={()=>{self.deleteOpen(i)}} /></OverlayTrigger></div>}
                                    leftAvatar={ <Avatar color={grey800} backgroundColor={pink100} style={{left: 8}} >{child.substring(0,1)}</Avatar>}
                                >
                                    <div><OverlayTrigger placement="bottom"  overlay={tooltipEdit} ><Edit style={iconStyles} color={grey600} className="edit"onClick={()=>{self.handleOpenForGroup(i)}}/></OverlayTrigger></div>
                                </ListItem>;
                            })
                        }
                    </List>
                    {/*Delete Group Dialog*/}
                </div>
                <Dialog

                    actions={actions}
                    modal={false}
                    open={this.state.deleteOpen}
                    onRequestClose={this.handleClose}
                >
                    Ready to delete  <strong>{resultNew[resultNum]}</strong> ?
                </Dialog>
            </div>


        )
    }
})