import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { MemoryRouter } from 'react-router-dom';
import App from './App';
// import { ApplicationState } from './redux/state';

it('renders without crashing', () => {
    const storeFake = (state: any) => ({
        default: () => { },
        subscribe: () => { },
        dispatch: () => { },
        getState: () => ({ ...state })
    });
    const store = storeFake({
        oidc: {
            user: undefined,
            isLoadingUser: false
        },
        router: {
            location: {
                hash: ""
            }
        }
    }) as any;
    ReactDOM.render(
        <Provider store={store}>
            <MemoryRouter>
                <App />
            </MemoryRouter>
        </Provider>, document.createElement('div'));
});
