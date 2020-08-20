import { Action, Reducer } from "redux";
import { UpdateSettingsAction, UPDATE_SETTINGS } from "../actions/settings";
import { Settings } from "../state/Settings";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<Settings> = (
    state: Settings | undefined,
    incomingAction: Action
): Settings => {
    if (state === undefined) {
        return {
            showDebuggingNotifications: false,
        };
    }

    const action = incomingAction as UpdateSettingsAction;

    switch (action.type) {
        case UPDATE_SETTINGS:
            return { ...action.payload };
        default:
            return state;
    }
};
