import { DetailedAmphora } from 'amphoradata'
import { StringToEntityMap } from './common';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

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
