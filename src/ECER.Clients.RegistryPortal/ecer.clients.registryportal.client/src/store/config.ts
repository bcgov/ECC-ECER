import type { UserManagerSettings } from "oidc-client-ts";
import { defineStore } from "pinia";

import { getConfiguration, getProvinceList } from "@/api/configuration";
import oidcConfig from "@/oidc-config";
import type { DropdownWrapper } from "@/types/form";
import type { Components } from "@/types/openapi";
import { sortArray } from "@/utils/functions";

export interface UserState {
  applicationConfiguration: Components.Schemas.ApplicationConfiguration;
  provinceList: DropdownWrapper<String>[];
}

export const useConfigStore = defineStore("config", {
  persist: {
    paths: ["applicationConfiguration"],
  },
  state: (): UserState => ({
    applicationConfiguration: {} as Components.Schemas.ApplicationConfiguration,
    provinceList: [] as DropdownWrapper<String>[],
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
      const configuration = await getConfiguration();
      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }

      const provinceList = await getProvinceList();
      if (provinceList !== null && provinceList !== undefined) {
        this.provinceList = provinceList
          .map((province) => {
            return {
              value: province.provinceId as string,
              title: province.provinceName as string,
            };
          })
          .sort((a, b) => sortArray(a, b, "title", ["Other"]));
      }
      return configuration;
    },
  },
});
