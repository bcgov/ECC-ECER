import type { UserManagerSettings } from "oidc-client-ts";
import { defineStore } from "pinia";
import {
  getConfiguration,
  getProvinceList,
  getCountryList,
  getAreaOfInstructionList,
} from "@/api/configuration";
import oidcConfig from "@/oidc-config";
import type { Components } from "@/types/openapi";
import { sortArray } from "@/utils/functions";
import { ProvinceTerritoryType } from "@/utils/constant";
export interface UserState {
  applicationConfiguration: Components.Schemas.ApplicationConfiguration;
  provinceList: Components.Schemas.Province[];
  countryList: Components.Schemas.Country[];
  areaOfInstructionList: Components.Schemas.AreaOfInstruction[];
}

export const useConfigStore = defineStore("config", {
  persist: {
    pick: ["applicationConfiguration"],
  },
  state: (): UserState => ({
    applicationConfiguration: {} as Components.Schemas.ApplicationConfiguration,
    provinceList: [] as Components.Schemas.Province[],
    countryList: [] as Components.Schemas.Country[],
    areaOfInstructionList: [] as Components.Schemas.AreaOfInstruction[],
  }),
  getters: {
    kcOidcConfiguration: (state): UserManagerSettings => {
      const oidc = state.applicationConfiguration?.clientAuthenticationMethods
        ? state.applicationConfiguration?.clientAuthenticationMethods["kc"]
        : null;

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
      return (provinceId: string) =>
        state.provinceList.find(
          (province) => province.provinceId === provinceId,
        )?.provinceName;
    },
    countryName(state) {
      return (countryId: string) =>
        state.countryList.find((country) => country.countryId === countryId)
          ?.countryName;
    },
    canada(state) {
      return state.countryList.find(
        (country) => country.countryName!.toLowerCase() === "canada",
      );
    },
    britishColumbia(state) {
      return state.provinceList.find(
        (province) =>
          province.provinceName!.toLowerCase() === "british columbia",
      );
    },
    areaOfInstructionNameById(state) {
      return (areaOfInstructionId: string) =>
        state.areaOfInstructionList.find(
          (area) => area.id === areaOfInstructionId,
        )?.name;
    },
  },

  actions: {
    async initialize(): Promise<
      Components.Schemas.ApplicationConfiguration | null | undefined
    > {
      const [configuration, provinceList, countryList, areaOfInstructionList] =
        await Promise.all([
          getConfiguration(),
          getProvinceList(),
          getCountryList(),
          getAreaOfInstructionList(),
        ]);

      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }
      if (provinceList !== null && provinceList !== undefined) {
        this.provinceList = provinceList.sort((a, b) =>
          sortArray(a, b, "provinceName", [ProvinceTerritoryType.OTHER]),
        );
      }
      if (countryList !== null && countryList !== undefined) {
        this.countryList = countryList.sort((a, b) =>
          sortArray(a, b, "countryName"),
        );
      }
      if (
        areaOfInstructionList !== null &&
        areaOfInstructionList !== undefined
      ) {
        this.areaOfInstructionList = areaOfInstructionList;
      }
      return configuration;
    },
  },
});
