import { Cache } from './common';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface PermissionsState {
    isLoading: boolean;
    purchase: Cache<boolean>;
    readContents: Cache<boolean>;
    writeContents: Cache<boolean>;
}
