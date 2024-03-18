import type { SignoutResponse, User } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Authority } from "@/types/authority";

import { useConfigStore } from "./config";

export interface UserState {
  userManagers: Record<Authority, { manager: UserManager }>;
}

export const useOidcStore = defineStore("oidc", {
  state: (): UserState => ({
    userManagers: {
      bcsc: { manager: new UserManager(useConfigStore().bcscOidcConfiguration) },
      bceid: { manager: new UserManager(useConfigStore().bceidOidcConfiguration) },
      kc: { manager: new UserManager(useConfigStore().kcOidcConfiguration) },
    },
  }),

  actions: {
    async login(authority: Authority, provider: string): Promise<void> {
      return await this.userManagers[authority].manager.signinRedirect({ extraQueryParams: { kc_idp_hint: provider } });
    },

    async removeUser(authority: Authority): Promise<void> {
      return await this.userManagers[authority].manager.removeUser();
    },

    async signinCallback(authority: Authority): Promise<User> {
      return await this.userManagers[authority].manager.signinRedirectCallback();
    },

    async silentCallback(authority: Authority): Promise<void> {
      return await this.userManagers[authority].manager.signinSilentCallback();
    },

    async signinSilent(authority: Authority): Promise<User | null> {
      return await this.userManagers[authority].manager.signinSilent();
    },

    async logout(authority: Authority): Promise<void> {
      return await this.userManagers[authority].manager.signoutRedirect();
    },

    async completeLogout(authority: Authority): Promise<SignoutResponse> {
      return await this.userManagers[authority].manager.signoutRedirectCallback();
    },
  },
});
