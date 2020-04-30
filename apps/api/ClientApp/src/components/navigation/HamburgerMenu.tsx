import * as React from 'react';
import { connect } from 'react-redux';
import { slide as PlainMenu } from 'react-burger-menu';
import { decorator as reduxBurgerMenu } from 'redux-burger-menu';
import userManager from '../../userManager';
import { NavItem, NavLink, Nav, Collapse, NavbarBrand } from 'reactstrap';
import { Link } from 'react-router-dom';
import { IBurgerMenuState } from '../../redux/state/plugins/burgerMenu'
import { ApplicationState } from '../../redux/state';
import { actionCreators } from '../../redux/actions/plugins/burgerMenu';

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
                <Nav vertical>
                    <NavbarBrand tag={Link} to="/" >Amphora Data</NavbarBrand> 

                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink href="/">Home</NavLink>
                    </NavItem>

                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-dark" to="/find">Find Data</NavLink>
                    </NavItem>
                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-dark" to="/amphora">My Amphora</NavLink>
                    </NavItem>


                    <NavItem onClick={() => { this.props.close() }}>
                        <NavLink tag={Link} className="text-dark" to="/user">User Info</NavLink>
                    </NavItem>
                </Nav>
            </Menu >
        );
    }

    private styles() {
        return {
            bmBurgerButton: {
                position: 'fixed',
                width: '22px',
                height: '20px',
                left: '18px',
                top: '18px'
            },
            bmBurgerBars: {
                background: '#373a47'
            },
            bmBurgerBarsHover: {
                background: '#a90000'
            },
            bmCrossButton: {
                height: '24px',
                width: '24px'
            },
            bmCross: {
                background: '#bdc3c7'
            },
            bmMenuWrap: {
                position: 'fixed',
                height: '100%'
            },
            bmMenu: {
                background: 'white',
                padding: '2.5em 1.5em 0',
                fontSize: '1.15em'
            },
            bmMorphShape: {
                fill: '#373a47'
            },
            bmItemList: {
                color: '#b8b7ad',
                padding: '0.8em'
            },
            bmItem: {
                display: 'inline-block'
            },
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