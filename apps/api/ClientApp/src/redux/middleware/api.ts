import axios from 'axios';
import { API_REQUEST } from "../actions/api";
import { IAction } from '../actions/action';
import { InputGroup } from 'reactstrap';

export const apiMiddleware = (store: any) => (next: any) => (action: IAction) => {
    next(action);
    if (action.type === API_REQUEST) {
        const state = store.getState()
        if (store.user) {
            axios.defaults.headers.common['Authorization'] = 'Bearer ' + state.oidc.user.access_token;
        }
        
        const { method, url, onSuccess, onError } = action.meta;
        // Send a POST request
        axios({
            method: method,
            url: url,
            data: action.payload,
        })
            .then(response => store.dispatch({ type: onSuccess, payload: response.data }))
            .catch(error => store.dispatch({ type: onError, payload: error }))
    }
}
