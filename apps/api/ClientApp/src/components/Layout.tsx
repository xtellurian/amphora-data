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

const Layout = (props: LayoutProps): React.ReactElement => {
  const isConnected = !!props.user;
  if (isConnected) {
    return (
      <React.Fragment>
        <HamburgerMenu isConnected={isConnected} path={props.location.pathname} />
        <AppNavMenu isConnected={isConnected} path={props.location.pathname} />
        <Container>
          {props.children}
        </Container>
      </React.Fragment>
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
