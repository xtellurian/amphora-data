import { Action } from "redux";
import * as toast from "../../../components/molecules/toasts";

import {
  CREATE_ERROR,
  CREATE_SUCCESS,
  CreateAmphoraSuccessAction,
} from "../../actions/amphora/create";
import {
  UPLOAD_FILE_SUCCESS,
  UPLOAD_FILE_FAIL,
  UploadFileSuccessAction,
} from "../../actions/amphora/files";

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
    case CREATE_SUCCESS:
      const createdAction = action as CreateAmphoraSuccessAction;
      toast.success({
        text: "Amphora Created!",
        path: `/amphora/detail/${createdAction.payload.id}`,
      });
      break;
    case UPLOAD_FILE_SUCCESS:
      const fileUploadSuccess = action as UploadFileSuccessAction;
      toast.success({
        text: `Uploaded File: ${fileUploadSuccess.payload.name}`,
      });
      break;
  }
};
