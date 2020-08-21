import React, { Suspense } from "react";
import { Route, Switch, Redirect, RouteProps } from "react-router-dom";
import { useLocation } from "react-router";
import { CallbackPage, IdentityContext } from "react-amphora";

import userManager from "./userManager";
import { Challenge } from "./components/auth/Challenge";
import { AccountPage } from "./components/account/AccountPage";

import withTour from "./components/tour/withTour";
import { Home } from "./components/Home";
import { User } from "oidc-client";

import { LoadingState } from "./components/molecules/empty";

import Search from "./components/amphorae/Search";
import { CreateAmphoraPage } from "./components/amphorae/CreateAmphora";
import { RequestAmphoraPage } from "./components/amphorae/RequestAmphora";
import { ApplicationsPage } from "./components/applications/ApplicationsPage";

// profile
import { ProfilePage } from "./components/account/ProfilePage";

// settings
import { SettingsPage } from "./components/settings/SettingsPage";
import { TermsOfUsePage } from "./components/terms/TermsOfUsePage";

import Pallete from "./components/hidden/PalletePage";
import { DiagnosticPage } from "./components/hidden/DiagnosticPage";

// try lazy loading
const Amphora = React.lazy(() => import("./components/amphorae/MyAmphoraPage"));

interface AuthenticatedProps {
    user: User;
}

const LazyLoader: React.FC = ({ children }) => {
    return <Suspense fallback={<LoadingState />}>{children}</Suspense>;
};

const LazyAmphora: React.FC = (props) => {
    return (
        <LazyLoader>
            <Amphora {...props} />
        </LazyLoader>
    );
};

const AuthenticatedRoutes: React.FunctionComponent<AuthenticatedProps> = (
    props
) => (
    <React.Fragment>
        <Switch>
            <Route exact path="/" component={Home} />
            <Route path="/applications" component={ApplicationsPage} />

            <Route path="/search" component={Search} />
            <Route path="/create" component={CreateAmphoraPage} />
            <Route path="/amphora" component={LazyAmphora} />
            <Route path="/terms" component={TermsOfUsePage} />
            <Route path="/request" component={RequestAmphoraPage} />
            <Route
                path="/profile"
                render={(p) => <ProfilePage user={props.user} {...p} />}
            />

            <Route path="/account" component={AccountPage} />
            <Route path="/settings" component={SettingsPage} />

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
