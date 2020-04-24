import { User } from 'oidc-client';
import { reducer as oidcReducer } from 'redux-oidc';
import { RouterState } from 'connected-react-router';
import * as WeatherForecasts from './WeatherForecasts';
import * as Counter from './Counter';
interface OidcState {
    isLoadingUser: boolean;
    user: User;
}

// The top-level state object
export interface ApplicationState {
    counter: Counter.CounterState | undefined;
    weatherForecasts: WeatherForecasts.WeatherForecastsState | undefined;
    oidc: OidcState;
    router: RouterState;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    counter: Counter.reducer,
    oidc: oidcReducer,
    weatherForecasts: WeatherForecasts.reducer
};


// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
