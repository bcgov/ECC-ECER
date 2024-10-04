import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getApplications, submitDraftApplication } from "@/api/application";
import type { ProfessionalDevelopmentExtended } from "@/components/inputs/EceProfessionalDevelopment.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import applicationWizardRenewAssistant from "@/config/application-wizard-renew-assistant";
import type { Components } from "@/types/openapi";
import type { ApplicationStage, Wizard } from "@/types/wizard";
import { humanFileSize } from "@/utils/functions";

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
  | "AssistantRenewal"
  | "OneYearRenewal"
  | "FiveYearRenewal"
  | "FiveYearWithIteRenewal"
  | "FiveYearWithSneRenewal"
  | "FiveYearWithIteAndSneRenewal"
  | "AssistantRegistrant"
  | "OneYearRegistrant"
  | "FiveYearRegistrant"
  | "FiveYearWithIteRegistrant"
  | "FiveYearWithSneRegistrant"
  | "FiveYearWithIteAndSneRegistrant"
  | "IteRegistrant"
  | "SneRegistrant"
  | "IteAndSneRegistrant"
  | "default";

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
    draftApplicationConfiguration(state): Wizard {
      // TODO: Implement the logic to return the correct wizard configuration based on the draft application flow

      // const certificationStore = useCertificationStore();
      // const userStore = useUserStore();

      switch (this.draftApplicationFlow) {
        case "Assistant":
        case "OneYear":
        case "FiveYear":
        case "FiveYearWithIte":
        case "FiveYearWithSne":
        case "FiveYearWithIteAndSne":
        case "AssistantRenewal":
        case "OneYearRenewal":
        case "FiveYearRenewal":
        case "FiveYearWithIteRenewal":
        case "FiveYearWithSneRenewal":
        case "FiveYearWithIteAndSneRenewal":
        case "AssistantRegistrant":
        case "OneYearRegistrant":
        case "FiveYearRegistrant":
        case "FiveYearWithIteRegistrant":
        case "FiveYearWithSneRegistrant":
        case "FiveYearWithIteAndSneRegistrant":
        case "IteRegistrant":
        case "SneRegistrant":
        case "IteAndSneRegistrant":
          return applicationWizardRenewAssistant;
        default:
          return applicationWizardRenewAssistant;
      }
    },
    draftApplicationFlow(state): ApplicationFlow {
      // TODO: Implement the logic to return all possible application flows

      //renewal flows
      if (state.draftApplication.applicationType === "Renewal" && state.draftApplication.certificationTypes?.includes("EceAssistant")) {
        return "AssistantRenewal";
      }

      //new application flows
      if (
        state.draftApplication.certificationTypes?.includes("FiveYears") &&
        state.draftApplication.certificationTypes?.includes("Ite") &&
        state.draftApplication.certificationTypes?.includes("Sne")
      ) {
        return "FiveYearWithIteAndSne";
      } else if (state.draftApplication.certificationTypes?.includes("FiveYears") && state.draftApplication.certificationTypes?.includes("Ite")) {
        return "FiveYearWithIte";
      } else if (state.draftApplication.certificationTypes?.includes("FiveYears") && state.draftApplication.certificationTypes?.includes("Sne")) {
        return "FiveYearWithSne";
      } else if (state.draftApplication.certificationTypes?.includes("FiveYears")) {
        return "FiveYear";
      } else if (state.draftApplication.certificationTypes?.includes("Ite") && state.draftApplication.certificationTypes?.includes("Sne")) {
        return "IteAndSneRegistrant";
      } else if (state.draftApplication.certificationTypes?.includes("Ite")) {
        return "IteRegistrant";
      } else if (state.draftApplication.certificationTypes?.includes("Sne")) {
        return "SneRegistrant";
      } else if (state.draftApplication.certificationTypes?.includes("EceAssistant")) {
        return "Assistant";
      } else if (state.draftApplication.certificationTypes?.includes("OneYear")) {
        return "OneYear";
      }

      return "default";
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
