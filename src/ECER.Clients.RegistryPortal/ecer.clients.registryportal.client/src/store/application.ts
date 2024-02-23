import { defineStore } from "pinia";

import { getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";
export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  currentApplication: Components.Schemas.DraftApplication;
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => ({
    applications: [],
    currentApplication: {
      certificationTypes: [],
      id: null,
      stage: "ContactInformation",
    },
  }),
  persist: {
    paths: ["currentApplication"],
  },
  getters: {
    inProgressCount(state): number {
      return state.applications?.length ?? 0;
    },
    completedCount(): number {
      return 0;
    },
  },
  actions: {
    async fetchApplications() {
      this.applications = await getApplications();
    },
  },
});
