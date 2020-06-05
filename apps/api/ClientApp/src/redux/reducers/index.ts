import { reducer as oidc } from "redux-oidc";
import { reducer as burgerMenu } from "redux-burger-menu";
import { reducer as amphora } from "./amphora";
import { reducer as terms } from "./terms";
import { reducer as self } from "./self";
import { reducer as search } from "./search";
import { reducer as maps } from "./maps";
import { reducer as permissions } from "./permissions";

export interface Reducers {
    amphora: any;
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
    amphora,
    maps,
    permissions,
    terms,
    self,
    search,
    burgerMenu,
    oidc,
};
