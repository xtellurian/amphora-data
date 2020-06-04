import { Action, Reducer } from "redux";
import * as actions from "../actions/maps/auth";
import { MapState } from "../state/MapState";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<MapState> = (
  state: MapState | undefined,
  incomingAction: Action
): MapState => {
  if (state === undefined) {
    return {};
  }

  switch (incomingAction.type) {
    case actions.FETCH_AZURE_MAP_TOKEN_SUCCESS:
      const onSuccessAction = incomingAction as actions.FetchTokenSuccessAction;
      return {
        subscriptionKey: onSuccessAction.payload,
      };
    default:
      return state;
  }
};
