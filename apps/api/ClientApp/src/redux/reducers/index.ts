
import { amphoraeReducer } from "./amphorae";
import { counterReducer } from "./counter";
import { reducer as weatherReducer } from './weatherForcast'
import { reducer as oidcReducer } from 'redux-oidc';

export const reducers = {
    counter: counterReducer,
    oidc: oidcReducer,
    weatherForecasts: weatherReducer,
    amphoras: amphoraeReducer,
};
