import { defineStore } from "pinia";

import { createDraftApplication, getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";

export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  currentApplication: {
    certificationTypes: Components.Schemas.CertificationType[];
  };
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => ({
    applications: [],
    currentApplication: {
      certificationTypes: [],
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

    async newDraftApplication(certificationTypes: Components.Schemas.CertificationType[]): Promise<string | null | undefined> {
      return await createDraftApplication(certificationTypes);
    },

    setCertificationTypes(types: Components.Schemas.CertificationType[]): void {
      this.currentApplication.certificationTypes = types;
    },
  },
});
