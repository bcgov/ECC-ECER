import { defineStore } from "pinia";
import type { Components } from "@/types/openapi";

export interface UserState {
  pspUserProfile: Components.Schemas.PspUserProfile | null;
  invitedProgramRepresentativeId: string | null;
  invitationToken: string | null;
}

export const useUserStore = defineStore("user", {
  persist: true,
  state: (): UserState => ({
    pspUserProfile: null,
    invitedProgramRepresentativeId: null,
    invitationToken: null,
  }),
  getters: {
    hasUserProfile: (state): boolean => state.pspUserProfile !== null,
    hasAcceptedTermsOfUse: (state): boolean => state.pspUserProfile?.hasAcceptedTermsOfUse ?? false,
    firstName: (state): string => state.pspUserProfile?.firstName ?? "",
    lastName: (state): string => state.pspUserProfile?.lastName ?? "",
    email: (state): string => state.pspUserProfile?.email ?? "",
    phone: (state): string => state.pspUserProfile?.phone ?? "",
    phoneExtension: (state): string => state.pspUserProfile?.phoneExtension ?? "",
    jobTitle: (state): string => state.pspUserProfile?.jobTitle ?? "",
    role: (state): Components.Schemas.PspUserRole => state.pspUserProfile?.role ?? "Primary",
    preferredName: (state): string => state.pspUserProfile?.preferredName ?? "",
  },
  actions: {
    setPspUserProfile(pspUserProfile: Components.Schemas.PspUserProfile | null): void {
      this.$patch({ pspUserProfile: pspUserProfile });
    },
    updatePspUserProfile(pspUserProfile: Components.Schemas.PspUserProfile | null): void {
      this.$patch({ pspUserProfile: { ...this.pspUserProfile, ...pspUserProfile } });
    },
    setInvitedProgramRepresentativeId(invitedProgramRepresentativeId: string | null): void {
      this.$patch({ invitedProgramRepresentativeId: invitedProgramRepresentativeId });
    },
    setInvitationToken(invitationToken: string | null): void {
      this.$patch({ invitationToken: invitationToken });
    },
  },
});
