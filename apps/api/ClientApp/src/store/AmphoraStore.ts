import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface AmphoraState {
    isLoading: boolean;
    startDateIndex?: number;
    amphoras: AmphoraModel[];
}

export interface AmphoraModel {
    id: string;
    name: string;
    price: number;
    description: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestAllAmphoraAction {
    type: 'REQUEST_ALL_AMPHORA';
    startDateIndex: number;
}

interface GetAllAmphoraAction {
    type: 'RECIEVE_ALL_AMPHORA';
    startDateIndex: number;
    amphoras: AmphoraModel[];
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestAllAmphoraAction | GetAllAmphoraAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestAllAmphora: (startDateIndex: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.amphoras && startDateIndex !== appState.amphoras.startDateIndex) {
            fetch(`api/search/amphorae`)
                .then(response => response.json() as Promise<AmphoraModel[]>)
                .then(data => {
                    dispatch({ type: 'RECIEVE_ALL_AMPHORA', startDateIndex: startDateIndex, amphoras: data });
                });

            dispatch({ type: 'REQUEST_ALL_AMPHORA', startDateIndex: startDateIndex });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: AmphoraState = { amphoras: [], isLoading: false };

export const reducer: Reducer<AmphoraState> = (state: AmphoraState | undefined, incomingAction: Action): AmphoraState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_ALL_AMPHORA':
            return {
                startDateIndex: action.startDateIndex,
                amphoras: state.amphoras,
                isLoading: true
            };
        case 'RECIEVE_ALL_AMPHORA':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.startDateIndex === state.startDateIndex) {
                return {
                    startDateIndex: action.startDateIndex,
                    amphoras: action.amphoras,
                    isLoading: false
                };
            }
            break;
    }

    return state;
};
