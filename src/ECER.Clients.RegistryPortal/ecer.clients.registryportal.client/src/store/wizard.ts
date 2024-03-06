import { defineStore } from "pinia";

import { AddressType } from "@/components/inputs/EceAddresses.vue";
import type { Components } from "@/types/openapi";
import type { Step, Wizard } from "@/types/wizard";

import { useUserStore } from "./user";

export interface WizardData {
  [key: string]: any;
}

export interface WizardState {
  step: number;
  wizardConfig: Wizard;
  wizardData: WizardData;
}

export const useWizardStore = defineStore("wizard", {
  state: (): WizardState => ({
    step: 1,
    wizardData: {} as WizardData,
    wizardConfig: {} as Wizard,
  }),
  persist: true,
  getters: {
    steps: (state) => Object.values(state.wizardConfig.steps),
    currentStep(state): Step {
      return this.steps[state.step - 1];
    },
    currentStepId(state): string {
      return this.steps[state.step - 1].id;
    },
    currentStepStage(state): Components.Schemas.PortalStage {
      return this.steps[state.step - 1].stage;
    },
  },
  actions: {
    initializeWizard(wizard: Wizard, draftApplication: Components.Schemas.DraftApplication): void {
      const userStore = useUserStore();

      this.wizardConfig = wizard;

      // set step to the index of steps where the stage matches the draft application stage
      this.step = Object.values(wizard.steps).findIndex((step) => step.stage === draftApplication.stage) + 1;

      this.wizardData = {
        // Certification Type step data
        [wizard.steps.certificationType.form.inputs.certificationSelection.id]: draftApplication.certificationTypes,

        // Declaration & Consent step data
        [wizard.steps.declaration.form.inputs.applicantLegalName.id]: userStore.fullName,
        [wizard.steps.declaration.form.inputs.signedDate.id]: draftApplication.signedDate
          ? new Date(draftApplication.signedDate).toISOString().slice(0, 10)
          : new Date().toISOString().slice(0, 10),
        [wizard.steps.declaration.form.inputs.consentCheckbox.id]: draftApplication.signedDate ? true : false,

        // Contact Information step data
        [wizard.steps.profile.form.inputs.legalLastName.id]: userStore.userProfile?.lastName || userStore.oidcUserInfo?.lastName,
        [wizard.steps.profile.form.inputs.legalFirstName.id]: userStore.userProfile?.firstName || userStore.oidcUserInfo?.firstName,
        [wizard.steps.profile.form.inputs.legalMiddleName.id]: userStore.userProfile?.middleName,
        [wizard.steps.profile.form.inputs.preferredName.id]: userStore.userProfile?.preferredName,
        [wizard.steps.profile.form.inputs.dateOfBirth.id]: userStore.userProfile?.dateOfBirth || userStore.oidcUserInfo?.dateOfBirth,
        [wizard.steps.profile.form.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userStore.userProfile?.residentialAddress || userStore.oidcAddress,
          [AddressType.MAILING]: userStore.userProfile?.mailingAddress || userStore.oidcAddress,
        },
        [wizard.steps.profile.form.inputs.primaryContactNumber.id]: userStore.userProfile?.phone || userStore.oidcUserInfo?.phone,
        [wizard.steps.profile.form.inputs.alternateContactNumber.id]: userStore.userProfile?.alternateContactPhone,
        [wizard.steps.profile.form.inputs.email.id]: userStore.userProfile?.email || userStore.oidcUserInfo?.email,
      };
    },
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
    incrementStep(): void {
      if (this.step < Object.keys(this.wizardConfig.steps).length) {
        this.step += 1;
      }
    },
    decrementStep(): void {
      if (this.step > 1) {
        this.step -= 1;
      }
    },
  },
});
