/**
 * Created by Xin on 2016/8/29.
 */
import React from 'react';
import IconMenu from 'material-ui/lib/menus/icon-menu';
import MenuItem from 'material-ui/lib/menus/menu-item';
import IconButton from 'material-ui/lib/icon-button';
import MoreVertIcon from 'material-ui/lib/svg-icons/navigation/more-vert';
import NavLink from '../NavLink'
var groupName;
export default React.createClass({
    getInitialState: function () {
        groupName = sessionStorage.getItem('group');
        this.setState({groupName: "test"});
        return{group: null};
    },
    click: function (name) {
        sessionStorage.setItem('username', name);
        sessionStorage.setItem('days', "7");
    },
    render() {
        var self = this;
        return(
            <div id="groupList_header">
                <div id="groupList_left">
                    <p className="groupList_title">GroupInfo</p>
                </div>
                <div id="groupList_right">
                    <div className="groupList_section">
                        <IconMenu
                            iconButtonElement={<IconButton><MoreVertIcon /></IconButton>}
                            anchorOrigin={{horizontal: 'left', vertical: 'top'}}
                            targetOrigin={{horizontal: 'left', vertical: 'top'}}
                        >
                            <NavLink to="/app" onClick={()=>{self.click(child)}}><MenuItem primaryText="chenchen"/></NavLink>
                            <NavLink to="/app"><MenuItem primaryText="fuyiman"/></NavLink>
                        </IconMenu>
                    </div>
                </div>
            </div>
        )
    }
})
