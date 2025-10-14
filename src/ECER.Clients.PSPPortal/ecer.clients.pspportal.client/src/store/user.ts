import { defineStore } from "pinia";
import type { Components } from "@/types/openapi";

export interface UserState {
  pspUserProfile: Components.Schemas.PspUserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: true,
  state: (): UserState => ({
    pspUserProfile: null,
  }),
  getters: {
    hasUserProfile: (state): boolean => state.pspUserProfile !== null,
    email: (state): string => state.pspUserProfile?.email ?? "",
    bceidBusinessId: (state): string => state.pspUserProfile?.bceidBusinessId ?? "",
  },
  actions: {
    setPspUserProfile(pspUserProfile: Components.Schemas.PspUserProfile | null): void {
      this.$patch({ pspUserProfile: pspUserProfile });
    },
  },
});
