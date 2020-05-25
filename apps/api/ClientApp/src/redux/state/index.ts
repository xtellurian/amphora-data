import { RouterState } from 'connected-react-router';
import * as Counter from './counter';
import { AmphoraState } from './amphora';
import { BurgerMenuState, MenuStates } from './plugins/burgerMenu';
import { OidcState } from './plugins/oidc';
import { UiState } from './ui';
import { Reducers } from '../reducers'


// The top-level state object
export interface ApplicationState extends Reducers {
    amphora: AmphoraState;
    burgerMenu: MenuStates;
    counter: Counter.CounterState | undefined;
    ui: UiState;
    oidc: OidcState;
    router: RouterState;
}