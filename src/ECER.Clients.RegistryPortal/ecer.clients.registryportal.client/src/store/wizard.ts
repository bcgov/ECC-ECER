import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";
import type { ReferenceStage, Step, Wizard } from "@/types/wizard";
import { AddressType } from "@/utils/constant";
import { CertificationType } from "@/utils/constant";

import { useOidcStore } from "./oidc";
import { useUserStore } from "./user";

export interface WizardData {
  [key: string]: any;
}

export interface WizardState {
  step: number;
  wizardConfig: Wizard;
  wizardData: WizardData;
  listComponentMode: "add" | "list";
}

export type PortalStageValidation = {
  [key in Components.Schemas.PortalStage]: boolean;
};

export const useWizardStore = defineStore("wizard", {
  state: (): WizardState => ({
    step: 1,
    wizardData: {} as WizardData,
    wizardConfig: {} as Wizard,
    listComponentMode: "list",
  }),
  persist: true,
  getters: {
    steps(state): Step[] {
      return Object.values(state.wizardConfig.steps);
    },
    currentStep(state): Step {
      return this.steps[state.step - 1];
    },
    currentStepId(state): string {
      return this.steps[state.step - 1].id;
    },
    currentStepStage(state): Components.Schemas.PortalStage | ReferenceStage {
      return this.steps[state.step - 1].stage;
    },
    validationState(state): PortalStageValidation {
      let totalHours = 0;
      const references: Components.Schemas.WorkExperienceReference[] = Object.values(
        state.wizardData[this.wizardConfig.steps.workReference.form.inputs.referenceList.id],
      );
      if (references.length > 0) {
        totalHours = references.reduce((sum, currentReference) => {
          return sum + (currentReference.hours || 0);
        }, 0);
      }

      const duplicateCharacterReferenceFound = references.some((workExperienceReference: Components.Schemas.WorkExperienceReference) => {
        return (
          workExperienceReference.firstName ===
            state.wizardData?.[this.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.firstName &&
          workExperienceReference.lastName ===
            state.wizardData?.[this.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.lastName &&
          workExperienceReference.emailAddress ===
            state.wizardData?.[this.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id]?.[0]?.emailAddress
        );
      });

      let numOfEducationRequired = 1;
      state.wizardData[this.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id]?.includes(CertificationType.SNE) &&
        numOfEducationRequired++;
      state.wizardData[this.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id]?.includes(CertificationType.ITE) &&
        numOfEducationRequired++;

      return {
        CertificationType: (state.wizardData[this.wizardConfig.steps.certificationType.form.inputs.certificationSelection.id].length || []) > 0,
        Declaration:
          state.wizardData[this.wizardConfig.steps.declaration.form.inputs.signedDate.id] !== null &&
          state.wizardData[this.wizardConfig.steps.declaration.form.inputs.consentCheckbox.id] === true,
        ContactInformation: true,
        Education: Object.values(state.wizardData[this.wizardConfig.steps.education.form.inputs.educationList.id]).length >= numOfEducationRequired,
        CharacterReferences: (state.wizardData[this.wizardConfig.steps.characterReferences.form.inputs.characterReferences.id].length || []) > 0,
        WorkReferences:
          Object.values(state.wizardData[this.wizardConfig.steps.workReference.form.inputs.referenceList.id]).length > 0 &&
          totalHours >= 500 &&
          !duplicateCharacterReferenceFound,
        Review: true,
      };
    },
    allStageValidations() {
      return !(Object.values(this.validationState).indexOf(false) > -1);
    },
  },
  actions: {
    async initializeWizard(wizard: Wizard, draftApplication: Components.Schemas.DraftApplication): Promise<void> {
      const userStore = useUserStore();
      const oidcStore = useOidcStore();

      const oidcUserInfo = await oidcStore.oidcUserInfo();
      const oidcAddress = await oidcStore.oidcAddress();

      this.wizardConfig = wizard;

      // set step to the index of steps where the stage matches the draft application stage
      this.step = Object.values(wizard.steps).findIndex((step) => step.stage === draftApplication.stage) + 1;

      const transcriptsDict = {} as { [id: string]: Components.Schemas.Transcript };
      const workReferencesDict = {} as { [id: string]: Components.Schemas.WorkExperienceReference };

      // Convert array to dictionary with keys as "1", "2", ..., "n"
      if (draftApplication.transcripts) {
        draftApplication.transcripts.forEach((transcript, index) => {
          const id = (index + 1).toString();
          transcriptsDict[id] = transcript;
        });
      }
      if (draftApplication.workExperienceReferences) {
        draftApplication.workExperienceReferences.forEach((reference, index) => {
          const id = (index + 1).toString();
          workReferencesDict[id] = reference;
        });
      }

      this.wizardData = {
        // Certification Type step data
        [wizard.steps.certificationType.form.inputs.certificationSelection.id]: draftApplication.certificationTypes,

        // Declaration & Consent step data
        [wizard.steps.declaration.form.inputs.applicantLegalName.id]: userStore.fullName,
        [wizard.steps.declaration.form.inputs.signedDate.id]: draftApplication.signedDate
          ? new Date(draftApplication.signedDate).toISOString().slice(0, 10)
          : new Date().toLocaleDateString("en-CA"),
        [wizard.steps.declaration.form.inputs.consentCheckbox.id]: draftApplication.signedDate ? true : false,

        // Contact Information step data
        [wizard.steps.profile.form.inputs.legalLastName.id]: userStore.userProfile?.lastName || oidcUserInfo?.lastName,
        [wizard.steps.profile.form.inputs.legalFirstName.id]: userStore.userProfile?.firstName || oidcUserInfo?.firstName,
        [wizard.steps.profile.form.inputs.legalMiddleName.id]: userStore.userProfile?.middleName,
        [wizard.steps.profile.form.inputs.preferredName.id]: userStore.userProfile?.preferredName,
        [wizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.userProfile?.dateOfBirth || oidcUserInfo?.dateOfBirth,
        [wizard.steps.profile.form.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userStore.userProfile?.residentialAddress || oidcAddress,
          [AddressType.MAILING]: userStore.userProfile?.mailingAddress || oidcAddress,
        },
        [wizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.userProfile?.phone || oidcUserInfo?.phone,
        [wizard.steps.profile.form.inputs.alternateContactNumber.id]: userStore.userProfile?.alternateContactPhone,
        [wizard.steps.profile.form.inputs.email.id]: userStore.userProfile?.email || oidcUserInfo?.email,

        // Education step data
        [wizard.steps.education.form.inputs.educationList.id]: transcriptsDict,

        // Character References step data
        [wizard.steps.characterReferences.form.inputs.characterReferences.id]: draftApplication?.characterReferences?.[0]
          ? draftApplication?.characterReferences
          : [],

        // Work References step data
        [wizard.steps.workReference.form.inputs.referenceList.id]: workReferencesDict,
      };
    },
    initializeWizardForCharacterReference(wizard: Wizard, portalInvitation: Components.Schemas.PortalInvitation) {
      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        applicantFirstName: portalInvitation.applicantFirstName,
        applicantLastName: portalInvitation.applicantLastName,
        inviteType: portalInvitation.inviteType,
        certificationTypes: portalInvitation.certificationTypes,
        [wizard.steps.review.form.inputs.confirmProvidedInformationIsRight.id]: false,
        [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        [wizard.steps.referenceEvaluation.form.inputs.characterReferenceEvaluation.id]: {} as Components.Schemas.CharacterReferenceEvaluation,
      });
    },
    initializeWizardForWorkExReference(wizard: Wizard, portalInvitation: Components.Schemas.PortalInvitation) {
      this.$reset();
      this.wizardConfig = wizard;

      this.setWizardData({
        applicantFirstName: portalInvitation.applicantFirstName,
        applicantLastName: portalInvitation.applicantLastName,
        inviteType: portalInvitation.inviteType,
        certificationTypes: portalInvitation.certificationTypes,
        workExperienceReferenceHours: portalInvitation.workExperienceReferenceHours,
        [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        [wizard.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]: {} as Components.Schemas.WorkExperienceReferenceDetails,
        [wizard.steps.assessment.form.inputs.workExperienceAssessment.id]: {} as Components.Schemas.WorkExperienceReferenceCompetenciesAssessment,
      });
    },
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
    setCurrentStep(stage: Components.Schemas.PortalStage | ReferenceStage): void {
      const item = Object.values(this.wizardConfig.steps).findIndex((step) => step.stage === stage) + 1;
      this.step = item;
    },
    incrementStep(): void {
      if (this.step < Object.keys(this.wizardConfig.steps).length) {
        this.step += 1;
        window.scrollTo(0, 0);
      }
    },
    decrementStep(): void {
      if (this.step > 1) {
        this.step -= 1;
        window.scrollTo(0, 0);
      }
    },
    setStep(step: number): void {
      this.step = step;
      window.scrollTo(0, 0);
    },
  },
});
