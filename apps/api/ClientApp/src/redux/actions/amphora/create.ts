import { Action } from 'redux';
import { DetailedAmphora, CreateAmphora } from 'amphoradata';

export const CREATE = '[amphorae] CREATE NEW';
export const RECIEVE_AMPHORAE_CREATED = '[amphorae] RECIEVE AMPHORA CREATED';
export const CREATE_ERROR = '[amphorae] AMPHORA CREATED ERROR';


export interface CreateAmphoraAction extends Action {
    type: typeof CREATE;
    payload: CreateAmphora;
}
export interface RecieveCreatedAmphoraAction extends Action {
    type: typeof RECIEVE_AMPHORAE_CREATED;
    payload: DetailedAmphora;
}
export interface CreateAmphoraFailedAction extends Action {
    type: typeof CREATE_ERROR;
    message: string;
}


export const actionCreators = {
    // creating amphora
    createNewAmphora: (a: CreateAmphora): CreateAmphoraAction => ({ type: CREATE, payload: a }),
    recieveCreated: (a: DetailedAmphora): RecieveCreatedAmphoraAction => ({ type: RECIEVE_AMPHORAE_CREATED, payload: a }),
    createdError: (m: string): CreateAmphoraFailedAction => ({ type: CREATE_ERROR, message: m })
    // listing amphora
}

export type CreateAction =
    CreateAmphoraAction