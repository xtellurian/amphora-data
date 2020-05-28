import * as React from 'react';
import { User } from 'oidc-client';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { Container } from 'reactstrap';
import AppNavMenu from './navigation/AppNavMenu';
import PublicNavMenu from './navigation/PublicNavMenu';
import HamburgerMenu from './navigation/HamburgerMenu';
import { ApplicationState } from '../redux/state';
import { Link } from 'react-router-dom';
import { Toaster } from './molecules/toasts/Toaster';

import 'react-toastify/dist/ReactToastify.css';
import '../components/core/layout.css';

interface LayoutProps {
  user: User;
  isLoadingUser: boolean;
  isMenuOpen: boolean;
  dispatch: Dispatch;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  location: any;
  children?: React.ReactNode;
}

const fixedMenuStyle: React.CSSProperties = {
  backgroundColor: "var(--ebony)",
  position: "fixed",
  width: "60px",
  height: "100%",
  margin: "0px 0px",
  paddingTop: "60px"
}

const Layout = (props: LayoutProps): React.ReactElement => {
  const isConnected = !!props.user;
  if (isConnected) {
    return (
      <div id="outer-container">
        {/* this is a fxed menu when hamburger is minimised */}
        <div style={fixedMenuStyle}>
          <Link to="/">
            <img alt="An Amphora logo" className="img-fluid p-2" src="/_content/sharedui/images/logos/amphora_white_transparent.svg" />
          </Link>
        </div>
        <Toaster />
        <HamburgerMenu isConnected={isConnected} path={props.location.pathname} pageWrapId="page-wrap" outerContainerId="outer-container" />
        <AppNavMenu isConnected={isConnected} path={props.location.pathname} />
        <Container fluid={true} id="page-wrap">
          <div className="row">
            <div className={`${props.isMenuOpen ? "col-8" : "col-11"}`}>
              {props.children}
            </div>
          </div>
        </Container>
      </div>
    );
  } else {
    return (
      <React.Fragment>
        <PublicNavMenu isConnected={isConnected} path={props.location.pathname} />
        <Container fluid={true}>
          {props.children}
        </Container>
      </React.Fragment>
    )
  }
}

function mapStateToProps(state: ApplicationState): any {
  return {
    user: state.oidc.user,
    isLoadingUser: state.oidc.isLoadingUser,
    location: state.router.location,
    isMenuOpen: state.burgerMenu.primary ? state.burgerMenu.primary.isOpen : false,
  };
}

function mapDispatchToProps(dispatch: Dispatch) {
  return {
    dispatch,
  };
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Layout as any);
