import { DateTime } from "luxon";
import { defineStore } from "pinia";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";
import { sortArrayDateDescending } from "@/utils/functions";

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
    latestCertificateStatus(state): Components.Schemas.CertificateStatusCode | undefined {
      return state.latestCertification?.statusCode;
    },
    latestExpiredMoreThan5Years(state): boolean {
      if (!state.latestCertification?.expiryDate) return false;
      const dt1 = DateTime.now();
      const dt2 = DateTime.fromISO(state.latestCertification?.expiryDate);
      const differenceInYears = Math.abs(dt1.diff(dt2, "years").years);
      return differenceInYears > 5;
    },
    latestCertificationExpiryDate(state): string | null | undefined {
      return state.latestCertification?.expiryDate;
    },
    latestIsEceAssistant(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.levels?.some((level) => level.type === "Assistant") ?? false;
    },
    latestIsEceFiveYear(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.levels?.some((level) => level.type === "ECE 5 YR") ?? false;
    },
    latestIsEceOneYear(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.levels?.some((level) => level.type === "ECE 1 YR") ?? false;
    },
    latestHasSNE(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.levels?.some((level) => level.type === "SNE") ?? false;
    },
    latestHasITE(state): boolean {
      if (!state.latestCertification) return false;
      return state.latestCertification.levels?.some((level) => level.type === "ITE") ?? false;
    },
    latestCertificationTypes(): Components.Schemas.CertificationType[] {
      const certificationTypes = [] as Components.Schemas.CertificationType[];
      if (this.latestIsEceAssistant) {
        certificationTypes.push("EceAssistant");
      }
      if (this.latestIsEceOneYear) {
        certificationTypes.push("OneYear");
      }
      if (this.latestIsEceFiveYear) {
        certificationTypes.push("FiveYears");
      }
      if (this.latestHasSNE) {
        certificationTypes.push("Sne");
      }
      if (this.latestHasITE) {
        certificationTypes.push("Ite");
      }
      return certificationTypes;
    },
    hasMultipleEceOneYearCertifications(state): boolean {
      let count = 0;
      if (!state.certifications || state.certifications?.length < 2) return false;
      for (const cert of state.certifications) {
        if (cert.levels?.some((level) => level.type === "ECE 1 YR")) {
          count++;
        }
      }
      return count >= 2;
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
      // Drop any existing certifications
      this.$reset();

      const { data: certifications } = await getCertifications();
      if (certifications?.length && certifications.length > 0) {
        this.certifications = certifications;
        this.latestCertification = certifications.sort((a, b) => sortArrayDateDescending(a, b, "expiryDate"))[0]; //the certificate with the latest expiry date should be the latest
      }
      return certifications;
    },
  },
});
