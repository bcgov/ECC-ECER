import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";

export interface Application {
  certificationTypes: Components.Schemas.CertificationType[];
  Id: string | null | undefined;
}
export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  currentApplication: Application;
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => ({
    applications: [],
    currentApplication: {
      certificationTypes: [],
      Id: null,
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

    async createOrUpdateDraftApplication(application: Application): Promise<string | null | undefined> {
      const applicationId = await createOrUpdateDraftApplication(application);
      this.currentApplication = application;
      this.currentApplication.Id = applicationId;
      return applicationId;
    },
  },
});
