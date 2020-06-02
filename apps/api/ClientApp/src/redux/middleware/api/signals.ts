import { amphoraApiClient } from "../../../clients/amphoraApiClient";
import * as createActions from "../../actions/signals/create";

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

export const signalsMiddleware = [createSignals];
