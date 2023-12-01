import type { UserManagerSettings } from "oidc-client-ts";

const oidcConfig: UserManagerSettings = {
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: `${import.meta.env.VITE_BASE_URL}/callback`, // Your callback URL
  post_logout_redirect_uri: `${import.meta.env.VITE_BASE_URL}/login`, // Your logout URL
  response_type: "code",
  scope: "openid profile email",
};

export default oidcConfig;
