import type { User } from "oidc-client-ts";
import { defineStore } from "pinia";

import type { Authority } from "@/types/authority";
import type { Components } from "@/types/openapi";

export interface UserState {
  oidcUser: User | null;
  authority: Authority | null;
  userProfile: Components.Schemas.UserProfile | null;
}

export const useUserStore = defineStore("user", {
  persist: {
    paths: ["oidcUser", "userProfile", "authority"],
  },
  state: (): UserState => ({
    oidcUser: null,
    authority: null,
    userProfile: null,
  }),
  getters: {
    hasUserInfo: (state): boolean => state.userProfile !== null,
    accessToken: (state): string => (state.authority === "bcsc" ? state.oidcUser?.id_token ?? "" : state.oidcUser?.access_token ?? ""),
    isAuthenticated(): boolean {
      return this.accessToken !== "";
    },
    oidcUserAsUserProfile: (state): Components.Schemas.UserProfile => {
      return {
        // dateOfBirth: state.authority == "bceid" ? null : state.oidcUser?.profile?.birthdate || null,
        firstName: state.authority == "bceid" ? state.oidcUser?.profile?.given_name || "" : state.oidcUser?.profile?.given_names || "",
        lastName: state.oidcUser?.profile?.family_name || "",
        phone: state.authority == "bceid" ? "" : "",
        email: state.oidcUser?.profile?.email || "",
        homeAddress:
          state.authority === "bcsc"
            ? ({
                street_address: state.oidcUser?.profile?.address?.street_address || "",
                city: state.oidcUser?.profile?.address?.locality || "",
                province: state.oidcUser?.profile?.address?.region || "",
                country: state.oidcUser?.profile?.address?.country || "",
                postalCode: state.oidcUser?.profile?.address?.postal_code || "",
              } as Components.Schemas.Address)
            : undefined,
        mailingAddress:
          state.authority === "bcsc"
            ? ({
                street_address: state.oidcUser?.profile?.address?.street_address || "",
                city: state.oidcUser?.profile?.address?.locality || "",
                province: state.oidcUser?.profile?.address?.region || "",
                country: state.oidcUser?.profile?.address?.country || "",
                postalCode: state.oidcUser?.profile?.address?.postal_code || "",
              } as Components.Schemas.Address)
            : undefined,
      } as Components.Schemas.UserProfile;
    },
  },
  actions: {
    setUser(user: User): void {
      this.oidcUser = user;
    },
    setUserProfile(userProfile: Components.Schemas.UserProfile): void {
      this.userProfile = userProfile;
    },
    setAuthority(authority: Authority | null): void {
      this.authority = authority;
    },
  },
});
