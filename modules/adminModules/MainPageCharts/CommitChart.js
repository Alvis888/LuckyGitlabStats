import React from 'react';
import  ReactD3 from 'react-d3-components'

var BarChart = ReactD3.BarChart;
var colorScale = d3.scale.category20();
var commitLast=[];
var commitNow=[];
var data=[];
var dataLast=[];
var dataNow=[];
var timeFrame;
var week = new Array("Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday");
var year = new Array("Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sept","Oct","Nov","Dec");

export default React.createClass({
    getInitialState: function () {
        timeFrame = sessionStorage.getItem('timeFrame');       //获取默认时间段：week
        return null;
    },
    componentWillMount: function () {
        this.getCommitData();      //获取现下与之前一个时间段的提交数据
        this.dataForeach();        //处理获取的数据
    },
    componentWillUpdate:function(){
        //判断时间段是否改变，若改变则重新赋值,且重新渲染
        if(sessionStorage.getItem('timeFrame') != timeFrame){
            timeFrame = sessionStorage.getItem('timeFrame');
            this.componentWillMount();
        }
    },
    //获取一个时间段内现在和过去的总提交次数
    getCommitData: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/allMember/GetMemberCommit_Weekly_Monthly_Yearly',
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
                        commitNow[i] = data.m_Item1[i + 1];
                        commitLast[i] = data.m_Item2[i + 1] - commitNow[i];
                    }
                    else if (!!data.m_Item2[i + 1]) {
                        commitNow[i] = 0;
                        commitLast[i] = data.m_Item2[i + 1];
                    }
                    else {
                        commitNow[i] = 0;
                        commitLast[i] = 0;
                    }
                }
            },
            error : function() {
                alert("failed to get commit data in CommitChart!");
            }
        });
    },
    dataForeach: function () {
        if(timeFrame == 7){
            for(var j = 0; j < timeFrame; j++){
                dataLast[j] = {x: week[j], y: commitLast[j]};
                dataNow[j] = {x: week[j], y: commitNow[j]};
            }
        }
        if(timeFrame==30){
            var trim="", str=" ", str1, num;
            for(var j = 0; j < timeFrame; j++){
                num = j.toString();
                if(num.substr(-1) == '1'){
                    str1 = 'st';
                }
                else if(num.substr(-1) == '2'){
                    str1 = 'nd';
                }
                else if(num.substr(-1) == '3'){
                    str1 = 'rd';
                }
                else {
                    str1 = 'th';
                }
                if((j + 1) % 3 == 0){
                    dataLast[j] = {x: j+1+str1, y: commitLast[j]};
                    dataNow[j] = {x: j+1+str1, y: commitNow[j]};
                }
                else{
                    trim = trim + str;
                    dataLast[j] = {x: trim, y: commitLast[j]};
                    dataNow[j] = {x: trim, y: commitNow[j]};
                }
            }
        }
        if(timeFrame == 365){
            for(var j = 0; j < 12; j++){
                dataLast[j] = {x: year[j], y: commitLast[j]};
                dataNow[j] = {x: year[j], y: commitNow[j]};
            }
        }
        data = [{label:'somethingA', values:dataLast}, {label:'somethingB', values:dataNow}];
        dataLast = [];
        dataNow = [];
    },
    render(){
        return (
            <div>
                <BarChart 
                          groupedBars
                          data={data}
                          width={700}
                          height={250}
                          margin={{top: 50, bottom: 20, left: 40, right: 30}}
                          colorScale={colorScale}
                          xAxis={{innerTickSize: 1,outerTickSize: 0,className: "axis"}}
                          yAxis={{label: "times"}}
                />
            </div>
        )
    }
})

