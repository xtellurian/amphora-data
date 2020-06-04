import { Action } from "redux";
import { OnFailedAction, toMessage } from "../fail";

export const FETCH_AZURE_MAP_TOKEN = "[maps] FETCH TOKEN";
export const FETCH_AZURE_MAP_TOKEN_SUCCESS = `${FETCH_AZURE_MAP_TOKEN} SUCCESS`;
export const FETCH_AZURE_MAP_TOKEN_FAIL = `${FETCH_AZURE_MAP_TOKEN} FAIL`;

export interface FetchTokenAction extends Action {
  type: typeof FETCH_AZURE_MAP_TOKEN;
}

export interface FetchTokenSuccessAction extends Action {
  type: typeof FETCH_AZURE_MAP_TOKEN_SUCCESS;
  payload: string;
}

export const actionCreators = {
  // listing amphora
  fetchToken: (): FetchTokenAction => ({
    type: FETCH_AZURE_MAP_TOKEN,
  }),
  receiveToken: (token: string): FetchTokenSuccessAction => ({
    type: FETCH_AZURE_MAP_TOKEN_SUCCESS,
    payload: token
  }),

  fail: (e: any): OnFailedAction => ({
    type: FETCH_AZURE_MAP_TOKEN_FAIL,
    message: toMessage(e),
    failed: true,
  }),
};

export type AllMapAuthActions =
  | FetchTokenAction
  | FetchTokenSuccessAction
  | OnFailedAction;
