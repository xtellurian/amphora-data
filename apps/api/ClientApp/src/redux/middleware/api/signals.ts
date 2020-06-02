import { amphoraApiClient } from "../../../clients/amphoraApiClient";
import * as createActions from "../../actions/signals/create";
import * as fetchActions from "../../actions/signals/fetch";

const createSignals = (store: any) => (next: any) => (
  action: createActions.AllCreateSignalActions
) => {
  next(action);
  if (action.type === createActions.CREATE_SIGNAL) {
    const createAction = action as createActions.CreateSignalAction;
    amphoraApiClient
      .amphoraeSignalsCreateSignal(createAction.amphoraId, createAction.payload)
      .then((r) =>
        store.dispatch(
          createActions.actionCreators.recieveNewSignal(
            createAction.amphoraId,
            r.data
          )
        )
      )
      .catch((e) => store.dispatch(createActions.actionCreators.fail(e)));
  }
};

const fetchSignals = (store: any) => (next: any) => (
  action: createActions.AllCreateSignalActions
) => {
  next(action);
  if (action.type === fetchActions.FETCH_SIGNALS) {
    const fetchAction = action as fetchActions.FetchSignalsAction;
    amphoraApiClient
      .amphoraeSignalsGetSignals(fetchAction.amphoraId)
      .then((r) =>
        store.dispatch(
          fetchActions.actionCreators.recieveSignals(
            fetchAction.amphoraId,
            r.data
          )
        )
      )
      .catch((e) => store.dispatch(fetchActions.actionCreators.fail(e)));
  }
};

export const signalsMiddleware = [createSignals, fetchSignals];
