/* eslint-disable @typescript-eslint/camelcase */
import { createUserManager } from "react-amphora";
import { isLocalhost } from "./utilities";

const defaults = {
    authority: "https://identity.amphoradata.com",
    clientId: "spa",
    automaticSilentRenew: !isLocalhost,
    filterProtocolClaims: true,
    loadUserInfo: true,
    monitorSession: !isLocalhost,
};

const getAuthority = () => {
    // choose the authority based on the environment (develop, master, prod)
    if (window.location.host.includes("develop")) {
        return "https://develop.identity.amphoradata.com";
    } else if (window.location.host.includes("master")) {
        return "https://master.identity.amphoradata.com";
    } else if (window.location.host.includes("localhost")) {
        // do localhost
        return "http://localhost:6500";
    } else {
        return defaults.authority;
    }
};

const host = window.location.host;
const protocol = window.location.protocol; // protocol ends in :

const userManager = createUserManager({
    ...defaults,
    redirectUri: `${protocol}//${host}/#/callback`,
    authority: getAuthority(),
    scopes: [
        "amphora",
        "amphora.purchase",
        "openid",
        "profile",
        "web_api",
        "offline_access",
    ],
    silentRedirectUri: `${protocol}//${host}/silentRenew.html`,
});

export default userManager;
