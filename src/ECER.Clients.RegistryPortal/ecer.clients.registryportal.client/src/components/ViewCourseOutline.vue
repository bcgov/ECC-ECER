<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Course outlines or syllabi</h2>
      <p>You need to provide detailed course outlines or syllabi for:</p>
      <ul class="ml-10">
        <li>
          <b>{{ transcript?.programName }}</b>
        </li>
      </ul>
      <p>Ask your educational institution for detailed course outlines or syllabi. You cannot create these yourself.</p>
      <p>The outlines must:</p>
      <ul class="ml-10">
        <li>Include detailed descriptions of course content, learning goals, outcomes and expectations</li>
        <li>Be for the year(s) you completed the course(s)</li>
        <li>Be in English</li>
      </ul>
    </div>
    <h3>How will you provide your course outlines or syllabi?</h3>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus } from "@/api/application";
import Breadcrumb from "./Breadcrumb.vue";

export default defineComponent({
  name: "ViewCourseOutline",
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
          title: "Course oultine",
          disabled: true,
          href: `/manage-application/${this.applicationId}/transcript/${this.transcriptId}/course-outline`,
        },
      ],
    };
  },

  methods: {},
});
</script>
