import { defineStore } from "pinia";

import { createOrUpdateDraftIcraEligibility, getIcraEligibilities } from "@/api/icra";
import type { Components } from "@/types/openapi";

import { useWizardStore } from "./wizard";

export interface IcraState {
  icraEligibilities: Components.Schemas.ICRAEligibility[] | null | undefined;
  draftIcraEligibility: Components.Schemas.ICRAEligibility;
  icraEligibility: Components.Schemas.ICRAEligibility | null;
}

export const useIcraStore = defineStore("icra", {
  state: (): IcraState => ({
    icraEligibilities: [],
    draftIcraEligibility: {
      id: undefined,
      portalStage: "ContactInformation",
    },
    icraEligibility: null,
  }),
  persist: {
    pick: ["draftIcraEligibility", "icraEligibility"],
  },
  getters: {
    hasDraftIcraEligibility(state): boolean {
      return state.draftIcraEligibility.id !== undefined;
    },
    hasIcraEligibility(state): boolean {
      return state.icraEligibility !== null;
    },
    icraEligibilityStatus(state): Components.Schemas.ICRAStatus | undefined {
      return state.icraEligibility?.status;
    },
  },
  actions: {
    async fetchIcraEligibilities() {
      // Drop any existing draft icra eligibility
      this.$reset();

      const { data: icraEligibilities } = await getIcraEligibilities();

      const filteredIcraEligibilities = icraEligibilities?.filter(
        (icraEligibility) => icraEligibility.status !== "Eligible" && icraEligibility.status !== "Ineligible" && icraEligibility.status !== "Inactive",
      );
      // Load the first icra eligibility as the current draft icra eligibility
      if (filteredIcraEligibilities?.length && filteredIcraEligibilities.length > 0) {
        this.icraEligibilities = icraEligibilities;
        this.icraEligibility = filteredIcraEligibilities[0];

        const draftIcraEligibility = filteredIcraEligibilities.find((icraEligibility) => icraEligibility.status === "Draft");

        if (draftIcraEligibility) {
          this.draftIcraEligibility = draftIcraEligibility;
        }
      }

      return icraEligibilities;
    },
    prepareDraftIcraEligibilityFromWizard() {
      const wizardStore = useWizardStore();
      //  TODO: Implement
    },
    async upsertDraftIcraEligibility(): Promise<Components.Schemas.DraftICRAEligibilityResponse | null | undefined> {
      const { data: draftIcraEligibilityResponse } = await createOrUpdateDraftIcraEligibility(this.draftIcraEligibility);
      if (draftIcraEligibilityResponse !== null && draftIcraEligibilityResponse !== undefined) {
        this.draftIcraEligibility = draftIcraEligibilityResponse.eligibility!;
      }
      return draftIcraEligibilityResponse;
    },
    resetDraftIcraEligibility(): void {
      this.draftIcraEligibility = {
        id: undefined,
        portalStage: "ContactInformation",
        status: "Draft",
      };
    },
    async saveDraft(): Promise<Components.Schemas.DraftICRAEligibilityResponse | null | undefined> {
      this.prepareDraftIcraEligibilityFromWizard();
      return await this.upsertDraftIcraEligibility();
    },
    async patchDraft(draftIcraEligibility: Components.Schemas.ICRAEligibility): Promise<Components.Schemas.DraftICRAEligibilityResponse | null | undefined> {
      this.$patch({ draftIcraEligibility: draftIcraEligibility });
      return await this.upsertDraftIcraEligibility();
    },
  },
});
