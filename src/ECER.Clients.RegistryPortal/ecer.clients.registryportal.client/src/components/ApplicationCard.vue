<template>
  <v-card class="fill-height" flat color="primary">
    <v-card-item class="ma-4">
      <h3 class="text-white">{{ title }}</h3>
      <p class="small text-white mt-4">
        {{ subTitle }}
      </p>
    </v-card-item>
    <v-card-actions class="ma-4">
      <!-- Application status DRAFT -->
      <div v-if="applicationStore.applicationStatus === 'Draft'" class="d-flex flex-row justify-start ga-3 flex-wrap">
        <v-btn size="large" variant="flat" color="warning" @click="$router.push('/application')">
          <v-icon size="large" icon="mdi-arrow-right" />
          Open application
        </v-btn>
        <v-btn class="ma-0" size="large" variant="outlined" color="white" @click="$emit('cancel-application')">Cancel application</v-btn>
      </div>

      <!-- Application status Submitted -->
      <div v-if="applicationStore.applicationStatus === 'Submitted'" class="d-flex flex-row justify-start ga-3 flex-wrap">
        <v-btn variant="flat" size="large" color="warning" @click="handleManageApplication">
          <v-icon size="large" icon="mdi-arrow-right" />
          Manage Application
        </v-btn>
      </div>

      <!-- No application found -->
      <div v-if="applicationStore.applicationStatus === undefined" class="d-flex flex-row justify-start ga-3 flex-wrap">
        <v-btn variant="flat" size="large" color="warning" @click="handleStartNewApplication">
          <v-icon size="large" icon="mdi-arrow-right" />
          Apply Now
        </v-btn>
      </div>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "ApplicationCard",
  emits: ["cancel-application"],
  setup() {
    const applicationStore = useApplicationStore();

    return {
      applicationStore,
    };
  },
  computed: {
    title(): string {
      switch (this.applicationStore.application?.status) {
        case "Draft":
          return "Application in progress";
        case "Submitted":
          return "Application in progress";
        default:
          return "Apply for ECE Certification";
      }
    },
    subTitle(): string {
      switch (this.applicationStore.application?.status) {
        case "Draft":
          return `Started ${formatDate(this.applicationStore.application?.createdOn || "", "LLL dd, yyyy")}`;
        case "Submitted":
          return `Submitted ${formatDate(this.applicationStore.application?.submittedOn || "", "LLL dd, yyyy")}`;
        default:
          return "There are different types of certifications you can apply for. Visit the B.C. government website to learn about the types of Early Childhood Educator (ECE) certificates and which one you may qualify for.";
      }
    },
  },
  methods: {
    handleStartNewApplication() {
      this.applicationStore.upsertDraftApplication();
      this.$router.push("/application");
    },
    handleManageApplication() {
      this.$router.push("/manage-application");
    },
  },
});
</script>
