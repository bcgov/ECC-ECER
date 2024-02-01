import type { User } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Authority } from "@/types/authority";
import type { Components } from "@/types/openapi";

import { useOidcStore } from "./oidc";

export interface UserState {
  oidcUser: User | null;
  authority: Authority | null;
  userInfo: Components.Schemas.UserInfo | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["oidcUser", "userInfo", "authority"],
  },
  state: (): UserState => ({
    oidcUser: null,
    authority: null,
    userInfo: null,
  }),
  getters: {
    hasUserInfo: (state): boolean => state.userInfo !== null,
    accessToken: (state): string => (state.authority === "bcsc" ? state.oidcUser?.id_token ?? "" : state.oidcUser?.access_token ?? ""),
    isAuthenticated(): boolean {
      return this.accessToken !== "";
    },
    fullName: (state): string => (state.authority == "bcsc" ? `${state.userInfo?.firstName} ${state.userInfo?.lastName}` : `${state.userInfo?.firstName}`),
    email: (state): string => state.userInfo?.email || "",
    phoneNumber: (state): string => state.userInfo?.phone || "",
    oidcUserAsUserInfo: (state): Components.Schemas.UserInfo => {
      return {
        dateOfBirth: state.authority == "bceid" ? null : state.oidcUser?.profile?.birthdate || null,
        firstName: state.authority === "bceid" ? state.oidcUser?.profile?.given_name || "" : state.oidcUser?.profile?.given_names || "",
        lastName: state.oidcUser?.profile?.family_name || "",
        phone: state.authority === "bceid" ? "" : "",
        email: state.oidcUser?.profile?.email || "",
      } as Components.Schemas.UserInfo;
    },
    oidcAddress: (state): Components.Schemas.Address => {
      return {
        line1: state.oidcUser?.profile?.address?.street_address || "",
        line2: "",
        city: state.oidcUser?.profile?.address?.locality || "",
        province: state.oidcUser?.profile?.address?.region || "",
        country: state.oidcUser?.profile?.address?.country || "",
        postalCode: state.oidcUser?.profile?.address?.postal_code || "",
      };
    },
    oidcUserInfo: (state): Components.Schemas.UserInfo => {
      return {
        dateOfBirth: state.authority == "bceid" ? undefined : state.oidcUser?.profile?.birthdate || undefined,
        firstName: state.authority === "bceid" ? state.oidcUser?.profile?.given_name || "" : state.oidcUser?.profile?.given_names || "",
        lastName: state.oidcUser?.profile?.family_name || "",
        phone: state.authority === "bceid" ? "" : "",
        email: state.oidcUser?.profile?.email || "",
      } as Components.Schemas.UserInfo;
    },
  },
  actions: {
    setUser(user: User): void {
      this.oidcUser = user;
    },
    setUserInfo(userInfo: Components.Schemas.UserInfo): void {
      this.userInfo = userInfo;
    },
    setAuthority(authority: Authority | null): void {
      this.authority = authority;
    },
    async logout(): Promise<void> {
      const oidcStore = useOidcStore();
      if (this.isAuthenticated && this.authority) {
        if (this.authority == "bceid") {
          await oidcStore.logout(this.authority);
        }
        if (this.authority == "bcsc") {
          // BCSC does not support a session logout callback endpoint so just remove session data from client
          await oidcStore.removeUser(this.authority);
          this.$reset();
        }
      }
    },
  },
});
