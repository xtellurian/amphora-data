import { RouterState } from "connected-react-router";
import * as Counter from "./counter";
import { MenuStates } from "./plugins/burgerMenu";
import { Settings } from "./Settings";
import { Reducers } from "../reducers";

// The top-level state object
export interface ApplicationState extends Reducers {
    // amphora app states
    settings: Settings;
    // other states
    burgerMenu: MenuStates;
    counter: Counter.CounterState | undefined;
    router: RouterState;
}
