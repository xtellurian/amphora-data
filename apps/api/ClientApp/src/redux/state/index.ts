import { RouterState } from "connected-react-router";
import * as Counter from "./counter";
import { MapState } from "./MapState";
import { MenuStates } from "./plugins/burgerMenu";
import { Reducers } from "../reducers";

// The top-level state object
export interface ApplicationState extends Reducers {
    // amphora app states
    maps: MapState;
    // other states
    burgerMenu: MenuStates;
    counter: Counter.CounterState | undefined;
    router: RouterState;
}
