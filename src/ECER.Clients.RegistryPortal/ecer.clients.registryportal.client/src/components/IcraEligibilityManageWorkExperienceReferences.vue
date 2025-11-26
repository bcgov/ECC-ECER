<template>
  <Loading v-if="loadingStore.isLoading('icra_status_get')" />
  <v-container v-else>
    <Breadcrumb />
    <h1>Employment experience references</h1>
    <br />
    <h2>Provide proof of 2 years of independent ECE practice.</h2>
    <br />
    <p>Your employment experience must:</p>
    <br />
    <ul class="ml-10">
      <li>Have been completed within the last 5 years</li>
      <li>Not overlap (any overlapping employment experience will only be counted once)</li>
      <li>Have been completed while holding valid ECE certification</li>
    </ul>
    <br />
    <p>If your employment experience was completed at multiple locations:</p>
    <br />
    <ul class="ml-10">
      <li>Provide a reference from each person</li>
      <li>You may enter up to 6 references</li>
    </ul>
    <br />
    <!-- references list -->
    <v-card v-for="reference in icraEligibilityStatus?.employmentReferencesStatus" elevation="0" rounded="0" class="border-t border-b">
      <v-card-text>
        <v-row class="d-flex" :class="[smAndUp ? 'justify-space-between align-center' : 'flex-column']">
          <v-col cols="12" sm="4">
            <div v-if="reference.status !== 'ICRAEligibilitySubmitted'">
              <p>{{ cleanPreferredName(reference.firstName, reference.lastName) }}</p>
            </div>
            <a
              v-else
              href="#"
              @click.prevent="
                router.push({
                  name: 'view-icra-eligibility-work-experience-reference',
                  params: { icraEligibilityId: icraEligibilityId, referenceId: reference.id?.toString() },
                })
              "
            >
              <p class="text-links">{{ cleanPreferredName(reference.firstName, reference.lastName) }}</p>
            </a>
          </v-col>
          <v-col cols="12" sm="4" :align="smAndUp ? 'right' : ''">
            <v-sheet rounded width="200px" class="py-2 text-center" :class="{ 'mt-2': !smAndUp }" :color="sheetColor(reference)">
              <p>{{ statusText(reference) }}</p>
            </v-sheet>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
    <!-- end references list -->
    <br />
    <v-btn
      v-if="additionalWorkReferenceRequired"
      prepend-icon="mdi-plus"
      class="mt-10"
      color="primary"
      @click.prevent="router.push({ name: 'icra-eligibility-add-work-experience-reference', params: { icraEligibilityId: icraEligibilityId } })"
    >
      Add reference
    </v-btn>
    <Callout v-if="!additionalWorkReferenceRequired" type="warning" title="Waiting for references to respond">
      <p>No additional work experience may be added. We are waiting on a response from your one or more of your references.</p>
    </Callout>
    <br />
    <br />
    <a href="#" @click.prevent="() => router.push({ name: 'manage-icra-eligibility', params: { icraEligibilityId: icraEligibilityId } })">
      Back to application summary
    </a>
  </v-container>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { cleanPreferredName } from "@/utils/functions";

import { getIcraEligibilityStatus } from "@/api/icra";

import Breadcrumb from "@/components/Breadcrumb.vue";
import Callout from "@/components/Callout.vue";
import Loading from "./Loading.vue";

import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "ManageWorkExperienceReferenceList",
  components: { Breadcrumb, Callout, Loading },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const { smAndUp } = useDisplay();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      smAndUp,
      router,
      loadingStore,
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
  computed: {
    additionalWorkReferenceRequired(): boolean {
      //set our variables to check
      const someReferenceRejected = this.icraEligibilityStatus?.employmentReferencesStatus?.some((reference) => {
        if (reference.status === "Rejected") {
          return true;
        }
      });

      const totalReferencesWithoutRejections =
        this.icraEligibilityStatus?.employmentReferencesStatus?.filter((reference) => {
          if (reference.status !== "Rejected") {
            return true;
          }
        })?.length || 0;

      //begin checking for scenarios
      if (this.icraEligibilityStatus?.addAdditionalEmploymentExperienceReferences && totalReferencesWithoutRejections < 6) {
        return true;
      }

      if (someReferenceRejected && totalReferencesWithoutRejections <= 6) {
        return true;
      }

      //No action needed
      return false;
    },
  },
  methods: {
    statusText(reference: Components.Schemas.EmploymentReferenceStatus) {
      switch (reference.status) {
        case "ICRAEligibilitySubmitted":
          return "Not yet received";
        case "ApplicationSubmitted":
        case "Approved":
        case "Draft":
        case "EligibilityResponseSubmitted":
        case "InProgress":

        case "Submitted":
        case "UnderReview":
        case "WaitingforResponse":
          return "Received";
        case "Rejected":
          return "Cancelled";
        default:
          console.warn("Unhandled status:", reference.status);
          return "Unhandled Status";
      }
    },
    sheetColor(reference: Components.Schemas.EmploymentReferenceStatus) {
      return reference.status === "ICRAEligibilitySubmitted" ? "hawkes-blue" : "white-smoke";
    },
    cleanPreferredName,
  },
});
</script>
