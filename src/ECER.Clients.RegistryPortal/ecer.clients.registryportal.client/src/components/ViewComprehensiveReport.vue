<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Comprehensive Report</h2>
      <p>For education completed outside of Canada, you need to request a Comprehensive Report from BCIT's International Credential Evaluation Service.</p>
      <p>
        You may be eligible for a fee waiver to cover the costs of the report.
        <b>If you wish to apply for a fee waiver, please indicate this below before you request a report from BCIT.</b>
        The fee waiver is paid out directly to BCIT from the ECE Registry and cannot be used to reimburse the applicant.
      </p>
      <p>After we receive the report from BCIT, this item will be updated and marked as received.</p>
      <p>
        <a
          target="_blank"
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/pathways/international#education-requirements-equivalency-process:~:text=Apply%20for%20an%20International%20Credential%20Evaluation%20Service%20Comprehensive%20Report%C2%A0"
        >
          Learn more about the Comprehensive Report
        </a>
      </p>
    </div>
    <h3>How will you provide your Comprehensive Report?</h3>
    <v-form ref="updateProgramConfirmationOptionsAndDocuments" validate-on="input">
      <v-row class="mt-4">
        <v-radio-group id="programConfirmationRadio" v-model="comprehensiveReportOptions" :rules="[Rules.required()]" color="primary">
          <v-radio
            label="I wish to apply for a fee waiver before I request a report from BCIT. The Registry will send a message with more information."
            value="FeeWaiver"
          ></v-radio>
          <v-radio
            label="I have submitted an application to BCIT's International Credential Evaluation Service for a Comprehensive Report."
            value="InternationalCredentialEvaluationService"
          ></v-radio>
          <v-radio
            label="The ECE Registry already has my Comprehensive Report on file for the course or program relevant to this application and certificate type."
            value="RegistryAlreadyHas"
          ></v-radio>
        </v-radio-group>
      </v-row>
    </v-form>
    <v-row class="mt-6">
      <v-btn :loading="loadingStore.isLoading('application_update_transcript_post')" @click="handleSubmit" size="large" color="primary">Save</v-btn>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus, setTranscriptDocumentsAndOptions } from "@/api/application";
import Breadcrumb from "./Breadcrumb.vue";
import * as Rules from "@/utils/formRules";
import { useLoadingStore } from "@/store/loading";
import type { ComprehensiveReportOptions } from "@/types/openapi";
import type { VForm } from "vuetify/components";

export default defineComponent({
  name: "ViewComprehensiveReport",
  components: { Breadcrumb },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
    transcriptId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const route = useRoute();

    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    let comprehensiveReportOptions = ref<ComprehensiveReportOptions | undefined>(undefined);
    const items: { title: string; disabled: boolean; href: string }[] = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Application",
        disabled: false,
        href: `/manage-application/${props.applicationId}`,
      },
      {
        title: "Comprehensive Report",
        disabled: true,
        href: `/manage-application/${props.applicationId}/transcript/${props.transcriptId}/comprehensive-evaluation`,
      },
    ];

    const transcript = applicationStatus?.transcriptsStatus?.find((transcript) => transcript.id === props.transcriptId);

    if (!transcript) {
      router.back();
    } else {
      // Set comprehensiveReportOptions based on a field from transcript
      comprehensiveReportOptions = ref(transcript.comprehensiveReportOptions || undefined);
    }

    return { router, transcript, alertStore, Rules, comprehensiveReportOptions, items, loadingStore };
  },
  methods: {
    async handleSubmit() {
      // Validate the form
      const { valid } = await (this.$refs.updateProgramConfirmationOptionsAndDocuments as VForm).validate();
      if (valid) {
        const { error } = await setTranscriptDocumentsAndOptions({
          comprehensiveReportOptions: this.comprehensiveReportOptions,
          applicationId: this.applicationId,
          transcriptId: this.transcriptId,
        });
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Your changes have been saved.");
          this.router.push({ name: "manageApplication", params: { applicationId: this.applicationId } });
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
  },
});
</script>
