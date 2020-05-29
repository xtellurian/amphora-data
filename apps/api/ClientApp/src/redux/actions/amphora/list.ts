import { Action } from 'redux';
import { DetailedAmphora } from 'amphoradata';
import { OnFailedAction } from '../fail';

export const LIST_AMPHORAE = '[amphorae] LIST AMPHORA';
export const LIST_AMPHORAE_SUCCESS = `${LIST_AMPHORAE} SUCCESS`;
export const LIST_AMPHORAE_FAIL = `${LIST_AMPHORAE} FAIL`;

export interface ListMyAmphoraAction extends Action {
    type: typeof LIST_AMPHORAE;
    scope: Scope;
    accessType: AccessType;
}
export interface RecieveAmphoraListAction extends Action {
    type: typeof LIST_AMPHORAE_SUCCESS;
    payload: DetailedAmphora[];
    scope: Scope;
    accessType: AccessType;
}

const SELF_SCOPE = "self";
const ORG_SCOPE = "organisation";
const ACCESS_TYPE_CREATED = "created";
const ACCESS_TYPE_PURCHASED = "purchased";
export type Scope = typeof SELF_SCOPE | typeof ORG_SCOPE;
export type AccessType = typeof ACCESS_TYPE_CREATED | typeof ACCESS_TYPE_PURCHASED;

export const actionCreators = {
    // listing amphora
    listMyCreatedAmphora: (scope: Scope, accessType: AccessType): ListMyAmphoraAction => ({ type: LIST_AMPHORAE, scope, accessType }),
    recieveList: (data: DetailedAmphora[], scope: Scope, accessType: AccessType): RecieveAmphoraListAction => ({
        type: LIST_AMPHORAE_SUCCESS,
        payload: data,
        scope,
        accessType
    }),

    error: (e: any): OnFailedAction => ({
        failed: true,
        type: LIST_AMPHORAE_FAIL,
        message: `Failed to create: ${e}`
    })
}
