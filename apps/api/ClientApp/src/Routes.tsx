import { Home } from "./components/Home";

import * as React from "react";
import { connect } from "react-redux";
import { Route, Switch, Redirect } from "react-router-dom";

import CallbackPage from "./components/auth/CallbackPage";
import { Challenge } from "./components/auth/Challenge";
import { ApplicationState } from "./redux/state";
import { Dispatch } from "redux";

import withTour from "./components/tour/withTour";
import { User } from "oidc-client";
import Amphora from "./components/amphorae/MyAmphorae";
import { CreateAmphoraPage } from "./components/amphorae/CreateAmphora";
import { RequestAmphoraPage } from "./components/amphorae/RequestAmphora";
import Search from "./components/amphorae/Search";
import { ApplicationsPage } from "./components/applications/ApplicationsPage";

// profile
import { ProfilePage } from "./components/profile/ProfilePage";

import { TermsOfUsePage } from "./components/terms/TermsOfUsePage";

import Pallete from "./components/hidden/PalletePage";
import { DiagnosticPage } from "./components/hidden/DiagnosticPage";

import { MainPage } from "./components/public/MainPage";
import { LoadingState } from "./components/molecules/empty/LoadingState";

interface AuthenticatedProps {
    user: User;
}

interface RoutesModuleProps extends AuthenticatedProps {
    isLoadingUser: boolean;
    dispatch: Dispatch;
    location: any;
}

const AuthenticatedRoutes: React.FunctionComponent<AuthenticatedProps> = (
    props
) => (
    <React.Fragment>
        <Switch>
            <Route exact path="/" component={Home} />
            <Route path="/applications" component={ApplicationsPage} />

            <Route path="/search" component={Search} />
            <Route path="/create" component={CreateAmphoraPage} />
            <Route path="/amphora" component={Amphora} />
            <Route path="/terms" component={TermsOfUsePage} />
            <Route path="/request" component={RequestAmphoraPage} />
            <Route
                path="/profile"
                render={(p) => <ProfilePage user={props.user} {...p} />}
            />

            <Route path="/pallete" component={Pallete} />
            <Route path="/diagnostic" component={DiagnosticPage} />
        </Switch>
    </React.Fragment>
);

const RoutesWithTour = withTour(
    AuthenticatedRoutes
) as typeof AuthenticatedRoutes;

const RedirectToChallenge = () => {
    return (
        <React.Fragment>
            <Redirect to="/challenge" />
        </React.Fragment>
    );
};
const AnonymousRoutes = () => (
    <React.Fragment>
        <Switch>
            <Route exact path="/challenge" component={Challenge} />
            <Route path="/" component={RedirectToChallenge} />
        </Switch>
    </React.Fragment>
);

const Routes = (props: RoutesModuleProps) => {
    // wait for user to be loaded, and location is known
    if (props.isLoadingUser || !props.location) {
        return <LoadingState />;
    }

    // if location is callback page, return only CallbackPage route to allow signin process
    // IdentityServer 'bug' with hash history: if callback page contains a '#' params are appended with no delimiter
    // eg. /callbacktoken_id=...
    if (props.location.hash.substring(0, 10) === "#/callback") {
        const rest = props.location.hash.substring(10);
        return <CallbackPage {...props} signInParams={`${rest}`} />;
    }

    const isConnected = !!props.user;

    if (isConnected) {
        return <RoutesWithTour user={props.user} />;
    } else {
        return <AnonymousRoutes />;
    }
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

export default connect(mapStateToProps, mapDispatchToProps)(Routes as any);
