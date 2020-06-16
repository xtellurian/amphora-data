import * as fetchAmphoraActions from "../../actions/amphora/fetch";
import { actionCreators } from "../../actions/permissions/fetch";
import { Action } from "redux";

function getAccessLevel(
    scope: fetchAmphoraActions.Scope,
    accessType: fetchAmphoraActions.AccessType
): any {
    if (scope === "self") {
        if (accessType === "created") {
            return 64;
        } else if (accessType === "purchased") {
            return 32;
        }
    } else if (scope === "organisation") {
        if (accessType === "created") {
            return 64;
        } else if (accessType === "purchased") {
            return 32;
        }
    }
}

export const forPermissions = (store: any) => (next: any) => (
    action: Action
) => {
    if (action.type === fetchAmphoraActions.FETCH_AMPHORA_SUCCESS) {
        const fetchAmphoraAction = action as fetchAmphoraActions.FetchAmphoraSuccessAction;
        store.dispatch(
            actionCreators.fetchAccessLevels([
                { accessLevel: 24, amphoraId: fetchAmphoraAction.amphoraId },
                { accessLevel: 32, amphoraId: fetchAmphoraAction.amphoraId },
                { accessLevel: 64, amphoraId: fetchAmphoraAction.amphoraId },
            ])
        );
    } else if (action.type === fetchAmphoraActions.LIST_AMPHORAE_SUCCESS) {
        const listAmphoraSuccessAction = action as fetchAmphoraActions.RecieveAmphoraListAction;
        if(listAmphoraSuccessAction.payload && listAmphoraSuccessAction.payload.length > 0) {
            store.dispatch(
                actionCreators.fetchAccessLevels(
                    listAmphoraSuccessAction.payload.map((a) => {
                        return {
                            accessLevel: getAccessLevel(
                                listAmphoraSuccessAction.scope,
                                listAmphoraSuccessAction.accessType
                            ),
                            amphoraId: a.id,
                        };
                    })
                )
            );
        }
    }

    return next(action);
};
