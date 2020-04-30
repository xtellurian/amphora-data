import { Action } from "redux";
import * as listActions from "../actions/amphora/list";

import * as amphoradata from 'amphoradata';

const configuration = new amphoradata.Configuration({ basePath: "." });
const amphoraApiClient = new amphoradata.AmphoraeApi(configuration);

const listAmphora = (store: any) => (next: any) => (action: Action) => {
  next(action);
  if (action.type === listActions.LIST_MY_CREATED_AMPHORAE) {
    amphoraApiClient.amphoraeList("self", "created")
      .then(r => store.dispatch(listActions.actionCreators.recieve(r.data)))
      .catch(e => store.dispatch(listActions.actionCreators.error(e)))
  }
};


export { listAmphora }