import { Action } from "redux";
import { Settings } from "../state/Settings";

export const UPDATE_SETTINGS = "[settings] UPDATE";

export interface UpdateSettingsAction extends Action {
    type: typeof UPDATE_SETTINGS;
    payload: Settings;
}

export const actionCreators = {
    update: (payload: Settings): UpdateSettingsAction => ({
        type: UPDATE_SETTINGS,
        payload,
    }),
};
