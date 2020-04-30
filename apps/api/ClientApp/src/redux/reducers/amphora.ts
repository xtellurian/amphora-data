import * as listActions from "../actions/amphora/list";
import { AmphoraState } from "../state/amphora";
import { Reducer } from "redux";

export const reducer: Reducer<AmphoraState> = (state: AmphoraState | undefined, incomingAction: listActions.ListAction): AmphoraState => {

    if (state === undefined) {
        return {
            list: [],
            isLoading: false
        };
    }

    switch (incomingAction.type) {

        case listActions.RECIEVE_AMPHORAE_LIST:
            const recieveAmphoraAction = incomingAction as listActions.RecieveAmphoraAction
            if (recieveAmphoraAction.payload) {
                return {
                    isLoading: false,
                    list: incomingAction.payload
                }
            } else {
                return {
                    isLoading: false,
                    list: []
                }
            }

        default:
            return state;
    }
}