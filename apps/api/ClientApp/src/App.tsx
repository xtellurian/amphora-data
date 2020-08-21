import * as React from "react";
import { connect } from "react-redux";
import { useHistory } from "react-router-dom";
import ReactGA from "react-ga";
import Layout from "./components/Layout";
import { Routes } from "./Routes";
import "./components/core/fontawesome"; // Load FontAwesome library
import withSplashScreen from "./components/splash/withSplashScreen";

// Load all global css
import "./custom.css";
import "./components/core/core.css";
import "react-amphora/dist/index.css";

ReactGA.initialize("UA-164144906-5"); // v2 app

const GATracker: React.FC = () => {
    const history = useHistory();
    history.listen((location) => {
        ReactGA.pageview(location.pathname);
    });
    return <React.Fragment></React.Fragment>;
};
class App extends React.PureComponent {
    render() {
        return (
            <Layout>
                <GATracker />
                <Routes />
            </Layout>
        );
    }
}

const AppWithSplash = withSplashScreen(App);

export default connect()(AppWithSplash);
