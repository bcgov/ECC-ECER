<template>
  <!-- Messages -->
  <PageContainer :margin-top="false">
    <v-row v-if="showLoading" justify="center">
      <v-col cols="12" class="text-center">
        <v-progress-circular
          indeterminate
          class="mt-10 mb-3"
          color="primary"
          size="64"
        ></v-progress-circular>
        <p>Loading data, please wait...</p>
      </v-col>
    </v-row>

    <v-row v-if="!userStore.isVerified && !showLoading" justify="center">
      <v-col cols="12">
        <v-card :rounded="0" flat color="background-light" class="pa-4">
          <v-card-item class="ma-4">
            <h3>Your account is being reviewed</h3>
            <p class="mt-2">
              We're finishing setting up your account for you. Once we're done you'll be
              able to do things like view your certification, renew it or apply for new
              certification.
            </p>
            <p class="mt-2">
              We'll send you a message as soon as your account is ready. It may take 1-3
              business days.
            </p>
          </v-card-item>
        </v-card>
      </v-col>
    </v-row>
    <v-row v-if="messageStore.unreadMessageCount > 0" justify="center">
      <v-col>
        <v-row>
          <v-col cols="12">
            <Alert
              :rounded="mdAndUp"
              :class="smAndDown ? 'mt-n4 mx-n4' : ''"
              icon="mdi-bell"
              ><UnreadMessages
            /></Alert>
          </v-col>
        </v-row>
      </v-col>
    </v-row>

    <!-- Your ECE applications -->
    <v-row
      v-if="applications && userStore.isVerified && showApplicationCard"
      justify="center"
    >
      <v-col>
        <v-row>
          <v-col cols="12">
            <ApplicationCard
              :class="smAndDown ? 'mx-n6' : ''"
              @cancel-application="showCancelDialog = true"
            />
          </v-col>
        </v-row>
      </v-col>
    </v-row>

    <!-- Your ECE certifications -->
    <v-row v-if="userStore.isVerified" justify="center" class="mt-6">
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
              <CertificationCard
                :class="smAndDown ? 'mx-n6 mt-4' : 'mt-4'"
                :is-rounded="false"
              />
            </div>
            <p v-else class="small mt-4">
              You do not have an ECE certificate in your My ECE Registry account.
            </p>
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

          <v-col v-if="userStore.isVerified" cols="12" sm="6" lg="4">
            <ActionCard title="Your profile" icon="mdi-account-circle">
              <template #content
                >Manage your names, address and contact information.</template
              >
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
      <p>
        By cancelling your application, it will be removed from the system. You cannot
        undo this.
      </p>
      <p><b>Are you sure you want to proceed?</b></p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent, onMounted } from "vue";
import { useDisplay } from "vuetify";
import { useRouter } from "vue-router";
import { cancelDraftApplication } from "@/api/application";
import { getUserInfo } from "@/api/user";
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

export default defineComponent({
  name: "Dashboard",
  components: {
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
    let applications, certifications, userInfo, userProfile;
    onMounted(async () => {
      let user;
      try {
        user = await oidcStore.getUser();
        if (!user) {
          user = await oidcStore.signinCallback();
          router.replace("/");
        }
      } catch (error) {}

      if (!user) {
        window.location.href = "/login";
      }

      [applications, certifications, userInfo, userProfile] = await Promise.all([
        applicationStore.fetchApplications(),
        certificationStore.fetchCertifications(),
        getUserInfo(),
        getProfile(),
      ]);

      if (userInfo !== null) {
        userStore.setUserInfo(userInfo);
        userStore.setUserProfile(userProfile);
      } else {
        router.push("/new-user");
      }
    });
    return {
      userStore,
      applicationStore,
      alertStore,
      loadingStore,
      messageStore,
      certificationStore,
      certifications,
      applications,
      smAndDown,
      mdAndUp,
      router,
    };
  },
  data: () => ({
    showCancelDialog: false,
    drawer: null as boolean | null | undefined,
  }),
  computed: {
    showApplicationCard(): boolean {
      if (
        this.certificationStore.hasCertifications &&
        this.applicationStore.applicationStatus === undefined
      ) {
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
        this.loadingStore.isLoading("profile_get") ||
        this.loadingStore.isLoading("userinfo_get") ||
        this.loadingStore.isLoading("certification_get") ||
        this.loadingStore.isLoading("application_get")
      );
    },
    showOptions(): boolean {
      return (
        this.certificationStore.hasCertifications &&
        !this.showApplicationCard &&
        !this.showLoading
      );
    },
  },

  methods: {
    formatPhoneNumber,
    async cancelApplication() {
      this.showCancelDialog = false;
      const { data: cancelledApplicationId } = await cancelDraftApplication(
        this.applicationStore.draftApplication.id!
      );
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
