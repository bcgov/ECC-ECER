import type { SignoutResponse, User, UserProfile } from "oidc-client-ts";
import { UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import { useConfigStore } from "./config";

export interface UserState {
  profile: UserProfile | null;
  userManager: UserManager;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["profile"],
  },
  state: (): UserState => ({
    profile: null,
    userManager: new UserManager(useConfigStore().bcscOidcConfiguration),
  }),
  getters: {
    isAuthenticated: (state) => state.profile !== null,
  },
  actions: {
    clearProfile(): void {
      this.profile = null;
    },

    setProfile(user: UserProfile | null): void {
      this.profile = user;
    },

    async getAccessToken(): Promise<string | null> {
      const user = await this.userManager.getUser();
      return user?.access_token ?? null;
    },

    async getRefreshToken(): Promise<string | null> {
      const user = await this.userManager.getUser();
      return user?.access_token ?? null;
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
