import { permissionClient } from "../../../clients/amphoraApiClient";
import * as actions from "../../actions/permissions/fetch";

const fetchAccessPermissions = (store: any) => (next: any) => (
    action: actions.AllSelfActions
) => {
    next(action);
    if (action.type === actions.FETCH_PERMISSIONS) {
        const fetchAction = action as actions.FetchPermissionsAction;
        permissionClient
            .permissionGetPermissions({ accessQueries: fetchAction.query })
            .then((r) =>
                store.dispatch(
                    actions.actionCreators.recieveSelf(r.data.accessResponses || [])
                )
            )
            .catch((e) => store.dispatch(actions.actionCreators.fail(e)));
    }
};

export const permissionsMiddleware = [fetchAccessPermissions];
