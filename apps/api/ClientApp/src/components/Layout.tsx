import * as React from 'react';
import { User } from 'oidc-client';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { Container } from 'reactstrap';
import AppNavMenu from './navigation/AppNavMenu';
import PublicNavMenu from './navigation/PublicNavMenu';
import HamburgerMenu from './navigation/HamburgerMenu';
import { ApplicationState } from '../redux/state';

interface LayoutProps {
  user: User;
  isLoadingUser: boolean;
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
  margin: "0px 0px"
}

const Layout = (props: LayoutProps): React.ReactElement => {
  const isConnected = !!props.user;
  if (isConnected) {
    return (
      <div id="outer-container">
        <div style={fixedMenuStyle}>
        </div>
        <HamburgerMenu isConnected={isConnected} path={props.location.pathname} pageWrapId="page-wrap" outerContainerId="outer-container" />
        <AppNavMenu isConnected={isConnected} path={props.location.pathname} />
        <Container id="page-wrap">
          {/* adding these two divs so the menu doesn't push too far */}
          <div className="row">
            <div className="col-8">
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
        <Container>
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
