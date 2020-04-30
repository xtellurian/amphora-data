
import { reducer as oidc } from 'redux-oidc';
import { reducer as burgerMenu } from 'redux-burger-menu';
import { reducer as amphora } from "./amphora";
import { reducer as ui } from "./ui";

export interface IReducers {
    amphora: any;
    burgerMenu: any
    oidc: any;
    ui: any,
}

// make sure these match the names of state!
export const reducers: IReducers = {
    amphora,
    burgerMenu,
    oidc,
    ui,
};
