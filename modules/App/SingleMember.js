/**
 * Created by NetCenter-305 on 2016/7/7.
 */
import React from 'react'
import NavLink from './../NavLink'
export default React.createClass({
    render() {
        return (
            <div className="singleMember">
                <ul>
                    <li><NavLink to="/lastSevenDays">过去7天</NavLink></li>
                    <li><NavLink to="/lastFifteenDays">过去15天</NavLink></li>
                    <li><NavLink to="/lastMonth">过去30天</NavLink></li>
                    <li><NavLink to="/lastTwoMonths">过去60天</NavLink></li>
                    <li><NavLink to="/lastFourMonths">过去120天</NavLink></li>
                    <li><NavLink to="/lastSixMonths">过去180天</NavLink></li>
                </ul>
                {this.props.children}
            </div>

        )
    }
})
