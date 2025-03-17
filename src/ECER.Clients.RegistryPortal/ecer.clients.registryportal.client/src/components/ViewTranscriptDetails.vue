<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Transcript</h2>
      <p>Your transcript has not yet been received. Make sure that you have made a request to your educational institution to send it to us directly.</p>
      <p>The transcript must:</p>
      <ul class="ml-10">
        <li>Include your name, educational institution name and list of courses</li>
        <li>Be for the year(s) you completed the course(s)</li>
        <li>Be in English</li>
      </ul>
    </div>
    <ECEHeader title="Transcript details" />
    <div class="d-flex flex-column ga-3 my-6">
      <h3>Educational insitution</h3>
      <p>{{ transcript?.educationalInstitutionName }}</p>
    </div>
    <div class="d-flex flex-column ga-3 my-6">
      <h3>Program or course name</h3>
      <p>{{ transcript?.programName }}</p>
    </div>
    <p class="my-6">We will notify you once we receive your transcript. You will also see this item marked as “Received” in your application summary.</p>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus } from "@/api/application";
import Breadcrumb from "./Breadcrumb.vue";
import ECEHeader from "./ECEHeader.vue";

export default defineComponent({
  name: "ViewTranscriptDetails",
  components: { Breadcrumb, ECEHeader },

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
          title: "Transcript",
          disabled: true,
          href: `/manage-application/${this.applicationId}/transcript/${this.transcriptId}`,
        },
      ],
    };
  },

  methods: {},
});
</script>
