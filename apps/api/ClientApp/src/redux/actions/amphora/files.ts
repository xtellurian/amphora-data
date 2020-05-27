import { Action } from 'redux';
import { DetailedAmphora } from 'amphoradata';

export const UPLOAD_FILE = '[amphorae] UPLOAD FILE';
export const UPLOAD_FILE_SUCCESS = `${UPLOAD_FILE} SUCCESS`;
export const UPLOAD_FILE_FAIL = `${UPLOAD_FILE} FAIL`;

interface UploadFilePayload extends UploadFileSuccessPayload {
    file: File;
}
interface UploadFileSuccessPayload {
    amphora: DetailedAmphora;
    name: string;
}

export interface UploadFileAction extends Action {
    type: typeof UPLOAD_FILE;
    payload: UploadFilePayload;
}
export interface UploadFileSuccessAction extends Action {
    type: typeof UPLOAD_FILE_SUCCESS;
    payload: UploadFileSuccessPayload;
}
export interface UploadFileFailedAction extends Action {
    type: typeof UPLOAD_FILE_FAIL;
    message: string;
}


export const actionCreators = {
    uploadFile: (amphora: DetailedAmphora, file: File, name: string): UploadFileAction =>
        ({ type: UPLOAD_FILE, payload: { amphora, file, name } }),
    uploadFileSuccess: (amphora: DetailedAmphora, name: string): UploadFileSuccessAction =>
        ({ type: UPLOAD_FILE_SUCCESS, payload: { amphora, name } }),
    uploadFileError: (m: string): UploadFileFailedAction => ({ type: UPLOAD_FILE_FAIL, message: m })
}
