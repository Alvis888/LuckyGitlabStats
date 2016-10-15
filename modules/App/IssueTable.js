/**
 * Created by Xin on 2016/8/9.
 */
var React = require('react');
var ReactBsTable  = require('react-bootstrap-table');
var BootstrapTable = ReactBsTable.BootstrapTable;
var TableHeaderColumn = ReactBsTable.TableHeaderColumn;

var issueData;      //表格数据
var username;       //用户名
var days;           //表格显示的天数
const products = [];

export default React.createClass({
    getInitialState: function () {
        username = sessionStorage.getItem('username');
        days = sessionStorage.getItem('days');
        return null;
    },
    componentWillMount: function () {
        this.issueDataGet();
    },
    shouldComponentUpdate: function () {
        //判断天数是否改变，若改变则重新赋值
        if(sessionStorage.getItem('days') != days){
            days = sessionStorage.getItem('days');
            return true;
        }
        else 
            return false;
    },
    //重新渲染
    componentWillUpdate: function () {
        this.componentWillMount();
    },
    issueDataGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/single/getDataForIssueTable',
            data:{username: username, queryDays: days},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                products.length = 0;
                issueData = data;
                console.log(issueData);
                //将后台获取的数据填入表格对应位置
                for (var i = 0; i < issueData.length; i++) {
                    products.push({
                        issueId: issueData[i]["issueid"].substring(0, 8),
                        serialNumber: issueData[i]["object_attributes"]["assignee_id"],
                        projectName: issueData[i]["project"]["name"],
                        initiator: issueData[i]["user"]["username"],
                        completer: issueData[i]["assignee"]["username"],
                        startTime: issueData[i]["object_attributes"]["created_at"],
                        status: issueData[i]["object_attributes"]["state"]
                    });
                }
            },
            error : function() {
                alert("issueDataGet异常！");
            }
        });
    },
    render() {
        return(
            <div id="issueTable_issue">
                <div id="issueTable_title">
                    <span className="issueTable_text">the detailed information of issue</span>
                </div>
                <div>
                    <BootstrapTable data={products} striped={true} hover={true} pagination={true} search={true}>
                        <TableHeaderColumn isKey={true} dataField="issueId">IssueId</TableHeaderColumn>
                        <TableHeaderColumn dataField="serialNumber">SerialNumber</TableHeaderColumn>
                        <TableHeaderColumn dataField="projectName">ProjectName</TableHeaderColumn>
                        <TableHeaderColumn dataField="initiator">Initiator</TableHeaderColumn>
                        <TableHeaderColumn dataField="completer">Assignee</TableHeaderColumn>
                        <TableHeaderColumn dataField="startTime">StartTime</TableHeaderColumn>
                        <TableHeaderColumn dataField="status">Status</TableHeaderColumn>
                    </BootstrapTable>
                </div>
            </div>
        )
    }
})