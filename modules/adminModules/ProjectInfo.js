
import React from 'react';
import IconMenu from 'material-ui/lib/menus/icon-menu';
import MenuItem from 'material-ui/lib/menus/menu-item';
import IconButton from 'material-ui/lib/icon-button';
import MoreVertIcon from 'material-ui/lib/svg-icons/navigation/more-vert';
import NavLink from '../NavLink'


export default React.createClass({

    render() {
        return(
            <div>
                 <div id="proinfo_header">
                      <div id="proinfo_left">
                           <p className="proinfo_title">ProjectInfo</p>
                      </div>
                     <div id="proinfo_menu">
                         <IconMenu
                             iconButtonElement={<IconButton><MoreVertIcon /></IconButton>}
                             anchorOrigin={{horizontal: 'left', vertical: 'top'}}
                             targetOrigin={{horizontal: 'left', vertical: 'top'}}
                         >
                             <NavLink to="/singleproinfo" id="proinfo_mem1"><MenuItem primaryText="Member1" /></NavLink>
                             <NavLink to="/singleproinfo"  id="proinfo_mem2"><MenuItem primaryText="Member2" /></NavLink>
                         </IconMenu>
                     </div>
                 </div>

            </div>
                
       )}
})