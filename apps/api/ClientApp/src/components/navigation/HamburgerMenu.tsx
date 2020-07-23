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
import { BurgerMenuState } from "../../redux/state/plugins/burgerMenu";
import { ApplicationState } from "../../redux/state";
import { actionCreators } from "../../redux/actions/plugins/burgerMenu";
import Avatar from "./Avatar";

import "./burgerMenu.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
const menuId = "primary";
const Menu = reduxBurgerMenu(PlainMenu, menuId); // this line connects the burger menu to redux state
type HamburgerMenuProps = {
    pageWrapId?: string;
    outerContainerId?: string;
} & BurgerMenuState & // ... state we've requested from the Redux store
    typeof actionCreators & { path: string; isConnected: boolean }; // ... plus action creators we've requested // ... plus incoming routing parameters

interface HambuergerMenuState {
    openId?: string | null;
}

const ClickableMenuItem: React.FunctionComponent<{
    id?: string;
    to: string;
    icon?: IconProp;
    onClick?:
        | ((event: React.MouseEvent<HTMLElement, MouseEvent>) => void)
        | undefined;
}> = (props) => {
    return (
        <NavItem
            id={props.id}
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
    startOpen: boolean;
    title: string;
    icon?: IconProp;
}> = (props) => {
    const [open, setOpen] = React.useState(props.startOpen);
    const toggle = () => setOpen(!open);
    return (
        <div style={{ width: "100%" }}>
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

class HamburgerMenu extends React.PureComponent<
    HamburgerMenuProps,
    HambuergerMenuState
> {
    /**
     *
     */
    constructor(props: HamburgerMenuProps) {
        super(props);
        this.state = {};
    }
    componentDidMount() {
        this.props.open(menuId);
    }

    public render() {
        return (
            // noOverlay prevents greying out the main parts when triggering the menu
            <Menu
                disableAutoFocus
                noOverlay
                styles={this.styles()}
                outerContainerId={this.props.outerContainerId}
                pageWrapId={this.props.pageWrapId}
            >
                <NavbarBrand className="w-100 m-2" tag={Link} to="/">
                    <img
                        alt="The Amphora Data logo"
                        className="img-fluid"
                        src="/_content/sharedui/images/Amphora_Data_Logo_white.png"
                    />
                </NavbarBrand>
                <Avatar />
                {this.state.openId}
                <Nav vertical className="m-2 w-100">
                    <MenuSection
                        startOpen={false}
                        title="My Account"
                        icon="user-circle"
                    >
                        <ClickableMenuItem to="/">TODO</ClickableMenuItem>
                    </MenuSection>
                    <hr className="bg-white" />
                    <MenuSection
                        title="My Amphora"
                        icon="font"
                        startOpen={true}
                    >
                        <ClickableMenuItem to="/create" icon="plus-circle">
                            Create
                        </ClickableMenuItem>
                        <ClickableMenuItem to="/amphora" icon="cubes">
                            Collection
                        </ClickableMenuItem>
                        <ClickableMenuItem
                            id="search-button"
                            to="/search"
                            icon="search"
                        >
                            Search
                        </ClickableMenuItem>
                        <ClickableMenuItem to="/request" icon="hand-paper">
                            Request
                        </ClickableMenuItem>
                    </MenuSection>

                    <MenuSection
                        title="Management"
                        icon="tasks"
                        startOpen={false}
                    >
                        <ClickableMenuItem to="/terms" icon="file-contract">
                            Terms of Use
                        </ClickableMenuItem>
                    </MenuSection>

                    <MenuSection
                        title="More"
                        icon="ellipsis-h"
                        startOpen={false}
                    >
                        <NavItem>
                            <a
                                className="ml-3 text-light"
                                href="https://app.amphoradata.com/challenge"
                            >
                                Classic View
                            </a>
                        </NavItem>
                    </MenuSection>
                </Nav>
            </Menu>
        );
    }

    private styles() {
        return {
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
    }
}

function mapStateToProps(state: ApplicationState) {
    return {
        isOpen: state.burgerMenu.primary
            ? state.burgerMenu.primary.isOpen
            : false,
    };
}

export default connect(mapStateToProps, actionCreators)(HamburgerMenu);
