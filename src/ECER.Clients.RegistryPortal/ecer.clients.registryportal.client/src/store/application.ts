import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications } from "@/api/application";
import type { Components } from "@/types/openapi";

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
      signedDate: null,
      stage: "CertificationType",
      transcripts: [] as Components.Schemas.Transcript[],
    },
  }),
  persist: {
    paths: ["draftApplication"],
  },
  getters: {
    hasDraftApplication(state): boolean {
      return state.draftApplication.id !== undefined;
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
      // Drop any existing draft application
      this.$reset();

      const { data: applications } = await getApplications();
      // Load the first application as the current draft application
      if (applications?.length) {
        this.applications = applications;
        this.draftApplication = this.applications[0];
      }
    },
    prepareDraftApplicationFromWizard() {
      const wizardStore = useWizardStore();

      // Set wizard stage to the current step stage
      this.draftApplication.stage = wizardStore.currentStepStage;

      // Certification selection step data
      this.draftApplication.certificationTypes = wizardStore.wizardData[wizardStore.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id];

      // Declaration step data
      this.draftApplication.signedDate = wizardStore.wizardData[wizardStore.wizardConfig.steps.declaration.form.inputs.consentCheckbox.id]
        ? wizardStore.wizardData[wizardStore.wizardConfig.steps.declaration.form.inputs.signedDate.id]
        : null;

      // Education step data
      this.draftApplication.transcripts = Object.values(wizardStore.wizardData[wizardStore.wizardConfig.steps.education.form.inputs.educationList.id]);
    },
    async upsertDraftApplication(): Promise<Components.Schemas.DraftApplicationResponse | null | undefined> {
      const { data: draftApplicationResponse } = await createOrUpdateDraftApplication(this.draftApplication);
      if (draftApplicationResponse !== null && draftApplicationResponse !== undefined) {
        this.draftApplication.id = draftApplicationResponse.applicationId;
      }
      return draftApplicationResponse;
    },

    async saveDraft(): Promise<Components.Schemas.DraftApplicationResponse | null | undefined> {
      this.prepareDraftApplicationFromWizard();
      return await this.upsertDraftApplication();
    },
  },
});
