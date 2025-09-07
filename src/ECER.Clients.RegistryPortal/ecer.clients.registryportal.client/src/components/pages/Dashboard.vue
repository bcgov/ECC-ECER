<template>
  <!-- Messages -->
  <PageContainer :margin-top="false">
    <Loading v-if="showLoading"></Loading>
    <div v-else>
      <v-row v-if="!userStore.isVerified" justify="center">
        <v-col cols="12">
          <!-- user has not provided id -->
          <v-card
            v-if="userStore.userInfo?.status === 'Unverified' || userStore.userInfo?.status === 'PendingforDocuments'"
            :rounded="0"
            flat
            color="grey-pale-background"
            class="pa-4"
          >
            <v-card-item class="ma-4">
              <h3>ID needed to complete account setup</h3>
              <p class="mt-2">
                Before you can submit new applications or access existing certifications, we need to verify your identity by reviewing your ID.
              </p>
              <v-btn prepend-icon="mdi-card-account-details-outline" color="primary" class="mt-2" @click="router.push({ name: 'verifyIdentification' })">
                Verify my identity
              </v-btn>
            </v-card-item>
          </v-card>
          <!-- user provided id waiting for verification-->
          <v-card v-else-if="userStore.userInfo?.status === 'ReadyforIDVerification'" :rounded="0" flat color="grey-pale-background" class="pa-4">
            <v-card-item class="ma-4">
              <h3>ID Pending review</h3>
              <p class="mt-2">
                Before you can submit new applications or access existing certifications, we need to verify your identity by reviewing your ID.
              </p>
              <p class="mt-2 font-weight-bold">We have received your IDs. We will email you when our review is complete in 2-3 business days.</p>
            </v-card-item>
          </v-card>
          <!-- user has not been verified -->
          <v-card v-else :rounded="0" flat color="grey-pale-background" class="pa-4">
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

      <v-row>
        <v-col cols="12">
          <h1>{{ heading }}</h1>
          <p v-if="subheading" class="mt-3">{{ subheading }}</p>
        </v-col>
      </v-row>

      <!-- Your ECE applications -->
      <v-row v-if="applications && userStore.isVerified && (showApplicationCard || showIcraEligibilityCard)" justify="center">
        <v-col>
          <v-row>
            <v-col v-if="showIcraEligibilityCard" cols="12" :sm="showTransferCard ? 6 : 12">
              <IcraEligibilityCard @cancel-application="showCancelDialog = true" />
            </v-col>
            <v-col v-else-if="showApplicationCard" cols="12" :sm="showTransferCard ? 6 : 12">
              <ApplicationCard @cancel-application="showCancelDialog = true" />
            </v-col>
            <v-col v-if="showTransferCard" cols="12" sm="6">
              <TransferCard />
            </v-col>
            <v-col v-if="showIcraCard" cols="12" sm="6">
              <IcraCard />
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <!-- My current certification -->
      <template v-if="userStore.isVerified">
        <v-row justify="center" class="mt-6">
          <v-col>
            <v-row>
              <v-col cols="12">
                <ECEHeader title="My current certification" />
                <div v-if="certifications && certificationStore.hasCertifications">
                  <div class="d-flex flex-row justify-start ga-3 flex-wrap mt-4">
                    <p class="font-weight-bold">
                      ECE registration number:
                      {{ certificationStore.currentCertification?.number }}
                    </p>
                  </div>
                  <template v-if="certificationStore.currentCertification">
                    <CertificationCard
                      class="mt-4"
                      :is-rounded="false"
                      :certification="certificationStore.currentCertification"
                      :has-application="applicationStore.hasApplication"
                    />
                  </template>
                </div>
                <p v-else class="small mt-4">You do not have an ECE certificate in your My ECE Registry account.</p>
              </v-col>
            </v-row>
          </v-col>
        </v-row>

        <!-- My Other Certifications -->
        <v-row v-if="certifications && hasOtherCertifications()" justify="center" class="mt-6">
          <v-col>
            <v-row>
              <v-col cols="12">
                <div>
                  <v-btn block size="x-large" variant="outlined" color="primary" @click="router.push('/my-other-certifications')" class="force-full-content">
                    My other certifications
                    <v-icon size="large" icon="mdi-arrow-right" />
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </template>

      <!-- Options -->
      <v-row v-if="showOptions" justify="center" class="mt-6">
        <v-col>
          <v-row>
            <v-col cols="12">
              <ECEHeader title="Need other options?" />
            </v-col>
          </v-row>
          <v-row align="stretch">
            <v-col class="d-flex" cols="12" sm="6" md="4">
              <Card class="d-flex flex-column">
                <h2>Apply for new certification</h2>
                <p class="mt-4">Start an application for a new certificate level based on your education and work experience.</p>
                <div class="mt-auto">
                  <v-btn
                    :variant="certificationStore.holdsPostBasicCertification ? 'outlined' : 'flat'"
                    size="large"
                    class="mt-4"
                    color="primary"
                    id="btnNeedOtherOptions"
                    @click="handleStartNewApplication"
                  >
                    Apply now
                  </v-btn>
                </div>
              </Card>
            </v-col>
            <v-col class="d-flex" cols="12" sm="6" md="4">
              <Card class="d-flex flex-column">
                <h2>Transfer certification</h2>
                <p class="mt-4">If you are certified in another province or territory in Canada, you may be eligible to transfer your certification to B.C.</p>
                <div class="mt-auto">
                  <v-btn
                    :variant="certificationStore.holdsPostBasicCertification ? 'outlined' : 'flat'"
                    size="large"
                    class="mt-4"
                    color="primary"
                    id="btnNeedOtherOptions"
                    @click="handleTransfer"
                  >
                    Transfer
                  </v-btn>
                </div>
              </Card>
            </v-col>
            <v-col v-if="configurationStore.applicationConfiguration.icraFeatureEnabled" class="d-flex" cols="12" sm="6" md="4">
              <Card class="d-flex flex-column">
                <h2>Apply with international certification</h2>
                <p class="mt-4">
                  Apply for
                  <b>ECE Five Year Certification</b>
                  if you are internationally certified in a country that regulates the ECE profession and do not have 500 hours work experience supervised by a
                  Canadian-certified ECE.
                </p>
                <div class="mt-auto">
                  <router-link class="mt-4" :to="{ name: 'icra-eligibility' }">Learn more</router-link>
                </div>
              </Card>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </div>
  </PageContainer>

  <ConfirmationDialog
    :cancel-button-text="'Keep application'"
    :accept-button-text="'Cancel application'"
    :title="'Cancel application'"
    :show="showCancelDialog"
    :loading="cancelApplicationLoading"
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
import TransferCard from "@/components/TransferCard.vue";
import IcraCard from "@/components/IcraCard.vue";
import IcraEligibilityCard from "@/components/IcraEligibilityCard.vue";
import CertificationCard from "@/components/CertificationCard.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import UnreadMessages from "@/components/UnreadMessages.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useConfigStore } from "@/store/config";
import { useCertificationStore } from "@/store/certification";
import { useIcraStore } from "@/store/icra";
import { useMessageStore } from "@/store/message";
import { useUserStore } from "@/store/user";
import { useLoadingStore } from "@/store/loading";
import { useOidcStore } from "@/store/oidc";
import type { Application, Certification, UserInfo, UserProfile } from "@/types/openapi";
import Card from "@/components/Card.vue";

export default defineComponent({
  name: "Dashboard",
  components: {
    Loading,
    ConfirmationDialog,
    PageContainer,
    ApplicationCard,
    TransferCard,
    IcraCard,
    IcraEligibilityCard,
    CertificationCard,
    ECEHeader,
    ActionCard,
    Alert,
    UnreadMessages,
    Card,
  },
  async setup() {
    const oidcStore = useOidcStore();
    const userStore = useUserStore();
    const router = useRouter();
    const applicationStore = useApplicationStore();
    const configurationStore = useConfigStore();
    const certificationStore = useCertificationStore();
    const icraStore = useIcraStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const messageStore = useMessageStore();
    const { smAndDown, mdAndUp } = useDisplay();

    return {
      oidcStore,
      userStore,
      applicationStore,
      configurationStore,
      icraStore,
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

    // Fetch ICRA eligibilities if the feature is enabled
    if (this.configurationStore.applicationConfiguration.icraFeatureEnabled) {
      await this.icraStore.fetchIcraEligibilities();
    }
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
    heading(): string {
      return this.showTransferCard ? "Apply for ECE certification in B.C." : "My ECE Registry dashboard";
    },
    subheading(): string {
      return this.showTransferCard ? "Review your options to get certified in British Columbia." : "";
    },
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
    showTransferCard(): boolean {
      return (
        !this.certificationStore.hasCertifications &&
        !this.applicationStore.hasApplication &&
        !this.applicationStore.hasDraftApplication &&
        !this.icraStore.hasDraftIcraEligibility
      );
    },
    showIcraCard(): boolean {
      return (
        (this.configurationStore.applicationConfiguration.icraFeatureEnabled ?? false) &&
        !this.applicationStore.hasApplication &&
        !this.applicationStore.hasDraftApplication &&
        !this.icraStore.hasDraftIcraEligibility
      );
    },
    showIcraEligibilityCard(): boolean {
      return (this.configurationStore.applicationConfiguration.icraFeatureEnabled ?? false) && this.icraStore.hasDraftIcraEligibility;
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
      // If the user has certifications, does not have an application, and does not hold all certifications (Active or renewable)
      return (
        this.certificationStore.hasCertifications &&
        !this.icraStore.hasIcraEligibility &&
        !this.applicationStore.hasApplication &&
        !this.certificationStore.holdsAllCertifications
      );
    },
    cancelApplicationLoading(): boolean {
      return this.loadingStore.isLoading("draftapplication_delete") || this.loadingStore.isLoading("application_get");
    },
  },

  methods: {
    hasOtherCertifications() {
      const currentCert = this.certificationStore.currentCertification;
      if (!currentCert || !this.certificationStore.certifications) {
        return false;
      }
      return this.certificationStore.certifications.length > 1;
    },
    async cancelApplication() {
      const { data: cancelledApplicationId } = await cancelDraftApplication(this.applicationStore.draftApplication.id!);
      if (cancelledApplicationId) {
        await this.applicationStore.fetchApplications();
        this.alertStore.setSuccessAlert("Application successfully cancelled");
        this.showCancelDialog = false;
      } else {
        this.alertStore.setFailureAlert("Unable to cancel application.");
      }
    },
    handleStartNewApplication() {
      this.router.push({ name: "application-certification" });
    },
    handleTransfer() {
      this.router.push({ name: "application-transfer" });
    },
  },
});
</script>

<style scoped>
::v-deep(.force-full-content .v-btn__content) {
  flex: 1 1 auto !important;
  justify-content: space-between;
}
</style>
