<template>
  <!-- Messages -->
  <PageContainer :margin-top="false">
    <Loading v-if="showLoading"></Loading>
    <div v-else>
      <v-row v-if="!userStore.isUserVerifiedAndIdentityVerified" justify="center">
        <v-col cols="12">
          <!-- user has not provided id -->
          <v-card v-if="userStore.isIdentityStatusUnverified" :rounded="0" flat color="background-light" class="pa-4">
            <v-card-item class="ma-4">
              <h3>ID needed to complete account setup</h3>
              <p class="mt-2">
                Before you can submit new applications or access existing certifications, we need to verify your identity by reviewing your ID.
              </p>
              <v-btn prepend-icon="mdi-card-account-details-outline" color="primary" class="mt-2" @click="router.push({ name: 'upload-id-new-user' })">
                Verify my identity
              </v-btn>
            </v-card-item>
          </v-card>
          <!-- user provided id waiting for verification-->
          <v-card v-else-if="userStore.isIdentityStatusReadyForVerification" :rounded="0" flat color="background-light" class="pa-4">
            <v-card-item class="ma-4">
              <h3>ID Pending review</h3>
              <p class="mt-2">
                Before you can submit new applications or access existing certifications, we need to verify your identity by reviewing your ID.
              </p>
              <p class="mt-2 font-weight-bold">We have received your IDs. We will email you when our review is complete in 2-3 business days.</p>
            </v-card-item>
          </v-card>
          <!-- user has not been verified -->
          <v-card v-else="!userStore.isVerified" :rounded="0" flat color="background-light" class="pa-4">
            <v-card-item class="ma-4">
              <h3>Your account is being reviewed</h3>
              <p class="mt-2">
                We're finishing setting up your account for you. Once we're done you'll be able to do things like view your certification, renew it or apply for
                new certification.
              </p>
              <p class="mt-2">We'll send you a message as soon as your account is ready. It may take 1-3 business days.</p>
            </v-card-item>
          </v-card>
        </v-col>
      </v-row>

      <v-row v-if="messageStore.unreadMessageCount > 0" justify="center">
        <v-col>
          <v-row>
            <v-col cols="12">
              <Alert :rounded="mdAndUp" :class="smAndDown ? 'mt-n4 mx-n4' : ''" icon="mdi-bell"><UnreadMessages /></Alert>
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <!-- Your ECE applications -->
      <v-row v-if="applications && userStore.isUserVerifiedAndIdentityVerified && showApplicationCard" justify="center">
        <v-col>
          <v-row>
            <v-col cols="12">
              <ApplicationCard :class="smAndDown ? 'mx-n6' : ''" @cancel-application="showCancelDialog = true" />
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <!-- Your ECE certifications -->
      <v-row v-if="userStore.isUserVerifiedAndIdentityVerified" justify="center" class="mt-6">
        <v-col>
          <v-row>
            <v-col cols="12">
              <ECEHeader title="Your ECE certifications" />
              <div v-if="certifications && certificationStore.hasCertifications">
                <div class="d-flex flex-row justify-start ga-3 flex-wrap mt-4">
                  <p>ECE registration number</p>
                  <p>
                    {{ certificationStore.latestCertification?.number }}
                  </p>
                </div>
                <CertificationCard :class="smAndDown ? 'mx-n6 mt-4' : 'mt-4'" :is-rounded="false" />
              </div>
              <p v-else class="small mt-4">You do not have an ECE certificate in your My ECE Registry account.</p>
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <!-- Options -->
      <v-row v-if="showOptions" justify="center" class="mt-6">
        <v-col>
          <v-row>
            <v-col cols="12">
              <ECEHeader title="Options" />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" sm="6" lg="4">
              <RenewCard />
            </v-col>
            <RegistrantCard />
          </v-row>
        </v-col>
      </v-row>

      <!-- Your My ECE Registry account -->
      <v-row justify="center" class="mt-6">
        <v-col>
          <v-row>
            <v-col cols="12">
              <ECEHeader title="Your My ECE Registry account" />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" sm="6" lg="4">
              <ActionCard title="Messages" icon="mdi-bell">
                <template #content>
                  <UnreadMessages :linkable="false" />
                </template>
                <template #action>
                  <v-btn variant="text">
                    <router-link :to="{ name: 'messages' }">Read messages</router-link>
                  </v-btn>
                </template>
              </ActionCard>
            </v-col>

            <v-col v-if="userStore.isUserVerifiedAndIdentityVerified" cols="12" sm="6" lg="4">
              <ActionCard title="Your profile" icon="mdi-account-circle">
                <template #content>Manage your names, address and contact information.</template>
                <template #action>
                  <v-btn variant="text">
                    <router-link :to="{ name: 'profile' }">My profile</router-link>
                  </v-btn>
                </template>
              </ActionCard>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </div>
  </PageContainer>

  <ConfirmationDialog
    :cancel-button-text="'Keep Application'"
    :accept-button-text="'Cancel Application'"
    :title="'Cancel Application'"
    :show="showCancelDialog"
    @cancel="() => (showCancelDialog = false)"
    @accept="cancelApplication"
  >
    <template #confirmation-text>
      <p>By cancelling your application, it will be removed from the system. You cannot undo this.</p>
      <p><b>Are you sure you want to proceed?</b></p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { useRouter } from "vue-router";
import { cancelDraftApplication } from "@/api/application";
import { getUserInfo } from "@/api/user";
import Loading from "@/components/Loading.vue";
import { getProfile } from "@/api/profile";
import ActionCard from "@/components/ActionCard.vue";
import Alert from "@/components/Alert.vue";
import ApplicationCard from "@/components/ApplicationCard.vue";
import CertificationCard from "@/components/CertificationCard.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import RegistrantCard from "@/components/RegistrantCard.vue";
import RenewCard from "@/components/RenewCard.vue";
import UnreadMessages from "@/components/UnreadMessages.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useMessageStore } from "@/store/message";
import { useUserStore } from "@/store/user";
import { useLoadingStore } from "@/store/loading";
import { formatPhoneNumber } from "@/utils/format";
import { useOidcStore } from "@/store/oidc";
import type { Application, Certification, UserInfo, UserProfile } from "@/types/openapi";

export default defineComponent({
  name: "Dashboard",
  components: {
    Loading,
    ConfirmationDialog,
    PageContainer,
    ApplicationCard,
    CertificationCard,
    ECEHeader,
    ActionCard,
    Alert,
    UnreadMessages,
    RenewCard,
    RegistrantCard,
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const router = useRouter();
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const messageStore = useMessageStore();
    const { smAndDown, mdAndUp } = useDisplay();

    return {
      oidcStore,
      userStore,
      applicationStore,
      alertStore,
      loadingStore,
      messageStore,
      certificationStore,
      smAndDown,
      mdAndUp,
      router,
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
      window.location.href = "/login";
      return; //stops the rest of the component from loading. Prevents 401 calls for the methods below
    }

    [this.applications, this.certifications, this.userInfo, this.userProfile] = await Promise.all([
      this.applicationStore.fetchApplications(),
      this.certificationStore.fetchCertifications(),
      getUserInfo(),
      getProfile(),
    ]);
    this.dataNotFetched = false;

    if (this.userInfo !== null) {
      this.userStore.setUserInfo(this.userInfo);
      this.userStore.setUserProfile(this.userProfile);
    } else {
      this.router.push("/new-user");
    }
  },
  data: () => ({
    dataNotFetched: true,
    showCancelDialog: false,
    drawer: null as boolean | null | undefined,
    applications: null as Application[] | null | undefined,
    certifications: null as Certification[] | null | undefined,
    userInfo: null as UserInfo | null,
    userProfile: null as UserProfile | null,
  }),
  computed: {
    showApplicationCard(): boolean {
      if (this.certificationStore.hasCertifications && this.applicationStore.applicationStatus === undefined) {
        return false;
      }

      return (
        this.applicationStore.applicationStatus === undefined ||
        this.applicationStore.applicationStatus === "Draft" ||
        this.applicationStore.applicationStatus === "Submitted" ||
        this.applicationStore.applicationStatus === "Ready" ||
        this.applicationStore.applicationStatus === "InProgress" ||
        this.applicationStore.applicationStatus === "PendingQueue" ||
        this.applicationStore.applicationStatus === "Pending" ||
        this.applicationStore.applicationStatus === "Escalated"
      );
    },
    showLoading(): boolean {
      return (
        this.dataNotFetched ||
        this.loadingStore.isLoading("profile_get") ||
        this.loadingStore.isLoading("userinfo_get") ||
        this.loadingStore.isLoading("certification_get") ||
        this.loadingStore.isLoading("application_get")
      );
    },
    showOptions(): boolean {
      return this.certificationStore.hasCertifications && !this.showApplicationCard && this.userStore.isUserVerifiedAndIdentityVerified;
    },
  },

  methods: {
    formatPhoneNumber,
    async cancelApplication() {
      this.showCancelDialog = false;
      const { data: cancelledApplicationId } = await cancelDraftApplication(this.applicationStore.draftApplication.id!);
      if (cancelledApplicationId) {
        this.applicationStore.fetchApplications();
        this.alertStore.setSuccessAlert("Application successfully cancelled");
      } else {
        this.alertStore.setFailureAlert("Unable to cancel application.");
      }
    },
  },
});
</script>
