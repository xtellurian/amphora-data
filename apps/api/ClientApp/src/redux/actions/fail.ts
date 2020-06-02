import { Action } from "redux";

const FAILED = true;
export interface OnFailedAction extends Action {
  failed: typeof FAILED;
  message: string;
}

interface ErrorResponse {
  response: {
    data: string | any;
    status: number;
    statusText: string;
  };
}

export function toMessage(r: ErrorResponse) {
  if (r.response && r.response.data) {
    return `${r.response.data}`;
  } else if (r.response) {
    return `${r.response}`;
  } else {
    return `${r}`;
  }
}
