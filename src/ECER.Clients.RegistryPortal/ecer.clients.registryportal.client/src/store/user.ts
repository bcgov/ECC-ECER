import type { User } from "oidc-client-ts";
import { defineStore } from "pinia";

import { getUserInfo } from "@/api/user";
import type { Authority } from "@/types/authority";
import type { Components } from "@/types/openapi";

export interface UserState {
  user: User | null;
  accessToken: string;
  authority: Authority | null;
  userProfile: Components.Schemas.UserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["profile", "accessToken", "authority", "userProfile"],
  },
  state: (): UserState => ({
    user: null,
    accessToken: "",
    authority: null,
    userProfile: null,
  }),
  getters: {
    isAuthenticated: (state) => state.accessToken !== "",
    getAccessToken: (state) => state.accessToken,
    getAuthority: (state) => state.authority,
    getProfile: (state) => state.user,
    getUserProfile: (state) => state.userProfile,
  },
  actions: {
    setUser(user: User): void {
      this.accessToken = this.authority === "bcsc" ? user.id_token ?? "" : user.access_token;
      this.user = user;
    },
    setAuthority(authority: Authority | null): void {
      this.authority = authority;
    },
    async setUserProfile(): Promise<void> {
      //tempoary for testing purposes
      this.userProfile = await getUserInfo();
      console.debug(this.userProfile, this.user?.profile);
    },
  },
});
