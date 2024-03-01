import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";
export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  draftApplication: Components.Schemas.DraftApplication;
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => ({
    applications: [],
    draftApplication: {
      certificationTypes: [] as Components.Schemas.CertificationType[],
      id: undefined,
      signedDate: new Date().toISOString().slice(0, 10),
      stage: "CertificationType",
    },
  }),
  persist: {
    paths: ["draftApplication"],
  },
  getters: {
    hasDraftApplication(state): boolean {
      return state.draftApplication.id !== null;
    },
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
      // Load the first application as the current draft application
      if (this.applications?.length) {
        this.draftApplication = this.applications[0];
      }
    },
    async upsertDraftApplication() {
      const draftApplicationResponse = await createOrUpdateDraftApplication(this.draftApplication);
      if (draftApplicationResponse) this.draftApplication.id = draftApplicationResponse.applicationId;
    },
  },
});
