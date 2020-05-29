import { TermsOfUse } from 'amphoradata'
import { StringToEntityMap } from './common';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface TermsOfUseState {
    isLoading: boolean;
    termIds: string[];
    cache: StringToEntityMap<TermsOfUse>;
}
