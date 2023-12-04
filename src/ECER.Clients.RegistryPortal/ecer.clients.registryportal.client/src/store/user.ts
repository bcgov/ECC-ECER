import type { SignoutResponse, User } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import oidcConfig from "@/oidc-config";

export interface UserState {
  userInfo: User | null;
  userManager: UserManager;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["userInfo"],
  },
  state: (): UserState => ({
    userInfo: null,
    userManager: new UserManager(oidcConfig),
  }),
  getters: {
    isAuthenticated: (state) => state.userInfo !== null,
  },
  actions: {
    initializeUserManager: function () {
      this.userManager = new UserManager(oidcConfig);
    },

    clearUser(): void {
      this.userInfo = null;
    },

    setUser(user: User | null): void {
      this.userInfo = user;
    },

    async login(): Promise<void> {
      return await this.userManager.signinRedirect();
    },

    async signinCallback(): Promise<User> {
      return await this.userManager.signinRedirectCallback();
    },

    async silentCallback(): Promise<void> {
      return this.userManager.signinSilentCallback();
    },

    async signinSilent(): Promise<User | null> {
      return await this.userManager.signinSilent();
    },

    async logout(): Promise<void> {
      return this.userManager.signoutRedirect();
    },

    async completeLogout(): Promise<SignoutResponse> {
      return this.userManager.signoutRedirectCallback();
    },
  },
});
