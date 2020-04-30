import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import { History } from 'history';
import { ApplicationState } from './state';
import { reducers } from './reducers';
import createOidcMiddleware from 'redux-oidc';
import userManager from '../userManager';
import allMiddleware from './middleware';

export default function configureStore(history: History, initialState?: ApplicationState) {
    const oidcMiddleware = createOidcMiddleware(userManager);
    const middleware = [
        thunk,
        oidcMiddleware,
        routerMiddleware(history),
        ...allMiddleware
    ];

    const rootReducer = combineReducers({
        ...reducers,
        router: connectRouter(history)
    });

    const enhancers = [];
    const windowIfDefined = typeof window === 'undefined' ? null : window as any;
    if (windowIfDefined && windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__) {
        enhancers.push(windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__());
    }

    return createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers),
    );
}
