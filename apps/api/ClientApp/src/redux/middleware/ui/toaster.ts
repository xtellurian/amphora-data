import { Action } from "redux";
import * as toast from "../../../components/molecules/toasts";

import {
  CREATE_TERMS_SUCCESS,
  CreateTermsSuccessAction,
} from "../../actions/terms/create";
import {
  CREATE_SIGNAL_SUCCESS,
  CreateSignalSuccessAction
} from "../../actions/signals/create";

import { OnFailedAction } from "../../actions/fail";

export const toastsListener = (store: any) => (next: any) => (
  action: Action
) => {
  next(action);

  const failedAction = action as OnFailedAction;
  if (failedAction.failed) {
    toast.error({
      text: failedAction.message ? failedAction.message.toString() : "An error occurred.",
    });
  }

  // success actions
  switch (action.type) {
    case CREATE_TERMS_SUCCESS:
      const createTermsSuccessAction = action as CreateTermsSuccessAction;
      toast.success({
        text: `Created New Terms of Use: ${createTermsSuccessAction.payload.name}`,
      });
      break;
    case CREATE_SIGNAL_SUCCESS:
      const createSignalSuccessAction = action as CreateSignalSuccessAction;
      toast.success({
        text: `New Signal Created: ${createSignalSuccessAction.payload.property}`,
        path: `/amphora/detail/${createSignalSuccessAction.amphoraId}/signals`
      });
      break;
  }
};
