import { Action } from "redux";
import { DetailedAmphora } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";

export const LIST_AMPHORAE = "[amphorae] LIST AMPHORA";
export const LIST_AMPHORAE_SUCCESS = `${LIST_AMPHORAE} SUCCESS`;
export const LIST_AMPHORAE_FAIL = `${LIST_AMPHORAE} FAIL`;

export const FETCH_AMPHORA = "[amphora] FETCH";
export const FETCH_AMPHORA_SUCCESS = `${FETCH_AMPHORA} SUCCESS`;
export const FETCH_AMPHORA_FAIL = `${FETCH_AMPHORA} FAIL`;

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
export interface FetchAmphoraAction extends Action {
  type: typeof FETCH_AMPHORA;
  amphoraId: string;
}
export interface FetchAmphoraSuccessAction extends Action {
  type: typeof FETCH_AMPHORA_SUCCESS;
  amphoraId: string;
  payload: DetailedAmphora;
}

const SELF_SCOPE = "self";
const ORG_SCOPE = "organisation";
const ACCESS_TYPE_CREATED = "created";
const ACCESS_TYPE_PURCHASED = "purchased";
export type Scope = typeof SELF_SCOPE | typeof ORG_SCOPE;
export type AccessType =
  | typeof ACCESS_TYPE_CREATED
  | typeof ACCESS_TYPE_PURCHASED;

export const actionCreators = {
  // listing amphora
  listMyCreatedAmphora: (
    scope: Scope,
    accessType: AccessType
  ): ListMyAmphoraAction => ({ type: LIST_AMPHORAE, scope, accessType }),
  recieveList: (
    data: DetailedAmphora[],
    scope: Scope,
    accessType: AccessType
  ): RecieveAmphoraListAction => ({
    type: LIST_AMPHORAE_SUCCESS,
    payload: data,
    scope,
    accessType,
  }),

  fetchAmphora: (amphoraId: string): FetchAmphoraAction =>( {
    type: FETCH_AMPHORA,
    amphoraId
  }),
  fetchAmphoraSuccess: (amphoraId: string, payload: DetailedAmphora): FetchAmphoraSuccessAction =>( {
    type: FETCH_AMPHORA_SUCCESS,
    amphoraId,
    payload
  }),
  fetchError: (amphoraId: string, e: any): OnFailedAction =>( {
    type: FETCH_AMPHORA_SUCCESS,
    message: `Failed to fetch Amphora(${amphoraId}), ${toMessage(e)}`,
    failed: true
  }),
  error: (e: any): OnFailedAction => ({
    failed: true,
    type: LIST_AMPHORAE_FAIL,
    message: toMessage(e),
  }),
};
