import { defineStore } from "pinia";

import { getApplications, postApplication } from "@/api/application";
import type { Components } from "@/types/openapi";

export interface ApplicationState {
  applications: Components.Schemas.Application[] | null | undefined;
}

export const useApplicationStore = defineStore("application", {
  state: (): ApplicationState => {
    return {
      applications: [],
    };
  },
  actions: {
    async fetchApplications() {
      this.applications = await getApplications();
    },
    async newApplication(): Promise<string | null | undefined> {
      return await postApplication();
    },
  },
});
