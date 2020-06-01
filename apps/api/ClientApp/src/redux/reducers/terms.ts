import { Action, Reducer } from "redux";
import { TermsOfUseState } from "../state/terms";
import { emptyCache, StringToEntityMap } from "../state/common";
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
      isLoading: true,
      termIds: [],
    };
  }

  switch (incomingAction.type) {
    // listing
    case listActions.LIST_TERMS:
      return {
        lastLoaded: new Date(),
        cache: state.cache,
        isLoading: true, // just set loading to true
        termIds: state.termIds,
      };
    case listActions.LIST_TERMS_SUCCESS:
      const listAction = incomingAction as listActions.ListTermsSuccessAction;
      const allTerms = listAction.payload;
      const cache = state.cache || emptyCache<TermsOfUse>();
      allTerms.forEach((a) => {
        if (a.id) {
          cache[a.id] = a;
        }
      });
      return {
        lastLoaded: new Date(),
        cache,
        isLoading: false,
        termIds: allTerms.map((t) => t.id).filter((t) => t) as string[],
      };
    case listActions.LIST_TERMS_FAIL:
      return {
        lastLoaded: state.lastLoaded,
        cache: state.cache,
        isLoading: false,
        termIds: state.termIds,
      };
    // creating
    case createActions.CREATE_TERMS:
      return {
        lastLoaded: state.lastLoaded,
        cache: state.cache,
        isLoading: true, // just set loading to true
        termIds: state.termIds,
      };
    case createActions.CREATE_TERMS_SUCCESS:
      const createdTermsAction = incomingAction as createActions.CreateTermsSuccessAction;
      if (createdTermsAction.payload.id) {
        state.cache[createdTermsAction.payload.id] = createdTermsAction.payload;
        const termIds = [createdTermsAction.payload.id, ...state.termIds];
        return {
          lastLoaded: state.lastLoaded,
          cache: state.cache,
          isLoading: false,
          termIds: termIds,
        };
      } else {
        return state;
      }
    case createActions.CREATE_TERMS_FAIL:
      return {
        lastLoaded: state.lastLoaded,
        cache: state.cache,
        isLoading: false,
        termIds: state.termIds,
      };
    default:
      return state;
  }
};
