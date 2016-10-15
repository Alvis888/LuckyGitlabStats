/**
 * Created by Xin on 2016/9/17.
 */
import React from 'react';
import  ReactD3 from 'react-d3-components'

var BarChart = ReactD3.BarChart;
var colorScale = d3.scale.category20();
var issueOpenedNow=[];
var issueOpenedLast=[];
var issueClosedNow=[];
var issueClosedLast=[];
var dataOpenedLast=[], dataClosedLast=[], dataOpenedNow=[], dataClosedNow=[], dataLast=[], dataNow=[], data=[];
var timeFrame;
var week = new Array("Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday");
var year = new Array("Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sept","Oct","Nov","Dec");

export default React.createClass({
    getInitialState: function () {
        timeFrame = sessionStorage.getItem('timeFrame');     //获取默认时间段：week
        return null;
    },
    componentWillMount: function () {
        this.getIssueData();       //获取现下与之前一个时间段的issue数据
        this.dataForeach();        //处理获取的数据
    },
    componentWillUpdate:function(){
        //判断时间段是否改变，若改变则重新赋值,且重新渲染
        if(sessionStorage.getItem('timeFrame') != timeFrame){
            timeFrame = sessionStorage.getItem('timeFrame');
            this.componentWillMount();
        }
    },
    //获取一个时间段内issue情况
    getIssueData: function () {
        //获取一个时间段内未关闭的issue的数量
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberIssueOpened_Weekly_Monthly_Yearly',
            data:{queryDays: timeFrame},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                var start = 0, end, nowMax, totalMax;
                //如果时间段为year，则横轴刻度数为12，否则按天显示
                if(timeFrame == 365){
                    end = 12;
                }
                else {
                    end = timeFrame;
                }
                for (var i = start; i < end; i++) {
                    if (!!data.m_Item1[i + 1]) {
                        issueOpenedNow[i] = data.m_Item1[i + 1];
                        issueOpenedLast[i] = data.m_Item2[i + 1] - issueOpenedNow[i];
                    }
                    else if (!!data.m_Item2[i + 1]) {
                        issueOpenedNow[i] = 0;
                        issueOpenedLast[i] = data.m_Item2[i + 1];
                    }
                    else {
                        issueOpenedNow[i] = 0;
                        issueOpenedLast[i] = 0;
                    }
                }
            },
            error : function() {
                alert("failed to get data of opened issue in IssueChart!");
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
                var start = 0, end, nowMax, totalMax;
                //如果时间段为year，则横轴刻度数为12，否则按天显示
                if(timeFrame == 365){
                    end = 12;
                }
                else {
                    end = timeFrame;
                }
                for (var i = start; i < end; i++) {
                    if (!!data.m_Item1[i + 1]) {
                        issueClosedNow[i] = data.m_Item1[i + 1];
                        issueClosedLast[i] = data.m_Item2[i + 1] - issueClosedNow[i];
                    }
                    else if (!!data.m_Item2[i + 1]) {
                        issueClosedNow[i] = 0;
                        issueClosedLast[i] = data.m_Item2[i + 1];
                    }
                    else {
                        issueClosedNow[i] = 0;
                        issueClosedLast[i] = 0;
                    }
                }
            },
            error : function() {
                alert("failed to get data of closed issue in IssueChart!");
            }
        });
    },
    dataForeach: function () {
        if(timeFrame == 7){
            for(var j = 0; j < timeFrame; j++){
                dataOpenedLast[j] = {x: week[j], y: issueOpenedLast[j]};
                dataClosedLast[j] = {x: week[j], y: issueClosedLast[j]};
                dataOpenedNow[j] = {x: week[j], y: issueOpenedNow[j]};
                dataClosedNow[j] = {x: week[j], y: issueClosedNow[j]};
            }
        }
        if(timeFrame == 30){
            var trim = "", str = " ", str1, num;
            for(var j = 0; j < timeFrame; j++){
                num = (j+1).toString();
                if(num.charAt(num.length-1) == '1'){
                    str1 = 'st';
                }
                else if(num.charAt(num.length-1) == '2'){
                    str1 = 'nd';
                }
                else if(num.charAt(num.length-1) == '3'){
                    str1 = 'rd';
                }
                else {
                    str1 = 'th';
                }
                if((j + 1) % 3 == 0){
                    dataOpenedLast[j] = {x: j+1+str1, y: issueOpenedLast[j]};
                    dataClosedLast[j] = {x: j+1+str1, y: issueClosedLast[j]};
                    dataOpenedNow[j] = {x: j+1+str1, y: issueOpenedNow[j]};
                    dataClosedNow[j] = {x: j+1+str1, y: issueClosedNow[j]};
                }
                else{
                    trim = trim + str;
                    dataOpenedLast[j] = {x: trim, y: issueOpenedLast[j]};
                    dataClosedLast[j] = {x: trim, y: issueClosedLast[j]};
                    dataOpenedNow[j] = {x: trim, y: issueOpenedNow[j]};
                    dataClosedNow[j] = {x: trim, y: issueClosedNow[j]};
                }
            }
        }
        if(timeFrame == 365){
            for(var j = 0; j < 12; j++){
                dataOpenedLast[j] = {x: year[j], y: issueOpenedLast[j]};
                dataClosedLast[j] = {x: year[j], y: issueClosedLast[j]};
                dataOpenedNow[j] = {x: year[j], y: issueOpenedNow[j]};
                dataClosedNow[j] = {x: year[j], y: issueClosedNow[j]};
            }
        }
        dataLast = [{label:'closedLast', values:dataClosedLast}, {label:'openedLast', values:dataOpenedLast}];
        dataNow = [{label:'closedNow', values:dataClosedNow}, {label:'openedNow', values:dataOpenedNow}];
       // data = [{label:'last', values:dataLast}, {label:'now', values:dataNow}];
        data = [{label:'last1', values:dataOpenedNow},{label:'last2', values:dataClosedNow}, 
            {label:'now1', values:dataOpenedLast}, {label:'now2', values:dataClosedLast}];
        dataOpenedLast = [];
        dataClosedLast = [];
        dataOpenedNow = [];
        dataClosedNow = [];
    },
    render(){
        return (
            <div>
                <BarChart
                          groupedBars
                          data={data}
                          width={700}
                          height={250}
                          margin={{top: 40, bottom: 20, left: 40, right: 30}}
                          colorScale={colorScale}
                          xAxis={{innerTickSize: 1,outerTickSize: 0,className: "axis"}}
                          yAxis={{label: "times"}}
                />
            </div>
        )
    }
})

