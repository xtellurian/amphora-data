import { RouterState } from "connected-react-router";
import * as Counter from "./counter";
import { AmphoraState } from "./amphora";
import { TermsOfUseState } from "./terms";
import { Self } from "./self";
import { MenuStates } from "./plugins/burgerMenu";
import { OidcState } from "./plugins/oidc";
import { UiState } from "./ui";
import { Reducers } from "../reducers";

// The top-level state object
export interface ApplicationState extends Reducers {
  // amphora app states
  amphora: AmphoraState;
  terms: TermsOfUseState;
  self: Self;
  // other states
  burgerMenu: MenuStates;
  counter: Counter.CounterState | undefined;
  ui: UiState;
  oidc: OidcState;
  router: RouterState;
}
