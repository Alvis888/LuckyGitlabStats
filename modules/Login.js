/**
 * Created by Xin on 2016/7/13.
 */
import React from 'react'
import { Lifecycle } from 'react-router'
import TextField from 'material-ui/lib/text-field';
import RaisedButton from 'material-ui/lib/raised-button';
import NavLink from './NavLink'

var result;  //后台返回值

export default React.createClass({
    mixins: [Lifecycle ],
    routerWillLeave(nextLocation) {
        if(document.getElementById("login_toRegister").name == "noCheck")
            return true;
        if(document.getElementById("login_forget").name == "noCheck"){
            return true;
        }
        else{
            return this.check();
        }
    },
    getInitialState: function() {
        return {name: null, psw: null, html: null};
    },
    userName: function(event) {
        this.setState({name: event.target.value});
    },
    password: function(event) {
        this.setState({psw: event.target.value});
    },
    check: function() {
        if(!this.state.name){
            this.setState({html: "<span class='login_error'>Enter username!</span>"});
            return false;
        }
        else if(!this.state.psw)
        {
            this.setState({html: "<span class='login_error'>Enter password!</span>"});
            return false;
        }
        else{
            this.dataGet();
            if(result == 0){
                this.setState({html: "<span class='login_error'>Username and password do not match!</span>"});
                return false;
            }
            else if(result == 1){
                this.setState({html: ""});
                sessionStorage.setItem('identity', result);
                sessionStorage.setItem('username', this.state.name);
                sessionStorage.setItem('days', "7");
                return true;
            }
            else {
                this.setState({html: ""});
                sessionStorage.setItem('username', this.state.name);
                sessionStorage.setItem('days', "7");
                return true;
            }
        }
    },
    toRegister: function () {
        document.getElementById("login_toRegister").name="noCheck";
    },
    toFindPsw: function () {
        document.getElementById("login_forget").name="noCheck";
    },
    dataGet: function () {
        $.ajax({
            url:'http://202.196.96.79:1500/api/Common/GetLoginCheckWithRank',
            data:{username : this.state.name, password : this.state.psw},
            type:"get",
            cache:false,
            async: false,
            dataType:'json',
            success: function (data) {
                result = data;
            },
            error : function() {
                console.log("Login dataGet is error!");
            }
        });
    },
    render(){
        return (
            <div>
                <div id="login_div">
                    <img src="../photo/7.jpg" />
                </div>
                <div id="login_form">
                    <p className="login_title">Login</p>
                    <div id="login_textField">
                        <TextField id="login_textFieldOne"
                                   hintText="Username"
                                   onChange={this.userName}
                        /><br/><br/>
                        <TextField id="login_textFieldTwo"
                                   hintText="Password"
                                   type="password" 
                                   onChange={this.password}
                        /><br/><br/>
                        <div id="login_forget" name="forget"><NavLink to="/findPsw" onClick={this.toFindPsw}>forget password?</NavLink></div>
                        <div dangerouslySetInnerHTML={{__html:this.state.html}}></div>
                    </div>
                    <div id="login_button">
                        <NavLink to="/app">
                            <RaisedButton label="Login" secondary={true}/>
                        </NavLink><br/><br/>
                        <div id="login_toRegister" name="toRegister">
                            <NavLink to="/register" onClick={this.toRegister}>No account ? Go register>></NavLink>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
})
