import { reducer as oidc } from "redux-oidc";
import { reducer as burgerMenu } from "redux-burger-menu";
import { reducer as amphora } from "./amphora";
import { reducer as terms } from "./terms";
import { reducer as self } from "./self";
import { reducer as search } from "./search";

export interface Reducers {
  amphora: any;
  terms: any;
  self: any;
  search: any;
  burgerMenu: any;
  oidc: any;
}

// make sure these match the names of state!
export const reducers: Reducers = {
  amphora,
  terms,
  self,
  search,
  burgerMenu,
  oidc,
};
