<template><div></div></template>

<script lang="ts">
import { getProfile } from "@/api/profile";
import { getUserInfo } from "@/api/user";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

export default {
  setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();

    return { oidcStore, userStore };
  },
  mounted() {
    this.handleCallback();
  },
  methods: {
    async handleCallback(): Promise<void> {
      // Complete signin and retrieve oidc user
      const user = await this.oidcStore.signinCallback();

      // Attempt to get user info
      const userInfo: Components.Schemas.UserInfo | null = await getUserInfo();
      if (userInfo) {
        // Maybe user has a profile already
        const profileInfo: Components.Schemas.UserProfile | null = await getProfile();

        // Set user info and profile info in the store
        this.userStore.setUserInfo(userInfo);
        this.userStore.setUserProfile(profileInfo);
      } else {
        // Push user to the new user page if they don't have user info
        this.$router.push("/new-user");
      }

      // Redirect user to the page they were trying to access
      const redirectTo = (user as any)?.state?.redirect_to;
      if (!redirectTo) {
        this.$router.push("/");
      } else {
        this.$router.push(redirectTo);
      }
    },
  },
};
</script>
