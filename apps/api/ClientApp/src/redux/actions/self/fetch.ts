import { Action } from "redux";
import { AmphoraUser } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";

export const FETCH_SELF = "[signals] FETCH SELF";
export const FETCH_SELF_SUCCESS = `${FETCH_SELF} SUCCESS`;
export const FETCH_SELF_FAIL = `${FETCH_SELF} FAIL`;

export interface FetchSelfAction extends Action {
  type: typeof FETCH_SELF;
}

export interface FetchSelfSuccessAction extends Action {
  type: typeof FETCH_SELF_SUCCESS;
  payload: AmphoraUser;
}

export const actionCreators = {
  // listing amphora
  fetchSelf: (): FetchSelfAction => ({
    type: FETCH_SELF,
  }),
  recieveSelf: ( payload: AmphoraUser): FetchSelfSuccessAction => ({
    type: FETCH_SELF_SUCCESS,
    payload,
  }),

  fail: (e: any): OnFailedAction => ({
    type: FETCH_SELF_FAIL,
    message: `Missing self information, ${toMessage(e)}`,
    failed: true,
  }),
};

export type AllSelfActions =
  | FetchSelfAction
  | FetchSelfSuccessAction
  | OnFailedAction;
