
import { Action } from "redux";
import { DetailedAmphora } from "amphoradata";
import { Alert } from "../state/ui";

export const OPEN_AMPHORA_DETAIL_MODAL = '[ui] OPEN AMPHORA DETAIL MODAL';
export const CLOSE_AMPHORA_DETAIL_MODAL = '[ui] CLOSE AMPHORA DETAIL MODAL';
export const POST_ALERT = '[ui] POST ALERT';
export const POP_ALERT = '[ui] POP ALERT';

export interface OpenAmphoraModelAction extends Action {
  payload: DetailedAmphora;
}

export interface PostAlertAction extends Action {
  type: typeof POST_ALERT;
  payload: Alert;
}

export interface PopAlertAction extends Action {
  type: typeof POP_ALERT;
  id: string;
}

export interface PostAlertAction extends Action {
  type: typeof POST_ALERT;
  payload: Alert;
}

export const actionCreators = {
  open: (amphora: DetailedAmphora): OpenAmphoraModelAction => {
    return { type: OPEN_AMPHORA_DETAIL_MODAL, payload: amphora }
  },
  close: (): Action => ({ type: CLOSE_AMPHORA_DETAIL_MODAL }),
  pushAlert: (alert: Alert): PostAlertAction => ({ type: POST_ALERT, payload: alert }),
  popAlert: (id: string): PopAlertAction => ({ type: POP_ALERT, id })
}
