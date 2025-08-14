<template>
  <v-container>
    <Breadcrumb />
    <ApplicationCertificationTypeHeader :certification-types="applicationStore.applications?.[0]?.certificationTypes || []" class="pb-5" />
    <h2>Status</h2>
    <p class="pb-3">It is a 3-step process to apply.</p>
    <!-- Step 1 Start-->
    <v-card elevation="0" color="white-smoke" class="border-top mt-5" rounded="0">
      <v-card-text>
        <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
          <v-row>
            <v-col cols="12"><h3 :class="currentStep === 1 ? 'white' : ''">Step 1</h3></v-col>
            <v-col cols="12"><h3 :class="currentStep === 1 ? 'white' : ''">Submit application</h3></v-col>
          </v-row>
          <p class="large" :class="currentStep === 1 ? 'white' : ''">
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
            <v-col cols="12"><h3 :class="currentStep === 2 ? 'white' : ''">Step 2</h3></v-col>
            <v-col cols="12"><h3 :class="currentStep === 2 ? 'white' : ''">References and documents</h3></v-col>
          </v-row>
          <p class="large" :class="currentStep === 2 ? 'white' : ''">
            <v-icon v-if="stepTwoIcon" :icon="stepTwoIcon" />
            {{ stepTwoStatusText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <div v-if="currentStep === 2">
      <template v-for="transcript in applicationStatus?.transcriptsStatus" :key="transcript.id?.toString()">
        <ApplicationSummaryHeader :text="getTranscriptHeaderString(transcript)" />
        <ApplicationSummaryActionListItem
          :active="!transcript.transcriptReceivedByRegistry"
          :text="`Transcript: ${transcript.educationalInstitutionName}`"
          :go-to="() => router.push({ name: 'viewTranscriptDetails', params: { applicationId: route.params.applicationId, transcriptId: transcript.id } })"
        />
        <ApplicationSummaryActionListItem
          v-if="transcript.educationRecognition === 'NotRecognized'"
          :active="!transcript.courseOutlineReceivedByRegistry"
          :text="`Course outlines or syllabi: ${transcript.educationalInstitutionName}`"
          :go-to="() => router.push({ name: 'viewCourseOutline', params: { applicationId: route.params.applicationId, transcriptId: transcript.id } })"
        />
        <ApplicationSummaryActionListItem
          v-if="transcript.educationRecognition === 'NotRecognized' && !applicationStatus?.certificationTypes?.includes('EceAssistant')"
          :active="!transcript.programConfirmationReceivedByRegistry"
          :text="`Program Confirmation Form: ${transcript.educationalInstitutionName}`"
          :go-to="() => router.push({ name: 'viewProgramConfirmation', params: { applicationId: route.params.applicationId, transcriptId: transcript.id } })"
        />
        <ApplicationSummaryActionListItem
          v-if="
            transcript.educationRecognition === 'NotRecognized' &&
            transcript.country?.countryName?.toLowerCase() !== configStore.canada?.countryName?.toLowerCase()
          "
          :active="!transcript.comprehensiveReportReceivedByRegistry"
          :text="`Comprehensive Report: ${transcript.educationalInstitutionName}`"
          :go-to="() => router.push({ name: 'viewComprehensiveReport', params: { applicationId: route.params.applicationId, transcriptId: transcript.id } })"
        />
      </template>
      <ApplicationSummaryHeader text="References" />
      <ApplicationSummaryActionListItem
        v-if="showWorkExperience"
        :active="totalObservedWorkExperienceHours < totalRequiredWorkExperienceHours"
        :text="`${totalRequiredWorkExperienceHours} hours of work experience with reference`"
        :go-to="() => router.push({ name: 'manageWorkExperienceReferences', params: { applicationId: route.params.applicationId } })"
      />
      <ApplicationSummaryCharacterReferenceListItem
        v-for="reference in applicationStatus?.characterReferencesStatus"
        :key="reference.id?.toString()"
        :name="cleanPreferredName(reference.firstName, reference.lastName)"
        :status="reference.status"
        :go-to="
          () =>
            router.push({
              name: 'view-character-reference',
              params: { applicationId: route.params.applicationId, referenceId: reference.id?.toString() },
            })
        "
        :will-provide-reference="reference.willProvideReference ? true : false"
      />
      <ApplicationSummaryActionListItem
        v-if="!hasCharacterReference"
        text="Add character reference"
        :go-to="() => router.push({ name: 'addCharacterReference', params: { applicationId: route.params.applicationId } })"
      />
      <ApplicationSummaryHeader v-if="showOtherInformation" text="Other information" />
      <ApplicationSummaryActionListItem
        v-for="(previousName, index) in userStore.unverifiedPreviousNames"
        :key="index"
        :text="`Proof of previous name  ${cleanPreferredName(previousName.firstName, previousName.lastName)}`"
        :go-to="() => router.push({ name: 'profile' })"
      />
      <ApplicationSummaryActionListItem
        v-for="(previousName, index) in userStore.pendingforDocumentsPreviousNames"
        :key="index"
        :text="`Proof of previous name ${cleanPreferredName(previousName.firstName, previousName.lastName)}`"
        :go-to="() => router.push({ name: 'profile' })"
      />
      <ApplicationSummaryActionListItem
        v-for="(previousName, index) in userStore.readyForVerificationPreviousNames"
        :key="index"
        :text="`Proof of previous name ${cleanPreferredName(previousName.firstName, previousName.lastName)}`"
        :go-to="() => router.push({ name: 'profile' })"
        :active="false"
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
            <v-col cols="12"><h3 :class="currentStep === 3 ? 'white' : ''">Step 3</h3></v-col>
            <v-col cols="12"><h3 :class="currentStep === 3 ? 'white' : ''">ECE Registry assessment</h3></v-col>
          </v-row>
          <p class="large" :class="currentStep === 3 ? 'white' : ''">
            <v-icon v-if="stepThreeIcon" :icon="stepThreeIcon" />
            {{ stepThreeStatusText }}
          </p>
        </div>
      </v-card-text>
    </v-card>
    <div v-if="currentStep !== 3">
      <v-card elevation="0" rounded="0" class="border-t border-b">
        <v-card-text>
          <p>We will review your application after we receive all references and documents.</p>
        </v-card-text>
      </v-card>
    </div>
    <div v-if="currentStep === 3">
      <v-card v-if="!hasStepThreeTasks" elevation="0" rounded="0" class="border-t border-b">
        <v-card-text>
          <p v-if="stepThreeStatusText !== 'Action required'">
            We are reviewing your application. We will contact you with questions or once assessment is complete.
          </p>
          <p v-if="stepThreeStatusText === 'Action required'">
            We are waiting for additional information before we continue to review your application.
            <router-link to="/messages">Read your messages</router-link>
            or
            <a href="https://www2.gov.bc.ca/gov/content?id=9376DE7539D44C64B3E667DB53320E71">contact us</a>
            for more information.
          </p>
        </v-card-text>
      </v-card>
      <div v-if="hasStepThreeTasks">
        <v-card elevation="0" rounded="0" class="border-t border-b">
          <v-card-text>
            <p>You need to provide the following items.</p>
          </v-card-text>
        </v-card>
        <template v-for="transcript in waitingForDetailsTranscripts" :key="transcript.id?.toString()">
          <ApplicationSummaryHeader :text="getTranscriptHeaderString(transcript)" />
          <ApplicationSummaryActionListItem
            :active="!transcript.transcriptReceivedByRegistry"
            :text="`Transcript: ${transcript.educationalInstitutionName}`"
            :go-to="() => router.push({ name: 'manageTranscript', params: { applicationId: route.params.applicationId } })"
          />
          <ApplicationSummaryActionListItem
            v-if="transcript.educationRecognition === 'NotRecognized'"
            :active="!transcript.courseOutlineReceivedByRegistry"
            :text="`Course outlines or syllabi: ${transcript.educationalInstitutionName}`"
            :go-to="() => router.push({ name: 'manageCourseOutline', params: { applicationId: route.params.applicationId } })"
          />
          <ApplicationSummaryActionListItem
            v-if="transcript.educationRecognition === 'NotRecognized' && !applicationStatus?.certificationTypes?.includes('EceAssistant')"
            :active="!transcript.programConfirmationReceivedByRegistry"
            :text="`Program Confirmation Form: ${transcript.educationalInstitutionName}`"
            :go-to="() => router.push({ name: 'manageTranscript', params: { applicationId: route.params.applicationId } })"
          />
          <ApplicationSummaryActionListItem
            v-if="
              transcript.educationRecognition === 'NotRecognized' &&
              transcript.country?.countryName?.toLowerCase() !== configStore.canada?.countryName?.toLowerCase()
            "
            :active="!transcript.comprehensiveReportReceivedByRegistry"
            :text="`Comprehensive Report: ${transcript.educationalInstitutionName}`"
            :go-to="() => router.push({ name: 'manageComprehensiveReport', params: { applicationId: route.params.applicationId } })"
          />
        </template>
        <ApplicationSummaryHeader
          v-if="
            !hasCharacterReference ||
            waitingForResponseCharacterReferences.length > 0 ||
            addMoreProfessionalDevelopmentFlag ||
            addMoreWorkExperienceReferencesFlag
          "
          text="References"
        />
        <ApplicationSummaryActionListItem
          v-if="!hasCharacterReference"
          text="Add character reference"
          :go-to="() => router.push({ name: 'addCharacterReference', params: { applicationId: route.params.applicationId } })"
        />
        <ApplicationSummaryCharacterReferenceListItem
          v-for="reference in waitingForResponseCharacterReferences"
          :key="reference.id?.toString()"
          :name="`${cleanPreferredName(reference.firstName, reference.lastName)}`"
          :status="reference.status"
          :go-to="
            () =>
              router.push({
                name: 'view-character-reference',
                params: { applicationId: route.params.applicationId, referenceId: reference.id?.toString() },
              })
          "
          :will-provide-reference="reference.willProvideReference ? true : false"
        />
        <ApplicationSummaryActionListItem
          v-if="addMoreWorkExperienceReferencesFlag"
          :text="`${totalRequiredWorkExperienceHours} approved hours of work experience with reference`"
          :go-to="() => router.push({ name: 'manageWorkExperienceReferences', params: { applicationId: route.params.applicationId } })"
        />
        <ApplicationSummaryActionListItem
          v-if="addMoreProfessionalDevelopmentFlag"
          :text="`${totalRequiredProfessionalDevelopmentHours} hours of professional development`"
          :go-to="() => router.push({ name: 'manageProfessionalDevelopment', params: { applicationId: route.params.applicationId } })"
        />
      </div>
    </div>
    <!-- Step 3 End-->
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import { cleanPreferredName } from "@/utils/functions";
import { getApplicationStatus } from "@/api/application";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";
import { CertificationType, WorkExperienceType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { getProfile } from "@/api/profile";
import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ApplicationSummaryActionListItem from "./ApplicationSummaryActionListItem.vue";
import ApplicationSummaryCharacterReferenceListItem from "./ApplicationSummaryCharacterReferenceListItem.vue";
import ApplicationSummaryTranscriptListItem from "./ApplicationSummaryTranscriptListItem.vue";
import ApplicationSummaryHeader from "./ApplicationSummaryHeader.vue";
import { useConfigStore } from "@/store/config";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "ApplicationSummary",
  components: {
    ApplicationCertificationTypeHeader,
    ApplicationSummaryTranscriptListItem,
    ApplicationSummaryCharacterReferenceListItem,
    ApplicationSummaryActionListItem,
    ApplicationSummaryHeader,
    Breadcrumb,
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const alertStore = useAlertStore();
    const configStore = useConfigStore();
    const applicationStore = useApplicationStore();
    const userStore = useUserStore();
    const router = useRouter();
    const route = useRoute();

    await applicationStore.fetchApplications();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;
    userStore.setUserProfile(await getProfile());
    return {
      applicationStore,
      userStore,
      alertStore,
      configStore,
      CertificationType,
      cleanPreferredName,
      applicationStatus,
      smAndUp,
      formatDate,
      router,
      route,
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
    addMoreProfessionalDevelopmentFlag(): boolean {
      return this.applicationStatus?.addMoreProfessionalDevelopment ?? false;
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
        this.addMoreProfessionalDevelopmentFlag ||
        this.hasWaitingForDetailsTranscript
      );
    },
    totalObservedWorkExperienceHours(): number {
      return this.applicationStatus?.workExperienceReferencesStatus?.reduce((acc, reference) => acc + (reference.totalNumberofHoursObserved ?? 0), 0) || 0;
    },
    showWorkExperience(): boolean {
      return !!this.applicationStatus?.workExperienceReferencesStatus?.length;
    },
    totalRequiredWorkExperienceHours(): number {
      // Check for work experience reference (status), if it's null return 500
      let has400Hours = this.applicationStatus?.workExperienceReferencesStatus?.some((reference) => reference.type === WorkExperienceType.IS_400_Hours);
      return has400Hours ? 400 : 500;
    },
    totalRequiredProfessionalDevelopmentHours(): number {
      return 40;
    },
    showOtherInformation(): boolean {
      return (
        this.userStore.unverifiedPreviousNames.length > 0 ||
        this.userStore.pendingforDocumentsPreviousNames.length > 0 ||
        this.userStore.readyForVerificationPreviousNames.length > 0
      );
    },
  },
  methods: {
    getTranscriptHeaderString(transcript: Components.Schemas.TranscriptStatus): string {
      // Check if program name is null, if it is return educational institution
      return transcript.programName ? `${transcript.educationalInstitutionName} - ${transcript.programName}` : `${transcript.educationalInstitutionName} `;
    },
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
