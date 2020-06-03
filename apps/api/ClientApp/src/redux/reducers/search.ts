import { Action, Reducer } from "redux";
import * as searchAmphoraActions from "../actions/search/searchAmphora";
import { SearchState } from "../state/search";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<SearchState> = (
  state: SearchState | undefined,
  incomingAction: Action
): SearchState => {
  if (state === undefined) {
    return {
      query: { page: 0, term: "" },
      results: [],
    };
  }

  switch (incomingAction.type) {
    case searchAmphoraActions.SEARCH_AMPHORA:
      const searchAction = incomingAction as searchAmphoraActions.SearchAmphoraAction;
      return {
        isLoading: true,
        query: searchAction.query,
        results: [],
      };
    case searchAmphoraActions.SEARCH_AMPHORA_SUCCESS:
      const searchSuccessAction = incomingAction as searchAmphoraActions.SearchAmphoraSuccessAction;
      return {
        isLoading: false,
        results: searchSuccessAction.payload,
        query: searchSuccessAction.query,
      };
    default:
      return {
        isLoading: false,
        results: state.results,
        query: state.query,
      };
  }
};
