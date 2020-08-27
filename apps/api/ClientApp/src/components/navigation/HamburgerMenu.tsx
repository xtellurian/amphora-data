import * as React from "react";
import { connect } from "react-redux";
import { push as PlainMenu } from "react-burger-menu";
import { decorator as reduxBurgerMenu } from "redux-burger-menu";
import {
    NavItem,
    NavLink,
    Nav,
    NavbarBrand,
    Collapse,
    CardBody,
} from "reactstrap";
import { Link } from "react-router-dom";
import { useWindowSize } from "../../utilities/useWindowSize";
import { BurgerMenuState } from "../../redux/state/plugins/burgerMenu";
import { ApplicationState } from "../../redux/state";
import { actionCreators } from "../../redux/actions/plugins/burgerMenu";
import { Avatar } from "./Avatar";

import "./burgerMenu.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { useLocation } from "react-router";
const menuId = "primary";
const Menu = reduxBurgerMenu(PlainMenu, menuId); // this line connects the burger menu to redux state
type HamburgerMenuProps = {
    pageWrapId?: string;
    outerContainerId?: string;
} & BurgerMenuState & // ... state we've requested from the Redux store
    typeof actionCreators & { path: string; isConnected: boolean }; // ... plus action creators we've requested // ... plus incoming routing parameters

const ClickableMenuItem: React.FunctionComponent<{
    className?: string;
    to: string;
    icon?: IconProp;
    onClick?:
        | ((event: React.MouseEvent<HTMLElement, MouseEvent>) => void)
        | undefined;
}> = (props) => {
    return (
        <NavItem
            className={props.className}
            onClick={(e) => props.onClick && props.onClick(e)}
        >
            <NavLink tag={Link} className="text-light" to={props.to}>
                {props.icon && (
                    <FontAwesomeIcon
                        icon={props.icon}
                        style={{ marginRight: "0.5rem" }}
                    />
                )}
                {props.children}
            </NavLink>
        </NavItem>
    );
};

const MenuSection: React.FunctionComponent<{
    className?: string;
    startOpen: boolean;
    title: string;
    icon?: IconProp;
}> = (props) => {
    const [open, setOpen] = React.useState(props.startOpen);
    const toggle = () => setOpen(!open);
    return (
        <div style={{ width: "100%" }} className={props.className}>
            <h5
                onClick={() => toggle()}
                style={{ cursor: "pointer", marginTop: "1rem" }}
            >
                {props.icon && (
                    <FontAwesomeIcon
                        icon={props.icon}
                        style={{ marginRight: "0.75rem" }}
                    />
                )}
                {props.title}
            </h5>
            <Collapse isOpen={open}>
                <CardBody>{props.children}</CardBody>
            </Collapse>
        </div>
    );
};

const customStyles = {
    /* Position and sizing of burger button */
    bmBurgerButton: {
        position: "fixed",
        width: "22px",
        height: "20px",
        left: "18px",
        top: "18px",
    },
    /* Color/shape of burger icon bars */
    bmBurgerBars: {
        background: "var(--amphora-white)",
    },
    /* Color/shape of burger icon bars on hover*/
    bmBurgerBarsHover: {
        background: "#a90000",
    },
    /* Position and sizing of clickable cross button */
    bmCrossButton: {
        height: "24px",
        width: "24px",
    },
    /* Color/shape of close button cross */
    bmCross: {
        background: "#bdc3c7",
    },
    /*
    Sidebar wrapper styles
    Note: Beware of modifying this element as it can break the animations - you should not need to touch it in most cases
    */
    bmMenuWrap: {
        position: "fixed",
        height: "100%",
    },
    /* General sidebar styles */
    bmMenu: {
        background: "var(--ebony)",
        padding: "2.5em 1.5em 0",
        fontSize: "1.15em",
    },
    /* Morph shape necessary with bubble or elastic */
    bmMorphShape: {
        fill: "#373a47",
    },
    /* Wrapper for item list */
    bmItemList: {
        color: "#b8b7ad",
        width: "100%",
        padding: "0.8em",
    },
    /* Individual item */
    bmItem: {
        display: "inline-block",
    },
    /* Styling of overlay */
    bmOverlay: {
        background: "rgba(0, 0, 0, 0.3)",
    },
};

const windowWidthBehaviourThreshold = 1024;
const HamburgerMenu: React.FC<HamburgerMenuProps> = (props) => {
    const windowSize = useWindowSize();
    const location = useLocation();
    // change the default based on the window sizew
    React.useEffect(() => {
        if (
            windowSize.width &&
            windowSize.width > windowWidthBehaviourThreshold
        ) {
            props.open(menuId);
        } else {
            props.close(menuId);
        }
    }, [windowSize, location, props]);

    return (
        // noOverlay prevents greying out the main parts when triggering the menu
        <Menu
            disableAutoFocus
            noOverlay
            styles={customStyles}
            outerContainerId={props.outerContainerId}
            pageWrapId={props.pageWrapId}
        >
            <NavbarBrand className="w-100 m-2" tag={Link} to="/">
                <img
                    alt="The Amphora Data logo"
                    className="img-fluid"
                    src="/_content/sharedui/images/Amphora_Data_Logo_white.png"
                />
            </NavbarBrand>
            <Avatar />
            <Nav vertical className="m-2 w-100">
                <MenuSection
                    startOpen={false}
                    title="My Account"
                    icon="user-circle"
                >
                    <ClickableMenuItem icon="id-card" to="/profile">
                        Profile
                    </ClickableMenuItem>
                    <ClickableMenuItem icon="credit-card" to="/account">
                        Account
                    </ClickableMenuItem>
                </MenuSection>
                <hr className="bg-white" />
                <MenuSection title="My Amphora" icon="font" startOpen={true}>
                    <ClickableMenuItem
                        className="tour-search-button"
                        to="/search"
                        icon="search"
                    >
                        Search
                    </ClickableMenuItem>
                    <ClickableMenuItem
                        className="tour-create-button"
                        to="/create"
                        icon="plus-circle"
                    >
                        Create
                    </ClickableMenuItem>
                    <ClickableMenuItem
                        className="tour-my-amphora-button"
                        to="/amphora"
                        icon="cubes"
                    >
                        Collection
                    </ClickableMenuItem>
                    <ClickableMenuItem
                        className="tour-request-button"
                        to="/request"
                        icon="hand-paper"
                    >
                        Request
                    </ClickableMenuItem>
                </MenuSection>

                <MenuSection title="Management" icon="tasks" startOpen={false}>
                    <ClickableMenuItem to="/applications" icon="window-restore">
                        Applications
                    </ClickableMenuItem>
                    <ClickableMenuItem to="/terms" icon="file-contract">
                        Terms of Use
                    </ClickableMenuItem>
                </MenuSection>

                <MenuSection
                    className="tour-tour-button"
                    title="More"
                    icon="ellipsis-h"
                    startOpen={false}
                >
                    <NavItem>
                        <span
                            style={{
                                cursor: "pointer",
                            }}
                            className="text-light nav-link"
                            onClick={() => {
                                localStorage.setItem("tour", "accept");
                                window.location.reload(false);
                            }}
                        >
                            <FontAwesomeIcon
                                icon="sign"
                                style={{
                                    marginRight: "0.5rem",
                                }}
                            />
                            Tour
                        </span>
                    </NavItem>
                    <NavItem>
                        <a
                            className="text-light nav-link text-nowrap"
                            href="https://app.amphoradata.com/challenge"
                        >
                            <FontAwesomeIcon
                                icon="window-restore"
                                style={{ marginRight: "0.5rem" }}
                            />
                            Classic View
                        </a>
                    </NavItem>
                    <ClickableMenuItem to="/settings" icon="cog">
                        Settings
                    </ClickableMenuItem>
                </MenuSection>
            </Nav>
        </Menu>
    );
};

function mapStateToProps(state: ApplicationState) {
    return {
        isOpen: state.burgerMenu.primary
            ? state.burgerMenu.primary.isOpen
            : false,
    };
}

export default connect(mapStateToProps, actionCreators)(HamburgerMenu);
