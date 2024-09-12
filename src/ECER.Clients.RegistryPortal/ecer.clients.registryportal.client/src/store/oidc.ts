import { type SignoutResponse, User } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";

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

      return {
        dateOfBirth: user ? user.profile.birthdate ?? undefined : undefined,
        firstName: user ? user.profile.given_name ?? "" : "",
        lastName: user ? user.profile.family_name ?? "" : "",
        phone: user ? user.profile.phone_number ?? "" : "",
        email: user ? user.profile.email ?? "" : "",
        address: user ? user.profile.address ?? "" : "",
      };
    },
    async oidcAddress(): Promise<Components.Schemas.Address> {
      const user = await this.getUser();
      return {
        line1: user ? user.profile.address?.street_address ?? "" : "",
        line2: "",
        city: user ? user.profile.address?.locality ?? "" : "",
        province: user ? user.profile.address?.region ?? "" : "",
        country: user ? user.profile.address?.country ?? "" : "",
        postalCode: user ? user.profile.address?.postal_code ?? "" : "",
      };
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
