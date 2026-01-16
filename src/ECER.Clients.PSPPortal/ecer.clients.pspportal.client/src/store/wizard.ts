import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";
import type { ProgramStage, Step, Wizard } from "@/types/wizard";

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
      const currentStep = this.steps[state.step - 1];
      if (!currentStep) throw new Error("No current step found");
      return currentStep;
    },
    currentStepId(state): string {
      const stepId = this.steps[state.step - 1]?.id;
      if (!stepId) throw new Error("No current step id found");
      return stepId;
    },
    currentStepStage(state): ProgramStage {
      console.log(this.steps);
      console.log(state);
      const stage = this.steps[state.step - 1]?.stage;
      if (!stage) throw new Error("No current step stage found");
      return stage;
    },
    hasStep() {
      return (step: ProgramStage) => {
        return this.steps.some((s) => s.stage === step);
      };
    },
  },
  actions: {
    async initializeWizard(
      wizard: Wizard,
      draftApplication: Components.Schemas.Program,
    ): Promise<void> {
      const userStore = useUserStore();
      const oidcStore = useOidcStore();

      // Get all data sources
      const oidcUserInfo = await oidcStore.oidcUserInfo();

      // Store the config
      this.wizardConfig = wizard;

      // Set the current step based on draft application stage
      this.step =
        Object.values(wizard.steps).findIndex(
          (step) => step.stage === draftApplication.portalStage,
        ) + 1;

      // Prepare data sources object
      const dataSources = {
        userProfile: userStore.pspUserProfile,
        oidcUserInfo,
        draftApplication,
      };

      // Build wizardData by iterating through all inputs in all steps
      const wizardData: WizardData = {};

      for (const step of Object.values(wizard.steps)) {
        for (const component of Object.values(step.form?.components)) {
          // If input has a getValue function, use it to populate wizardData
          if (component.getValue) {
            const value = await Promise.resolve(
              component.getValue(dataSources),
            );
            if (value !== undefined) {
              wizardData[component.id] = value;
            }
          }
        }
      }

      this.wizardData = wizardData;
    },
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
    setCurrentStep(stage: ProgramStage): void {
      this.step =
        Object.values(this.wizardConfig.steps).findIndex(
          (step) => step.stage === stage,
        ) + 1;
      globalThis.scrollTo({
        top: 0,
        behavior: "smooth",
      });
    },
    incrementStep(): void {
      if (this.step < Object.keys(this.wizardConfig.steps).length) {
        this.step += 1;
        globalThis.scrollTo(0, 0);
      }
    },
    decrementStep(): void {
      if (this.step > 1) {
        this.step -= 1;
        globalThis.scrollTo(0, 0);
      }
    },
    setStep(step: number): void {
      this.step = step;
      globalThis.scrollTo(0, 0);
    },
  },
});
