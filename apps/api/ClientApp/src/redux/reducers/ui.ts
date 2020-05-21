import * as actions from "../actions/ui";
import { UiState, Alert } from "../state/ui";
import { Reducer, Action } from "redux";

interface ThingWithId {
    id: string;
}

function removeAlert(things: Alert[] | undefined, id: string): Alert[] | undefined {
    if (things) {
        // get index of object with id:37
        const removeIndex = things.map(function (item) { return item.id; }).indexOf(id);
        // remove object
        if (removeIndex >= 0) {

            return [
                ...things.slice(0, removeIndex),
                ...things.slice(removeIndex + 1)
            ];
        } else {
            return things;
        }
    }
}

export const reducer: Reducer<UiState> = (state: UiState | undefined, incomingAction: Action): UiState => {
    if (state === undefined) {
        return {
            alerts: [],
            current: undefined,
            isAmphoraDetailOpen: false,
        };
    }

    switch (incomingAction.type) {
        case actions.OPEN_AMPHORA_DETAIL_MODAL:
            const a = incomingAction as actions.OpenAmphoraModelAction;
            return {
                alerts: state.alerts,
                current: a.payload,
                isAmphoraDetailOpen: true
            }
        case actions.CLOSE_AMPHORA_DETAIL_MODAL:
            return {
                alerts: state.alerts,
                current: state.current,
                isAmphoraDetailOpen: false
            }
        case actions.POST_ALERT:
            const postAction = incomingAction as actions.PostAlertAction;
            const alerts = (state.alerts || []);
            alerts.push(postAction.payload);
            return {
                alerts: [...alerts], // keeping the same array means components wont update
                current: state.current,
                isAmphoraDetailOpen: state.isAmphoraDetailOpen
            }
        case actions.POP_ALERT:
            const popAction = incomingAction as actions.PopAlertAction;
            return {
                alerts: removeAlert(state.alerts, popAction.id),
                current: state.current,
                isAmphoraDetailOpen: state.isAmphoraDetailOpen
            };
        default:
            return state;
    }
};
