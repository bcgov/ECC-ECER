import { defineStore } from "pinia";

import type { Wizard } from "@/types/wizard";

interface WizardData {
  [key: string]: any;
}

interface WizardState {
  step: number;
  wizardConfig: Wizard;
  wizardData: WizardData;
}

export const userWizardStore = defineStore("wizard", {
  state: (): WizardState => ({
    step: 1,
    wizardData: {} as WizardData,
    wizardConfig: {} as Wizard,
  }),
  actions: {
    setWizardData(wizardData: WizardData): void {
      this.wizardData = { ...this.wizardData, ...wizardData };
    },
  },
});
