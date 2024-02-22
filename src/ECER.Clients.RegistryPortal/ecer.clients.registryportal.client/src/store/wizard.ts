import { defineStore } from "pinia";

import type { Wizard } from "@/types/wizard";

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
  actions: {
    initializeWizard(wizard: Wizard, wizardData: WizardData): void {
      this.wizardConfig = wizard;
      this.wizardData = wizardData;
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
