import { reducer as burgerMenu } from "redux-burger-menu";
import { reducer as maps } from "./maps";

export interface Reducers {
    maps: any;
    burgerMenu: any;
}

// make sure these match the names of state!
export const reducers: Reducers = {
    maps,
    burgerMenu,
};
