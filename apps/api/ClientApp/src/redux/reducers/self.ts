import { Action, Reducer } from "redux";
import * as actions from "../actions/self/fetch";
import { Self } from "../state/self";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<Self> = (
  state: Self | undefined,
  incomingAction: Action
): Self => {
  if (state === undefined) {
    return {};
  }

  switch (incomingAction.type) {
    case actions.FETCH_SELF_SUCCESS:
      const onSuccessAction = incomingAction as actions.FetchSelfSuccessAction;
      return {
        userInfo: onSuccessAction.payload,
      };
    default:
      return state;
  }
};
