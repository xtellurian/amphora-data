import { User } from 'oidc-client';

export interface OidcState {
    isLoadingUser: boolean;
    user?: User;
}
