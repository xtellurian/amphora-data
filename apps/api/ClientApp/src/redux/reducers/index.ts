import { reducer as burgerMenu } from "redux-burger-menu";
import { reducer as maps } from "./maps";
import { reducer as settings } from "./settings";

export interface Reducers {
    maps: any;
    burgerMenu: any;
    settings: any;
}

// make sure these match the names of state!
export const reducers: Reducers = {
    maps,
    burgerMenu,
    settings,
};
