<template>
  <PageContainer :margin-top="false">
    <Loading v-if="isLoading"></Loading>
    <div v-else>
      <!-- Unread messages banner -->
      <v-row v-if="messageStore?.unreadMessageCount > 0" justify="center">
        <v-col>
          <v-row>
            <v-col cols="12">
              <Alert
                :rounded="mdAndUp"
                :class="smAndDown ? 'mt-n4 mx-n4' : ''"
                icon="mdi-bell"
              >
                <UnreadMessages />
              </Alert>
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <!-- Page title -->
      <v-row justify="center">
        <v-col cols="12">
          <h1>My PSP dashboard</h1>
        </v-col>
      </v-row>

      <!-- Program review callout -->
      <v-row
        v-if="
          programsRequiringReview != null && programsRequiringReview?.length > 0
        "
        justify="center"
      >
        <v-col cols="12">
          <Card color="primary" class="d-flex flex-column">
            <h2 color="surface">You have program profiles to review</h2>
            <p color="surface" class="mt-4">
              The ECE Registry reviews programs on an annual basis. Your program
              profile review can now be completed online.
            </p>
            <div class="mt-auto">
              <v-btn
                size="large"
                class="mt-4"
                color="warning"
                id="btnReviewProgramProfile"
                @click="router.push('/program-profiles')"
              >
                <v-icon size="large" icon="mdi-arrow-right" />
                Review now
              </v-btn>
            </div>
          </Card>
        </v-col>
      </v-row>

      <!-- Institution management section -->
      <v-sheet color="hawkes-blue" rounded class="pa-6 mt-4">
        <v-row>
          <v-col cols="12">
            <ECEHeader title="Institution management" />
            <p class="mt-1">
              Manage your institution's profile, locations and user access.
            </p>
          </v-col>
        </v-row>

        <!-- Institution profile -->
        <v-row>
          <v-col cols="12">
            <h3>Institution profile</h3>
          </v-col>
        </v-row>
        <v-row align="stretch">
          <v-col class="d-flex" cols="12" md="6">
            <Card v-if="educationInstitution" class="d-flex flex-column">
              <v-row align="start" justify="space-between" no-gutters>
                <v-col>
                  <p class="font-weight-bold">
                    {{ educationInstitution.name }}
                    <span v-if="educationInstitution.institutionType">
                      ({{ formattedInstitutionType }})
                    </span>
                  </p>
                </v-col>
                <v-col cols="auto">
                  <v-tooltip text="Edit Institution Information" location="top">
                    <template #activator="{ props }">
                      <v-btn
                        v-bind="props"
                        icon="mdi-pencil"
                        variant="plain"
                        @click="router.push('/education-institution/edit')"
                      />
                    </template>
                  </v-tooltip>
                </v-col>
              </v-row>
              <p class="mt-2">
                <span class="font-weight-bold">Address:</span>
              </p>
              <p>{{ formattedAddress }}</p>
            </Card>
          </v-col>

          <!-- Campus and satellite locations -->
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Campus and satellite locations</h3>
              <p class="mt-4">
                View and manage where your institution offers programs.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="btnViewLocations"
                  @click="
                    router.push({
                      name: 'education-institution',
                      params: {
                        institutionId: educationInstitution?.id,
                      },
                    })
                  "
                >
                  View locations
                </v-btn>
              </div>
            </Card>
          </v-col>
        </v-row>

        <!-- Institution administration -->
        <v-row class="mt-2">
          <v-col cols="12">
            <h3>Institution administration</h3>
          </v-col>
        </v-row>
        <v-row align="stretch">
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Messages</h3>
              <p class="mt-4">
                View or send a new message to the ECE Registry.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="btnMessages"
                  @click="router.push('/messages')"
                >
                  Go to messages
                </v-btn>
              </div>
            </Card>
          </v-col>
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>User management</h3>
              <p class="mt-4">
                Manage which users at your institution have access to this
                portal.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="btnManageUsers"
                  @click="
                    router.push({
                      name: 'manage-users',
                      params: {
                        educationInstitutionName: educationInstitution?.name,
                      },
                    })
                  "
                >
                  Manage users
                </v-btn>
              </div>
            </Card>
          </v-col>
        </v-row>
      </v-sheet>

      <div class="px-6">
        <!-- Programs section -->
        <v-row class="mt-6">
          <v-col cols="12">
            <ECEHeader title="Programs" />
            <p class="mt-1">
              View and manage your institution's program profiles and change
              requests.
            </p>
          </v-col>
        </v-row>
        <v-row align="stretch">
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Program profiles</h3>
              <p class="mt-4">
                View or manage your institution's annual program profiles.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="btnViewProgramProfiles"
                  @click="router.push('/program-profiles')"
                >
                  View program profiles
                </v-btn>
              </div>
            </Card>
          </v-col>
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Change requests</h3>
              <p class="mt-4">
                Request a program change (for example, adding or removing
                courses) that affects program requirements or competencies.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="btnRequestChange"
                  @click="
                    router.push({
                      name: 'new-message',
                      params: {
                        initialCategory: 'ProgramChangeRequest',
                      },
                    })
                  "
                >
                  Request a change
                </v-btn>
              </div>
            </Card>
          </v-col>
        </v-row>

        <!-- Applications section -->
        <v-row class="mt-6">
          <v-col cols="12">
            <ECEHeader title="Applications" />
            <p class="mt-1">
              Submit and manage applications for new programs and delivery
              methods.
            </p>
          </v-col>
        </v-row>
        <v-row align="stretch">
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Apply for a new program</h3>
              <p class="mt-4">
                Submit your program details online to begin your application for
                a new program.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="createProgramApplications"
                  @click="
                    router.push({
                      name: 'programApplicationInfo',
                      params: {
                        applicationType:
                          ProgramApplicationType.NewBasicECEPostBasicProgram,
                      },
                    })
                  "
                >
                  Begin application
                </v-btn>
              </div>
            </Card>
          </v-col>
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>Online or hybrid delivery</h3>
              <p class="mt-4">
                Submit an application to expand an existing recognized program
                to include online or hybrid delivery.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="createProgramApplicationAddOnlineOrHybridDelivery"
                  @click="
                    router.push({
                      name: 'programApplicationInfo',
                      params: {
                        applicationType:
                          ProgramApplicationType.AddOnlineorHybridDeliveryMethod,
                      },
                    })
                  "
                >
                  Add delivery method
                </v-btn>
              </div>
            </Card>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="d-flex" cols="12" md="6">
            <Card class="d-flex flex-column">
              <h3>View all applications</h3>
              <p class="mt-4">
                View all applications for my educational institution and edit
                any draft applications.
              </p>
              <div class="mt-auto">
                <v-btn
                  variant="outlined"
                  size="large"
                  class="mt-4"
                  color="primary"
                  id="viewProgramApplications"
                  @click="router.push('/program-applications')"
                >
                  View applications
                </v-btn>
              </div>
            </Card>
          </v-col>
        </v-row>
      </div>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import Alert from "@/components/Alert.vue";
import UnreadMessages from "@/components/UnreadMessages.vue";
import { useDisplay } from "vuetify";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";
import { getPspUserProfile, registerPspUser } from "@/api/psp-rep";
import type {
  PspUserProfile,
  PspRegistrationError,
  RegisterPspUserRequest,
  EducationInstitution,
  Program,
} from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";
import { formatAddress, formatInstitutionType } from "@/utils/format";
import ECEHeader from "@/components/ECEHeader.vue";
import Card from "@/components/Card.vue";
import { useMessageStore } from "@/store/message";
import { getEducationInstitution } from "@/api/education-institution";
import { getPrograms } from "@/api/program";
import { ProgramApplicationType } from "@/utils/constant";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
    Loading,
    ECEHeader,
    Card,
    Alert,
    UnreadMessages,
  },
  data() {
    return {
      pspUserProfile: null as PspUserProfile | null,
      educationInstitution: null as EducationInstitution | null,
      programsRequiringReview: null as Program[] | null,
      loading: true,
      ProgramApplicationType: ProgramApplicationType,
    };
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const messageStore = useMessageStore();

    const { smAndDown, mdAndUp } = useDisplay();
    return {
      oidcStore,
      router,
      userStore,
      loadingStore,
      messageStore,
      smAndDown,
      mdAndUp,
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
    } catch (error) {
      console.log(`Exception while mounting dashboard: ${error}`);
    }

    if (!user) {
      this.router.replace("/login");
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }

    // Check if the user has a profile, if not, register them
    // Pass BCeID info to heal institutions missing a BCeID GUID (ECER-6203)
    this.pspUserProfile = await getPspUserProfile(
      user.profile.bceid_business_guid as string,
      user.profile.bceid_business_name as string,
    );

    if (!this.pspUserProfile) {
      if (
        !this.userStore.invitationToken ||
        !this.userStore.invitedProgramRepresentativeId
      ) {
        this.router.replace("/required-invitation");
        return;
      }

      // Register a new PSP user profile with typed request
      const request: RegisterPspUserRequest = {
        token: this.userStore.invitationToken as string,
        programRepresentativeId: this.userStore
          .invitedProgramRepresentativeId as string,
        bceidBusinessId: user.profile.bceid_business_guid as string,
        bceidBusinessName: user.profile.bceid_business_name as string,
        profile: {
          firstName: user.profile.given_name as string,
          lastName: user.profile.family_name as string,
          email: user.profile.email as string,
        },
      };

      const registrationResult = await registerPspUser(request);
      if (registrationResult && "errorCode" in registrationResult) {
        // Handle registration errors based on error code from PspRegistrationErrorResponse
        const errorCode: PspRegistrationError | undefined =
          registrationResult.errorCode;

        switch (errorCode) {
          case "PortalInvitationTokenInvalid":
          case "PortalInvitationWrongStatus":
            this.router.replace({ name: "invalid-invitation" });
            break;
          case "BceidBusinessIdDoesNotMatch":
            this.router.replace({ name: "access-denied-mismatch" });
            break;
          case "PostSecondaryInstitutionNotFound":
            this.router.replace({ name: "access-denied" });
            break;
          case "BceidBusinessIdMissing":
            this.router.replace({
              name: "generic-registration-error",
              query: { reason: "BceidBusinessIdMissing" },
            });
            break;
          case "GenericError":
          default:
            this.router.replace({ name: "generic-registration-error" });
            break;
        }
        return;
      }

      this.pspUserProfile = await getPspUserProfile();
    }

    if (this.pspUserProfile && this.pspUserProfile.hasAcceptedTermsOfUse) {
      await Promise.all([
        this.getInstitutionData(),
        this.getProgramProfileData(),
      ]);
    } else {
      this.router.replace("/new-user");
    }

    this.setUserStoreValues();

    this.loading = false;
  },
  computed: {
    isLoading(): boolean {
      return (
        this.loadingStore.isLoading("psp_user_profile_get") ||
        this.loadingStore.isLoading("psp_user_register_post") ||
        this.loadingStore.isLoading("education_institution_get") ||
        this.loadingStore.isLoading("education_institution_put") ||
        this.loadingStore.isLoading("program_get") ||
        this.loading
      );
    },
    formattedInstitutionType(): string {
      return formatInstitutionType(this.educationInstitution?.institutionType);
    },
    formattedAddress(): string {
      return formatAddress(this.educationInstitution);
    },
  },
  methods: {
    async getInstitutionData() {
      this.educationInstitution = await getEducationInstitution();
    },
    async getProgramProfileData() {
      const { data: programResults } = await getPrograms("", ["Draft"]);
      this.programsRequiringReview = programResults?.programs ?? null;
    },
    setUserStoreValues() {
      if (this.pspUserProfile) {
        this.userStore.setPspUserProfile(this.pspUserProfile);
      }
      if (this.educationInstitution) {
        this.userStore.setEducationInstitution(this.educationInstitution);
      }
    },
  },
});
</script>
