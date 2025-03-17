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
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/pathways/international#education-requirements-equivalency-process:~:text=Apply%20for%20an%20International%20Credential%20Evaluation%20Service%20Comprehensive%20Report%C2%A0"
        >
          Learn more about the Comprehensive Report
        </a>
      </p>
    </div>
    <h3>How will you provide your Comprehensive Report?</h3>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus } from "@/api/application";
import Breadcrumb from "./Breadcrumb.vue";

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
    const router = useRouter();
    const route = useRoute();

    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    const transcript = applicationStatus?.transcriptsStatus?.find((transcript) => transcript.id === props.transcriptId);

    if (!transcript) {
      router.back();
    }

    return { transcript, alertStore };
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
          disabled: false,
          href: `/manage-application/${this.applicationId}`,
        },
        {
          title: "Comprehensive evaluation",
          disabled: true,
          href: `/manage-application/${this.applicationId}/transcript/${this.transcriptId}/comprehensive-evaluation`,
        },
      ],
    };
  },

  methods: {},
});
</script>
