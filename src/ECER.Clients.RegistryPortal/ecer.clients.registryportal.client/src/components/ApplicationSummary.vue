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
        <p>Completed on {{ formatDate(applicationStatus?.submittedOn as string, "LLLL d, yyyy") }}</p>
      </v-card-text>
    </v-card>
    <!-- Step 1 End-->
    <!-- Step 2 Start-->
    <v-card elevation="0" :color="currentStep === 2 ? 'primary' : 'white-smoke'" class="mt-5" :class="[{ 'border-top': currentStep !== 2 }]" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row>
            <v-col cols="12"><h3>Step 2</h3></v-col>
            <v-col cols="12"><h3>References and documents</h3></v-col>
          </v-row>
          <p class="large">
            <v-icon v-if="stepTwoIcon" :icon="stepTwoIcon" />
            {{ stepTwoStatusText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <div v-if="currentStep === 2">
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
        v-for="(previousName, index) in userStore.unverifiedPreviousNames"
        :key="index"
        :text="`Proof of previous name ${previousName.firstName} ${previousName.lastName}`"
        :go-to="() => $router.push({ name: 'profile' })"
      />
      <ApplicationSummaryActionListItem
        v-if="showWorkExperience"
        :active="totalObservedWorkExperienceHours < 500"
        text="500 approved hours of work experience with reference"
        :go-to="() => $router.push({ name: 'manageWorkExperienceReferences', params: { applicationId: $route.params.applicationId } })"
      />
    </div>
    <v-card v-if="currentStep === 3" elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <p>Completed on {{ formatDate(applicationStatus?.readyForAssessmentDate as string, "LLLL d, yyyy") }}</p>
      </v-card-text>
    </v-card>
    <!-- Step 2 End-->
    <!-- Step 3 Start-->
    <v-card elevation="0" :color="currentStep === 3 ? 'primary' : 'white-smoke'" class="mt-5" :class="[{ 'border-top': currentStep !== 3 }]" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row>
            <v-col cols="12"><h3>Step 3</h3></v-col>
            <v-col cols="12"><h3>ECE Registry assessment</h3></v-col>
          </v-row>
          <p class="large">
            <v-icon v-if="stepThreeIcon" :icon="stepThreeIcon" />
            {{ stepThreeStatusText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <v-card v-if="!hasStepThreeTasks" elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <p v-if="currentStep !== 3">We'll review your application after we receive all references and documents.</p>
        <p v-if="currentStep === 3 && stepThreeStatusText !== 'Action required'">
          We're reviewing your application. We'll contact you with questions or once assessment is complete.
        </p>
        <p v-if="currentStep === 3 && stepThreeStatusText === 'Action required'">
          We’re waiting for additional information before we continue to review your application.
          <router-link to="/messages">Read your messages</router-link>
          or
          <a href="https://www2.gov.bc.ca/gov/content?id=9376DE7539D44C64B3E667DB53320E71">contact us</a>
          for more information.
        </p>
      </v-card-text>
    </v-card>
    <div v-if="currentStep === 3 && hasStepThreeTasks">
      <v-card elevation="0" rounded="0" class="border-t border-b">
        <v-card-text>
          <p>You need to provide the following items.</p>
        </v-card-text>
      </v-card>
      <ApplicationSummaryTranscriptListItem
        v-for="transcript in waitingForDetailsTranscripts"
        :key="transcript.id?.toString()"
        :name="transcript.educationalInstitutionName"
        :status="transcript.status"
      />
      <ApplicationSummaryActionListItem
        v-if="!hasCharacterReference"
        text="Add character reference"
        :go-to="() => $router.push({ name: 'addCharacterReference', params: { applicationId: $route.params.applicationId } })"
      />
      <ApplicationSummaryCharacterReferenceListItem
        v-for="reference in waitingForResponseCharacterReferences"
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
        v-if="addMoreWorkExperienceReferencesFlag"
        text="500 approved hours of work experience with reference"
        :go-to="() => $router.push({ name: 'manageWorkExperienceReferences', params: { applicationId: $route.params.applicationId } })"
      />
    </div>
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
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";
import { formatDate } from "@/utils/format";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ApplicationSummaryActionListItem from "./ApplicationSummaryActionListItem.vue";
import ApplicationSummaryCharacterReferenceListItem from "./ApplicationSummaryCharacterReferenceListItem.vue";
import ApplicationSummaryTranscriptListItem from "./ApplicationSummaryTranscriptListItem.vue";

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
    const userStore = useUserStore();

    await applicationStore.fetchApplications();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    return {
      applicationStore,
      userStore,
      alertStore,
      CertificationType,
      applicationStatus,
      smAndUp,
      formatDate,
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
    currentStep() {
      switch (this.applicationStatus?.status) {
        case "InProgress":
        case "Escalated":
        case "PendingQueue":
        case "Ready":
        case "Pending":
          return 3;
        case "Submitted":
          return 2;
        default:
          console.warn(`This should not happen, unmapped application status ${this.applicationStatus?.status}`);
          return 1;
      }
    },
    stepTwoStatusText() {
      switch (this.currentStep) {
        case 2:
          return "In progress";
        case 3:
          return "Complete";
        default:
          return "";
      }
    },
    stepThreeStatusText() {
      switch (this.currentStep) {
        case 2:
          return "Not yet started";
        case 3:
          if (
            this.applicationStatus?.status === "PendingQueue" &&
            (this.applicationStatus?.subStatus === "MoreInformationRequired" || this.applicationStatus.subStatus === "PendingDocuments")
          ) {
            return "Action required";
          } else {
            return "In progress";
          }
        default:
          return "";
      }
    },
    stepTwoIcon() {
      switch (this.stepTwoStatusText) {
        case "In progress":
          return "mdi-arrow-right";
        case "Complete":
          return "mdi-check";
        default:
          return "";
      }
    },
    stepThreeIcon() {
      switch (this.stepThreeStatusText) {
        case "Not yet started":
          return "";
        case "In progress":
          return "mdi-arrow-right";
        case "Action required":
          return "mdi-alert-circle";
        default:
          return "";
      }
    },
    hasCharacterReference(): boolean {
      return this.applicationStatus?.characterReferencesStatus?.some((reference) => reference.status !== "Rejected") || false;
    },
    addMoreWorkExperienceReferencesFlag(): boolean {
      return this.applicationStatus?.addMoreWorkExperienceReference ?? false;
    },
    hasWaitingForDetailsTranscript(): boolean {
      return this.applicationStatus?.transcriptsStatus?.some((transcript) => transcript.status === "WaitingforDetails") || false;
    },
    waitingForDetailsTranscripts(): Components.Schemas.TranscriptStatus[] {
      return this.applicationStatus?.transcriptsStatus?.filter((transcript) => transcript.status === "WaitingforDetails") || [];
    },
    waitingForResponseCharacterReferences(): Components.Schemas.CharacterReferenceStatus[] {
      return (
        this.applicationStatus?.characterReferencesStatus?.filter((reference) => reference.status === "Draft" || reference.status === "ApplicationSubmitted") ||
        []
      );
    },
    hasStepThreeTasks(): boolean {
      return (
        !this.hasCharacterReference ||
        this.waitingForResponseCharacterReferences.length > 0 ||
        this.addMoreWorkExperienceReferencesFlag ||
        this.hasWaitingForDetailsTranscript
      );
    },
    totalObservedWorkExperienceHours(): number {
      return this.applicationStatus?.workExperienceReferencesStatus?.reduce((acc, reference) => acc + (reference.totalNumberofHoursObserved ?? 0), 0) || 0;
    },
    showWorkExperience(): boolean {
      return !!this.applicationStatus?.certificationTypes?.includes(CertificationType.FIVE_YEAR);
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
