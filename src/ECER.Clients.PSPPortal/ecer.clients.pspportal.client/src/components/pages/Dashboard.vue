<template>
  <PageContainer :margin-top="false">
    <Loading v-if="loadingStore.isLoading('psp_user_profile_get') || loadingStore.isLoading('psp_user_register_post')">
    </Loading>
    <div v-else>
      <h1>Dashboard is under development</h1>
      <p>This page is currently under development. Please check back soon.</p>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";
import { getPspUserProfile, registerPspUser } from "@/api/psp-rep";
import type { PspUserProfile, PspRegistrationError, PspRegistrationErrorResponse, RegisterPspUserRequest } from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
    Loading,
  },
  data() {
    return {
      pspUserProfile: null as PspUserProfile | null,
    };
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      oidcStore, router, userStore, loadingStore
    };
  },
  async mounted() {
    let user;
    try {
      user = await this.oidcStore.getUser();

      if (!user) {
        user = await this.oidcStore.signinCallback();
        this.router.replace("/");
      }
    } catch (error) { }

    if (!user) {
      this.router.replace("/login");
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }

    // Check if the user has a profile, if not, register them
    [this.pspUserProfile] = await Promise.all([
      getPspUserProfile(),
    ]);

    if (!this.pspUserProfile) {
      if (!this.userStore.invitationToken || !this.userStore.invitedProgramRepresentativeId) {
        this.router.replace("/required-invitation");
        return;
      }

      // Register a new PSP user profile with typed request
      const request: RegisterPspUserRequest = {
        token: this.userStore.invitationToken as string,
        programRepresentativeId: this.userStore.invitedProgramRepresentativeId as string,
        bceidBusinessId: user.profile.bceid_business_guid as string,
        profile: {
          firstName: user.profile.given_name as string,
          lastName: user.profile.family_name as string,
          email: user.profile.email as string,
        },
      };

      try {
        await registerPspUser(request);
        this.pspUserProfile = await getPspUserProfile();
      } catch (error: any) {
        // Handle registration errors based on error code from PspRegistrationErrorResponse
        const errorResponse: PspRegistrationErrorResponse | undefined = error?.response?.data;
        const errorCode: PspRegistrationError | undefined = errorResponse?.errorCode;

        switch (errorCode) {
          case "PortalInvitationTokenInvalid":
          case "PortalInvitationWrongStatus":
            this.router.replace("/invalid-invitation");
            break;
          case "BceidBusinessIdDoesNotMatch":
          case "PostSecondaryInstitutionNotFound":
            this.router.replace("/access-denied");
            break;
          default:
            this.router.replace("/generic-registration-error");
        }
        return;
      }
    }

    if (this.pspUserProfile) {
      this.userStore.setPspUserProfile(this.pspUserProfile);
    }
  },
});
</script>
