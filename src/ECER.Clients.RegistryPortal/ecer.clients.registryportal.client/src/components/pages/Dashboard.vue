<template>
  <!-- Messages -->
  <PageContainer :margin-top="false">
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
    <v-row v-if="applications && showApplicationCard" justify="center">
      <v-col>
        <v-row>
          <v-col cols="12">
            <ApplicationCard :class="smAndDown ? 'mx-n6' : ''" @cancel-application="showCancelDialog = true" />
          </v-col>
        </v-row>
      </v-col>
    </v-row>

    <!-- Your ECE certifications -->
    <v-row justify="center" class="mt-6">
      <v-col>
        <v-row>
          <v-col cols="12">
            <ECEHeader title="Your ECE certifications" />
            <div v-if="certificationStore.hasCertifications">
              <p class="mt-4">
                ECE registration number
                <b>{{ certificationStore.latestCertification?.number }}</b>
              </p>
              <CerticationCard :class="smAndDown ? 'mx-n6 mt-4' : 'mt-4'" :is-rounded="false" />
            </div>
            <p v-else class="small mt-4">You do not have an ECE certificate in your My ECE Registry account.</p>
          </v-col>
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
            <ActionCard
              title="Messages"
              icon="mdi-bell"
              :links="[
                {
                  text: 'Read messages',
                  to: '/messages',
                },
              ]"
            >
              <UnreadMessages :linkable="false" />
            </ActionCard>
          </v-col>
          <v-col cols="12" sm="6" lg="4">
            <ActionCard
              title="Your profile"
              icon="mdi-account-circle"
              :links="[
                {
                  text: 'My profile',
                  to: '/profile',
                },
              ]"
            >
              Manage your names, address and contact information.
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
      <p>By cancelling your application, it will be removed from the system. You cannot undo this.</p>
      <p><b>Are you sure you want to proceed?</b></p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { cancelDraftApplication } from "@/api/application";
import { getCertificateFileById } from "@/api/certification";
import { getUserInfo } from "@/api/user";
import ActionCard from "@/components/ActionCard.vue";
import Alert from "@/components/Alert.vue";
import ApplicationCard from "@/components/ApplicationCard.vue";
import CerticationCard from "@/components/CertificationCard.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import UnreadMessages from "@/components/UnreadMessages.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useMessageStore } from "@/store/message";
import { useUserStore } from "@/store/user";
import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "Dashboard",
  components: { ConfirmationDialog, PageContainer, ApplicationCard, CerticationCard, ECEHeader, ActionCard, Alert, UnreadMessages },
  async setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    const alertStore = useAlertStore();
    const messageStore = useMessageStore();
    const { smAndDown, mdAndUp } = useDisplay();

    const applications = await applicationStore.fetchApplications();
    const certifications = await certificationStore.fetchCertifications();
    if (certifications && certifications.length > 0) {
      const file = await getCertificateFileById(certifications[0].id ?? "");
      console.log(file);
    }

    // Refresh userInfo from the server
    const userInfo = await getUserInfo();
    if (userInfo !== null) {
      userStore.setUserInfo(userInfo);
    }

    return { userStore, applicationStore, alertStore, messageStore, certificationStore, applications, certifications, smAndDown, mdAndUp };
  },
  data: () => ({
    showCancelDialog: false,
    drawer: null as boolean | null | undefined,
  }),
  computed: {
    showApplicationCard(): boolean {
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
