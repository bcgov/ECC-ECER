import { type UserManagerSettings, WebStorageStateStore } from "oidc-client-ts";

const oidcConfig: UserManagerSettings = {
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: `${window.location.origin}/signin-callback`, // Your callback URL
  post_logout_redirect_uri: `${window.location.origin}/logout-callback`, // Your logout URL
  silent_redirect_uri: `${window.location.origin}/silent-renew`, // Your silent renew URL
  response_type: "code",
  scope: "openid profile email",
};

export default oidcConfig;
