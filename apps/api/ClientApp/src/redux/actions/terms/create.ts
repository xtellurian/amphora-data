import { Action } from "redux";
import { TermsOfUse, CreateTermsOfUse } from "amphoradata";
import { OnFailedAction } from "../fail";

export const CREATE_TERMS = "[terms] CREATE TERMS";
export const CREATE_TERMS_SUCCESS = `${CREATE_TERMS} SUCCESS`;
export const CREATE_TERMS_FAIL = `${CREATE_TERMS} FAIL`;

export interface CreateTermsAction extends Action {
  type: typeof CREATE_TERMS;
  payload: CreateTermsOfUse;
}

export interface CreateTermsSuccessAction extends Action {
  type: typeof CREATE_TERMS_SUCCESS;
  payload: TermsOfUse;
}

export const actionCreators = {
  // listing amphora
  createNewTerms: (payload: CreateTermsOfUse): CreateTermsAction => ({
    type: CREATE_TERMS,
    payload,
  }),
  recieveTerms: (data: TermsOfUse): CreateTermsSuccessAction => ({
    type: CREATE_TERMS_SUCCESS,
    payload: data,
  }),

  fail: (e: any): OnFailedAction => ({
    type: CREATE_TERMS_FAIL,
    message: `Failed to create terms [${e}]`,
    failed: true,
  }),
};

export type CreateTermsActions =
  | CreateTermsAction
  | CreateTermsSuccessAction
  | OnFailedAction;
