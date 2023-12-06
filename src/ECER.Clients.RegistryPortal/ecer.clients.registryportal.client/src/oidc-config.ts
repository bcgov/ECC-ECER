import type { UserManagerSettings } from "oidc-client-ts";

// Pick specific fields from UserManagerSettings to create PartialUserManagerSettings
type BaseUserManagerSettings = Pick<
  UserManagerSettings,
  "redirect_uri" | "post_logout_redirect_uri" | "silent_redirect_uri" | "response_type"
>;

// Create an instance of the partial type using oidcConfig
const oidcConfig: BaseUserManagerSettings = {
  redirect_uri: `${window.location.origin}/signin-callback`,
  post_logout_redirect_uri: `${window.location.origin}/logout-callback`,
  silent_redirect_uri: `${window.location.origin}/silent-renew`,
  response_type: "code",
};

export default oidcConfig;
