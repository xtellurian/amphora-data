/* eslint-disable @typescript-eslint/camelcase */
import { createUserManager } from 'redux-oidc';
import { UserManagerSettings } from 'oidc-client';

const userManagerConfig: UserManagerSettings = {
  client_id: 'spa',
  redirect_uri: 'https://localhost:5001/#/callback',
  response_type: 'token id_token',
  scope:"openid profile web_api offline_access",
  authority: 'http://localhost:6500',
  silent_redirect_uri: 'http://localhost:5100/silentRenew.html',
  automaticSilentRenew: true,
  filterProtocolClaims: true,
  loadUserInfo: true,
  monitorSession: true
};

const userManager = createUserManager(userManagerConfig);

export default userManager;