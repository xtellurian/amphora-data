import { Action as listActions } from 'redux';
import { TermsOfUse } from 'amphoradata';
import { OnFailedAction } from '../fail';

export const LIST_TERMS = '[terms] LIST TERMS';
export const LIST_TERMS_SUCCESS = `${LIST_TERMS} SUCCESS`;
export const LIST_TERMS_FAIL = `${LIST_TERMS} FAIL`;

export interface ListTermsAction extends listActions {
    type: typeof LIST_TERMS;
}

export interface ListTermsSuccessAction extends listActions {
    type: typeof LIST_TERMS_SUCCESS;
    payload: TermsOfUse[];
}

export const actionCreators = {
    // listing amphora
    listTerms: (): ListTermsAction => ({ type: LIST_TERMS }),
    recieveTerms: (data: TermsOfUse[]): ListTermsSuccessAction => ({
        type: LIST_TERMS_SUCCESS,
        payload: data,
    }),

    fail: (e: any): OnFailedAction => ({
        type: LIST_TERMS_FAIL,
        message: `Failed to get Terms, ${e}`,
        failed: true
    })
}

export type TermsAction = ListTermsAction | ListTermsSuccessAction | OnFailedAction;
