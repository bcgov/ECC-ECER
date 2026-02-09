import type { UserManagerSettings } from "oidc-client-ts";
import { defineStore } from "pinia";
import {
  getConfiguration,
  getProvinceList,
  getCountryList,
  getSystemMessages,
  getIdentificationTypes,
  getPostSecondaryInstitutionList,
  getDefaultContent,
} from "@/api/configuration";
import oidcConfig from "@/oidc-config";
import type { Components } from "@/types/openapi";
import { ProvinceTerritoryType } from "@/utils/constant";
import { sortArray } from "@/utils/functions";
export interface UserState {
  applicationConfiguration: Components.Schemas.ApplicationConfiguration;
  systemMessages: Components.Schemas.SystemMessage[];
  provinceList: Components.Schemas.Province[];
  countryList: Components.Schemas.Country[];
  postSecondaryInstitutionList: Components.Schemas.PostSecondaryInstitution[];
  identificationTypes: Components.Schemas.IdentificationType[];
  defaultContents: Components.Schemas.DefaultContent[];
}

export const useConfigStore = defineStore("config", {
  persist: {
    pick: ["applicationConfiguration"],
  },
  state: (): UserState => ({
    applicationConfiguration: {} as Components.Schemas.ApplicationConfiguration,
    provinceList: [] as Components.Schemas.Province[],
    countryList: [] as Components.Schemas.Country[],
    postSecondaryInstitutionList:
      [] as Components.Schemas.PostSecondaryInstitution[],
    systemMessages: [] as Components.Schemas.SystemMessage[],
    identificationTypes: [] as Components.Schemas.IdentificationType[],
    defaultContents: [] as Components.Schemas.DefaultContent[],
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
    primaryIdentificationType(state) {
      return state.identificationTypes.filter((type) => type.forPrimary);
    },
    secondaryIdentificationType(state) {
      return state.identificationTypes.filter((type) => type.forSecondary);
    },
  },

  actions: {
    async initialize(): Promise<
      Components.Schemas.ApplicationConfiguration | null | undefined
    > {
      const [
        configuration,
        provinceList,
        countryList,
        postSecondaryInstitutionList,
        identificationTypes,
        systemMessages,
        defaultContents,
      ] = await Promise.all([
        getConfiguration(),
        getProvinceList(),
        getCountryList(),
        getPostSecondaryInstitutionList(),
        getIdentificationTypes(),
        getSystemMessages(),
        getDefaultContent(),
      ]);

      if (configuration !== null && configuration !== undefined) {
        this.applicationConfiguration = configuration;
      }
      if (systemMessages !== null && systemMessages !== undefined) {
        this.systemMessages = systemMessages;
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
        postSecondaryInstitutionList !== null &&
        postSecondaryInstitutionList !== undefined
      ) {
        this.postSecondaryInstitutionList = postSecondaryInstitutionList.sort(
          (a, b) => sortArray(a, b, "name"),
        );
      }
      if (identificationTypes !== null && identificationTypes !== undefined) {
        this.identificationTypes = identificationTypes;
      }
      if (defaultContents !== null && defaultContents !== undefined) {
        this.defaultContents = defaultContents;
      }
      return configuration;
    },
  },
});
