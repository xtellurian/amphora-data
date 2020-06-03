import {actionCreators as fetchSelfActions} from '../../actions/self/fetch';
import { Action } from 'redux';



export const onLogin = (store: any) => (next: any) => (action: Action) => {

    if (action.type === "redux-oidc/USER_FOUND") {
      store.dispatch(fetchSelfActions.fetchSelf())
    }
    
    return next(action);
}
