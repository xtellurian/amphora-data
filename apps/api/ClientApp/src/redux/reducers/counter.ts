import { Action, Reducer } from 'redux';
import { CounterState } from '../state/counter'

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<CounterState> = (state: CounterState | undefined, incomingAction: Action): CounterState => {
    if (state === undefined) {
        return { count: 0 };
    }

    const action = incomingAction;
    switch (action.type) {
        case 'INCREMENT_COUNT':
            return { count: state.count + 1 };
        case 'DECREMENT_COUNT':
            return { count: state.count - 1 };
        default:
            return state;
    }
};
