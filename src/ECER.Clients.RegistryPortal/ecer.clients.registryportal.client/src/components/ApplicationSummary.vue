<template>
  <v-container>
    <v-breadcrumbs :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <ApplicationCertificationTypeHeader :certification-types="applicationStore.applications?.[0]?.certificationTypes || []" class="pb-5" />
    <h3>Status</h3>
    <div class="pb-3">It's a 3-step process to apply</div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { CertificationType } from "@/utils/constant";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";

export default defineComponent({
  name: "ApplicationSummary",
  components: { ApplicationCertificationTypeHeader },
  setup: async () => {
    const alertStore = useAlertStore();
    const applicationStore = useApplicationStore();

    applicationStore.fetchApplications();

    return { applicationStore, alertStore, CertificationType };
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
  computed: {},
  methods: {},
});
</script>
<style scoped>
.test {
  border-top: 1px solid black;
}
</style>
