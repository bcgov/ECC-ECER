<template>
  <h1>My Certifications</h1>
  <v-expansion-panels v-if="$vuetify.display.mobile">
    <v-expansion-panel elevation="0">
      <v-expansion-panel-title>
        <v-icon size="large" icon="mdi-progress-check" class="mr-2"></v-icon>
        In Progress
      </v-expansion-panel-title>
      <v-expansion-panel-text>Placeholder</v-expansion-panel-text>
    </v-expansion-panel>
    <v-expansion-panel elevation="0">
      <v-expansion-panel-title>
        <v-icon size="large" icon="mdi-check-circle-outline" class="mr-2"></v-icon>
        Completed
      </v-expansion-panel-title>
      <v-expansion-panel-text>Placeholder</v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>

  <v-tabs align-tabs="start" color="links" v-if="!$vuetify.display.mobile">
    <v-tab :style="{ 'text-transform': 'none' }" to="in-progress">In Progress ({{ applicationStore.inProgressCount }})</v-tab>
    <v-tab :style="{ 'text-transform': 'none' }" to="completed">Completed ({{ applicationStore.completedCount }})</v-tab>
  </v-tabs>
  <router-view class="mt-4"></router-view>
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
});
</script>
