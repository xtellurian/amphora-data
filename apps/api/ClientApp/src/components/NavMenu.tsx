import * as React from 'react';
import userManager from '../userManager';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export default class NavMenu extends React.Component<{ path: string, isConnected: boolean }, { isOpen: boolean }> {
    public state = {
        isOpen: false
    };

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
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">app</NavbarBrand>
                        <NavbarToggler onClick={this.toggle} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={this.state.isOpen} navbar>
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/amphora">Amphora</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/user">User Info</NavLink>
                                </NavItem>
                                {this.props.isConnected ? (
                                    <button className="btn btn-default btn-sm" onClick={event => this.logout(event)}>
                                        Logout
                                    </button>
                                ) : (
                                        <button className="btn btn-primary btn-sm" onClick={() => this.login()}>
                                            Login
                                        </button>
                                    )}
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }

    private toggle = () => {
        this.setState({
            isOpen: !this.state.isOpen
        });
    }
}
