<template>
  <v-card flat color="primary">
    <v-card-item class="ma-4">
      <h2 class="text-white">Continue your application for ECE Five Year certification</h2>
    </v-card-item>

    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <!-- Application status Draft -->
      <v-card-actions>
        <v-btn size="large" variant="flat" color="warning" @click="handleStartApplication">
          <v-icon size="large" icon="mdi-arrow-right" />
          Open application
        </v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useRouter } from "vue-router";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "IcraEligibilityStep2StartCard",
  props: {},
  setup() {
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return {
      router,
      applicationStore,
    };
  },
  computed: {},
  methods: {
    handleStartApplication() {
      //setup draft application for ICRA
      this.applicationStore.$patch({ draftApplication: { certificationTypes: ["FiveYears"], applicationType: "ICRA", stage: "Education" } });

      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
