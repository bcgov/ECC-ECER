import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";

import { useCertificationTypeStore } from "./certificationType";
import { useWizardStore } from "./wizard";
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
    prepareDraftApplicationFromWizard() {
      const wizardStore = useWizardStore();
      const certificationTypeStore = useCertificationTypeStore();

      this.draftApplication.stage = wizardStore.currentStepStage;

      this.draftApplication.certificationTypes = certificationTypeStore.certificationTypes;

      this.draftApplication.signedDate = wizardStore.wizardData[wizardStore.wizardConfig.steps.declaration.form.inputs.signedDate.id];
    },
    async upsertDraftApplication() {
      const draftApplicationResponse = await createOrUpdateDraftApplication(this.draftApplication);
      if (draftApplicationResponse) this.draftApplication.id = draftApplicationResponse.applicationId;
    },
  },
});
