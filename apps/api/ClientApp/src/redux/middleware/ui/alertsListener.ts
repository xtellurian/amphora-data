import { Action } from 'redux';
import { actionCreators } from '../../actions/ui';


import { CREATE_ERROR, RECIEVE_AMPHORAE_CREATED } from '../../actions/amphora/create';

export const alertsListener = (store: any) => (next: any) => (action: Action) => {
    const alertId = new Date().getTime().toString()
    next(action);
    if (action.type === CREATE_ERROR) {
        store.dispatch(actionCreators.pushAlert({ content: "Error Creating Amphora", id: alertId, level: "ERROR" }));
    } else if (action.type === RECIEVE_AMPHORAE_CREATED) {
        store.dispatch(actionCreators.pushAlert({ content: "Amphora Created!", id: alertId, level: "INFO" }));
    }
};