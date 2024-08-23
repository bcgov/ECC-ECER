import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";
import type { ApplicationStage, ReferenceStage, RenewStage, Step, Wizard } from "@/types/wizard";
import { AddressType } from "@/utils/constant";

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
    currentStepStage(state): ApplicationStage | ReferenceStage | RenewStage {
      return this.steps[state.step - 1].stage;
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

        // wizard data may not have referenceList depending on the certification type. So we need to hardcode the value.
        referenceList: workReferencesDict,
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
        [wizard.steps.review.form.inputs.recaptchaToken.id]: "",
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
        [wizard.steps.review.form.inputs.confirmProvidedInformationIsRight.id]: false,
        [wizard.steps.contactInformation.form.inputs.referenceContactInformation.id]: {} as Components.Schemas.ReferenceContactInformation,
        [wizard.steps.workExperienceEvaluation.form.inputs.workExperienceEvaluation.id]: {} as Components.Schemas.WorkExperienceReferenceDetails,
        [wizard.steps.assessment.form.inputs.workExperienceAssessment.id]: {} as Components.Schemas.WorkExperienceReferenceCompetenciesAssessment,
        [wizard.steps.review.form.inputs.recaptchaToken.id]: "",
      });
    },
    async initializeWizardRenewOneYearActive(wizard: Wizard, draftApplication: Components.Schemas.DraftApplication): Promise<void> {
      const userStore = useUserStore();
      const oidcStore = useOidcStore();

      const oidcUserInfo = await oidcStore.oidcUserInfo();
      const oidcAddress = await oidcStore.oidcAddress();

      this.wizardConfig = wizard;

      // set step to the index of steps where the stage matches the draft application stage
      this.step = Object.values(wizard.steps).findIndex((step) => step.stage === draftApplication.stage) + 1;

      this.wizardData = {
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
        // Explanation Letter
        [wizard.steps.explanationLetter.form.inputs.explanationLetter.id]: draftApplication?.explanationLetter || "",
        // Character References step data
        [wizard.steps.characterReferences.form.inputs.characterReferences.id]: draftApplication?.characterReferences?.[0]
          ? draftApplication?.characterReferences
          : [],
      };
    },
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
    setCurrentStep(stage: ApplicationStage | ReferenceStage): void {
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
