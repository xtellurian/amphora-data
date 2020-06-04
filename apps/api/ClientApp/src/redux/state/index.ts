import { RouterState } from "connected-react-router";
import * as Counter from "./counter";
import { AmphoraState } from "./amphora";
import { TermsOfUseState } from "./terms";
import { Self } from "./self";
import { MapState } from "./MapState";
import { SearchState } from "./search";
import { MenuStates } from "./plugins/burgerMenu";
import { OidcState } from "./plugins/oidc";
import { Reducers } from "../reducers";

// The top-level state object
export interface ApplicationState extends Reducers {
    // amphora app states
    amphora: AmphoraState;
    terms: TermsOfUseState;
    self: Self;
    search: SearchState;
    maps: MapState;
    // other states
    burgerMenu: MenuStates;
    counter: Counter.CounterState | undefined;
    oidc: OidcState;
    router: RouterState;
}
