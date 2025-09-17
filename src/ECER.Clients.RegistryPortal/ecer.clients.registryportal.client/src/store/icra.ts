import { defineStore } from "pinia";

import { createOrUpdateDraftIcraEligibility, getIcraEligibilities } from "@/api/icra";
import type { Components } from "@/types/openapi";
import type { FileItem } from "@/components/UploadFileItem.vue";
import { humanFileSize } from "@/utils/functions";

import { useWizardStore } from "./wizard";
import type { IcraEligibilityStage } from "@/types/wizard";
import type { InternationalCertificationExtended } from "@/components/inputs/EceInternationalCertification.vue";

export interface IcraState {
  icraEligibilities: Components.Schemas.ICRAEligibility[] | null | undefined;
  draftIcraEligibility: Components.Schemas.ICRAEligibility;
  icraEligibility: Components.Schemas.ICRAEligibility | null | undefined;
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

        const draftIcraEligibility = filteredIcraEligibilities.find(
          (icraEligibility) => icraEligibility.status === "Draft" || icraEligibility.status === "Active",
        );

        if (draftIcraEligibility) {
          this.draftIcraEligibility = draftIcraEligibility;
        }
      }

      return icraEligibilities;
    },
    prepareDraftIcraEligibilityFromWizard() {
      const wizardStore = useWizardStore();
      // Get the IDs of the form inputs up front, if they are defined in the wizard config. If the ID is not defined, subsequent checks will
      // be skipped. Thus, the ID must be defined in the wizard config for the data to be set in the draft application object.

      const internationalCertificationId = wizardStore.wizardConfig?.steps?.internationalCertification?.form?.inputs?.internationalCertification?.id;
      const employmentExperienceId = wizardStore.wizardConfig?.steps?.employmentExperience?.form?.inputs?.employmentExperience?.id;

      // Set wizard stage to the current step stage
      this.draftIcraEligibility.portalStage = wizardStore.currentStepStage as IcraEligibilityStage;

      if (internationalCertificationId) {
        //remove all newFilesWithData elements and add them to newFiles as ID's
        const internationalCertificationCleaned = wizardStore.wizardData[internationalCertificationId].map((item: InternationalCertificationExtended) => {
          if (item?.newFilesWithData) {
            //we meed to convert newFilesWithData to an array of ID's for newFiles
            for (const each of item?.newFilesWithData as FileItem[]) {
              item.newFiles?.push(each.fileId);
            }
            delete item["newFilesWithData"];
          }
          return item;
        });
        this.draftIcraEligibility.internationalCertifications = internationalCertificationCleaned;
      } else {
        this.draftIcraEligibility.internationalCertifications = [];
      }

      if (employmentExperienceId) {
        console.log("employment experience TODO not implemented"); //TODO
      }
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
