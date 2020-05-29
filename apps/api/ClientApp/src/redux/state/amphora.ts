import { DetailedAmphora } from 'amphoradata'
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface StringToEntityMap<T> {
    [id: string]: T;
}

export interface AmphoraState {
    isLoading: boolean;
    collections?: {
        self?: {
            created?: string[];
            purchased?: string[];
        };
        organisation?: {
            created?: string[];
            purchased?: string[];
        };
    };
    cache: StringToEntityMap<DetailedAmphora>;
}
