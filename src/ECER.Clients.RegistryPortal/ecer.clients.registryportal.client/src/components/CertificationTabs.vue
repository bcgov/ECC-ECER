<template>
  <v-row class="ma-4">
    <v-col cols="12">
      <h1>My Certifications</h1>
    </v-col>
  </v-row>
  <v-expansion-panels v-if="$vuetify.display.mobile" :model-value="expansionPanelIndex" @update:model-value="updateTab">
    <v-expansion-panel elevation="0">
      <v-expansion-panel-title>
        <v-icon size="large" icon="mdi-progress-check" class="mr-2" />
        In Progress ({{ applicationStore.inProgressCount }})
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <router-view class="mt-4" />
      </v-expansion-panel-text>
    </v-expansion-panel>
    <v-expansion-panel elevation="0">
      <v-expansion-panel-title>
        <v-icon size="large" icon="mdi-check-circle-outline" class="mr-2" />
        Completed ({{ applicationStore.completedCount }})
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <router-view class="mt-4" />
      </v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>

  <template v-if="!$vuetify.display.mobile">
    <v-tabs align-tabs="start" color="links">
      <v-tab :style="{ 'text-transform': 'none' }" to="in-progress">In Progress ({{ applicationStore.inProgressCount }})</v-tab>
      <v-tab :style="{ 'text-transform': 'none' }" to="completed">Completed ({{ applicationStore.completedCount }})</v-tab>
    </v-tabs>
    <router-view class="mt-4"></router-view>
  </template>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import { ApplicationState, RouteName } from "@/utils/constant";

export default defineComponent({
  name: "CertificationTabs",
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
  computed: {
    expansionPanelIndex() {
      return this.$route.name === RouteName.IN_PROGRESS ? ApplicationState.IN_PROGRESS : ApplicationState.COMPLETED;
    },
  },
  methods: {
    updateTab(expansionPanel: any) {
      this.$router.push({ name: `${expansionPanel === ApplicationState.IN_PROGRESS ? RouteName.IN_PROGRESS : RouteName.COMPLETED}` });
    },
  },
});
</script>
