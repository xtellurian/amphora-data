import { Action } from "redux";
import { OnFailedAction } from "../fail";
import { DetailedAmphora, CreateAmphora } from "amphoradata";

export const CREATE = "[amphorae] CREATE NEW";
export const CREATE_SUCCESS = `${CREATE} SUCCESS`;
export const CREATE_ERROR = `${CREATE} ERROR`;

export interface CreateAmphoraAction extends Action {
  type: typeof CREATE;
  payload: CreateAmphora;
}
export interface CreateAmphoraSuccessAction extends Action {
  type: typeof CREATE_SUCCESS;
  payload: DetailedAmphora;
}

export const actionCreators = {
  // creating amphora
  createNewAmphora: (a: CreateAmphora): CreateAmphoraAction => ({
    type: CREATE,
    payload: a,
  }),
  recieveCreated: (a: DetailedAmphora): CreateAmphoraSuccessAction => ({
    type: CREATE_SUCCESS,
    payload: a,
  }),
  createdError: (m: string): OnFailedAction => ({
    failed: true,
    type: CREATE_ERROR,
    message: m,
  }),
  // listing amphora
};

export type CreateAction = CreateAmphoraAction;
