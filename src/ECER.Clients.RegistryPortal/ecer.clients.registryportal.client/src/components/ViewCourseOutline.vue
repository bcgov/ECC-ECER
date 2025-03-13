<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h2>Course outlines or syllabi</h2>
    <p>TODO BUILD PAGE</p>
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
