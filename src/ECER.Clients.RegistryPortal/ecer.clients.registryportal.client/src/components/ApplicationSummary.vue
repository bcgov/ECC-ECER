<template>
  <v-container>
    <v-breadcrumbs class="breadcrumb" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>

    <ApplicationCertificationTypeHeader :certification-types="applicationStore.applications?.[0]?.certificationTypes || []" class="pb-5" />
    <h3>Status</h3>
    <div class="pb-3">It's a 3-step process to apply</div>
    <v-card elevation="0" color="#f8f8f8" class="border-top mt-5" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row no-gutters>
            <v-col cols="12"><strong>Step 1</strong></v-col>
            <v-col cols="12"><strong>Submit Application</strong></v-col>
          </v-row>
          <div>
            <v-icon icon="mdi-check" size="x-large" />
            Complete
          </div>
        </div>
      </v-card-text>
    </v-card>
    <v-card elevation="0" color="#003366" class="mt-5" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row no-gutters>
            <v-col cols="12"><strong>Step 2</strong></v-col>
            <v-col cols="12"><strong>References and documents</strong></v-col>
          </v-row>
          <div>
            <v-icon v-if="step2ReferenceDocumentIcon" :icon="step2ReferenceDocumentIcon" size="x-large" />
            {{ step2ReferenceDocumentText }}
          </div>
        </div>
      </v-card-text>
    </v-card>
    <v-card elevation="0" color="#f8f8f8" class="border-top mt-5" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row no-gutters>
            <v-col cols="12"><strong>Step 3</strong></v-col>
            <v-col cols="12"><strong>Ece Registry assessment</strong></v-col>
          </v-row>
          <div>
            <v-icon v-if="step3RegistryAssessmentIcon" :icon="step3RegistryAssessmentIcon" size="x-large" />
            {{ step3RegistryAssessmentText }}
          </div>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";
import { useDisplay } from "vuetify";

import { getApplicationStatus } from "@/api/application";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { CertificationType } from "@/utils/constant";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";

export default defineComponent({
  name: "ApplicationSummary",
  components: { ApplicationCertificationTypeHeader },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const route = useRoute();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();

    await applicationStore.fetchApplications();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    return { applicationStore, alertStore, CertificationType, applicationStatus, smAndUp };
  },
  data() {
    return {
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Application",
          disabled: true,
          href: "application",
        },
      ],
    };
  },
  computed: {
    step2ReferenceDocumentIcon() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return "mdi-arrow-right";
        case "Ready":
        case "PendingQueue":
        case "Escalated":
          return "mdi-check";
        default:
          return "";
      }
    },
    step2ReferenceDocumentText() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return "In Progress";
        case "Ready":
        case "PendingQueue":
        case "Escalated":
          return "Complete";
        default:
          return "";
      }
    },
    step3RegistryAssessmentIcon() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return "";
        case "Ready":
        case "PendingQueue":
          return "mdi-arrow-right";
        default:
          return "";
      }
    },
    step3RegistryAssessmentText() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return "Not yet started";
        case "Ready":
        case "PendingQueue":
          return "In progress";
        default:
          return "";
      }
    },
  },
});
</script>
<style scoped>
.border-top {
  border-top: 2px solid black;
}
.breadcrumb {
  padding-left: 0;
}

.breadcrumb li {
  padding-left: 0;
}
</style>
