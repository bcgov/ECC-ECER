import type { User, UserProfile } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Authority } from "@/types/authority";

export interface UserState {
  profile: UserProfile | null;
  accessToken: string;
  authority: Authority | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["profile", "accessToken", "authority"],
  },
  state: (): UserState => ({
    profile: null,
    accessToken: "",
    authority: null,
  }),
  getters: {
    isAuthenticated: (state) => state.accessToken !== "",
    getAccessToken: (state) => state.accessToken,
    getAuthority: (state) => state.authority,
    getProfile: (state) => state.profile,
  },
  actions: {
    setUser(user: User): void {
      this.accessToken = user.access_token;
      this.profile = user.profile;
    },
    setAuthority(authority: Authority | null): void {
      this.authority = authority;
    },
  },
});
