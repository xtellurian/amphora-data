import { DetailedAmphora } from 'amphoradata'
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface AmphoraState {
    isLoading: boolean;
    list: DetailedAmphora[];
}
