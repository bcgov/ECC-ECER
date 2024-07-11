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
    latestCertification(state): Components.Schemas.Certification | undefined {
      return state.certifications?.[0];
    },
    latestTitle(): string {
      return this.latestCertification?.level || "";
    },
  },
  actions: {
    async fetchCertifications() {
      const { data: certifications } = await getCertifications();
      if (certifications?.length) {
        this.certifications = certifications;
      }
      return certifications;
    },
  },
});
