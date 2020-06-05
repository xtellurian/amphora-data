import { Action } from "redux";
import { AccessLevelQuery, AccessLevelResponse } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";

export const FETCH_PERMISSIONS = "[permissions] FETCH";
export const FETCH_PERMISSIONS_SUCCESS = `${FETCH_PERMISSIONS} SUCCESS`;
export const FETCH_PERMISSIONS_FAIL = `${FETCH_PERMISSIONS} FAIL`;

export const Purchase = 24;
export const ReadContents = 32;
export const WriteContents = 64;

export interface LevelQuery extends AccessLevelQuery {
    accessLevel: typeof Purchase | typeof ReadContents | typeof WriteContents;
}

export interface FetchPermissionsAction extends Action {
    type: typeof FETCH_PERMISSIONS;
    query: LevelQuery[];
}

export interface FetchPermissionsSuccessAction extends Action {
    type: typeof FETCH_PERMISSIONS_SUCCESS;
    payload: AccessLevelResponse[];
}

export const actionCreators = {
    // listing amphora
    fetchAccessLevels: (query: LevelQuery[]): FetchPermissionsAction => ({
        type: FETCH_PERMISSIONS,
        query,
    }),
    recieveSelf: (
        payload: AccessLevelResponse[]
    ): FetchPermissionsSuccessAction => ({
        type: FETCH_PERMISSIONS_SUCCESS,
        payload,
    }),

    fail: (e: any): OnFailedAction => ({
        type: FETCH_PERMISSIONS_FAIL,
        message: `Couldn't retrieve permission information, ${toMessage(e)}`,
        failed: true,
    }),
};

export type AllSelfActions =
    | FetchPermissionsAction
    | FetchPermissionsSuccessAction
    | OnFailedAction;
