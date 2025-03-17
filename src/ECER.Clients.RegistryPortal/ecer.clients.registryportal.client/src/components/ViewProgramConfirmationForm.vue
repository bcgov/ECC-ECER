<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Program Confirmation Form</h2>
      <p>You need to provide additional details about the program:</p>
      <ul class="ml-10">
        <li>
          <b>{{ transcript?.programName }}</b>
        </li>
      </ul>
      <p>You will need to:</p>
      <ol class="ml-10">
        <li>
          Download a
          <a href="https://www2.gov.bc.ca/assets/download/1DD5579B6A474ED2B095FD13B3268DA0 ">Program Confirmation Form (16KB, PDF).</a>
        </li>
        <li>Complete Section 1 of the form.</li>
        <li>
          Ask your educational institution to complete the rest of the form in English. If they cannot complete it in English, ask them to send the completed
          form directly to a professional translator.
        </li>
        <li>Upload the completed form after you get it back from your educational institution or translator.</li>
      </ol>
    </div>
    <h3>How will you provide your Program Confirmation Form?</h3>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus } from "@/api/application";
import Breadcrumb from "./Breadcrumb.vue";

export default defineComponent({
  name: "ViewProgramConfirmationForm",
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
          title: "Program confirmation",
          disabled: true,
          href: `/manage-application/${this.applicationId}/transcript/${this.transcriptId}/program-confirmation`,
        },
      ],
    };
  },

  methods: {},
});
</script>
