import { defineStore } from "pinia";
import { DateTime } from "luxon";
import type { Components } from "@/types/openapi";

export interface UserState {
  userInfo: Components.Schemas.UserInfo | null;
  userProfile: Components.Schemas.UserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: true,
  state: (): UserState => ({
    userInfo: null,
    userProfile: null,
  }),
  getters: {
    hasUserInfo: (state): boolean => state.userInfo !== null,
    hasUserProfile: (state): boolean => state.userProfile !== null,
    preferredName: (state): string =>
      state.userProfile?.preferredName ?? state.userInfo?.firstName ?? "",
    legalName: (state): string => {
      return `${state.userProfile?.firstName ?? ""} ${state.userProfile?.middleName ?? ""} ${state.userProfile?.lastName}`.trim();
    },
    unverifiedPreviousNames: (state): Components.Schemas.PreviousName[] => {
      return (
        state.userProfile?.previousNames?.filter(
          (name) => name.status === "Unverified",
        ) ?? []
      );
    },
    readyForVerificationPreviousNames: (
      state,
    ): Components.Schemas.PreviousName[] => {
      return (
        state.userProfile?.previousNames?.filter(
          (name) => name.status === "ReadyforVerification",
        ) ?? []
      );
    },
    verifiedPreviousNames: (state): Components.Schemas.PreviousName[] => {
      return (
        state.userProfile?.previousNames?.filter(
          (name) => name.status === "Verified",
        ) ?? []
      );
    },
    pendingforDocumentsPreviousNames: (
      state,
    ): Components.Schemas.PreviousName[] => {
      return (
        state.userProfile?.previousNames?.filter(
          (name) => name.status === "PendingforDocuments",
        ) ?? []
      );
    },
    firstName: (state): string => state.userInfo?.firstName ?? "",
    middleName: (state): string => state.userInfo?.middleName ?? "",
    lastName: (state): string => state.userInfo?.lastName ?? "",
    fullName: (state): string =>
      `${state.userInfo?.firstName ?? ""} ${state.userInfo?.middleName ?? ""} ${state.userInfo?.lastName}`.trim(),
    email: (state): string => state.userInfo?.email ?? "",
    phoneNumber: (state): string => state.userProfile?.phone ?? "",
    isRegistrant: (state): boolean => state.userInfo?.isRegistrant ?? false,
    isVerified: (state): boolean => state.userInfo?.status === "Verified",
    isUnder19: (state): boolean => {
      const dateOfBirth = state.userInfo?.dateOfBirth;
      if (!dateOfBirth) return false;

      const birthDate = DateTime.fromISO(dateOfBirth);
      const today = DateTime.now();
      const age = today.diff(birthDate, "years").years;

      return age < 19;
    },
  },
  actions: {
    setUserInfo(userInfo: Components.Schemas.UserInfo | null): void {
      this.$patch({ userInfo: userInfo });
    },
    setUserProfile(userProfile: Components.Schemas.UserProfile | null): void {
      this.$patch({ userProfile: userProfile });
    },
  },
});
