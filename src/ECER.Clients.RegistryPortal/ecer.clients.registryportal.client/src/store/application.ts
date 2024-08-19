import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications, submitDraftApplication } from "@/api/application";
import type { Components } from "@/types/openapi";
import type { ApplicationStage } from "@/types/wizard";

import { useWizardStore } from "./wizard";
export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  draftApplication: Components.Schemas.DraftApplication;
  application: Components.Schemas.Application | null;
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => ({
    applications: [],
    draftApplication: {
      certificationTypes: [] as Components.Schemas.CertificationType[],
      id: undefined,
      signedDate: null,
      stage: "ContactInformation",
      transcripts: [] as Components.Schemas.Transcript[],
      characterReferences: [] as Components.Schemas.CharacterReference[],
      workExperienceReferences: [] as Components.Schemas.WorkExperienceReference[],
      applicationType: "New",
      createdOn: null,
    },
    application: null,
  }),
  persist: {
    paths: ["draftApplication", "application"],
  },
  getters: {
    hasDraftApplication(state): boolean {
      return state.draftApplication.id !== undefined;
    },
    hasApplication(state): boolean {
      return state.application !== null;
    },
    applicationStatus(state): Components.Schemas.ApplicationStatus | undefined {
      return state.application?.status;
    },
    workExperienceReferenceById: (state) => {
      return (referenceId: string) => state.application?.workExperienceReferences?.find((ref) => ref.id === referenceId);
    },
    characterReferenceById: (state) => {
      return (referenceId: string) => state.application?.characterReferences?.find((ref) => ref.id === referenceId);
    },
    totalWorkExperienceHours(state): number {
      return (
        state.draftApplication.workExperienceReferences?.reduce((sum, currentReference) => {
          return sum + (currentReference.hours || 0);
        }, 0) ?? 0
      );
    },
    isDraftCertificateTypeEceAssistant(state): boolean {
      return !!state.draftApplication.certificationTypes?.includes("EceAssistant");
    },
    isDraftCertificateTypeOneYear(state): boolean {
      return !!state.draftApplication.certificationTypes?.includes("OneYear");
    },
    isDraftCertificateTypeFiveYears(state): boolean {
      return !!state.draftApplication.certificationTypes?.includes("FiveYears");
    },
    isDraftCertificateTypeIte(state): boolean {
      return !!state.draftApplication.certificationTypes?.includes("Ite");
    },
    isDraftCertificateTypeSne(state): boolean {
      return !!state.draftApplication.certificationTypes?.includes("Sne");
    },
    isDraftApplicationRenewal(state): boolean {
      return state.draftApplication.applicationType === "Renewal";
    },
  },
  actions: {
    async fetchApplications() {
      // Drop any existing draft application
      this.$reset();

      const { data: applications } = await getApplications();

      let filteredApplications = applications?.filter((application) => application.status !== "Decision");
      // Load the first application as the current draft application
      if (filteredApplications?.length && filteredApplications.length > 0) {
        this.applications = applications;
        this.application = filteredApplications[0];

        let draftApplication = filteredApplications.find((app) => app.status === "Draft");

        if (draftApplication) {
          this.draftApplication = draftApplication;
        }
      }

      return applications;
    },
    prepareDraftApplicationFromWizard() {
      const wizardStore = useWizardStore();

      // Set wizard stage to the current step stage
      this.draftApplication.stage = wizardStore.currentStepStage as ApplicationStage;

      // Education step data
      if (wizardStore.wizardConfig.steps?.education?.form.inputs.educationList.id) {
        this.draftApplication.transcripts = Object.values(wizardStore.wizardData[wizardStore.wizardConfig.steps.education.form.inputs.educationList.id]);
      }

      // Work References step data
      if (wizardStore.wizardData.referenceList) {
        this.draftApplication.workExperienceReferences = Object.values(wizardStore.wizardData.referenceList);
      }

      // Character References step data
      if (
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.firstName &&
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.lastName &&
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.emailAddress
      ) {
        this.draftApplication.characterReferences =
          wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id];
      } else if (
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.firstName === "" &&
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.lastName === "" &&
        wizardStore.wizardData[wizardStore.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.emailAddress === ""
      ) {
        this.draftApplication.characterReferences = [];
      }
    },
    async upsertDraftApplication(): Promise<Components.Schemas.DraftApplicationResponse | null | undefined> {
      const { data: draftApplicationResponse } = await createOrUpdateDraftApplication(this.draftApplication);
      if (draftApplicationResponse !== null && draftApplicationResponse !== undefined) {
        this.draftApplication.id = draftApplicationResponse.applicationId;
      }
      return draftApplicationResponse;
    },
    async submitApplication(): Promise<Components.Schemas.SubmitApplicationResponse | null | undefined> {
      const { data: submitApplicationResponse } = await submitDraftApplication(this.draftApplication.id!);
      return submitApplicationResponse;
    },
    async saveDraft(): Promise<Components.Schemas.DraftApplicationResponse | null | undefined> {
      this.prepareDraftApplicationFromWizard();
      return await this.upsertDraftApplication();
    },
    async patchDraft(draftApplication: Components.Schemas.DraftApplication): Promise<Components.Schemas.DraftApplicationResponse | null | undefined> {
      this.$patch({ draftApplication: draftApplication });
      return await this.upsertDraftApplication();
    },
  },
});
