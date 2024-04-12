import { defineStore } from "pinia";

import type { OperationMethods } from "@/types/openapi";

export type LoadingOperation = keyof OperationMethods;

export type LoadingState = {
  [key in LoadingOperation]: boolean;
};

export const useLoadingStore = defineStore("loading", {
  state: (): LoadingState => ({
    application_post: false,
    configuration_get: false,
    profile_get: false,
    profile_put: false,
    userinfo_get: false,
    userinfo_post: false,
    message_get: false,
    message_status_get: false,
    draftapplication_put: false,
    application_get: false,
    draftapplication_delete: false,
  }),
  getters: {
    isLoading() {
      return (key: LoadingOperation) => this[key];
    },
  },
  actions: {
    async setLoading(key: LoadingOperation, loading: boolean) {
      this.$state[key] = loading;
    },
  },
});
