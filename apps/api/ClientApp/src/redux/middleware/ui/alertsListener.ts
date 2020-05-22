import { Action } from 'redux';
import { actionCreators } from '../../actions/ui';

import { CREATE_ERROR, RECIEVE_AMPHORAE_CREATED, CreateAmphoraFailedAction, RecieveCreatedAmphoraAction } from '../../actions/amphora/create';


export const alertsListener = (store: any) => (next: any) => (action: Action) => {
    const alertId = new Date().getTime().toString()
    next(action);
    if (action.type === CREATE_ERROR) {
        const createErrorAction = action as CreateAmphoraFailedAction;
        store.dispatch(actionCreators.pushAlert({ content: `Error Creating Amphora. ${createErrorAction.message}`, id: alertId, level: "ERROR" }));
    } else if (action.type === RECIEVE_AMPHORAE_CREATED) {
        const createdAction = action as RecieveCreatedAmphoraAction;
        store.dispatch(actionCreators.pushAlert({ 
            content: "Amphora Created!", 
            path: `/amphora/detail/${createdAction.payload.id}`, 
            id: alertId, 
            level: "SUCCESS" }));
    }
};