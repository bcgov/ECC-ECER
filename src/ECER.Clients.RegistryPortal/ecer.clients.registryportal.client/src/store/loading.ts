// Can be used in various components to check whether an API call is waiting for a response for loading spinners
// Example in Application.vue we do this in a button :loading="loadingStore.isLoading('application_post')"
// Loading states are set by our apiResultHandler.ts where we provide a key based on the API operations we have ex. application_post

import { defineStore } from "pinia";

import type { OperationMethods } from "@/types/openapi";

export type LoadingOperation = keyof OperationMethods;

export type LoadingState = {
  [key in LoadingOperation]: boolean;
};

export interface LoadingStateConfig {
  loadingStates: LoadingState;
}

export const useLoadingStore = defineStore("loading", {
  state: (): LoadingStateConfig => ({
    loadingStates: {} as LoadingState,
  }),

  getters: {
    isLoading() {
      return (key: LoadingOperation) => this.loadingStates[key];
    },
  },
  actions: {
    async setLoading(key: LoadingOperation, loading: boolean) {
      this.$patch({ loadingStates: { [key]: loading } });
    },
  },
});
