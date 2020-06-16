import { axiosClient } from "../../../clients/amphoraApiClient";
import * as authActions from '../../actions/maps/auth';

const getToken = (store: any) => (next: any) => (
  action: authActions.AllMapAuthActions
) => {
  next(action);
  if (action.type === authActions.FETCH_AZURE_MAP_TOKEN) {
    axiosClient.get("api/maps/key")
      .then((r) => store.dispatch(authActions.actionCreators.receiveToken(r.data)))
      .catch((e) => store.dispatch(authActions.actionCreators.fail(e)));
  }
};

export const mapsMiddleware = [getToken];
