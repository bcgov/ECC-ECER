import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications, submitDraftApplication } from "@/api/application";
import type { ProfessionalDevelopmentExtended } from "@/components/inputs/EceProfessionalDevelopment.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import applicationWizardAssistantAndOneYear from "@/config/application-wizard-assistant-and-one-year";
import applicationWizardFiveYear from "@/config/application-wizard-five-year";
import applicationWizardRenewAssistant from "@/config/application-wizard-renew-assistant";
import applicationWizardRenewFiveYearActive from "@/config/application-wizard-renew-five-year-active";
import applicationWizardRenewFiveYearExpiredLessThanFiveYears from "@/config/application-wizard-renew-five-year-expired-less-than-five-years";
import applicationWizardRenewFiveYearExpiredMoreThanFiveYears from "@/config/application-wizard-renew-five-year-expired-more-than-five-years";
import applicationWizardRenewOneYearActive from "@/config/application-wizard-renew-one-year-active";
import applicationWizardRenewOneYearExpired from "@/config/application-wizard-renew-one-year-expired";
import applicationWizardLaborMobilityAssistantAndOneYear from "@/config/application-wizard-labor-mobility-assistant-and-one-year";
import applicationWizardLaborMobilityFiveYear from "@/config/application-wizard-labor-mobility-five-year";
import type { Components } from "@/types/openapi";
import type { ApplicationStage, Wizard } from "@/types/wizard";
import { humanFileSize } from "@/utils/functions";

import { useCertificationStore } from "./certification";
import { useUserStore } from "./user";
import { useWizardStore } from "./wizard";

export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
  draftApplication: Components.Schemas.DraftApplication;
  application: Components.Schemas.Application | null;
}

export type ApplicationFlow =
  | "Assistant"
  | "OneYear"
  | "FiveYear"
  | "FiveYearWithIte"
  | "FiveYearWithSne"
  | "FiveYearWithIteAndSne"
  | "AssistantLaborMobility"
  | "OneYearLaborMobility"
  | "FiveYearLaborMobility"
  | "AssistantRenewal"
  | "OneYearActiveRenewal"
  | "OneYearExpiredRenewal"
  | "FiveYearActiveRenewal"
  | "FiveYearExpiredLessThanFiveYearsRenewal"
  | "FiveYearExpiredMoreThanFiveYearsRenewal"
  | "AssistantRegistrant"
  | "OneYearRegistrant"
  | "FiveYearRegistrant"
  | "FiveYearWithIteRegistrant"
  | "FiveYearWithSneRegistrant"
  | "FiveYearWithIteAndSneRegistrant"
  | "IteRegistrant"
  | "SneRegistrant"
  | "IteAndSneRegistrant";

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
      professionalDevelopments: [] as Components.Schemas.ProfessionalDevelopment[],
      applicationType: "New",
      createdOn: null,
    },
    application: null,
  }),
  persist: {
    pick: ["draftApplication", "application"],
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
    isDraftApplicationNew(state): boolean {
      return state.draftApplication.applicationType === "New";
    },
    isDraftApplicationLaborMobility(state): boolean {
      return state.draftApplication.applicationType === "LaborMobility";
    },
    applicationConfiguration(): Wizard {
      switch (this.draftApplicationFlow) {
        case "Assistant":
        case "AssistantRegistrant":
        case "OneYear":
        case "OneYearRegistrant":
          return applicationWizardAssistantAndOneYear;
        case "FiveYear":
        case "FiveYearWithIte":
        case "FiveYearWithSne":
        case "FiveYearWithIteAndSne":
        case "FiveYearRegistrant":
        case "FiveYearWithIteRegistrant":
        case "FiveYearWithSneRegistrant":
        case "FiveYearWithIteAndSneRegistrant":
        case "IteRegistrant":
        case "SneRegistrant":
        case "IteAndSneRegistrant":
          return applicationWizardFiveYear;
        case "AssistantRenewal":
          return applicationWizardRenewAssistant;
        case "OneYearActiveRenewal":
          return applicationWizardRenewOneYearActive;
        case "OneYearExpiredRenewal":
          return applicationWizardRenewOneYearExpired;
        case "FiveYearActiveRenewal":
          return applicationWizardRenewFiveYearActive;
        case "FiveYearExpiredLessThanFiveYearsRenewal":
          return applicationWizardRenewFiveYearExpiredLessThanFiveYears;
        case "FiveYearExpiredMoreThanFiveYearsRenewal":
          return applicationWizardRenewFiveYearExpiredMoreThanFiveYears;
        case "AssistantLaborMobility":
        case "OneYearLaborMobility":
          return applicationWizardLaborMobilityAssistantAndOneYear;
        case "FiveYearLaborMobility":
          return applicationWizardLaborMobilityFiveYear;
        default:
          return applicationWizardAssistantAndOneYear;
      }
    },
    draftApplicationFlow(state): ApplicationFlow {
      const userStore = useUserStore();
      const certificationStore = useCertificationStore();

      // RENEWAL flows
      if (state.draftApplication.applicationType === "Renewal") {
        if (this.isDraftCertificateTypeEceAssistant) return "AssistantRenewal";
        if (this.isDraftCertificateTypeOneYear) {
          if (certificationStore.latestCertificateStatus === "Active") return "OneYearActiveRenewal";
          return "OneYearExpiredRenewal";
        }
        if (this.isDraftCertificateTypeFiveYears) {
          if (certificationStore.latestCertificateStatus === "Active") return "FiveYearActiveRenewal";
          if (certificationStore.latestExpiredMoreThan5Years) return "FiveYearExpiredMoreThanFiveYearsRenewal";
          return "FiveYearExpiredLessThanFiveYearsRenewal";
        }
      }

      // REGISTRANT flows
      if (userStore.isRegistrant) {
        if (this.isDraftCertificateTypeEceAssistant) return "AssistantRegistrant";
        if (this.isDraftCertificateTypeOneYear) return "OneYearRegistrant";
        if (this.isDraftCertificateTypeFiveYears) {
          if (this.isDraftCertificateTypeIte && this.isDraftCertificateTypeSne) return "FiveYearWithIteAndSneRegistrant";
          if (this.isDraftCertificateTypeIte) return "FiveYearWithIteRegistrant";
          if (this.isDraftCertificateTypeSne) return "FiveYearWithSneRegistrant";
          return "FiveYearRegistrant";
        }
        if (this.isDraftCertificateTypeIte && this.isDraftCertificateTypeSne) return "IteAndSneRegistrant";
        if (this.isDraftCertificateTypeIte) return "IteRegistrant";
        if (this.isDraftCertificateTypeSne) return "SneRegistrant";
      }

      // NEW flows
      if (state.draftApplication.applicationType === "New") {
        if (this.isDraftCertificateTypeFiveYears && this.isDraftCertificateTypeIte && this.isDraftCertificateTypeSne) return "FiveYearWithIteAndSne";
        if (this.isDraftCertificateTypeFiveYears && this.isDraftCertificateTypeIte) return "FiveYearWithIte";
        if (this.isDraftCertificateTypeFiveYears && this.isDraftCertificateTypeSne) return "FiveYearWithSne";
        if (this.isDraftCertificateTypeFiveYears) return "FiveYear";
        if (this.isDraftCertificateTypeOneYear) return "OneYear";
        if (this.isDraftCertificateTypeEceAssistant) return "Assistant";
      }

      // LABOR MOBILITY flows
      if (state.draftApplication.applicationType === "LaborMobility") {
        if (this.isDraftCertificateTypeFiveYears) return "FiveYearLaborMobility";
        if (this.isDraftCertificateTypeOneYear) return "OneYearLaborMobility";
        if (this.isDraftCertificateTypeEceAssistant) return "AssistantLaborMobility";
      }

      return "Assistant";
    },
  },
  actions: {
    async fetchApplications() {
      // Drop any existing draft application
      this.$reset();

      const { data: applications } = await getApplications();

      const filteredApplications = applications?.filter((application) => application.status !== "Decision");
      // Load the first application as the current draft application
      if (filteredApplications?.length && filteredApplications.length > 0) {
        this.applications = applications;
        this.application = filteredApplications[0];

        const draftApplication = filteredApplications.find((app) => app.status === "Draft");

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

      // One year renewal explanation letter
      if (
        wizardStore.wizardConfig.steps?.oneYearRenewalExplanation?.form?.inputs?.oneYearRenewalExplanation?.id &&
        wizardStore.wizardConfig.steps?.oneYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id
      ) {
        this.draftApplication.oneYearRenewalExplanationChoice =
          wizardStore.wizardData[wizardStore.wizardConfig.steps.oneYearRenewalExplanation.form.inputs.oneYearRenewalExplanation.id];
        this.draftApplication.renewalExplanationOther =
          wizardStore.wizardData[wizardStore.wizardConfig.steps.oneYearRenewalExplanation.form.inputs.renewalExplanationOther.id];
      }

      // Five year renewal explanation letter
      if (
        wizardStore.wizardConfig.steps?.fiveYearRenewalExplanation?.form?.inputs?.fiveYearRenewalExplanation?.id &&
        wizardStore.wizardConfig.steps?.fiveYearRenewalExplanation?.form?.inputs?.renewalExplanationOther?.id
      ) {
        this.draftApplication.fiveYearRenewalExplanationChoice =
          wizardStore.wizardData[wizardStore.wizardConfig.steps.fiveYearRenewalExplanation.form.inputs.fiveYearRenewalExplanation.id];
        this.draftApplication.renewalExplanationOther =
          wizardStore.wizardData[wizardStore.wizardConfig.steps.fiveYearRenewalExplanation.form.inputs.renewalExplanationOther.id];
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

      if (wizardStore.wizardData.professionalDevelopments) {
        //remove all newFilesWithData elements and add them to newFiles as ID's
        const professionalDevelopmentCleaned = wizardStore.wizardData.professionalDevelopments.map((item: ProfessionalDevelopmentExtended) => {
          if (item?.newFilesWithData) {
            for (const each of item?.newFilesWithData as FileItem[]) {
              item.newFiles?.push(each.fileId);

              //we need to change wizardData to match what's been done on the server (added files)
              const addedFile: Components.Schemas.FileInfo = {
                id: each.fileId,
                size: humanFileSize(each.fileSize),
                name: each.fileName,
              };

              item.files?.push(addedFile);
            }
            delete item["newFilesWithData"];
          }

          return item;
        });

        this.draftApplication.professionalDevelopments = professionalDevelopmentCleaned;
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
