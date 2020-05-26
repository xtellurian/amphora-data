import * as actions from "../actions/ui";
import { UiState } from "../state/ui";
import { Reducer, Action } from "redux";

export const reducer: Reducer<UiState> = (state: UiState | undefined, incomingAction: Action): UiState => {
    if (state === undefined) {
        return {
            current: undefined,
            isAmphoraDetailOpen: false,
        };
    }

    switch (incomingAction.type) {
        case actions.OPEN_AMPHORA_DETAIL_MODAL:
            const a = incomingAction as actions.OpenAmphoraModelAction;
            return {
                current: a.payload,
                isAmphoraDetailOpen: true
            }
        case actions.CLOSE_AMPHORA_DETAIL_MODAL:
            return {
                current: state.current,
                isAmphoraDetailOpen: false
            }
        default:
            return state;
    }
};
