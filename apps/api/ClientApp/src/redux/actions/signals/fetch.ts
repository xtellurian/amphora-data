import { Action } from "redux";
import { Signal } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";

export const FETCH_SIGNALS = "[signals] FETCH SIGNAL METADATA";
export const FETCH_SIGNALS_SUCCESS = `${FETCH_SIGNALS} SUCCESS`;
export const FETCH_SIGNALS_FAIL = `${FETCH_SIGNALS} FAIL`;

export interface FetchSignalsAction extends Action {
  type: typeof FETCH_SIGNALS;
  amphoraId: string;
}

export interface FetchSignalsSuccessAction extends Action {
  type: typeof FETCH_SIGNALS_SUCCESS;
  amphoraId: string;
  payload: Signal[];
}

export const actionCreators = {
  // listing amphora
  fetchSignals: (amphoraId: string): FetchSignalsAction => ({
    type: FETCH_SIGNALS,
    amphoraId,
  }),
  recieveSignals: (amphoraId: string, payload: Signal[]): FetchSignalsSuccessAction => ({
    type: FETCH_SIGNALS_SUCCESS,
    amphoraId,
    payload,
  }),

  fail: (e: any): OnFailedAction => ({
    type: FETCH_SIGNALS_FAIL,
    message: toMessage(e),
    failed: true,
  }),
};

export type AllCreateSignalActions =
  | FetchSignalsAction
  | FetchSignalsSuccessAction
  | OnFailedAction;
