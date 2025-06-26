import { defineStore } from "pinia";
import { orderBy } from "lodash";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";
import { expiredMoreThan5Years } from "@/utils/functions";

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
      return expiredMoreThan5Years(state.latestCertification ?? {});
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
    otherCertifications(state): Components.Schemas.Certification[] {
      if (!state.certifications || !state.latestCertification) return [];
      return state.certifications.filter((cert) => cert.id !== state.latestCertification?.id);
    },
    hasOtherCertifications(): boolean {
      return this.otherCertifications.length > 0;
    },
    holdsEceAssistantCertification(state): boolean {
      return state.certifications?.some((cert) => cert.levels?.some((level) => level.type === "Assistant")) ?? false;
    },
    holdsEceOneYearCertification(state): boolean {
      return state.certifications?.some((cert) => cert.levels?.some((level) => level.type === "ECE 1 YR")) ?? false;
    },
    holdsEceFiveYearCertification(state): boolean {
      return state.certifications?.some((cert) => cert.levels?.some((level) => level.type === "ECE 5 YR")) ?? false;
    },
    holdsAllCertifications(state): boolean {
      if (!state.certifications || state.certifications.length === 0) return false;

      const isRenewable = (certification: Components.Schemas.Certification): boolean => {
        return certification.statusCode === "Active" || (certification.statusCode === "Expired" && !expiredMoreThan5Years(certification));
      };

      return (
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "Assistant") && isRenewable(cert)) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "ECE 1 YR") && isRenewable(cert)) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "ECE 5 YR")) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "SNE")) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "ITE"))
      );
    },

    holdsPostBasicCertification(state): boolean {
      if (!state.certifications || state.certifications.length === 0) return false;
      return (
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "ECE 5 YR")) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "SNE")) &&
        state.certifications.some((cert) => cert.levels?.some((level) => level.type === "ITE"))
      );
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
  },
  actions: {
    isEceAssistant(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "Assistant") ?? false;
    },
    isEceFiveYear(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ECE 5 YR") ?? false;
    },
    isEceOneYear(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ECE 1 YR") ?? false;
    },
    expiredMoreThan5Years(certification: Components.Schemas.Certification): boolean {
      return expiredMoreThan5Years(certification);
    },
    hasSNE(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "SNE") ?? false;
    },
    hasITE(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ITE") ?? false;
    },
    certificationTypes(certification: Components.Schemas.Certification): Components.Schemas.CertificationType[] {
      const certificationTypes = [] as Components.Schemas.CertificationType[];
      if (this.isEceAssistant(certification)) {
        certificationTypes.push("EceAssistant");
      }
      if (this.isEceOneYear(certification)) {
        certificationTypes.push("OneYear");
      }
      if (this.isEceFiveYear(certification)) {
        certificationTypes.push("FiveYears");
      }
      if (this.hasSNE(certification)) {
        certificationTypes.push("Sne");
      }
      if (this.hasITE(certification)) {
        certificationTypes.push("Ite");
      }
      return certificationTypes;
    },
    async fetchCertifications() {
      // Drop any existing certifications
      this.$reset();

      const { data: certifications } = await getCertifications();
      if (certifications?.length && certifications.length > 0) {
        this.certifications = certifications;
        this.latestCertification = getLatestCertification(certifications); //the certificate with the latest expiry date should be the latest
      }
      return certifications;
    },
  },
});

const getLatestCertification = (certifications: Components.Schemas.Certification[]): Components.Schemas.Certification => {
  //sorts certifications by status code first and then expiry date and then certificate type. Returns first one in the list which should be the latest certificate
  return orderBy(
    certifications,
    [
      ({ statusCode }) => {
        switch (statusCode) {
          case "Active":
            return 1;
          case "Cancelled":
            return 2;
          case "Suspended":
            return 3;
          case "Expired":
            return 4;
          default:
            return 5;
        }
      },
      "expiryDate",
      ({ levels }) => {
        //in case expiry date is the same, we will also rank it based on certificateType 5Y+SNE+ITE -> 5Y + SNE||ITE -> 5Y -> Assistant -> 1YR
        if (
          levels?.some((level) => level.type === "ECE 5 YR") &&
          levels?.some((level) => level.type === "ITE") &&
          levels?.some((level) => level.type === "SNE")
        ) {
          return 1;
        }

        if (
          (levels?.some((level) => level.type === "ECE 5 YR") && levels?.some((level) => level.type === "ITE")) ||
          (levels?.some((level) => level.type === "ECE 5 YR") && levels?.some((level) => level.type === "SNE"))
        ) {
          return 2;
        }

        if (levels?.some((level) => level.type === "ECE 5 YR")) {
          return 3;
        }

        if (levels?.some((level) => level.type === "Assistant")) {
          return 4;
        }

        if (levels?.some((level) => level.type === "ECE 1 YR")) {
          return 5;
        }

        console.warn(`unmapped level type ${levels}`);
        return 6;
      },
    ],
    ["asc", "desc", "asc"],
  )[0];
};
