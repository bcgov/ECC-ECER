import { defineStore } from "pinia";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";

export interface CertificationState {
  certifications: Components.Schemas.Certification[] | null | undefined;
  latestCertification?: Components.Schemas.Certification | null;
}

export const useCertificationStore = defineStore("certification", {
  state: (): CertificationState => ({
    certifications: [],
    latestCertification: null,
  }),
  persist: true,
  getters: {
    hasCertifications(state): boolean {
      return state.certifications !== null && state.certifications !== undefined && state.certifications.length > 0;
    },
    latestHasTermsAndConditions(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.hasConditions ?? false;
    },
    latestTitleArray(state) {
      if (!state.latestCertification || !state.latestCertification.levels) return null;
      return state.latestCertification?.levels
        ?.map((level: Components.Schemas.CertificationLevel) => {
          switch (level.type) {
            case "ITE":
              return "+ Infant and Toddler";
            case "SNE":
              return "+ Special Needs Educator (SNE)";
            case "ECE 1 YR":
              return "ECE One Year";
            case "ECE 5 YR":
              return "ECE Five Year";
            case "Assistant":
              return "ECE Assistant";
            default:
              return "";
          }
        })
        .sort((a: string, b: string) => {
          // Move strings starting with '+' to the end of the array
          if (a.startsWith("+") && !b.startsWith("+")) {
            return 1;
          } else if (!a.startsWith("+") && b.startsWith("+")) {
            return -1;
          } else {
            return 0;
          }
        });
    },
  },
  actions: {
    async fetchCertifications() {
      const { data: certifications } = await getCertifications();
      if (certifications?.length && certifications.length > 0) {
        this.certifications = certifications;
        this.latestCertification = certifications[0];
      }
      return certifications;
    },
  },
});
