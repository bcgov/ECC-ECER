<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="primary">
    <v-card-item class="ma-4">
      <h2 class="text-white">{{ title }}</h2>
      <p class="small text-white mt-4">
        {{ subTitle }}
      </p>
    </v-card-item>

    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <!-- Application status Draft -->
      <v-card-actions v-if="applicationStore.applicationStatus === 'Draft' && applicationStore.applicationOrigin !== 'Manual'">
        <v-btn size="large" variant="flat" color="warning" @click="router.push('/application')">
          <v-icon size="large" icon="mdi-arrow-right" />
          Open application
        </v-btn>
        <v-btn class="ma-0" size="large" variant="outlined" color="white" @click="$emit('cancel-application')">Cancel application</v-btn>
      </v-card-actions>

      <!-- Application status Submitted, Ready, In Progress, Pending Queue -->
      <v-card-actions
        v-if="
          applicationStore.applicationStatus === 'Submitted' ||
          applicationStore.applicationStatus === 'Ready' ||
          applicationStore.applicationStatus === 'InProgress' ||
          applicationStore.applicationStatus === 'PendingQueue' ||
          applicationStore.applicationStatus === 'Pending' ||
          applicationStore.applicationStatus === 'Escalated'
        "
      >
        <v-btn variant="flat" size="large" color="warning" @click="handleManageApplication">
          <v-icon size="large" icon="mdi-arrow-right" />
          Manage Application
        </v-btn>
      </v-card-actions>

      <!-- No application found -->
      <v-card-actions v-if="applicationStore.applicationStatus === undefined">
        <v-btn variant="flat" size="large" color="warning" id="btnApplyNow" @click="handleStartNewApplication">
          <v-icon size="large" icon="mdi-arrow-right" />
          Apply now
        </v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import { formatDate } from "@/utils/format";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "ApplicationCard",
  props: {
    isRounded: {
      type: Boolean,
      default: true,
    },
  },
  emits: ["cancel-application"],
  setup() {
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return {
      applicationStore,
      router,
    };
  },
  computed: {
    title(): string {
      switch (this.applicationStore.application?.status) {
        case "Draft":
        case "Submitted":
        case "InProgress":
        case "Escalated":
        case "PendingQueue":
        case "Ready":
        case "Pending":
          return "Application in progress";
        default:
          return "Apply for ECE Certification";
      }
    },
    subTitle(): string {
      switch (this.applicationStore.application?.status) {
        case "Draft":
          return `Started ${formatDate(this.applicationStore.application?.createdOn || "", "LLLL d, yyyy")}`;
        case "Submitted":
        case "InProgress":
        case "Escalated":
        case "PendingQueue":
        case "Ready":
        case "Pending":
          return `Submitted ${formatDate(this.applicationStore.application?.submittedOn || "", "LLLL d, yyyy")}`;
        default:
          return "There are different types of certifications you can apply for. Visit the B.C. government website to learn about the types of Early Childhood Educator (ECE) certificates and which one you may qualify for.";
      }
    },
  },
  methods: {
    handleStartNewApplication() {
      this.router.push({ name: "application-certification" });
    },
    handleManageApplication() {
      this.router.push({ name: "manageApplication", params: { applicationId: this.applicationStore?.application?.id } });
    },
  },
});
</script>
