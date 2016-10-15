/**
 * Created by NetCenter-305 on 2016/5/29.
 */
import React from 'react'
import Type from '../App/Type'

var version;        //项目版本号

export default React.createClass({
    getInitialState: function() {
        //初始化时即获取项目版本号
        this.getVersion();
        return null;
    },
    //获取项目的版本号
    getVersion: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/common/GetProjectVersion',
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                version = data;
            },
            error : function() {
                alert("failed to get version!");
            }
        });
    },
    render() {
        return(<div id="header_title">
            <div className="header_local">HDDevTeam LuckyGitlabStat
                <span id="header_version">{version}</span>
            </div>
            <div id="header_menu">
                <Type/>
            </div>
        </div>)
    }
})

