<template>
  <PageContainer :margin-top="false">
    <h1>Hello World</h1>
    <p>Welcome to the dashboard!</p>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";
import type { PspUserProfile } from "@/types/openapi";
import { getPspUserProfile, registerPspUser } from "@/api/psp-rep";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
  },
  data() {
    return {
      pspUserProfile: null as PspUserProfile | null,
    };
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const router = useRouter();

    const oidcUserInfo = await oidcStore.oidcUserInfo();

    return {
      oidcStore, router, userStore, oidcUserInfo
    };
  },
  async mounted() {
    let user;
    try {
      user = await this.oidcStore.getUser();

      if (!user) {
        user = await this.oidcStore.signinCallback();
        console.log("user", user);
        this.router.replace("/");
      }
    } catch (error) { }

    if (!user) {
      globalThis.location.href = "/login";
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }

    [this.pspUserProfile] = await Promise.all([
      getPspUserProfile(),
    ]);

    if (!this.pspUserProfile) {
      // Register a new PSP user profile
      this.pspUserProfile = await registerPspUser({
        firstName: user.profile.firstName as string,
        lastName: user.profile.lastName as string,
        email: user.profile.email,
        programRepresentativeId: this.userStore.invitedProgramRepresentativeId as string,
        bceidBusinessId: this.oidcUserInfo.bceidBusinessId as string,
      });
    }
  },
});
</script>
