import type { SignoutResponse, User } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Authority } from "@/types/authority";

import { useConfigStore } from "./config";

export interface UserState {
  bceidUserManager: UserManager;
  bcscUserManager: UserManager;
}

export const useOidcStore = defineStore("oidc", {
  state: (): UserState => ({
    bceidUserManager: new UserManager(useConfigStore().bceidOidcConfiguration),
    bcscUserManager: new UserManager(useConfigStore().bcscOidcConfiguration),
  }),

  actions: {
    async login(authority: Authority): Promise<void> {
      return authority === "bceid" ? await this.bceidUserManager.signinRedirect() : await this.bcscUserManager.signinRedirect();
    },

    async removeUser(authority: Authority): Promise<void> {
      return authority === "bceid" ? await this.bceidUserManager.removeUser() : await this.bcscUserManager.removeUser();
    },

    async signinCallback(authority: Authority): Promise<User> {
      return authority === "bceid" ? await this.bceidUserManager.signinRedirectCallback() : await this.bcscUserManager.signinRedirectCallback();
    },

    async silentCallback(authority: Authority): Promise<void> {
      return authority === "bceid" ? await this.bceidUserManager.signinSilentCallback() : await this.bcscUserManager.signinSilentCallback();
    },

    async signinSilent(authority: Authority): Promise<User | null> {
      return authority === "bceid" ? await this.bceidUserManager.signinSilent() : await this.bcscUserManager.signinSilent();
    },

    async logout(authority: Authority): Promise<void> {
      return authority === "bceid" ? await this.bceidUserManager.signoutRedirect() : await this.bcscUserManager.signoutRedirect();
    },

    async completeLogout(authority: Authority): Promise<SignoutResponse> {
      return authority === "bceid" ? await this.bceidUserManager.signoutRedirectCallback() : await this.bcscUserManager.signoutRedirectCallback();
    },
  },
});
