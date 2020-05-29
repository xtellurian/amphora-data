
import { reducer as oidc } from 'redux-oidc';
import { reducer as burgerMenu } from 'redux-burger-menu';
import { reducer as amphora } from "./amphora";
import { reducer as ui } from "./ui";
import { reducer as terms } from './terms';

export interface Reducers {
    amphora: any;
    terms: any;
    burgerMenu: any;
    oidc: any;
    ui: any;
}

// make sure these match the names of state!
export const reducers: Reducers = {
    amphora,
    terms,
    burgerMenu,
    oidc,
    ui,
};
