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
      let authority = "";
      let client_id = "";
      let scope = "";

      // Check if authenticationMethods is not null
      if (state.applicationConfiguration) {
        // Iterate through each authentication method
        for (const method in state.applicationConfiguration) {
          if (
            Object.prototype.hasOwnProperty.call(
              state.applicationConfiguration,
              // eslint-disable-next-line prettier/prettier
              method
            )
          ) {
            const oidcAuthenticationSettings =
              state.applicationConfiguration[
                method as keyof Components.Schemas.ApplicationConfiguration
              ];

            if (oidcAuthenticationSettings) {
              // Access individual properties of OidcAuthenticationSettings
              authority = <string>oidcAuthenticationSettings.authority;
              client_id = <string>oidcAuthenticationSettings.clientId;
              scope = <string>oidcAuthenticationSettings.scope;
            }
          }
        }
      }

      const combinedConfig: UserManagerSettings = {
        ...oidcConfig,
        client_id,
        authority,
        scope,
      };

      return combinedConfig;
    },
  },
  actions: {
    async initialize(): Promise<
      Components.Schemas.ApplicationConfiguration | null | undefined
    > {
      const configuration = await getConfiguration();
      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }
      return configuration;
    },
  },
});
