
import { Action } from "redux";
import { DetailedAmphora } from "amphoradata";

export const OPEN_AMPHORA_DETAIL_MODAL = '[ui] OPEN AMPHORA DETAIL MODAL';
export const CLOSE_AMPHORA_DETAIL_MODAL = '[ui] CLOSE AMPHORA DETAIL MODAL';
export const POST_ALERT = '[ui] POST ALERT';
export const POP_ALERT = '[ui] POP ALERT';

export interface OpenAmphoraModelAction extends Action {
  payload: DetailedAmphora;
}

export interface PopAlertAction extends Action {
  type: typeof POP_ALERT;
  id: string;
}

export const actionCreators = {
  open: (amphora: DetailedAmphora): OpenAmphoraModelAction => {
    return { type: OPEN_AMPHORA_DETAIL_MODAL, payload: amphora }
  },
  close: (): Action => ({ type: CLOSE_AMPHORA_DETAIL_MODAL })
}
