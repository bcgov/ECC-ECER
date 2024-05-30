<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>

    <ApplicationCertificationTypeHeader :certification-types="applicationStore.applications?.[0]?.certificationTypes || []" class="pb-5" />
    <h3>Status</h3>
    <div class="pb-3">It's a 3-step process to apply</div>
    <!-- Step 1 Start-->
    <v-card elevation="0" color="white-smoke" class="border-top mt-5" rounded="0">
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
    <v-card elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <div>Completed on {{ formatDate(applicationStatus?.submittedOn as string, "LLL d, yyyy") }}</div>
      </v-card-text>
    </v-card>
    <!-- Step 1 End-->
    <!-- Step 2 Start-->
    <v-card
      elevation="0"
      :color="step2Progress === IN_PROGRESS ? 'primary' : 'white-smoke'"
      class="mt-5"
      :class="[{ 'border-top': step2Progress !== IN_PROGRESS }]"
      rounded="0"
    >
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
    <ApplicationSummaryTranscriptReferenceListItem
      v-for="transcript in applicationStatus?.transcriptsStatus"
      :key="transcript.id?.toString()"
      :name="transcript.educationalInstitutionName"
      type="transcript"
      :status="transcript.status"
      :go-to="() => goTo(transcript.id?.toString())"
    />
    <ApplicationSummaryTranscriptReferenceListItem
      v-for="reference in applicationStatus?.characterReferencesStatus"
      :key="reference.id?.toString()"
      :name="`${reference.firstName} ${reference.lastName}`"
      type="character"
      :status="reference.status"
      :go-to="
        () =>
          $router.push({
            name: 'viewCharacterReference',
            params: { applicationId: $route.params.applicationId, referenceId: reference.id?.toString() },
          })
      "
      :will-provide-reference="reference.willProvideReference"
    />
    <ApplicationSummaryTranscriptReferenceListItem
      v-for="reference in applicationStatus?.workExperienceReferencesStatus"
      :key="reference.id?.toString()"
      :name="`${reference.firstName} ${reference.lastName}`"
      type="workExperience"
      :status="reference.status"
      :go-to="
        () =>
          $router.push({
            name: 'viewWorkExperienceReference',
            params: { applicationId: $route.params.applicationId, referenceId: reference.id?.toString() },
          })
      "
      :will-provide-reference="reference.willProvideReference"
    />
    <!-- Step 2 End-->
    <!-- Step 3 Start-->
    <v-card
      elevation="0"
      :color="step3Progress === IN_PROGRESS ? 'primary' : 'white-smoke'"
      class="mt-5"
      :class="[{ 'border-top': step3Progress !== IN_PROGRESS }]"
      rounded="0"
    >
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
    <v-card elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <div v-if="step3Progress === NOT_STARTED">We'll review your application after we receive all references and documents.</div>
        <div v-if="step3Progress === IN_PROGRESS">We're reviewing your application. We'll contact you with questions or once assessment is complete.</div>
      </v-card-text>
    </v-card>
    <!-- Step 3 End-->
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
import { formatDate } from "@/utils/format";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ApplicationSummaryTranscriptReferenceListItem from "./ApplicationSummaryTranscriptReferenceListItem.vue";

export default defineComponent({
  name: "ApplicationSummary",
  components: { ApplicationCertificationTypeHeader, ApplicationSummaryTranscriptReferenceListItem },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const route = useRoute();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();

    await applicationStore.fetchApplications();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;
    const IN_PROGRESS = "inProgress";
    const NOT_STARTED = "notStarted";
    const COMPLETE = "complete";

    return { applicationStore, alertStore, CertificationType, applicationStatus, smAndUp, formatDate, IN_PROGRESS, NOT_STARTED, COMPLETE };
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
      switch (this.step2Progress) {
        case this.IN_PROGRESS:
          return "mdi-arrow-right";
        case this.COMPLETE:
          return "mdi-check";
        default:
          return "";
      }
    },
    step2ReferenceDocumentText() {
      switch (this.step2Progress) {
        case this.IN_PROGRESS:
          return "In Progress";
        case this.COMPLETE:
          return "Complete";
        default:
          return "";
      }
    },
    step2Progress() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return this.IN_PROGRESS;
        case "Ready":
        case "PendingQueue":
        case "Escalated":
          return this.COMPLETE;
        default:
          return "";
      }
    },
    step3RegistryAssessmentIcon() {
      switch (this.step3Progress) {
        case this.NOT_STARTED:
          return "";
        case this.IN_PROGRESS:
          return "mdi-arrow-right";
        default:
          return "";
      }
    },
    step3RegistryAssessmentText() {
      switch (this.step3Progress) {
        case this.NOT_STARTED:
          return "Not yet started";
        case this.IN_PROGRESS:
          return "In progress";
        default:
          return "";
      }
    },
    step3Progress() {
      switch (this.applicationStatus?.status) {
        case "Submitted":
        case "InProgress":
          return this.NOT_STARTED;
        case "Ready":
        case "PendingQueue":
          return this.IN_PROGRESS;
        default:
          return "";
      }
    },
  },
  methods: {
    goTo(id: string | undefined) {
      this.alertStore.setSuccessAlert("not implemented yet this will go to another route " + id);
    },
  },
});
</script>
<style scoped>
.border-top {
  border-top: 2px solid black;
}
</style>
