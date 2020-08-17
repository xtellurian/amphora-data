import * as React from "react";
import userManager from "../../userManager";
import { Container, Navbar, NavItem, NavLink } from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import { Avatar } from "./Avatar";
import { SignInButton } from "react-amphora";

export default class PublicNavMenu extends React.Component<
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
                <Navbar
                    className="border-bottom box-shadow mb-3 justify-content-end"
                    light
                >
                    <Container className="">
                        <div className="d-inline-flex ml-auto">
                            <ul className="mr-2 navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink
                                        tag={Link}
                                        className="text-dark"
                                        to="/"
                                    >
                                        Home
                                    </NavLink>
                                </NavItem>
                            </ul>
                            <SignInButton>Sign In</SignInButton>
                            <Avatar />
                        </div>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
