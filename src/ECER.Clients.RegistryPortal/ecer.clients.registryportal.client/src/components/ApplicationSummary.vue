<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>

    <ApplicationCertificationTypeHeader :certification-types="applicationStore.applications?.[0]?.certificationTypes || []" class="pb-5" />
    <h2>Status</h2>
    <p class="pb-3">It's a 3-step process to apply.</p>
    <!-- Step 1 Start-->
    <v-card elevation="0" color="white-smoke" class="border-top mt-5" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row>
            <v-col cols="12"><h3>Step 1</h3></v-col>
            <v-col cols="12"><h3>Submit application</h3></v-col>
          </v-row>
          <p class="large">
            <v-icon icon="mdi-check" />
            Complete
          </p>
        </div>
      </v-card-text>
    </v-card>
    <v-card elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <p>Completed on {{ formatDate(applicationStatus?.submittedOn as string, "LLL d, yyyy") }}</p>
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
          <v-row>
            <v-col cols="12"><h3>Step 2</h3></v-col>
            <v-col cols="12"><h3>References and documents</h3></v-col>
          </v-row>
          <p class="large">
            <v-icon v-if="step2ReferenceDocumentIcon" :icon="step2ReferenceDocumentIcon" />
            {{ step2ReferenceDocumentText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <div v-if="step2Progress !== COMPLETE">
      <ApplicationSummaryTranscriptListItem
        v-for="transcript in applicationStatus?.transcriptsStatus"
        :key="transcript.id?.toString()"
        :name="transcript.educationalInstitutionName"
        :status="transcript.status"
      />
      <ApplicationSummaryCharacterReferenceListItem
        v-for="reference in applicationStatus?.characterReferencesStatus"
        :key="reference.id?.toString()"
        :name="`${reference.firstName} ${reference.lastName}`"
        :status="reference.status"
        :go-to="
          () =>
            $router.push({
              name: 'viewCharacterReference',
              params: { applicationId: $route.params.applicationId, referenceId: reference.id?.toString() },
            })
        "
        :will-provide-reference="reference.willProvideReference ? true : false"
      />
      <ApplicationSummaryActionListItem
        v-if="!hasCharacterReference"
        text="Add character reference"
        :go-to="() => $router.push({ name: 'addCharacterReference', params: { applicationId: $route.params.applicationId } })"
      />
      <ApplicationSummaryActionListItem
        :active="totalObservedWorkExperienceHours < 500"
        text="500 approved hours of work experience with reference"
        :go-to="() => $router.push({ name: 'manageWorkExperienceReferences', params: { applicationId: $route.params.applicationId } })"
      />
    </div>
    <v-card v-if="step2Progress === COMPLETE" elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <p>Completed on {{ formatDate(applicationStatus?.readyForAssessmentDate as string, "LLL d, yyyy") }}</p>
      </v-card-text>
    </v-card>
    <!-- Step 2 End-->
    <!-- Step 3 Start-->
    <v-card
      elevation="0"
      :color="step3Progress === IN_PROGRESS || step3Progress === ACTION_REQUIRED ? 'primary' : 'white-smoke'"
      class="mt-5"
      :class="[{ 'border-top': step3Progress !== IN_PROGRESS }]"
      rounded="0"
    >
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row>
            <v-col cols="12"><h3>Step 3</h3></v-col>
            <v-col cols="12"><h3>ECE Registry assessment</h3></v-col>
          </v-row>
          <p class="large">
            <v-icon v-if="step3RegistryAssessmentIcon" :icon="step3RegistryAssessmentIcon" />
            {{ step3RegistryAssessmentText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <v-card elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <p v-if="step3Progress === NOT_STARTED">We'll review your application after we receive all references and documents.</p>
        <p v-if="step3Progress === IN_PROGRESS">We're reviewing your application. We'll contact you with questions or once assessment is complete.</p>
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
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";
import { formatDate } from "@/utils/format";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ApplicationSummaryActionListItem from "./ApplicationSummaryActionListItem.vue";
import ApplicationSummaryCharacterReferenceListItem from "./ApplicationSummaryCharacterReferenceListItem.vue";
import ApplicationSummaryTranscriptListItem from "./ApplicationSummaryTranscriptListItem.vue";

type ApplicationStepProgress = "complete" | "inProgress" | "actionRequired" | "notStarted";
type ApplicationProcessMap = {
  [key in Components.Schemas.ApplicationStatus]:
    | { [key in Components.Schemas.ApplicationStatusReasonDetail]?: ApplicationStepProgress }
    | ApplicationStepProgress
    | undefined;
};

const Step2ApplicationStatusSubDetailMap: ApplicationProcessMap = {
  Complete: undefined,
  InProgress: { BeingAssessed: "complete" },
  Draft: undefined,
  Submitted: { PendingDocuments: "inProgress", ValidatingIDs: "inProgress", ReceivePhysicalTranscripts: "inProgress" },
  Reconsideration: { Denied: undefined, Certified: undefined },
  Cancelled: undefined,
  Escalated: "complete",
  Decision: { Certified: undefined, Denied: undefined },
  Withdrawn: undefined,
  Ready: { ReadyforAssessment: "complete" },
  PendingQueue: {
    MoreInformationRequired: "complete",
    PendingDocuments: "complete",
    SupervisorConsultationNeeded: "complete",
    OperationSupervisorManagerofCertificationsConsultationNeeded: "complete",
    InvestigationsConsultationNeeded: "complete",
  },
  ReconsiderationDecision: undefined,
  Pending: undefined,
  AppealDecision: undefined,
};

const Step3ApplicationStatusSubDetailMap: ApplicationProcessMap = {
  Complete: undefined,
  InProgress: { BeingAssessed: "inProgress" },
  Draft: undefined,
  Submitted: { PendingDocuments: "notStarted", ValidatingIDs: "notStarted", ReceivePhysicalTranscripts: "notStarted" },
  Reconsideration: { Denied: undefined, Certified: undefined },
  Cancelled: undefined,
  Escalated: "inProgress",
  Decision: { Certified: undefined, Denied: undefined },
  Withdrawn: undefined,
  Ready: { ReadyforAssessment: "inProgress" },
  PendingQueue: {
    MoreInformationRequired: "actionRequired",
    PendingDocuments: "actionRequired",
    SupervisorConsultationNeeded: "inProgress",
    OperationSupervisorManagerofCertificationsConsultationNeeded: "inProgress",
    InvestigationsConsultationNeeded: "inProgress",
  },
  ReconsiderationDecision: undefined,
  Pending: undefined,
  AppealDecision: undefined,
};

export default defineComponent({
  name: "ApplicationSummary",
  components: {
    ApplicationCertificationTypeHeader,
    ApplicationSummaryTranscriptListItem,
    ApplicationSummaryCharacterReferenceListItem,
    ApplicationSummaryActionListItem,
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const route = useRoute();
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();

    await applicationStore.fetchApplications();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;
    const IN_PROGRESS: ApplicationStepProgress = "inProgress";
    const NOT_STARTED: ApplicationStepProgress = "notStarted";
    const COMPLETE: ApplicationStepProgress = "complete";
    const ACTION_REQUIRED: ApplicationStepProgress = "actionRequired";

    return {
      applicationStore,
      alertStore,
      CertificationType,
      applicationStatus,
      smAndUp,
      formatDate,
      IN_PROGRESS,
      NOT_STARTED,
      COMPLETE,
      ACTION_REQUIRED,
    };
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
          return "In progress";
        case this.COMPLETE:
          return "Complete";
        default:
          return "";
      }
    },
    step2Progress() {
      return this.findApplicationStepProgress(Step2ApplicationStatusSubDetailMap, "step2");
    },
    step3RegistryAssessmentIcon() {
      switch (this.step3Progress) {
        case this.NOT_STARTED:
          return "";
        case this.IN_PROGRESS:
          return "mdi-arrow-right";
        case this.ACTION_REQUIRED:
          return "mdi-alert-circle";
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
        case this.ACTION_REQUIRED:
          return "Action required";
        default:
          return "";
      }
    },
    step3Progress() {
      return this.findApplicationStepProgress(Step3ApplicationStatusSubDetailMap, "step3");
    },
    hasCharacterReference(): boolean {
      return this.applicationStatus?.characterReferencesStatus?.some((reference) => reference.status !== "Rejected") || false;
    },
    totalObservedWorkExperienceHours(): number {
      return this.applicationStatus?.workExperienceReferencesStatus?.reduce((acc, reference) => acc + (reference.totalNumberofHoursObserved ?? 0), 0) || 0;
    },
  },
  methods: {
    goTo(id: string | undefined) {
      this.alertStore.setSuccessAlert("not implemented yet this will go to another route " + id);
    },
    findApplicationStepProgress(applicationProcessMap: ApplicationProcessMap, step: string) {
      const status = this.applicationStatus?.status;
      const statusReasonDetail = this.applicationStatus?.subStatus;

      if (status) {
        const subDetail = applicationProcessMap[status];

        //this checks if we have found the ApplicationStepProgress.
        if (typeof subDetail === "string") {
          return subDetail;
        }

        if (subDetail && statusReasonDetail) {
          if (subDetail[statusReasonDetail]) {
            return subDetail[statusReasonDetail];
          }
        }
      }

      console.warn(
        `This should not happen, unmapped status and statusReasonDetail combination please check ApplicationStatusSubDetailMap for ${step} :: status: ${status} statusReasonDetail: ${statusReasonDetail}`,
      );
      return "";
    },
  },
});
</script>
<style scoped>
.border-top {
  border-top: 2px solid black;
}
</style>
