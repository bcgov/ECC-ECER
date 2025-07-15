import { defineStore } from "pinia";
import { orderBy } from "lodash";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";
import { expiredMoreThan5Years } from "@/utils/functions";

export interface CertificationState {
  certifications: Components.Schemas.Certification[] | null | undefined;
}

export const useCertificationStore = defineStore("certification", {
  state: (): CertificationState => ({
    certifications: [],
  }),
  persist: true,
  getters: {
    hasCertifications(state): boolean {
      return state.certifications !== null && state.certifications !== undefined && state.certifications.length > 0;
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
    activeEceFiveYearCertification(state): Components.Schemas.Certification | null {
      return state.certifications?.find((cert) => cert.levels?.some((level) => level.type === "ECE 5 YR") && cert.statusCode === "Active") ?? null;
    },
    holdsAllCertifications(state): boolean {
      if (!state.certifications || state.certifications.length === 0) return false;

      const isRenewable = (certification: Components.Schemas.Certification): boolean => {
        return (
          certification.statusCode === "Active" ||
          ((certification.statusCode === "Expired" || certification.statusCode === "Suspended") && !expiredMoreThan5Years(certification))
        );
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
    currentCertification(state): Components.Schemas.Certification | null {
      if (!state.certifications || state.certifications.length === 0) {
        return null;
      }

      //sorts certifications by status code first and then expiry date and then certificate type. Returns first one in the list which should be the latest certificate
      return orderBy(
        state.certifications,
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
    },
  },
  actions: {
    getCertificationById(certificateId: string | null | undefined): Components.Schemas.Certification | undefined {
      return this.certifications?.find((cert) => cert.id === certificateId);
    },
    certificateStatus(certificateId: string | null | undefined): Components.Schemas.CertificateStatusCode | undefined {
      const certification = this.getCertificationById(certificateId);
      return certification?.statusCode;
    },
    expiredMoreThan5Years(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      return expiredMoreThan5Years(certification ?? {});
    },
    certificationExpiryDate(certificateId: string | null | undefined): string | null | undefined {
      const certification = this.getCertificationById(certificateId);
      return certification?.expiryDate;
    },
    certificationEffectiveDate(certificateId: string | null | undefined): string | null | undefined {
      const certification = this.getCertificationById(certificateId);
      return certification?.effectiveDate;
    },
    isEceAssistant(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return false;
      return certification.levels?.some((level) => level.type === "Assistant") ?? false;
    },
    isEceFiveYear(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return false;
      return certification.levels?.some((level) => level.type === "ECE 5 YR") ?? false;
    },
    isEceOneYear(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return false;
      return certification.levels?.some((level) => level.type === "ECE 1 YR") ?? false;
    },
    hasSNE(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return false;
      return certification.levels?.some((level) => level.type === "SNE") ?? false;
    },
    hasITE(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return false;
      return certification.levels?.some((level) => level.type === "ITE") ?? false;
    },
    certificationTypes(certificateId: string | null | undefined): Components.Schemas.CertificationType[] {
      const certification = this.getCertificationById(certificateId);
      if (!certification) return [];

      const certificationTypes = [] as Components.Schemas.CertificationType[];
      if (certification.levels?.some((level) => level.type === "Assistant")) {
        certificationTypes.push("EceAssistant");
      }
      if (certification.levels?.some((level) => level.type === "ECE 1 YR")) {
        certificationTypes.push("OneYear");
      }
      if (certification.levels?.some((level) => level.type === "ECE 5 YR")) {
        certificationTypes.push("FiveYears");
      }
      if (certification.levels?.some((level) => level.type === "SNE")) {
        certificationTypes.push("Sne");
      }
      if (certification.levels?.some((level) => level.type === "ITE")) {
        certificationTypes.push("Ite");
      }
      return certificationTypes;
    },
    hasOtherCertifications(certificateId: string | null | undefined): boolean {
      if (!this.certifications) return false;
      return this.certifications.filter((cert) => cert.id !== certificateId).length > 0;
    },
    checkIsEceAssistant(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "Assistant") ?? false;
    },
    checkIsEceFiveYear(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ECE 5 YR") ?? false;
    },
    checkIsEceOneYear(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ECE 1 YR") ?? false;
    },
    checkHasSNE(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "SNE") ?? false;
    },
    checkHasITE(certification: Components.Schemas.Certification): boolean {
      return certification.levels?.some((level) => level.type === "ITE") ?? false;
    },
    // Action method for checking if a certification is expired more than 5 years
    checkExpiredMoreThan5Years(certificateId: string | null | undefined): boolean {
      const certification = this.getCertificationById(certificateId);
      return expiredMoreThan5Years(certification ?? {});
    },
    getCertificationTypesForCertification(certification: Components.Schemas.Certification): Components.Schemas.CertificationType[] {
      const certificationTypes = [] as Components.Schemas.CertificationType[];
      if (this.checkIsEceAssistant(certification)) {
        certificationTypes.push("EceAssistant");
      }
      if (this.checkIsEceOneYear(certification)) {
        certificationTypes.push("OneYear");
      }
      if (this.checkIsEceFiveYear(certification)) {
        certificationTypes.push("FiveYears");
      }
      if (this.checkHasSNE(certification)) {
        certificationTypes.push("Sne");
      }
      if (this.checkHasITE(certification)) {
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
      }
      return certifications;
    },
  },
});
