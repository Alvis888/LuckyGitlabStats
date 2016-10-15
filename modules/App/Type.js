/**
 * Created by Xin on 2016/7/14.
 */
import React from 'react';
import IconMenu from 'material-ui/lib/menus/icon-menu';
import MenuItem from 'material-ui/lib/menus/menu-item';
import IconButton from 'material-ui/lib/icon-button';
import MoreVertIcon from 'material-ui/lib/svg-icons/action/list';
import NavLink from '../NavLink'

const style = {
  fontFamily: 'Consolas'
};
const styleAdmin = {
    fontFamily: 'Consolas',
    textAlign: 'center'
};
var identity;
export default React.createClass({
    getInitialState: function () {
        identity = sessionStorage.getItem('identity');
        return null;
    },
    barClick:function(event){
        app_areachart.style.display='none';
        app_barchart.style.display='';
        if(app_pie.style.display=='none'&&table.style.display=='none'){
            table.style.display='none';
        }
        if(app_pie.style.display==''||table.style.display==''){
            app_pie.style.display='none';
            table.style.display='';
        }
    },
    areaClick:function(event){
        app_areachart.style.display='';
        app_barchart.style.display='none';
        if(table.style.display=='none'&&app_pie.style.display=='none'){
            app_pie.style.display='none';
        }
        if(app_pie.style.display==''||table.style.display==''){
            table.style.display='none';
            app_pie.style.display='';
        }
    },

    render(){
        if(identity == 1){
            return(
                <div id="type">
                    <IconMenu
                        iconButtonElement={<IconButton><MoreVertIcon /></IconButton>}
                        anchorOrigin={{horizontal: 'left', vertical: 'top'}}
                        targetOrigin={{horizontal: 'left', vertical: 'top'}}
                    >
                        <a  onClick={this.barClick} id="type_build"><MenuItem primaryText="Build Info" style={style}/></a>
                        <a  onClick={this.areaClick}  id="type_commit" ><MenuItem primaryText="Commit Info" style={style}/></a>
                        <NavLink to="/mainpage"><MenuItem primaryText="Admin" style={styleAdmin}/></NavLink>
                    </IconMenu>
                </div>
            )
        }
        else {
            return (
                <div id="type">
                    <IconMenu
                        iconButtonElement={<IconButton><MoreVertIcon /></IconButton>}
                        anchorOrigin={{horizontal: 'left', vertical: 'top'}}
                        targetOrigin={{horizontal: 'left', vertical: 'top'}}
                    >
                        <a onClick={this.barClick} id="type_build"><MenuItem primaryText="Build Info" style={style}/></a>
                        <a onClick={this.areaClick} id="type_commit"><MenuItem primaryText="Commit Info" style={style}/></a>
                    </IconMenu>
                </div>
            )
        }
    }
})
