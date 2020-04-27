import { IAction } from "../actions/action"

export const logger = (store: any) => (next: any) => (action: IAction) => {
    console.log('dispatching', action)
    console.log('store', store)
    let result = next(action)
    console.log('next state', store.getState())
    return result
}

export const crashReporter = (store: any) => (next: any) => (action: IAction) => {
    try {
        return next(action)
    } catch (err) {
        console.error('Caught an exception!', err)
        throw err
    }
}
