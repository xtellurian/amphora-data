import { Action } from "redux";
import axios from 'axios';
import * as listActions from "../actions/amphora/list";
import * as createActions from "../actions/amphora/create";
import * as fileActions from '../actions/amphora/files';
import { amphoraApiClient } from '../../clients/amphoraApiClient';
import { UploadResponse, DetailedAmphora } from "amphoradata";

// backup upload url
const backupUploadUrl = (id: string, name: string) => `/api/amphorae/${id}/files/${name}`;
// helper
function completeUpload(store: any, amphora: DetailedAmphora, name: string, file: File, res?: UploadResponse | null | undefined) {
  const data = new FormData()

  data.append('name', name)
  data.append('content', file)

  console.log('starting upload')
  if (res && res.url) {
    axios.put(res.url, data)
      .then(k => store.dispatch(fileActions.actionCreators.uploadFileSuccess(amphora, name)))
      .catch(e => store.dispatch(fileActions.actionCreators.uploadFileError(e)))
  } else if (amphora.id) {
    // try backup
    axios.put(backupUploadUrl(amphora.id, name), data)
      .then(k => store.dispatch(fileActions.actionCreators.uploadFileSuccess(amphora, name)))
      .catch(e => store.dispatch(fileActions.actionCreators.uploadFileError(e)))
  } else {
    store.dispatch(fileActions.actionCreators.uploadFileError("Oops, something went wrong."))
  }
}


const listAmphora = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === listActions.LIST_AMPHORAE) {
    const listAction = action as listActions.ListMyAmphoraAction;
    amphoraApiClient.amphoraeList(listAction.scope, listAction.accessType)
      .then(r => store.dispatch(listActions.actionCreators.recieveList(r.data, listAction.scope, listAction.accessType)))
      .catch(e => store.dispatch(listActions.actionCreators.error(e)))
  }
};

const createAmphora = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === createActions.CREATE) {
    const createAction = action as createActions.CreateAmphoraAction;
    amphoraApiClient.amphoraeCreate(createAction.payload)
      .then(r => store.dispatch(createActions.actionCreators.recieveCreated(r.data)))
      .catch(e => store.dispatch(createActions.actionCreators.createdError(e)))
  }
};

const uploadFile = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === fileActions.UPLOAD_FILE) {
    const uploadAction = action as fileActions.UploadFileAction;
    const amphora = uploadAction.payload.amphora;
    if (amphora.id) {
      const id = amphora.id;
      amphoraApiClient.amphoraeFilesCreateFileRequest(id, uploadAction.payload.name)
        .then(r => completeUpload(store, amphora, uploadAction.payload.name, uploadAction.payload.file, r.data))
        .catch(e => store.dispatch(fileActions.actionCreators.uploadFileError(e)))
    } else {
      store.dispatch(createActions.actionCreators.createdError("Local error"));
    }
  }
};



export const amphoraMiddleware = [listAmphora, createAmphora, uploadFile]