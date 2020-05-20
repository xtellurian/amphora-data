import * as listActions from "../actions/amphora/list";
import { AmphoraState, StringToEntityMap } from "../state/amphora";
import { Reducer } from "redux";
import { DetailedAmphora } from "amphoradata";

export const reducer: Reducer<AmphoraState> = (state: AmphoraState | undefined, incomingAction: listActions.ListAction): AmphoraState => {

    if (state === undefined) {
        return {
            list: [],
            isLoading: false,
            cache: {}
        };
    }

    switch (incomingAction.type) {

        case listActions.RECIEVE_AMPHORAE_LIST:
            const recieveAmphoraAction = incomingAction as listActions.RecieveAmphoraAction
            const allAmphora = recieveAmphoraAction.payload;
            const cache: StringToEntityMap<DetailedAmphora> =  state.cache || {};
            allAmphora.forEach((a) => {
                if(a.id) {
                    cache[a.id] = a;
                }
            })
            if (recieveAmphoraAction.payload) {
                return {
                    isLoading: false,
                    list: incomingAction.payload,
                    cache
                }
            } else {
                return {
                    isLoading: false,
                    list: [],
                    cache: {}
                }
            }

        default:
            return state;
    }
}