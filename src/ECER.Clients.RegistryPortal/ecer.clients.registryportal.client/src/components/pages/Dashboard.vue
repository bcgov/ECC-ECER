<template>
  <PageContainer>
    <v-row justify="center">
      <v-col cols="12" xl="8">
        <ApplicationCard v-if="applications && applications?.length > 0" @cancel-application="showCancelDialog = true" />
      </v-col>
    </v-row>
  </PageContainer>
  <ConfirmationDialog
    :cancel-button-text="'Keep Application'"
    :accept-button-text="'Cancel Application'"
    :title="'Cancel Application'"
    :show="showCancelDialog"
    @cancel="showCancelDialog = false"
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

import { cancelDraftApplication } from "@/api/application";
import ApplicationCard from "@/components/ApplicationCard.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "Dashboard",
  components: { ConfirmationDialog, PageContainer, ApplicationCard },
  async setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();

    const applications = await applicationStore.fetchApplications();

    return { userStore, applicationStore, alertStore, applications };
  },
  data: () => ({
    showCancelDialog: false,
    drawer: null as boolean | null | undefined,
  }),
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
