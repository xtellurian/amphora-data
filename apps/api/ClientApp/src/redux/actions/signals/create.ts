import { Action } from "redux";
import { Signal } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";

export const CREATE_SIGNAL = "[signals] CREATE SIGNAL";
export const CREATE_SIGNAL_SUCCESS = `${CREATE_SIGNAL} SUCCESS`;
export const CREATE_SIGNAL_FAIL = `${CREATE_SIGNAL} FAIL`;

export interface CreateSignalAction extends Action {
  type: typeof CREATE_SIGNAL;
  amphoraId: string;
  payload: Signal;
}

export interface CreateSignalSuccessAction extends Action {
  type: typeof CREATE_SIGNAL_SUCCESS;
  amphoraId: string;
  payload: Signal;
}

export const actionCreators = {
  // listing amphora
  createSignal: (amphoraId: string, payload: Signal): CreateSignalAction => ({
    type: CREATE_SIGNAL,
    amphoraId,
    payload,
  }),
  recieveNewSignal: (amphoraId: string, payload: Signal): CreateSignalSuccessAction => ({
    type: CREATE_SIGNAL_SUCCESS,
    amphoraId,
    payload,
  }),

  fail: (e: any): OnFailedAction => ({
    type: CREATE_SIGNAL_FAIL,
    message: toMessage(e),
    failed: true,
  }),
};

export type AllCreateSignalActions =
  | CreateSignalAction
  | CreateSignalSuccessAction
  | OnFailedAction;
