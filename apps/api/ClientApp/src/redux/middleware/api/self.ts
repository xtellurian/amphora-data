import { usersClient } from "../../../clients/amphoraApiClient";
import * as actions from "../../actions/self/fetch";

const fetchSelf = (store: any) => (next: any) => (
  action: actions.AllSelfActions
) => {
  next(action);
  if (action.type === actions.FETCH_SELF) {
    usersClient
      .usersReadSelf()
      .then((r) => store.dispatch(actions.actionCreators.recieveSelf(r.data)))
      .catch((e) => store.dispatch(actions.actionCreators.fail(e)));
  }
};

export const selfMiddleware = [fetchSelf];
