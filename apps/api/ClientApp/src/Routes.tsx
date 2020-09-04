import React, { Suspense } from "react";
import { Route, Switch, Redirect, RouteProps } from "react-router-dom";
import { useLocation, useHistory } from "react-router";
import { CallbackPage, IdentityContext } from "react-amphora";

import userManager from "./userManager";
import { Challenge } from "./components/auth/Challenge";
import { AccountPage } from "./components/account/AccountPage";

// toasts
import { toastOnSignIn, toastOnSignInError } from "./utilities/toasts";

import withTour from "./components/tour/withTour";
import { Home } from "./components/Home";
import { User } from "oidc-client";

import { LoadingState } from "./components/molecules/empty";

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
const SearchPage = React.lazy(
    () => import("./components/amphorae/search/SearchPage")
);
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
const LazySearch: React.FC = (props) => {
    return (
        <LazyLoader>
            <SearchPage {...props} />
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

            <Route path="/search" component={LazySearch} />
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
    const history = useHistory();
    // if location is callback page, return only CallbackPage route to allow signin process
    // IdentityServer 'bug' with hash history: if callback page contains a '#' params are appended with no delimiter
    // eg. /callbacktoken_id=...
    if (location.hash.substring(0, 10) === "#/callback") {
        const rest = location.hash.substring(10);
        return (
            <CallbackPage
                onSignIn={(u) => {
                    toastOnSignIn(u);
                    history.replace("/");
                }}
                onSignInError={(e) => {
                    toastOnSignInError(e);
                }}
                signInParams={`${rest}`}
                userManager={userManager}
            >
                <LoadingState>
                    <p>Just getting things ready.</p>
                    <p className="text-primary" onClick={() => window.location.reload(false)}>
                        If you're stuck trying to sign in, refresh the page, or try signing out.
                    </p>
                </LoadingState>
            </CallbackPage>
        );
    }

    if (idState.user) {
        return <RoutesWithTour user={idState.user} />;
    } else {
        return <AnonymousRoutes />;
    }
};
