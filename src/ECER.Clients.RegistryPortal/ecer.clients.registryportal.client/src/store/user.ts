import type { User } from "oidc-client-ts";
import { Log, UserManager } from "oidc-client-ts";
import { defineStore } from "pinia";

import oidcConfig from "@/oidc-config";

export interface UserState {
  userInfo: User | null;
  userManager: UserManager;
}

export const useUserStore = defineStore("user", {
  state: (): UserState => {
    return {
      userInfo: null,
      userManager: new UserManager(oidcConfig),
    };
  },
  getters: {
    isAuthenticated: (state) => state.userInfo != null,
    user: (state) => state.userManager?.getUser(),
  },
  actions: {
    initializeUserManager: function () {
      this.userManager = new UserManager(oidcConfig);
      Log.setLogger(console);
    },
    async getUser() {
      this.userInfo = await this.userManager.getUser();
    },

    login: function () {
      return this.userManager.signinRedirect();
    },

    callback: function () {
      return this.userManager.signinRedirectCallback();
    },

    logout: function () {
      return this.userManager.signoutRedirect();
    },

    completeLogout: function () {
      return this.userManager.signoutRedirectCallback();
    },
  },
  persist: {
    paths: ["userInfo", "userManager"],
  },
});
