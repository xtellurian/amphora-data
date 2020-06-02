import { Action, Reducer } from "redux";
import { TermsOfUseState } from "../state/terms";
import { emptyCache } from "../state/common";
import { TermsOfUse } from "amphoradata";
import * as listActions from "../actions/terms/list";
import * as createActions from "../actions/terms/create";

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<TermsOfUseState> = (
  state: TermsOfUseState | undefined,
  incomingAction: Action
): TermsOfUseState => {
  if (state === undefined) {
    return {
      cache: emptyCache<TermsOfUse>(),
      isLoading: false,
      termIds: [],
    };
  }

  switch (incomingAction.type) {
    // listing
    case listActions.LIST_TERMS:
      return {
        cache: state.cache,
        isLoading: true, // just set loading to true
        termIds: state.termIds,
      };
    case listActions.LIST_TERMS_SUCCESS:
      const listAction = incomingAction as listActions.ListTermsSuccessAction;
      const allTerms = listAction.payload;
      const cache = state.cache || emptyCache<TermsOfUse>();
      cache.lastUpdated = new Date();
      allTerms.forEach((a) => {
        if (a.id) {
          cache.store[a.id] = a;
        }
      });
      return {
        cache,
        isLoading: false,
        termIds: allTerms.map((t) => t.id).filter((t) => t) as string[],
      };
    case listActions.LIST_TERMS_FAIL:
      return {
        cache: state.cache,
        isLoading: false,
        termIds: state.termIds,
      };
    // creating
    case createActions.CREATE_TERMS:
      return {
        cache: state.cache,
        isLoading: true, // just set loading to true
        termIds: state.termIds,
      };
    case createActions.CREATE_TERMS_SUCCESS:
      const createdTermsAction = incomingAction as createActions.CreateTermsSuccessAction;
      state.cache.lastUpdated = new Date();
      if (createdTermsAction.payload.id) {
        state.cache.store[createdTermsAction.payload.id] = createdTermsAction.payload;
        const termIds = [createdTermsAction.payload.id, ...state.termIds];
        return {
          cache: state.cache,
          isLoading: false,
          termIds: termIds,
        };
      } else {
        return state;
      }
    case createActions.CREATE_TERMS_FAIL:
      return {
        cache: state.cache,
        isLoading: false,
        termIds: state.termIds,
      };
    default:
      return state;
  }
};
