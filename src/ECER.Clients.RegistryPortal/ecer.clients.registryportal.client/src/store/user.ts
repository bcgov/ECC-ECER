import type { User, UserProfile } from "oidc-client-ts";
import { defineStore } from "pinia";

import { getUserInfo } from "@/api/user";
import type { Authority } from "@/types/authority";
import type { Components } from "@/types/openapi";

export interface UserState {
  profile: UserProfile | null;
  accessToken: string;
  authority: Authority | null;
  userProfile: Components.Schemas.UserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["profile", "accessToken", "authority", "userProfile"],
  },
  state: (): UserState => ({
    profile: null,
    accessToken: "",
    authority: null,
    userProfile: null,
  }),
  getters: {
    isAuthenticated: (state) => state.accessToken !== "",
    getAccessToken: (state) => state.accessToken,
    getAuthority: (state) => state.authority,
    getProfile: (state) => state.profile,
    getUserProfile: (state) => state.userProfile,
  },
  actions: {
    setUser(user: User): void {
      this.accessToken = user.access_token;
      this.profile = user.profile;
    },
    setAuthority(authority: Authority | null): void {
      this.authority = authority;
    },
    async setUserProfile(): Promise<void> {
      this.userProfile = await getUserInfo();
    },
  },
});
