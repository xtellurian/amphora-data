import * as React from 'react';
import { Action } from 'redux';
import { success, info, warning, error } from '../../../components/molecules/toasts';
import { toast } from 'react-toastify';

import { CREATE_ERROR, RECIEVE_AMPHORAE_CREATED, CreateAmphoraFailedAction, RecieveCreatedAmphoraAction } from '../../actions/amphora/create';


export const toastsListener = (store: any) => (next: any) => (action: Action) => {
    const alertId = new Date().getTime().toString()
    next(action);

    if (action.type === CREATE_ERROR) {
        const createErrorAction = action as CreateAmphoraFailedAction;

        toast(error({ text: `Error Creating Amphora. ${createErrorAction.message}` }), {
            className: "toast-error",
        })
    } else if (action.type === RECIEVE_AMPHORAE_CREATED) {
        const createdAction = action as RecieveCreatedAmphoraAction;
        toast(success({
            text: "Amphora Created!",
            path: `/amphora/detail/${createdAction.payload.id}`
        }), {
            className: "toast-success",
            autoClose: 3000
        });
    };
}