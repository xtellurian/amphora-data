import { Action } from 'redux';
import { TermsOfUse } from 'amphoradata';

export const LIST_TERMS = '[terms] LIST TERMS';
export const LIST_TERMS_SUCCESS = `${LIST_TERMS} SUCCESS`;
export const LIST_TERMS_FAIL = `${LIST_TERMS} FAIL`;

export interface ListTermsAction extends Action {
    type: typeof LIST_TERMS;
}

export interface ListTermsSuccessAction extends Action {
    type: typeof LIST_TERMS_SUCCESS;
    payload: TermsOfUse[];
}
export interface ListTermsFailAction extends Action {
    type: typeof LIST_TERMS_FAIL;
}

export const actionCreators = {
    // listing amphora
    listTerms: (): ListTermsAction => ({ type: LIST_TERMS }),
    recieveTerms: (data: TermsOfUse[]): ListTermsSuccessAction => ({
        type: LIST_TERMS_SUCCESS,
        payload: data,
    }),

    fail: (e: any): Action => ({
        type: LIST_TERMS_FAIL,
    })
}

export type TermsAction = ListTermsAction | ListTermsSuccessAction | ListTermsFailAction;
