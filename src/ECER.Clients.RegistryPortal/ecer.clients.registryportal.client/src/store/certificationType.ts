import { defineStore } from "pinia";

import type { Components } from "@/types/openapi";

export interface CertificationTypeState {
  selection: Components.Schemas.CertificationType | null;
  subSelection: Components.Schemas.CertificationType[];
  mode: "selection" | "terms";
}

export const useCertificationTypeStore = defineStore("certificationType", {
  state: (): CertificationTypeState => ({
    selection: null,
    subSelection: [],
    mode: "selection",
  }),
  getters: {
    certificationTypes(state): Components.Schemas.CertificationType[] {
      return state.selection === null || typeof state.selection === "undefined" ? [] : [state.selection, ...state.subSelection];
    },
  },
});
