import { defineStore } from "pinia";

export interface CertificationLookupState {
  certificationSearchResults: any[] | undefined;
  certificationRecord: any | undefined;
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
    setCertificationSearchResults(data: any): void {
      this.certificationSearchResults = data;
    },
    setCertificationRecord(record: any): void {
      this.certificationRecord = record;
    },
  },
});
