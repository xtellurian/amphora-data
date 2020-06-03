import axios from "axios";
import { Action } from "redux";

interface UserFoundAction extends Action {
  payload: { access_token: string; id_token: string };
}

export const axiosUpdater = (store: any) => (next: any) => (
  action: UserFoundAction
) => {
  if (action.type === "redux-oidc/USER_FOUND") {
    if (action.payload) {
      if (action.payload.access_token) {
        // Set the access token for axios
        axios.defaults.headers.common["Authorization"] =
          "Bearer " + action.payload.access_token;
      }
    }
  }

  return next(action);
};
