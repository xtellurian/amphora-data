import { TermsOfUse } from 'amphoradata'
import { StringToEntityMap, StateCachedFromServer } from './common';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface TermsOfUseState extends StateCachedFromServer {
    isLoading: boolean;
    termIds: string[];
    cache: StringToEntityMap<TermsOfUse>;
}
