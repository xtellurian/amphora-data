import * as React from "react";
import { connect } from "react-redux";
import { push as PlainMenu } from "react-burger-menu";
// import { slide as PlainMenu } from 'react-burger-menu';
import { decorator as reduxBurgerMenu } from "redux-burger-menu";
import { NavItem, NavLink, Nav, NavbarBrand } from "reactstrap";
import { Link } from "react-router-dom";
import { BurgerMenuState } from "../../redux/state/plugins/burgerMenu";
import { ApplicationState } from "../../redux/state";
import { actionCreators } from "../../redux/actions/plugins/burgerMenu";
import Avatar from "./Avatar";
const menuId = "primary";
const Menu = reduxBurgerMenu(PlainMenu, menuId); // this line connects the burger menu to redux state
type HamburgerMenuProps = {
    pageWrapId?: string;
    outerContainerId?: string;
} & BurgerMenuState & // ... state we've requested from the Redux store
    typeof actionCreators & { path: string; isConnected: boolean }; // ... plus action creators we've requested // ... plus incoming routing parameters

const ClickableMenuItem: React.FunctionComponent<{
    id?: string;
    to: string;
    onClick?:
        | ((event: React.MouseEvent<HTMLElement, MouseEvent>) => void)
        | undefined;
}> = (props) => {
    return (
        <NavItem id={props.id} onClick={(e) => props.onClick && props.onClick(e)}>
            <NavLink tag={Link} className="text-light" to={props.to}>
                {props.children}
            </NavLink>
        </NavItem>
    );
};

const MenuSection: React.FunctionComponent<{ title: string }> = (props) => {
    return (
        <React.Fragment>
            <h5>{props.title}</h5>
            {props.children}
        </React.Fragment>
    );
};

class HamburgerMenu extends React.PureComponent<HamburgerMenuProps> {
    componentDidMount() {
        this.props.open(menuId);
    }

    private onItemClick() {
        // this.props.close();
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

                <Nav vertical className="m-2">
                    <hr className="bg-white" />
                    <MenuSection title="Find Data">
                        <ClickableMenuItem id="search-button" to="/search">
                            Search
                        </ClickableMenuItem>
                        <ClickableMenuItem to="/request">
                            Request
                        </ClickableMenuItem>
                    </MenuSection>

                    <MenuSection title="My Data">
                        <ClickableMenuItem to="/amphora">
                            Amphora
                        </ClickableMenuItem>
                        <ClickableMenuItem to="/create">
                            Create
                        </ClickableMenuItem>
                    </MenuSection>

                    <MenuSection title="Management">
                        <ClickableMenuItem to="/terms">
                            Terms of Use
                        </ClickableMenuItem>
                    </MenuSection>

                    <hr className="bg-white" />

                    <NavItem>
                        <a className="ml-3 text-light" href="https://app.amphoradata.com/challenge">
                            Classic View
                        </a>
                    </NavItem>
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
