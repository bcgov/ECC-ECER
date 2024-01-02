<template>
  <div>
    <v-tabs align-tabs="start" color="links">
      <v-tab :style="{ 'text-transform': 'none' }" to="in-progress">In Progress ({{ applicationStore.inProgressCount }})</v-tab>
      <v-tab :style="{ 'text-transform': 'none' }" to="completed">Completed ({{ applicationStore.completedCount }})</v-tab>
    </v-tabs>
    <router-view class="mt-4"></router-view>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "CertificationTabs",
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
  created() {
    this.applicationStore.fetchApplications();
  },
});
</script>
