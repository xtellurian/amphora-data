import { Action } from "redux";
import * as listActions from "../actions/amphora/list";
import * as createActions from "../actions/amphora/create";

import * as amphoradata from 'amphoradata';

const configuration = new amphoradata.Configuration({ basePath: "." });
const amphoraApiClient = new amphoradata.AmphoraeApi(configuration);

const listAmphora = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === listActions.LIST_MY_CREATED_AMPHORAE) {
    amphoraApiClient.amphoraeList("self", "created")
      .then(r => store.dispatch(listActions.actionCreators.recieveList(r.data)))
      .catch(e => store.dispatch(listActions.actionCreators.error(e)))
  }
};

const createAmphora = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === createActions.CREATE) {
    const createAction = action as createActions.CreateAmphoraAction;
    console.log("DOING THE THIGN@")
    amphoraApiClient.amphoraeCreate(createAction.payload)
      .then(r => store.dispatch(createActions.actionCreators.recieveCreated(r.data)))
      .catch(e => store.dispatch(createActions.actionCreators.createdError(e)))
  }
};

export const amphoraMiddleware = [listAmphora, createAmphora]