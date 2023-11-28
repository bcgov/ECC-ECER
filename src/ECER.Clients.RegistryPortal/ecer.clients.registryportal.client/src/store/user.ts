import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({ accessToken: "", refreshToken: "" }),
  getters: {
    isAuthenticated: (state) => state.accessToken !== "",
  },
  actions: {
    setUser() {
      this.accessToken = "testing";
    },
  },
});
