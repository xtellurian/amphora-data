import * as React from "react";
import userManager from "../../userManager";
import { Container, Navbar, NavItem, NavLink } from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import { Avatar } from "./Avatar";

export default class AppNavMenu extends React.Component<
    { path: string; isConnected: boolean },
    { isOpen: boolean }
> {
    public state = {
        isOpen: false,
    };

    logout(event: any) {
        event.preventDefault();
        userManager.signoutRedirect();
        userManager.removeUser();
    }

    login() {
        // pass the current path to redirect to the correct page after successfull login
        userManager.signinRedirect({
            data: { path: this.props.path },
        });
    }

    public render() {
        return (
            <header>
                <Navbar className="box-shadow mb-3 justify-content-end" light>
                    <Container className="">
                        <div className="d-inline-flex ml-auto">
                            <ul className="mr-2 navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink
                                        tag={Link}
                                        className="text-dark welcome-anchor"
                                        to="/"
                                    >
                                        Home
                                    </NavLink>
                                </NavItem>
                            </ul>
                            {this.props.isConnected ? (
                                <button
                                    className="btn btn-default"
                                    onClick={(event) => this.logout(event)}
                                >
                                    Logout
                                </button>
                            ) : (
                                <button
                                    className="btn btn-primary btn-sm"
                                    onClick={() => this.login()}
                                >
                                    Login
                                </button>
                            )}
                            <Avatar />
                        </div>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
