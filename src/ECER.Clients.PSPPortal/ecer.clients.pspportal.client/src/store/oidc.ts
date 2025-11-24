import { type SignoutResponse, User } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";
import { parseFirstNameLastName } from "../utils/functions";

import { useConfigStore } from "./config";

export interface UserState {
  userManager: UserManager;
}

export const useOidcStore = defineStore("oidc", {
  state: (): UserState => ({
    userManager: new UserManager(useConfigStore().kcOidcConfiguration),
  }),
  actions: {
    async oidcUserInfo(): Promise<any> {
      const user = await this.getUser();

      let { firstName, lastName } = parseFirstNameLastName(user?.profile.given_name || "");
      return {
        dateOfBirth: undefined,
        firstName: firstName,
        givenName: "",
        lastName: lastName,
        phone: "",
        email: user ? (user.profile.email ?? "") : "",
        address: "",
        bceidBusinessId: user ? (user.profile.bceid_business_guid ?? "") : "",
      };
    },

    async oidcIdentityProvider(): Promise<any> {
      const user = await this.getUser();
      return user ? (user.profile.identity_provider ?? "") : "";
    },

    async getUser(): Promise<User | null> {
      return await this.userManager.getUser();
    },

    async login(provider: string, redirectTo?: string): Promise<void> {
      const params: any = { extraQueryParams: { kc_idp_hint: provider } };

      // Include redirect_to in state to round trip with auth provider
      if (redirectTo) {
        params.state = { redirect_to: redirectTo };
      }

      return await this.userManager.signinRedirect(params);
    },

    async removeUser(): Promise<void> {
      return await this.userManager.removeUser();
    },

    async signinCallback(): Promise<User> {
      return await this.userManager.signinRedirectCallback();
    },

    async silentCallback(): Promise<void> {
      return await this.userManager.signinSilentCallback();
    },

    async signinSilent(): Promise<User | null> {
      return await this.userManager.signinSilent();
    },

    async logout(): Promise<void> {
      return await this.userManager.signoutRedirect();
    },

    async completeLogout(): Promise<SignoutResponse> {
      return await this.userManager.signoutRedirectCallback();
    },
  },
});
