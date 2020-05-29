import { Action, Reducer } from 'redux';
import { TermsOfUseState } from '../state/terms'
import { emptyCache, StringToEntityMap } from '../state/common';
import { TermsOfUse } from 'amphoradata';
import * as actions from '../actions/terms/list';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<TermsOfUseState> = (state: TermsOfUseState | undefined, incomingAction: Action): TermsOfUseState => {
    if (state === undefined) {
        return {
            cache: emptyCache<TermsOfUse>(),
            isLoading: true,
            termIds: []
        };
    }

    switch (incomingAction.type) {
        case actions.LIST_TERMS:
            return {
                cache: state.cache,
                isLoading: true, // just set loading to true
                termIds: state.termIds
            };
        case actions.LIST_TERMS_SUCCESS:
            const listAction = incomingAction as actions.ListTermsSuccessAction;
            const allTerms = listAction.payload;
            const cache = state.cache || emptyCache<TermsOfUse>();
            allTerms.forEach((a) => {
                if (a.id) {
                    cache[a.id] = a;
                }
            });
            return {
                cache,
                isLoading: false,
                termIds: allTerms.map(t => t.id).filter(t => t) as string[]
            };
        case actions.LIST_TERMS_FAIL:
            return {
                cache: state.cache,
                isLoading: false,
                termIds: state.termIds
            }
        default:
            return state;
    }
};
