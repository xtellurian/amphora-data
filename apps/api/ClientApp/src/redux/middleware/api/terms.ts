import { termsOfUseApiClient } from '../../../clients/amphoraApiClient';
import * as listActions from '../../actions/terms/list';
import * as createActions from '../../actions/terms/create';

const listTerms = (store: any) => (next: any) => (action: listActions.TermsAction) => {
    next(action);
    if (action.type === listActions.LIST_TERMS) {
        termsOfUseApiClient.termsOfUseList()
            .then(r => store.dispatch(listActions.actionCreators.recieveTerms(r.data)))
            .catch(e => store.dispatch(listActions.actionCreators.fail(e)))
    }
};

const createNewTerms = (store: any) => (next: any) => (action: createActions.CreateTermsActions) => {
    next(action);
    if (action.type === createActions.CREATE_TERMS) {
        const createTermsAction = action as createActions.CreateTermsAction
        termsOfUseApiClient.termsOfUseCreate(createTermsAction.payload)
            .then(r => store.dispatch(createActions.actionCreators.recieveTerms(r.data)))
            .catch(e => store.dispatch(createActions.actionCreators.fail(e)))
    }
};

export const termsMiddleware = [listTerms, createNewTerms];