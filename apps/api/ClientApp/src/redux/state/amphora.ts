import { DetailedAmphora } from 'amphoradata'
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface StringToEntityMap<T> {
    [id: string]: T;
}

export interface AmphoraState {
    isLoading: boolean;
    list: DetailedAmphora[];
    cache: StringToEntityMap<DetailedAmphora>;
}
