import { Action } from 'redux';

export const logger = (store: any) => (next: any) => (action: Action) => {
    console.log('dispatching', action);
    console.log(`state before action ${action.type}`, store.getState());
    const result = next(action);
    console.log(`state after ${action.type}`, store.getState());
    return result;
}

export const crashReporter = (store: any) => (next: any) => (action: Action) => {
    try {
        return next(action);
    } catch (err) {
        console.error('Caught an exception!', err);
        throw err;
    }
}
