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
    setCertificationSearchResults(data: Components.Schemas.CertificationLookupResponse[]): void {
      this.certificationSearchResults = data;
    },
    setCertificationRecord(record: Components.Schemas.CertificationLookupResponse): void {
      this.certificationRecord = record;
    },
  },
});
