/* eslint-disable @typescript-eslint/camelcase */
import { createUserManager } from 'redux-oidc';
import { UserManagerSettings } from 'oidc-client';


const settings = {
  authority: 'http://localhost:6500',
  redirect_uri: 'https://localhost:5001/#/callback',
  silent_redirect_uri: 'http://localhost:5001/silentRenew.html',
  client_id: 'spa',
  response_type: 'token id_token',
  scope: "openid profile web_api offline_access",
  automaticSilentRenew: true,
  filterProtocolClaims: true,
  loadUserInfo: true,
  monitorSession: true
};


function getSettings(): UserManagerSettings {

  const host = window.location.host;
  const protocol = window.location.protocol;

  if (window.location.host.includes("develop")) {
    settings.authority = "https://develop.identity.amphoradata.com";
    settings.redirect_uri = `${protocol}://${host}/#/callback`
    settings.silent_redirect_uri = `${protocol}://${host}/silentRenew.html`
  }

  console.log(settings)
  return settings;
}

const userManager = createUserManager(getSettings());

export default userManager;