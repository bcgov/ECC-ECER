import { DateTime } from "luxon";
import { defineStore } from "pinia";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";

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
    getCertificationById: (state) => {
      return (certificateId: string): Components.Schemas.Certification | undefined => {
        return state.certifications?.find((cert) => cert.id === certificateId);
      };
    },
    certificateStatus: (state) => {
      return (certificateId: string): Components.Schemas.CertificateStatusCode | undefined => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        return certification?.statusCode;
      };
    },
    expiredMoreThan5Years: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification?.expiryDate) return false;
        const dt1 = DateTime.now().startOf("day");
        const dt2 = DateTime.fromISO(certification.expiryDate);
        const differenceInYears = Math.abs(dt1.diff(dt2, "years").years);
        return differenceInYears > 5;
      };
    },
    certificationExpiryDate: (state) => {
      return (certificateId: string): string | null | undefined => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        return certification?.expiryDate;
      };
    },
    certificationEffectiveDate: (state) => {
      return (certificateId: string): string | null | undefined => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        return certification?.effectiveDate;
      };
    },
    isEceAssistant: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification) return false;
        return certification.levels?.some((level) => level.type === "Assistant") ?? false;
      };
    },
    isEceFiveYear: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification) return false;
        return certification.levels?.some((level) => level.type === "ECE 5 YR") ?? false;
      };
    },
    isEceOneYear: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification) return false;
        return certification.levels?.some((level) => level.type === "ECE 1 YR") ?? false;
      };
    },
    hasSNE: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification) return false;
        return certification.levels?.some((level) => level.type === "SNE") ?? false;
      };
    },
    hasITE: (state) => {
      return (certificateId: string): boolean => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
        if (!certification) return false;
        return certification.levels?.some((level) => level.type === "ITE") ?? false;
      };
    },
    certificationTypes: (state) => {
      return (certificateId: string): Components.Schemas.CertificationType[] => {
        const certification = state.certifications?.find((cert) => cert.id === certificateId);
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
      };
    },
    otherCertifications: (state) => {
      return (certificateId: string): Components.Schemas.Certification[] => {
        if (!state.certifications) return [];
        return state.certifications.filter((cert) => cert.id !== certificateId);
      };
    },
    hasOtherCertifications: (state) => {
      return (certificateId: string): boolean => {
        if (!state.certifications) return false;
        return state.certifications.filter((cert) => cert.id !== certificateId).length > 0;
      };
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
      }
      return certifications;
    },
  },
});
