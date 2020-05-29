import { termsOfUseApiClient } from '../../../clients/amphoraApiClient';
import * as actions from '../../actions/terms/list';

const listTerms = (store: any) => (next: any) => (action: actions.TermsAction) => {
    next(action);
    if (action.type === actions.LIST_TERMS) {
        termsOfUseApiClient.termsOfUseList()
            .then(r => store.dispatch(actions.actionCreators.recieveTerms(r.data)))
            .catch(e => store.dispatch(actions.actionCreators.fail(e)))
    }
};

export const termsMiddleware = [listTerms];