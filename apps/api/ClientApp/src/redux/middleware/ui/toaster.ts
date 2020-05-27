import { Action } from 'redux';
import * as toast from '../../../components/molecules/toasts';

import { CREATE_ERROR, RECIEVE_AMPHORAE_CREATED, CreateAmphoraFailedAction, RecieveCreatedAmphoraAction } from '../../actions/amphora/create';
import { UPLOAD_FILE_SUCCESS, UPLOAD_FILE_FAIL, UploadFileFailedAction, UploadFileSuccessAction } from '../../actions/amphora/files';


export const toastsListener = (store: any) => (next: any) => (action: Action) => {
    next(action);

    switch (action.type) {
        case CREATE_ERROR:
            const createErrorAction = action as CreateAmphoraFailedAction;
            toast.error({ text: `Error Creating Amphora. ${createErrorAction.message}` });
            break;
        case RECIEVE_AMPHORAE_CREATED:
            const createdAction = action as RecieveCreatedAmphoraAction;
            toast.success({
                text: "Amphora Created!",
                path: `/amphora/detail/${createdAction.payload.id}`
            });
            break;
        case UPLOAD_FILE_FAIL:
            const fileUploadErrorAction = action as UploadFileFailedAction;
            toast.error({ text: `Error Uploading File. ${fileUploadErrorAction.message}` });
            break;
        case UPLOAD_FILE_SUCCESS:
            const fileUploadSuccess = action as UploadFileSuccessAction;
            toast.success({ text: `Uploaded File: ${fileUploadSuccess.payload.name}` });
            break;

    }
}