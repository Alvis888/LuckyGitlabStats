/**
 * Created by Xin on 2016/8/5.
 */
import React from 'react';
import rd3 from 'react-d3-components';

var AreaChart = rd3.AreaChart;
var data = [];              //面积图数据
var returnSingleData=[];    //个人一段时间内每天的提交量
var returnAllData=[];       //所有人一段时间内每天的提交量
var username;               //登录用户名
var oldDays;                //一段时间的天数
var inverseDate=[];         //反序的一段时间的时间戳
var single=[];              //个人的数据
var all=[];                 //所有人的数据

export default React.createClass({
    getInitialState: function() {
        username = sessionStorage.getItem('username');
        oldDays = sessionStorage.getItem('days');
        return null;
    },
    componentWillMount: function () {
        this.dataGet();
        this.forEach();
    },
    componentWillUpdate:function(){
        //判断天数是否改变，若改变则重新赋值,且重新渲染
        if(sessionStorage.getItem('days')!=oldDays){
            oldDays=sessionStorage.getItem('days');
            this.componentWillMount();
        }
    },
    dataGet: function () {
        //获取个人提交量
        $.ajax({
            url:'http://202.196.96.79:1500/api/Single/GetPersonCommitByDays',
            data:{username: username, queryDays: oldDays},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                returnSingleData = data;
            },
            error : function() {
                alert("Failed to get person's submit times！");
            }
        });
        //获取所有人提交量
        $.ajax({
            url:'http://202.196.96.79:1500/api/AllMember/GetPersonsCommitTotalByDays',
            data:{queryDays: oldDays},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                returnAllData = data;
            },
            error : function() {
                alert("Failed to get persons' submit times！");
            }
        });
    },
    forEach: function () {
        var now = new Date();                       //获取当前时间
        var date = now.toLocaleDateString();        //将当前时间保留到日
        var timestamp = Date.parse(new Date(date)); //将当前时间转换为时间戳
        //根据当前时间戳计算一段时间内每天的时间戳
        for(var i = 0; i < oldDays; i++){
            if (i == 0) {
                inverseDate[i] = timestamp;
            }
            else {
                inverseDate[i] = inverseDate[i - 1] - 1000 * 60 * 60 * 24;
            }
        }
        //将数据与时间对应
        for(var j = 0; j < oldDays; j++){
            single[j] = {x: new Date(inverseDate[oldDays-j-1]), y: 0};
            all[j] = {x: new Date(inverseDate[oldDays-j-1]), y: 0};
        }
        for(let k in returnSingleData)
        {
            for(let p = 0; p < oldDays; p++)
            {
                if(inverseDate[oldDays-p-1]==k) //日期对应的时间戳相等
                {
                    single[p] = {x: new Date(inverseDate[oldDays-p-1]), y:returnSingleData[k]};
                }
            }
        }
        for(let k in returnAllData)
        {
            for(let p = 0; p < oldDays; p++)
            {
                if(inverseDate[oldDays-p-1]==k) //日期对应的时间戳相等
                {
                    all[p] = {x: new Date(inverseDate[oldDays-p-1]), y: returnAllData[k]};

                }
            }
        }
        data = [
            {label: 'single',values: single},
            {label: 'all',values: all}
        ];
        single = [];
        all = [];
    },
    render() {
        //如果天数为7，限制横轴刻度数为7，否则默认刻度显示个数
        if(oldDays == 7){
            for(var j = 0; j < oldDays; j++){
                single[j] = {x: new Date(inverseDate[oldDays-j-1]), y: 0};
                all[j] = {x: new Date(inverseDate[oldDays-j-1]), y: returnAllData[j]};
            }
            for(let k in returnSingleData)
            {
                for(let p = 0; p < oldDays; p++)
                {
                    if(inverseDate[oldDays-p-1]==k) //日期对应的时间戳相等
                    {
                        single[p] = {x: new Date(inverseDate[oldDays-p-1]), y:returnSingleData[k]};
                    }
                }
            }
            for(let k in returnAllData)
            {
                for(let p = 0; p < oldDays; p++)
                {
                    if(inverseDate[oldDays-p-1]==k) //日期对应的时间戳相等
                    {
                        all[p] = {x: new Date(inverseDate[oldDays-p-1]), y: returnAllData[k]};

                    }
                }
            }
            data = [
                {label: 'single',values: single},
                {label: 'all',values: all}
            ];
            single = [];
            all = [];
            return(
                <div id="AreaChart_outer">
                    <div className="AreaChart_title"><span className="Area_text">Times of commitment per day</span>
                    </div>
                    <div>
                        <div className="AreaChart_single">
                            <div className="AreaChart_allLabel">person:</div>
                            <div className="AreaChart_saturated"></div>
                        </div>
                        <br/>
                        <div className="AreaChart_all">
                            <div className="AreaChart_singleLabel">all:</div>
                            <div className="AreaChart_shallow"></div>
                        </div>
                    </div>
                    <AreaChart id="AreaChart_chart"
                               data={data}
                               width={850}
                               height={390}
                               margin={{top: 10, bottom: 50, left: 50, right: 30}}
                               xAxis={{innerTickSize: 0,tickPadding: 6, tickArguments: [7],tickFormat: d3.time.format("%b %d"),outerTickSize: 0,className: "axis"}}
                               yAxis={{label: "times"}}
                    />
                </div>
            )
        }
        else {
            return (
                <div id="AreaChart_outer">
                    <div className="AreaChart_title"><span className="Area_text">Times of commitment per day</span>
                    </div>
                    <div>
                        <div className="AreaChart_single">
                            <div className="AreaChart_allLabel">person:</div>
                            <div className="AreaChart_saturated"></div>
                        </div>
                        <br/>
                        <div className="AreaChart_all">
                            <div className="AreaChart_singleLabel">all:</div>
                            <div className="AreaChart_shallow"></div>
                        </div>
                    </div>
                    <AreaChart id="AreaChart_chart"
                               data={data}
                               width={850}
                               height={390}
                               margin={{top: 10, bottom: 50, left: 50, right: 30}}
                               xAxis={{innerTickSize: 0,tickPadding: 6,tickFormat: d3.time.format("%b %d"),outerTickSize: 0,className: "axis"}}
                               yAxis={{label: "times"}}
                    />
                </div>
            )
        }
    }
})

