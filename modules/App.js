import React from 'react'
import Header from './App/Header'
import AllMembers from './App/AllMembers'
import DataTable from './App/DataTable'
import IssueTable from './App/IssueTable'
import PieChart from "./App/Charts/PieChart"
import NavLink from './NavLink'

export default React.createClass({
    getInitialState: function() {
        return {licked: "你好"};
    },
    componentWillMount: function () {
    },
    componentDidMount: function () {
        var table=document.getElementById("table");
         table.style.display='none';
        app_barchart.style.display='none';
        app_barchart.style.display='none';
        app_areachart.style.display='';
        app_pie.style.display='';

    },


    render() {
        var chart=this.state.charts;
        return (
            <div className="app">
                <div id="top">
                    <div id="app_header">
                        <Header/>
                    </div>
                    <div id="app_type">

                    </div>
                </div>
                <AllMembers/>
                <div className="app_chartarea" id="app_barchart"> {this.props.barchart}</div>
                <div className="app_chartarea" id="app_areachart"> {this.props.areachart}</div>
                <div className="app_datatable" id="table"><DataTable/></div>
                <div className="app_piechart" id="app_pie"><PieChart/></div>
                <div className="app_issuetable" id="app_issue"><IssueTable/></div>
            </div>
        )
    }
})
