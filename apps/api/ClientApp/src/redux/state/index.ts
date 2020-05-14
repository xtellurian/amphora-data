import { RouterState } from 'connected-react-router';
import * as Counter from './counter';
import { AmphoraState } from './amphora';
import { IBurgerMenuState } from './plugins/burgerMenu';
import { OidcState } from './plugins/oidc';
import { UiState } from './ui';
import { IReducers } from '../reducers'


// The top-level state object
export interface ApplicationState extends IReducers {
    amphora: AmphoraState;
    burgerMenu: IBurgerMenuState
    counter: Counter.CounterState | undefined;
    ui: UiState;
    oidc: OidcState;
    router: RouterState;
}