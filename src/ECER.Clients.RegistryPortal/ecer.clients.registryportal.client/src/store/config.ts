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
    kcOidcConfiguration: (state): UserManagerSettings => {
      const oidc = state.applicationConfiguration?.clientAuthenticationMethods ? state.applicationConfiguration?.clientAuthenticationMethods["kc"] : null;

      const combinedConfig: UserManagerSettings = {
        ...oidcConfig,
        client_id: oidc?.clientId ?? "",
        authority: oidc?.authority ?? "",
        scope: oidc?.scope ?? "",
        loadUserInfo: true,
        extraQueryParams: { kc_idp_hint: oidc?.idp ?? "" },
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
