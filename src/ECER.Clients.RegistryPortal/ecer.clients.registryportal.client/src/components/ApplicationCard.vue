<template>
  <v-card class="rounded-lg fill-height" flat color="primary">
    <v-card-item class="ma-4">
      <h3 class="text-white">Apply for ECE certification</h3>
      <p class="small text-white mt-4">
        There are different types of certifications you can apply for. Visit the B.C. government website to learn about the types of Early Childhood Educator
        (ECE) certificates and which one you may qualify for.
      </p>
    </v-card-item>
    <v-card-actions class="ma-4">
      <div v-if="applicationStore.hasDraftApplication" class="d-flex flex-row justify-start ga-3 flex-wrap">
        <v-btn size="large" variant="flat" color="warning" @click="$router.push('/application')">
          <v-icon size="large" icon="mdi-arrow-right" />
          Open application
        </v-btn>
        <v-btn class="ma-0" size="large" variant="outlined" color="white" @click="$emit('cancel-application')">Cancel application</v-btn>
      </div>
      <v-btn v-else variant="flat" size="large" color="warning" @click="handleStartNewApplication">
        <v-icon size="large" icon="mdi-arrow-right" />
        Apply Now
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "ApplicationCard",
  emits: ["cancel-application"],
  setup() {
    const applicationStore = useApplicationStore();
    return {
      applicationStore,
    };
  },
  computed: {},
  methods: {
    handleStartNewApplication() {
      this.applicationStore.upsertDraftApplication();
      this.$router.push("/application");
    },
  },
});
</script>
