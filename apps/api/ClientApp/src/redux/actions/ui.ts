
import { Action } from "redux";
import { DetailedAmphora } from "amphoradata";

export const OPEN_AMPHORA_DETAIL_MODAL = '[ui] OPEN AMPHORA DETAIL MODAL';
export const CLOSE_AMPHORA_DETAIL_MODAL = '[ui] CLOSE AMPHORA DETAIL MODAL';

export interface OpenAmphoraModelAction extends Action {
  payload: DetailedAmphora;
}

export const actionCreators = {
  open: (amphora: DetailedAmphora): OpenAmphoraModelAction => {
    return { type: OPEN_AMPHORA_DETAIL_MODAL, payload: amphora }
  },
  close: (): Action => ({ type: CLOSE_AMPHORA_DETAIL_MODAL }),
}


// export const SHOW_SPINNER      = '[ui] show spinner';
// export const HIDE_SPINNER      = '[ui] hide spinner';

// export const showSpinner = () => ({
//   type: SHOW_SPINNER
// });

// export const hideSpinner = () => ({
//   type: HIDE_SPINNER
// });

