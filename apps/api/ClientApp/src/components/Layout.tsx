import * as React from "react";
import { connect } from "react-redux";
import { useLocation } from "react-router";
import { IdentityContext } from "react-amphora";
import { Container } from "reactstrap";
import AppNavMenu from "./navigation/AppNavMenu";
import PublicNavMenu from "./navigation/PublicNavMenu";
import HamburgerMenu from "./navigation/HamburgerMenu";
import { ApplicationState } from "../redux/state";
import { Link } from "react-router-dom";
import { Toaster } from "./molecules/toasts/Toaster";

import "react-toastify/dist/ReactToastify.css";
import "../components/core/layout.css";

interface LayoutProps {
    isMenuOpen: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    children?: React.ReactNode;
}

const fixedMenuStyle: React.CSSProperties = {
    backgroundColor: "var(--ebony)",
    position: "fixed",
    width: "60px",
    height: "100%",
    margin: "0px 0px",
    paddingTop: "60px",
};

const Layout: React.FC<LayoutProps> = (props) => {
    const idState = IdentityContext.useIdentityState();
    const [isConnected, setIsConnected] = React.useState(!!idState.user);

    React.useEffect(() => {
        setIsConnected(!!idState.user);
    }, [idState.user]);
    const location = useLocation();

    if (isConnected) {
        return (
            <div id="outer-container">
                {/* this is a fxed menu when hamburger is minimised */}
                <div style={fixedMenuStyle}>
                    <Link to="/">
                        <img
                            alt="An Amphora logo"
                            className="img-fluid p-2"
                            src="/_content/sharedui/images/logos/amphora_white_transparent.svg"
                        />
                    </Link>
                </div>
                <Toaster />
                <HamburgerMenu
                    isConnected={isConnected}
                    path={location.pathname}
                    pageWrapId="page-wrap"
                    outerContainerId="outer-container"
                />
                <AppNavMenu
                    isConnected={isConnected}
                    path={location.pathname}
                />
                <Container fluid={true} id="page-wrap">
                    <div className="row">
                        <div
                            className={`${
                                props.isMenuOpen ? "col-8" : "col-11"
                            }`}
                        >
                            {props.children}
                        </div>
                    </div>
                </Container>
            </div>
        );
    } else {
        return (
            <React.Fragment>
                <PublicNavMenu
                    isConnected={isConnected}
                    path={location.pathname}
                />
                <Container fluid={true}>{props.children}</Container>
            </React.Fragment>
        );
    }
};

function mapStateToProps(state: ApplicationState): any {
    return {
        isMenuOpen: state.burgerMenu.primary
            ? state.burgerMenu.primary.isOpen
            : false,
    };
}

export default connect(mapStateToProps, null)(Layout as any);
