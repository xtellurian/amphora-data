import { Home } from "./components/Home";

import * as React from "react";
import userManager from "./userManager";
import { CallbackPage, IdentityContext } from "react-amphora";
import { Route, Switch, Redirect, RouteProps } from "react-router-dom";

import { Challenge } from "./components/auth/Challenge";
import { AccountPage } from "./components/account/AccountPage";

import withTour from "./components/tour/withTour";
import { User } from "oidc-client";
import Amphora from "./components/amphorae/MyAmphorae";
import { CreateAmphoraPage } from "./components/amphorae/CreateAmphora";
import { RequestAmphoraPage } from "./components/amphorae/RequestAmphora";
import Search from "./components/amphorae/Search";
import { ApplicationsPage } from "./components/applications/ApplicationsPage";

// profile
import { ProfilePage } from "./components/account/ProfilePage";

import { TermsOfUsePage } from "./components/terms/TermsOfUsePage";

import Pallete from "./components/hidden/PalletePage";
import { DiagnosticPage } from "./components/hidden/DiagnosticPage";

import { LoadingState } from "./components/molecules/empty/LoadingState";
import { useLocation } from "react-router";

interface AuthenticatedProps {
    user: User;
}

interface RoutesModuleProps extends AuthenticatedProps {
    isLoadingUser: boolean;
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

            <Route path="/account" component={AccountPage} />

            <Route path="/pallete" component={Pallete} />
            <Route path="/diagnostic" component={DiagnosticPage} />
        </Switch>
    </React.Fragment>
);

const RoutesWithTour = withTour(
    AuthenticatedRoutes
) as typeof AuthenticatedRoutes;

const RedirectToChallenge = () => {
    console.log("redirecting to challenge");
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

export const Routes: React.FC<RouteProps> = (props) => {
    const location = useLocation();
    const idState = IdentityContext.useIdentityState();

    // if location is callback page, return only CallbackPage route to allow signin process
    // IdentityServer 'bug' with hash history: if callback page contains a '#' params are appended with no delimiter
    // eg. /callbacktoken_id=...
    if (location.hash.substring(0, 10) === "#/callback") {
        const rest = location.hash.substring(10);
        return (
            <CallbackPage signInParams={`${rest}`} userManager={userManager} />
        );
    }

    if (idState.user) {
        return <RoutesWithTour user={idState.user} />;
    } else {
        return <AnonymousRoutes />;
    }
};
