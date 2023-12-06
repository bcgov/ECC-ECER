import type { UserManagerSettings } from "oidc-client-ts";
import { defineStore } from "pinia";

import { getConfiguration } from "@/api/configuration";
import oidcConfig from "@/oidc-config";
import type { Components } from "@/types/openapi";

export interface UserState {
  applicationConfiguration: Components.Schemas.ApplicationConfiguration;
}

export const useConfigStore = defineStore("config", {
  persist: {
    paths: ["applicationConfiguration"],
  },
  state: (): UserState => ({
    applicationConfiguration: {} as Components.Schemas.ApplicationConfiguration,
  }),
  getters: {
    oidcConfiguration: (state): UserManagerSettings => {
      const oidc = state.applicationConfiguration?.authenticationMethods ? state.applicationConfiguration?.authenticationMethods["bceid"] : null;

      const combinedConfig: UserManagerSettings = {
        ...oidcConfig,
        client_id: oidc?.clientId ?? "",
        authority: oidc?.authority ?? "",
        scope: oidc?.scope ?? "",
      };

      return combinedConfig;
    },
  },
  actions: {
    async initialize(): Promise<Components.Schemas.ApplicationConfiguration | null | undefined> {
      const configuration = await getConfiguration();
      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }
      return configuration;
    },
  },
});
