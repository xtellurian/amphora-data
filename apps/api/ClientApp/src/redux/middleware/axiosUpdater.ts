import axios from 'axios';
import { IAction } from "../actions/action"

interface IUserFoundAction extends IAction {
    payload: {access_token: string}
}

export const axiosUpdater = (store: any) => (next: any) => (action: IUserFoundAction) => {

    if (action.type === "redux-oidc/USER_FOUND") {
        if (action.payload && action.payload.access_token) {
            // Set the authorization header for axios
            axios.defaults.headers.common['Authorization'] = 'Bearer ' + action.payload.access_token;
        }
    }
    return next(action)
}
