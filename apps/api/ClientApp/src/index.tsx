import 'bootstrap/dist/css/bootstrap.css';

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import configureStore from './redux/configureStore';
import App from './App';
import registerServiceWorker from './registerServiceWorker';

// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href') as string;
const history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
// const initialState: ApplicationState = {
//     amphorae: {
//         isLoading: true,
//         list: []
//     },
//     burgerMenu: {
//         isOpen: false
//     },
//     counter: {
//         count: 0
//     },
//     modal: {
//         current: undefined,
//         isAmphoraDetailOpen: false
//     },
//     oidc: {
//         isLoadingUser: false
//     }
// }

const store = configureStore(history);

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <App />
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root'));

registerServiceWorker();
