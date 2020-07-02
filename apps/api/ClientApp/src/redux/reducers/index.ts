import { reducer as oidc } from "redux-oidc";
import { reducer as burgerMenu } from "redux-burger-menu";
import { reducer as terms } from "./terms";
import { reducer as self } from "./self";
import { reducer as search } from "./search";
import { reducer as maps } from "./maps";
import { reducer as permissions } from "./permissions";

export interface Reducers {
    maps: any;
    permissions: any;
    terms: any;
    self: any;
    search: any;
    burgerMenu: any;
    oidc: any;
}

// make sure these match the names of state!
export const reducers: Reducers = {
    maps,
    permissions,
    terms,
    self,
    search,
    burgerMenu,
    oidc,
};
