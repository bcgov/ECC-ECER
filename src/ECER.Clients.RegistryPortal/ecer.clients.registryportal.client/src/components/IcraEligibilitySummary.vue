<template>
  <Loading v-if="loadingStore.isLoading('icra_status_get')" />
  <v-container v-else>
    <Breadcrumb />
    <h1>Apply with international certification</h1>
    <br />
    <h2>Status</h2>
    <p>It is a 3-step process to apply</p>
    <!-- Step 1 start -->
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
        <p>Completed on {{ formatDate(icraEligibilityStatus?.createdOn || "", "LLLL d, yyyy") }}</p>
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
    <ApplicationSummaryHeader text="International certification" />
    <ApplicationSummaryActionListItem
      v-for="certificate in icraEligibilityStatus?.internationalCertifications"
      :text="certificateNameDisplay(certificate)"
      :active="isCertificateReceived(certificate)"
      :goTo="() => {}"
      :show-link="false"
    />
    <ApplicationSummaryHeader text="References" />
    <ApplicationSummaryActionListItem
      text="Employment experience references"
      :active="actionNeededWorkReferences"
      :goTo="
        () => {
          router.push({ name: 'manage-icra-eligibility-work-experience-references', params: { icraEligibilityId: icraEligibilityId } });
        }
      "
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
    <!-- Step 2 End -->
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
    <!-- Step 3 End -->
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { formatDate } from "@/utils/format";
import { cleanPreferredName } from "@/utils/functions";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";

import { getIcraEligibilityStatus } from "@/api/icra";

import { useUserStore } from "@/store/user";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ApplicationSummaryActionListItem from "./ApplicationSummaryActionListItem.vue";
import ApplicationSummaryHeader from "./ApplicationSummaryHeader.vue";
import Loading from "./Loading.vue";
import { useConfigStore } from "@/store/config";
import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "IcraEligibilitySummary",
  components: {
    ApplicationSummaryHeader,
    ApplicationSummaryActionListItem,
    Breadcrumb,
    Loading,
  },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const { smAndUp } = useDisplay();
    const userStore = useUserStore();
    const configStore = useConfigStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      cleanPreferredName,
      formatDate,
      smAndUp,
      configStore,
      loadingStore,
      userStore,
      router,
    };
  },
  data() {
    return {
      icraEligibilityStatus: {} as Components.Schemas.ICRAEligibilityStatus,
    };
  },
  async mounted() {
    this.icraEligibilityStatus = (await getIcraEligibilityStatus(this.icraEligibilityId))?.data || {};
  },
  methods: {
    certificateNameDisplay(certificate: Components.Schemas.InternationalCertification): string {
      return `${this.configStore.countryName(certificate?.countryId || "")} - ${certificate?.certificateTitle || ""}`;
    },
    isCertificateReceived(certificate: Components.Schemas.InternationalCertification): boolean {
      switch (certificate.status) {
        case "ICRAEligibilitySubmitted":
        case "ApplicationSubmitted":
        case "Draft":
        case "InProgress":
        case "UnderReview":
        case "WaitingforResponse":
        case "Approved":
        case "Rejected":
        case "Inactive":
          return false;
        default:
          console.warn("unhandled certificate status:", certificate.status);
          return false;
      }
    },
  },
  computed: {
    actionNeededWorkReferences(): boolean {
      const totalReferencesWithoutRejections =
        this.icraEligibilityStatus?.employmentReferencesStatus?.filter((reference) => {
          if (reference.status !== "Rejected") {
            return true;
          }
        })?.length || 0;

      const someReferencesRequireResponse = this.icraEligibilityStatus?.employmentReferencesStatus?.some((reference) => {
        if (reference.status === "ICRAEligibilitySubmitted" || reference.status === "WaitingforResponse") {
          return true;
        }
      });

      // check if any references still require a response
      if (someReferencesRequireResponse) {
        return true;
      }

      //registry asks for more employement references
      if (this.icraEligibilityStatus?.addAdditionalEmploymentExperienceReferences && totalReferencesWithoutRejections < 6) {
        return true;
      }

      //No action needed
      return false;
    },
    currentStep(): number {
      switch (this.icraEligibilityStatus?.status) {
        case "Draft":
          return 1;
        case "InReview":
        case "Submitted":
        case "ReadyforReview":
        case "ReadyforAssessment":
          return 2;
        default:
          console.warn("unhandled icra eligibility status:", this.icraEligibilityStatus?.status);
          return 1;
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
      return "mdi-arrow-right";
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
      return "Not yet started";
    },
    showOtherInformation(): boolean {
      return (
        this.userStore.unverifiedPreviousNames.length > 0 ||
        this.userStore.pendingforDocumentsPreviousNames.length > 0 ||
        this.userStore.readyForVerificationPreviousNames.length > 0
      );
    },
  },
});
</script>
<style scoped>
.border-top {
  border-top: 2px solid black;
}
</style>
