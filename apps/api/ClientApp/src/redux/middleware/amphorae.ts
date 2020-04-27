import { GET_MY_CREATED_AMPHORAE, GET_MY_CREATED_AMPHORAE_SUCCESS, GET_MY_CREATED_AMPHORAE_ERROR, actionCreators } from "../actions/amphorae";
import { apiRequest } from "../actions/api";
import { IAction } from "../actions/action";

const baseUrl = "/api/amphorae";

// this middleware only care about the getBooks action
const getAmphoraFlow = (store: any) => (next: any) => (action: IAction) => {
  next(action);

  if (action.type === GET_MY_CREATED_AMPHORAE) {
    store.dispatch(apiRequest('GET', baseUrl, null, GET_MY_CREATED_AMPHORAE_SUCCESS, GET_MY_CREATED_AMPHORAE_ERROR));
    //   dispatch(showSpinner());
  }

};

// on successful GET, process the amphorae data
const processAmphoraeCollection = (store: any) => (next: any) => (action: IAction) => {
  next(action);

  if (action.type === GET_MY_CREATED_AMPHORAE_SUCCESS) {
    console.log("thingy")
    store.dispatch(actionCreators.updateAmphoraeList(action.payload));
    //   dispatch(hideSpinner())
  }
};


export { processAmphoraeCollection, getAmphoraFlow }