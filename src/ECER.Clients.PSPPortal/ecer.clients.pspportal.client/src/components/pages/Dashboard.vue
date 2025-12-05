<template>
  <PageContainer :margin-top="false">
    <Loading v-if="isLoading"></Loading>
    <div v-else>
      <v-row justify="center">
        <v-col cols="12">
          <h1>My PSP dashboard</h1>
        </v-col>
        <v-col v-if="educationInstitution" cols="12">
          <EducationInstitutionCard :education-institution="educationInstitution" />
        </v-col>
      </v-row>
      <v-row justify="center">
        <v-col cols="12">
          <ECEHeader title="Portal options" />
        </v-col>
      </v-row>
      <v-row align="stretch">
        <v-col class="d-flex" cols="12" sm="6" md="4">
          <Card class="d-flex flex-column">
            <h2>Program profiles</h2>
            <p class="mt-4">Program profiles View or manage your institution’s annual program profiles. View program profiles.</p>
            <div class="mt-auto">
              <v-btn variant="outlined" size="large" class="mt-4" color="primary" id="btnNeedOtherOptions" @click="router.push('/program-profiles')">
                View program profiles
              </v-btn>
            </div>
          </Card>
        </v-col>
        <v-col class="d-flex" cols="12" sm="6" md="4">
          <Card class="d-flex flex-column">
            <h2>Messsages</h2>
            <p class="mt-4">View or send a new message to the ECE Registry.</p>
            <div class="mt-auto">
              <v-btn variant="outlined" size="large" class="mt-4" color="primary" id="btnNeedOtherOptions" @click="router.push('/messages')">
                Go to messages
              </v-btn>
            </div>
          </Card>
        </v-col>
        <v-col class="d-flex" cols="12" sm="6" md="4">
          <Card class="d-flex flex-column">
            <h2>User management</h2>
            <p class="mt-4">Manage which users at your institution have access to this portal.</p>
            <div class="mt-auto">
              <v-btn
                variant="outlined"
                size="large"
                class="mt-4"
                color="primary"
                id="btnNeedOtherOptions"
                @click="router.push({ name: 'manage-users', params: { educationInstitutionName: educationInstitution?.name } })"
              >
                Manage users
              </v-btn>
            </div>
          </Card>
        </v-col>
      </v-row>
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
import { getEducationInstitution, getPspUserProfile, registerPspUser } from "@/api/psp-rep";
import type { PspUserProfile, PspRegistrationError, PspRegistrationErrorResponse, RegisterPspUserRequest, EducationInstitution } from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";
import ECEHeader from "@/components/ECEHeader.vue";
import Card from "@/components/Card.vue";
import EducationInstitutionCard from "@/components/EducationInstitutionCard.vue";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
    Loading,
    ECEHeader,
    Card,
    EducationInstitutionCard,
  },
  data() {
    return {
      pspUserProfile: null as PspUserProfile | null,
      educationInstitution: null as EducationInstitution | null,
      loading: true,
    };
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      oidcStore,
      router,
      userStore,
      loadingStore,
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
    } catch (error) {}

    if (!user) {
      this.router.replace("/login");
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }

    // Check if the user has a profile, if not, register them
    [this.pspUserProfile, this.educationInstitution] = await Promise.all([getPspUserProfile(), getEducationInstitution()]);

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

      const registrationResult = await registerPspUser(request);
      if (registrationResult && "errorCode" in registrationResult) {
        // Handle registration errors based on error code from PspRegistrationErrorResponse
        const errorCode: PspRegistrationError | undefined = registrationResult.errorCode;

        switch (errorCode) {
          case "PortalInvitationTokenInvalid":
          case "PortalInvitationWrongStatus":
            this.router.replace("/invalid-invitation");
            break;
          case "BceidBusinessIdDoesNotMatch":
          case "PostSecondaryInstitutionNotFound":
            this.router.replace("/access-denied");
            break;
          case "GenericError":
          default:
            this.router.replace("/generic-registration-error");
            break;
        }
        return;
      }

      [this.pspUserProfile, this.educationInstitution] = await Promise.all([getPspUserProfile(), getEducationInstitution()]);
    }

    if (this.pspUserProfile) {
      this.userStore.setPspUserProfile(this.pspUserProfile);
      if (!this.pspUserProfile.hasAcceptedTermsOfUse) {
        this.router.replace("/new-user");
      }
    }

    if (this.educationInstitution) {
      this.userStore.setEducationInstitution(this.educationInstitution);
    }

    this.loading = false;
  },
  computed: {
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("psp_user_profile_get") ||
        this.loadingStore.isLoading("psp_user_register_post") ||
        this.loadingStore.isLoading("education_institution_get") ||
        this.loadingStore.isLoading("education_institution_put") ||
        this.loading
      );
    },
  },
});
</script>
