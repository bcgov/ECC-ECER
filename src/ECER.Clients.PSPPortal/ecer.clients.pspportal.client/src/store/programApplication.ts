import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";

export interface ProgramApplicationState {
  programApplication: Components.Schemas.ProgramApplication | null;
}

export const useProgramApplicationStore = defineStore("programApplication", {
  state: (): ProgramApplicationState => ({
    programApplication: null,
  }),
  persist: true,
  getters: {},
  actions: {
    setProgramApplication(
      programApplication: Components.Schemas.ProgramApplication,
    ): void {
      this.$patch({ programApplication: programApplication });
    },
  },
});
