import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";

export interface UserState {
  userInfo: Components.Schemas.UserInfo | null;
  userProfile: Components.Schemas.UserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["oidcUser", "userInfo", "authority"],
  },
  state: (): UserState => ({
    userInfo: null,
    userProfile: null,
  }),
  getters: {
    hasUserInfo: (state): boolean => state.userInfo !== null,
    hasUserProfile: (state): boolean => state.userProfile !== null,
    preferredName: (state): string => state.userProfile?.preferredName ?? state.userInfo?.firstName ?? "",
    legalName: (state): string => {
      if (state.userProfile?.middleName) {
        return `${state.userProfile?.firstName} ${state.userProfile?.middleName} ${state.userProfile?.lastName}`;
      } else {
        return `${state.userProfile?.firstName} ${state.userProfile?.lastName}`;
      }
    },
    firstName: (state): string => state.userInfo?.firstName ?? "",
    fullName: (state): string => (state.userInfo?.lastName ? `${state.userInfo?.firstName} ${state.userInfo?.lastName}` : `${state.userInfo?.firstName}`),
    email: (state): string => state.userInfo?.email ?? "",
    phoneNumber: (state): string => state.userInfo?.phone ?? "",
  },
  actions: {
    setUserInfo(userInfo: Components.Schemas.UserInfo | null): void {
      this.userInfo = userInfo;
    },
    setUserProfile(userProfile: Components.Schemas.UserProfile | null): void {
      this.userProfile = userProfile;
    },
  },
});
