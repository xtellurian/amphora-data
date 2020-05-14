import * as React from 'react';
import { connect } from 'react-redux';
import { slide as PlainMenu } from 'react-burger-menu';
import { decorator as reduxBurgerMenu } from 'redux-burger-menu';
import userManager from '../../userManager';
import { NavItem, NavLink, Nav, NavbarBrand } from 'reactstrap';
import { Link } from 'react-router-dom';
import { IBurgerMenuState } from '../../redux/state/plugins/burgerMenu'
import { ApplicationState } from '../../redux/state';
import { actionCreators } from '../../redux/actions/plugins/burgerMenu';
import Avatar from './Avatar';

const Menu = reduxBurgerMenu(PlainMenu); // this line connects the burger menu to redux state
type HamburgerMenuProps =
    IBurgerMenuState // ... state we've requested from the Redux store
    & typeof actionCreators // ... plus action creators we've requested
    & { path: string, isConnected: boolean }; // ... plus incoming routing parameters


class HamburgerMenu extends React.PureComponent<HamburgerMenuProps> {

    logout(event: any) {
        event.preventDefault();
        userManager.signoutRedirect();
        userManager.removeUser();
    };

    login() {
        // pass the current path to redirect to the correct page after successfull login
        userManager.signinRedirect({
            data: { path: this.props.path },
        });
    };


    public render() {
        return (
            <Menu styles={this.styles()}>
                <h3>Menu</h3>
                <NavbarBrand className="m-2 bg-white w-100" tag={Link} to="/" >Amphora Data</NavbarBrand>
                <Avatar />

                <Nav vertical className="m-2">

                    <hr className="bg-white" />
                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-light" to="/amphora">My Amphora</NavLink>
                    </NavItem>
                    <NavItem className="menu-item" onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-light" to="/find">Create</NavLink>
                    </NavItem>
                    <NavItem className="menu-item" onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-light" to="/find">Request</NavLink>
                    </NavItem>

                    <hr className="bg-white" />

                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-light" to="/user">User Info</NavLink>
                    </NavItem>
                </Nav>
            </Menu >
        );
    }

    private styles() {
        return {
            /* Position and sizing of burger button */
            bmBurgerButton: {
                position: 'fixed',
                width: '22px',
                height: '20px',
                left: '18px',
                top: '18px'
            },
            /* Color/shape of burger icon bars */
            bmBurgerBars: {
                background: '#373a47'
            },
            /* Color/shape of burger icon bars on hover*/
            bmBurgerBarsHover: {
                background: '#a90000'
            },
            /* Position and sizing of clickable cross button */
            bmCrossButton: {
                height: '24px',
                width: '24px'
            },
            /* Color/shape of close button cross */
            bmCross: {
                background: '#bdc3c7'
            },
            /*
            Sidebar wrapper styles
            Note: Beware of modifying this element as it can break the animations - you should not need to touch it in most cases
            */
            bmMenuWrap: {
                position: 'fixed',
                height: '100%'
            },
            /* General sidebar styles */
            bmMenu: {
                background: 'rgb(56,56,56)',
                padding: '2.5em 1.5em 0',
                fontSize: '1.15em'
            },
            /* Morph shape necessary with bubble or elastic */
            bmMorphShape: {
                fill: '#373a47'
            },
            /* Wrapper for item list */
            bmItemList: {
                color: '#b8b7ad',
                width: "100%",
                padding: '0.8em'
            },
            /* Individual item */
            bmItem: {
                display: 'inline-block',
            },
            /* Styling of overlay */
            bmOverlay: {
                background: 'rgba(0, 0, 0, 0.3)'
            }
        }
    }
}

function mapStateToProps(state: ApplicationState) {
    return {
        isOpen: state.burgerMenu.isOpen,
    };
}

export default connect(
    mapStateToProps,
    actionCreators
)(HamburgerMenu);