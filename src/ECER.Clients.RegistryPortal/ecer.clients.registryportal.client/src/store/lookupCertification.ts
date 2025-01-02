import { defineStore } from "pinia";
import type { Components } from "@/types/openapi";

export interface CertificationLookupState {
  certificationSearchResults: Components.Schemas.CertificationLookupResponse[] | undefined;
  certificationRecord: Components.Schemas.CertificationLookupResponse | undefined;
  firstName: string;
  lastName: string;
  registrationNumber: string;
}

export const useLookupCertificationStore = defineStore("lookupCertification", {
  state: (): CertificationLookupState => ({
    certificationSearchResults: undefined,
    certificationRecord: undefined,
    firstName: "",
    lastName: "",
    registrationNumber: "",
  }),
  getters: {},
  actions: {
    setCertificationSearchResults(data: Components.Schemas.CertificationLookupResponse[] | undefined): void {
      this.certificationSearchResults = data;
    },
    setCertificationRecord(record: Components.Schemas.CertificationLookupResponse): void {
      this.certificationRecord = record;
    },
    generateCertificateLevelName(levels: Components.Schemas.CertificationLevel[]) {
      if (levels.some((level) => level.type === "ECE 1 YR")) {
        return "ECE One Year";
      }

      if (levels.some((level) => level.type === "Assistant")) {
        return "ECE Assistant";
      }

      if (levels.some((level) => level.type === "ECE 5 YR") && levels.some((level) => level.type === "ITE") && levels.some((level) => level.type === "SNE")) {
        return "ECE Five Year with Infant and Toddler Educator (ITE) and Special Needs Educator (SNE)";
      }

      if (levels.some((level) => level.type === "ECE 5 YR") && levels.some((level) => level.type === "ITE")) {
        return "ECE Five Year with Infant and Toddler Educator (ITE)";
      }

      if (levels.some((level) => level.type === "ECE 5 YR") && levels.some((level) => level.type === "SNE")) {
        return "ECE Five Year with Special Needs Educator (SNE)";
      }

      if (levels.some((level) => level.type === "ECE 5 YR")) {
        return "ECE Five Year";
      }

      console.warn(`generateCertificateLevelName:: unmapped level type:: ${levels}`);
      return "";
    },
  },
});
