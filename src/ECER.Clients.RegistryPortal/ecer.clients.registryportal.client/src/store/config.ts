import type { UserManagerSettings } from "oidc-client-ts";
import { defineStore } from "pinia";

import { getConfiguration, getProvinceList, getSystemMessages } from "@/api/configuration";
import oidcConfig from "@/oidc-config";
import type { DropdownWrapper } from "@/types/form";
import type { Components, SystemMessage } from "@/types/openapi";
import { ProvinceTerritoryType } from "@/utils/constant";
import { sortArray } from "@/utils/functions";

export interface UserState {
  applicationConfiguration: Components.Schemas.ApplicationConfiguration;
  systemMessages: SystemMessage[];
  provinceList: DropdownWrapper<String>[];
}

export const useConfigStore = defineStore("config", {
  persist: {
    pick: ["applicationConfiguration"],
  },
  state: (): UserState => ({
    applicationConfiguration: {} as Components.Schemas.ApplicationConfiguration,
    provinceList: [] as DropdownWrapper<String>[],
    systemMessages: [] as SystemMessage[],
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
    provinceName(state) {
      return (provinceId: string) => state.provinceList.find((province) => province.value === provinceId)?.title;
    },
  },

  actions: {
    async initialize(): Promise<Components.Schemas.ApplicationConfiguration | null | undefined> {
      const [configuration, provinceList, systemMessages] = await Promise.all([getConfiguration(), getProvinceList(), getSystemMessages()]);

      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }

      if (systemMessages !== null && systemMessages !== undefined) {
        this.systemMessages = systemMessages;
      }
      if (provinceList !== null && provinceList !== undefined) {
        this.provinceList = provinceList
          .map((province) => {
            return {
              value: province.provinceId as string,
              title: province.provinceName as string,
            };
          })
          .sort((a, b) => sortArray(a, b, "title", [ProvinceTerritoryType.OTHER]));
      }
      return configuration;
    },
  },
});
