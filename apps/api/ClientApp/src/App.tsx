import * as React from 'react';
import { connect } from 'react-redux';
import Layout from './components/Layout';
import Routes from './Routes'
import './components/core/fontawesome'; // Load FontAwesome library

import withSplashScreen from './components/splash/withSplashScreen'
import withTour from './components/tour/withTour'

// Load all global css
import './custom.css';
import './components/core/core.css';
import 'react-amphora/dist/index.css'

class App extends React.PureComponent {

    render() {
        return (
            <Layout >
                <Routes />
            </Layout>
        );
    }
}

const AppWithTour = withTour(App)
const AppWithSplash = withSplashScreen(AppWithTour)

export default connect()(AppWithSplash);

