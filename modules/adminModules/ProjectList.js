/**
 * Created by DELL on 2016/8/18.
 */
import React from 'react'
import List from 'material-ui/lib/lists/list';
import ListItem from 'material-ui/lib/lists/list-item';
import ActionGrade from 'material-ui/lib/svg-icons/action/grade';
import Divider from 'material-ui/lib/divider';
import Avatar from 'material-ui/lib/avatar';
import {pinkA200, transparent,grey600,pink100,grey800} from 'material-ui/lib/styles/colors';
import Edit from 'material-ui/lib/svg-icons/editor/mode-edit';
import Delete from 'material-ui/lib/svg-icons/action/delete';
import Search from 'material-ui/lib/svg-icons/action/search';
import Dialog from 'material-ui/lib/dialog';
import FlatButton from 'material-ui/lib/flat-button';
import RaisedButton from 'material-ui/lib/raised-button';
import ProjectDialog from './ProjectDialog';
import Tooltip from 'react-bootstrap/lib/Tooltip';
import OverlayTrigger from 'react-bootstrap/lib/OverlayTrigger';
import Select from 'react-select';
import NavLink from '../NavLink'
import TextField from 'material-ui/lib/text-field';

var resultAll = [];//所有project信息
var resultSearch = [];//projectName选项数组
var resultNum;//删除选项下标
var resultNew=[];//所有projectName
var resultGroupInfo=[];//所有组信息
var resultSearchGroupName=[];//groupName选项数组
var selectGroup=[];//Project Edit Dialog选中的组（Array）
var group=[];//Project Edit Dialog选中的组名(逗号隔开的String)
var resultMon;//获取目标Project的Monitor
var resultMems;//获取目标Project的Members
var resultGroup;//所有的groupName
var resultProject;//编辑前的项目名
var monitor;//编辑后的projectMonitor
var groupMems;//编辑后的projectMembers
var mark;//是否点Edit Dialog的groupName选择框
var isSearch=0;//是否已搜索
var searchValue;//搜索的projectName




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
const iconStyles = {
    marginRight: 10,
};



export default React.createClass({
    //初始化组件
    getInitialState: function () {
        return {
            options:[], projectMembers: [],
            groupName:[],groupMonitor:[],
            projectMonitor:[],
            editProject: false, newGroup:false,
            newUser:false,projectname:null,
            member:null,
            result:[],
            searchText:null,
            value:null,
            select:true,
            deleteOpen:false,
            projectEdit:null,
            GroupName:null,
            optionsGroupName:[],
            optionsMembers:[],
            mem:[],
            test_groupName:null


        };
    },
    //渲染组件的准备
    componentWillMount: function () {
        this.projectListGet();
        this.searchforeach();
        this.groupListGet();
        this.searchforGroupName();
        this.setState({select:true});
        this.setState({result:resultNew});
        this.setState({options:resultSearch});
        this.setState({optionsGroupName:resultSearchGroupName});
        this.setState({projectName:""});

    },
    //更新组件
    componentWillUpdate:function(){
        if(sessionStorage.getItem('result')!="unknown"){
            console.log("result:"+sessionStorage.getItem('result'));
            this.componentWillMount();
            sessionStorage.setItem('result',"unknown");
        }
    },
    //关闭Edit Dialog
    handleClose :function() {
        this.setState({open: false});
        this.setState({editProject:false});
    },
    //打开Edit Dialog
    handleOpenForProject :function (i){
        this.setState({editProject:true});
        this.setState({test_groupName:""});
        var j;
        var editNum;
        /*
        判断是否已搜索
        * */
        if(isSearch==1){
            for(j=0;j<resultNew.length;j++){
                if(resultNew[j]==editValue){
                   editNum=j;
                    break;
                }
            }
        }
        if(isSearch==0){
            editNum=i;
        }
        resultProject=resultNew[editNum];
        this.setState({projectEdit:resultNew[editNum]});
        this.searchEach();
        this.setState({groupName:resultGroup});
        this.setState({groupMonitor:resultMon});
        this.setState({groupMembers:resultMems});
        this.searchMembersStart();

    },
    //Edit Dialog的groupName输入框onChange事件
    handleChange: function (event) {
        this.setState({value: event.target.value});
    },
    //打开Delete Dialog
    deleteOpen:function (i) {
        var j;
        /*
        * 判断是否已搜索*/
        if(isSearch==0){
            resultNum=i;
        }
        else{
            for(j=0;j<resultNew.length;j++){
                if(searchValue==resultNew[j]){
                    resultNum=j;
                    break;
                }
            }

        }
        this.setState({deleteOpen:true});
    },
    //关闭Delete Dialog
    deleteClose:function () {
        this.setState({deleteOpen:false});

    },
    //实现search功能
    handleSelectProjectMonitor :function(projectName) {
        this.setState({ projectName });
        if(projectName.value){
            this.setState({result:projectName.value});
            isSearch=1;
            searchValue=projectName.value;
        }
        else{
            this.setState({result:resultNew});
            isSearch=0;
        }
    },
    //Edit Dialog的projectGroup select
    handleSelectgroupName :function(groupName) {
        this.setState({ groupName });
        group=groupName;
        selectGroup = groupName.split(",");
        mark=1;
        if(group.length!=0){
            this.setState({test_groupName:""});
        }
    },
    //Edit Dialog的projectMonitor select
    handleSelectgroupMonitor :function(groupMonitor) {
        this.setState({ groupMonitor });
        monitor=groupMonitor;


    },
    //Edit Dialog的projectMembers select
    selectGroupMembers:function (groupMembers) {
        this.setState({ groupMembers });
        groupMems=groupMembers;
    },
    //获取所有project信息
    projectListGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/project/GetAllProjectInfo',
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
    //删除项目
    projectDelete: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Project/GetDeleteProject',
            data:{projectname:resultNew[resultNum]},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                if(data){
                    alert("Delete the project successfully!");
                    sessionStorage.setItem('result',"success");
                }
                else{
                    alert("Fail to delete the project!");
                }

            },
            error : function() {
                alert("Error in program!");
            }
        });
        this.deleteClose();
    },
    //编辑项目
    getUpdateProjectInfo:function () {
        if(mark==1&&group.length==0){
            this.setState({test_groupName:"*Not null"});
            mark=0;
        }
        else{
            if(monitor==null){
                monitor=resultMon;
            }

            if(groupMems==null){
                groupMems=resultMems;
            }

            if(group.length==0){
                group=resultGroup;
            }
            $.ajax({
                url:"http://202.196.96.79:1500/api/Project/GetUpdateProject",
                data:{projectName:this.state.projectEdit,projectMonitor:monitor,projectMembers:groupMems,groupName:group},
                contentType:"application/json",
                type:"get",
                cache:false,
                async:false,
                dataType:"json",
                success:function(data){
                    if(data=="更新成功"){
                        alert("Edit the project successfully!");
                        sessionStorage.setItem('result',"success");
                    }
                    else{
                        alert("Fail to edit the project!");
                    }
                },
                error: function () {
                    alert("Error in program!");
                }
            });
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
                resultGroupInfo = data;
            },
            error : function() {
                alert("Error in program!");
            }
        });
        isSearch=0;
    },
    //获取所有组名选项
    searchforGroupName:function () {
        var i = 0;
        var j = 0;
        resultSearchGroupName=[];
        for(;i<resultGroupInfo.length;i++){
            if(resultGroupInfo[i].isDelete==false){
                resultSearchGroupName[j] = {label:resultGroupInfo[i].GroupName,value:resultGroupInfo[i].GroupName};
                j++;
            }
        }
    },
    //获取编辑前的Monitor，Members的选项
    searchMembersStart:function () {
        var i;
        var j;
        var k=0;
        var f;
        var monitor=[];
        var group=resultGroup.split(",");
        var members=[];
        var resultMem=[];
        for(i=0;i<group.length;i++){
                for(j=0;j<resultGroupInfo.length;j++){
                    if(group[i]==resultGroupInfo[j].GroupName){
                        members=resultGroupInfo[j].GroupMembers.split(",");
                        monitor=resultGroupInfo[j].GroupMonitor.split(",");
                        for(f=0;f<members.length;f++){
                            resultMem[k]={label: members[f],value: members[f]};
                            k++;
                        }
                        for(f=0;f<monitor.length;f++){
                            resultMem[k]={label: monitor[f],value: monitor[f]};
                            k++;
                        }
                    }
                }

        }
        this.setState({optionsMembers:resultMem});
    },
    //获取编辑时的Monitor，Members的选项
    searchMembers:function () {
        var i;
        var j;
        var k=0;
        var f;
        var groupArray=group.split(",");
        var members=[];
        var monitor=[];
        var resultMem=[];
        for(i=0;i<groupArray.length;i++){
                for(j=0;j<resultGroupInfo.length;j++){
                    if(groupArray[i]==resultGroupInfo[j].GroupName){
                        members=resultGroupInfo[j].GroupMembers.split(",");
                        monitor=resultGroupInfo[j].GroupMonitor.split(",");
                        for(f=0;f<members.length;f++){
                            resultMem[k]={label: members[f],value: members[f]};
                            k++;
                        }
                        for(f=0;f<monitor.length;f++){
                            resultMem[k]={label: monitor[f],value: monitor[f]};
                            k++;
                        }
                    }
                }

        }
        group=[];
        this.setState({optionsMembers:resultMem});
    },
    //Edit Dialog select框的onFocus事件
    searchFocus:function () {
        if(group.length==0){
            this.searchMembersStart();
        }
        else{
            this.searchMembers();
        }
    },
    //获取所有的projectGroup,projectMonitor,projectMembers
    searchEach:function () {
        var i=0;
        for(;i<resultAll.length;i++){
            if(resultProject==resultAll[i].projectName){
                resultGroup=resultAll[i].groupName;
                resultMon=resultAll[i].projectMonitor;
                resultMems=resultAll[i].projectMembers;
                break;
            }
        }
    },
    //获取所有projectName，projectName选项
    searchforeach:function () {
        var i = 0;
        var j = 0;
        resultNew=[];
        for(;i<resultAll.length;i++){
            if(resultAll[i].isDelete==false){
                resultNew[j]=resultAll[i].projectName;
                resultSearch[j] = {label:resultAll[i].projectName,value:resultAll[i].projectName};
                j++;
            }
        }
    },
    render() {
        var self = this;
        {/*Edit Dialog Buttons*/}
        const actionsEdit = [
            <FlatButton
                label="Submit"
                primary={true}
                keyboardFocused={true}
                onTouchTap={this.getUpdateProjectInfo}
            />
            ,
            <FlatButton
                label="Cancel"
                primary={true}
                onTouchTap={this.handleClose}
            />
        ];
        {/*Delete Dialog Button*/}
        const actions = [
            <NavLink to="/projectlist">
                <FlatButton
                    label="Yes"
                    primary={true}
                    onTouchTap={this.projectDelete}
                />
            </NavLink>,
            <FlatButton
                label="No"
                primary={true}
                onTouchTap={this.deleteClose}
            />
        ];

        return(
            <div id="projectList_project">
                <div id="projectList_header">
                    {/*title*/}
                    <div id="projectList_left">
                        <p className="projectList_title">Project List</p>
                    </div>
                    {/*搜索框*/}
                    <div id="projectList_right">
                        <div className="projectList_section">
                            <Select  id="select" value={this.state.projectName}  resetValue="reset" options={this.state.options}  placeholder="Search" autoBlur={this.state.select} onChange={this.handleSelectProjectMonitor}/>
                        </div>
                    </div>
                </div>
                {/*project list */}
                <div id="projectList_list">
                    <List id="projectList_realList">
                        {
                            React.Children.map(this.state.result, function (child,i) {
                                return  <ListItem

                                    style={testStyle}
                                    primaryText={<NavLink to="projectinfo">{child}</NavLink>}
                                    rightAvatar={ <div ><OverlayTrigger placement="bottom" overlay={tooltipDelete} ><Delete style={iconStyles} color={grey600} className="delete" onClick={()=>{self.deleteOpen(i)}}/></OverlayTrigger></div>}
                                    leftAvatar={ <Avatar color={grey800} backgroundColor={pink100} style={{left: 8}} >{child.substring(0,1)}</Avatar>}
                                >
                                    <div><OverlayTrigger placement="bottom"  overlay={tooltipEdit} ><Edit style={iconStyles} color={grey600} className="edit" onClick={()=>{self.handleOpenForProject(i)}}/></OverlayTrigger></div>

                                </ListItem> ;

                            })
                        }
                    </List>
                </div>
                {/*Edit Dialog */}
                <div id="DialogClass">

                    <Dialog
                        actions={actionsEdit}
                        modal={false}
                        open={this.state.editProject}

                        onRequestClose={this.handleClose} >
                        <div id="dialog_content">
                        <div id="Dialog_Title">
                            <h2 id="title">Edit {resultProject}</h2><br/>

                        </div>
                        <div id="textField">

                            <div id="textFieldProject">


                                <span id="spanProject">ProjectName:</span>    <TextField
                                value={this.state.projectEdit}
                                hintText="GroupName"
                                onChange={this.handleChange}
                                disabled={true}
                            /><br/><br/>
                            </div>
                            <div className="section">
                                <div className="section-groupname"><p >GroupName:</p></div>
                                <div className="sectionEdit"><Select multi simpleValue value={this.state.groupName} options={this.state.optionsGroupName} placeholder="Select Group" autoBlur={this.state.select} onChange={this.handleSelectgroupName} />
                                </div>
                                <div id="groupName" dangerouslySetInnerHTML={{__html: this.state.test_groupName}}></div>
                            </div>
                            <div className="section">
                                <div className="section-heading"><p >GroupMonitor:</p></div>
                                <div className="sectionEdit"><Select multi simpleValue  value={this.state.groupMonitor}  options={this.state.optionsMembers} placeholder="Select Monitor" autoBlur={this.state.select} onChange={this.handleSelectgroupMonitor} onFocus={this.searchFocus} /></div>
                            </div>
                            <div className="section">
                                <div className="section-heading"><p >GroupMembers:</p></div>
                                <div className="sectionEdit"><Select multi simpleValue  value={this.state.groupMembers}  options={this.state.optionsMembers} placeholder="Select Members"  autoBlur={this.state.select} onChange={this.selectGroupMembers}  onFocus={this.searchFocus} /></div>
                            </div>
                        </div>
                        </div>
                    </Dialog>
                </div>
                {/*Delete Dialog */}
                <div>
                    <Dialog
                        actions={actions}
                        modal={false}
                        open={this.state.deleteOpen}
                        onRequestClose={this.handleClose}
                    >

                        Ready to delete  <strong>{resultNew[resultNum]}</strong> ?
                    </Dialog>
                </div>
            </div>


        )
    }
})