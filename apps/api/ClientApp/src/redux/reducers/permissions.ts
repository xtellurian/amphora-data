import { Action, Reducer } from "redux";
import * as actions from "../actions/permissions/fetch";
import { PermissionsState } from "../state/permissions";
import { emptyCache } from "../state/common";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<PermissionsState> = (
    state: PermissionsState | undefined,
    incomingAction: Action
): PermissionsState => {
    if (state === undefined) {
        return {
            purchase: emptyCache<boolean>(),
            readContents: emptyCache<boolean>(),
            writeContents: emptyCache<boolean>(),
            isLoading: false,
        };
    }

    switch (incomingAction.type) {
        case actions.FETCH_PERMISSIONS:
            return {
                purchase: state.purchase,
                readContents: state.readContents,
                writeContents: state.writeContents,
                isLoading: true,
            };
        case actions.FETCH_PERMISSIONS_SUCCESS:
            const onSuccessAction = incomingAction as actions.FetchPermissionsSuccessAction;
            const purchaseStore = { ...state.purchase.store };
            const readContentsStore = { ...state.readContents.store };
            const writeContentsStore = { ...state.writeContents.store };
            onSuccessAction.payload.forEach((a) => {
                if (a.accessLevel === actions.Purchase) {
                    if(a.amphoraId) purchaseStore[a.amphoraId] = a.isAuthorized || false;
                } else if (a.accessLevel === actions.ReadContents) {
                    if(a.amphoraId) readContentsStore[a.amphoraId] = a.isAuthorized || false;
                } else if (a.accessLevel === actions.WriteContents) {
                    if(a.amphoraId) writeContentsStore[a.amphoraId] = a.isAuthorized || false;
                }
            });
            return {
                isLoading: false,
                purchase: {
                    lastUpdated: new Date(),
                    store: purchaseStore,
                },
                readContents: {
                    lastUpdated: new Date(),
                    store: readContentsStore,
                },
                writeContents: {
                    lastUpdated: new Date(),
                    store: writeContentsStore,
                },
            };
        default:
            return state;
    }
};
