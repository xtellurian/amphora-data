import Home from './components/Home';
import Counter from './components/Counter';

import * as React from 'react';
import { connect } from 'react-redux';
import { Route, Switch } from 'react-router-dom';

import CallbackPage from './components/CallbackPage';
import { ApplicationState } from './redux/state';

import userManager from './userManager';
import { User } from 'oidc-client';
import UserInfo from './components/UserInfo';
import Amphora from './components/amphorae/MyAmphorae';
import { Dispatch } from 'redux';

interface RoutesModuleProps {
    user: User;
    isLoadingUser: boolean;
    dispatch: Dispatch;
    location: any;
}

const Routes = (props: RoutesModuleProps) => {

    // wait for user to be loaded, and location is known
    if (props.isLoadingUser || !props.location) {
        return <div>Loading...</div>;
    }

    // if location is callback page, return only CallbackPage route to allow signin process
    // IdentityServer 'bug' with hash history: if callback page contains a '#' params are appended with no delimiter
    // eg. /callbacktoken_id=...
    if (props.location.hash.substring(0,10) === "#/callback") {
        const rest = props.location.hash.substring(10);
        return <CallbackPage {...props} signInParams={`${rest}`} />;
    }

    // check if user is signed in
    userManager.getUser().then(user => {
        if (user && !user.expired) {
            // Set the authorization header for axios
            // axios.defaults.headers.common['Authorization'] = 'Bearer ' + user.access_token;
        }
    });

    // const isConnected: boolean = !!props.user;
    return (
        <React.Fragment>
            <Switch>
                <Route exact path='/' component={Home} />
                <Route path='/counter' component={Counter} />
                <Route path="/user" component={UserInfo} />
                <Route path="/amphora" component={Amphora} />
            </Switch>
        </React.Fragment>
    );
};

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
)(Routes as any);