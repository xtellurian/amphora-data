import * as React from 'react';
import { connect } from 'react-redux';
import Layout from './components/Layout';
import Routes from './Routes'

import './custom.css'

class App extends React.PureComponent {

    render() {
        return (
            <Layout >
                <Routes />
            </Layout>
        );
    }
}

export default connect()(App);

