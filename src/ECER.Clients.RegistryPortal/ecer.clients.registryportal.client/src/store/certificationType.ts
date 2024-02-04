import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";

export interface CertificationTypeState {
  selection: Components.Schemas.CertificationType | null;
  subSelection: Components.Schemas.CertificationType[];
}

export const useCertificationTypeStore = defineStore("certificationType", {
  state: (): CertificationTypeState => ({
    selection: null,
    subSelection: [],
  }),
  getters: {
    certificationTypes(state): Components.Schemas.CertificationType[] {
      return state.selection === null ? [] : [state.selection, ...state.subSelection];
    },
  },
});
