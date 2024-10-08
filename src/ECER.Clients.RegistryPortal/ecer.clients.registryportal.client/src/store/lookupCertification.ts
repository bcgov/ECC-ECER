import { defineStore } from "pinia";

export interface CertificationLookupState {
  certificationSearchResults: any[] | undefined;
  certificationRecord: any | undefined;
}

export const useLookupCertificationStore = defineStore("lookupCertification", {
  state: (): CertificationLookupState => ({
    certificationSearchResults: undefined,
    certificationRecord: undefined,
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
