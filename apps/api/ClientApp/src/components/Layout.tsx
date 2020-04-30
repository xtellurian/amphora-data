import * as React from 'react';
import { User } from 'oidc-client';
import { Dispatch } from 'redux';
import {connect} from 'react-redux';
import { Container } from 'reactstrap';
import NavMenu from './navigation/NavMenu';
import HamburgerMenu from './navigation/HamburgerMenu';
import { ApplicationState } from '../redux/state';

interface LayoutProps {
    user: User;
    isLoadingUser: boolean;
    dispatch: Dispatch;
    location: any;
    children?: React.ReactNode
}

const Layout = (props: LayoutProps) => {
    const isConnected: boolean = !!props.user;
    return (
        <React.Fragment>
            <HamburgerMenu isConnected={isConnected} path={props.location.pathname} />
            <NavMenu isConnected={isConnected} path={props.location.pathname} />
            <Container>
                {props.children}
            </Container>
        </React.Fragment>
    );
}

function mapStateToProps(state: ApplicationState) {
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
