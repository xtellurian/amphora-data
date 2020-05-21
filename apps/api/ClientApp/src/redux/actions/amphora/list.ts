import { Action } from 'redux';
import { DetailedAmphora, CreateAmphora } from 'amphoradata';

export const LIST_MY_CREATED_AMPHORAE = '[amphorae] LIST MY CREATED';
export const LIST_ORGANISATION_CREATED_AMPHORAE = '[amphorae] LIST ORGANISATION CREATED';
export const LIST_MY_PURCHASED_AMPHORAE = '[amphorae] LIST ORGANISATION PURCHASED';
export const LIST_ORGANISATION_PURCHASED_AMPHORAE = '[amphorae] LIST ORGANISATION PURCHASED';

export const RECIEVE_AMPHORAE_LIST = '[amphorae] RECIEVE AMPHORA LIST';
export const ERROR_AMPHORAE_LIST = '[amphorae] ERROR AMPHORA LIST';

interface ListMyCreatedAmphora extends Action {
    type: typeof LIST_MY_CREATED_AMPHORAE;
}
interface ListOrganisationCreatedAmphora extends Action {
    type: typeof LIST_ORGANISATION_CREATED_AMPHORAE;
}
interface ListMyPurchasedAmphora extends Action {
    type: typeof LIST_MY_PURCHASED_AMPHORAE;
}
interface ListOrganisationPurchasedAmphora extends Action {
    type: typeof LIST_ORGANISATION_PURCHASED_AMPHORAE;
}

export interface RecieveAmphoraListAction extends Action {
    type: typeof RECIEVE_AMPHORAE_LIST;
    payload: DetailedAmphora[];
}

export const actionCreators = {
    // listing amphora
    listMyCreatedAmphora: (): ListMyCreatedAmphora => ({ type: LIST_MY_CREATED_AMPHORAE }),
    listOrganisationCreatedAmphora: (): ListOrganisationCreatedAmphora => ({ type: LIST_ORGANISATION_CREATED_AMPHORAE }),
    listMyPurchasedAmphora: (): ListMyPurchasedAmphora => ({ type: LIST_MY_PURCHASED_AMPHORAE }),
    listOrganisationPurchasedAmphora: (): ListOrganisationPurchasedAmphora => ({ type: LIST_ORGANISATION_PURCHASED_AMPHORAE }),
    recieveList: (data: DetailedAmphora[]): RecieveAmphoraListAction => ({
        type: RECIEVE_AMPHORAE_LIST,
        payload: data
    }),

    error: (e: any): Action => ({
        type: ERROR_AMPHORAE_LIST,
    })
}

export type ListAction =
    ListMyCreatedAmphora
    | ListOrganisationCreatedAmphora
    | ListMyPurchasedAmphora
    | ListOrganisationPurchasedAmphora
    | RecieveAmphoraListAction;