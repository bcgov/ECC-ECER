<template>
  <v-card flat rounded="lg" class="pa-8 text-center">
    <v-progress-circular indeterminate color="primary" size="56" class="mb-4" />
    <h3 class="mb-3">Preparing your request</h3>
    <p class="ma-0">
      We are preparing your request. This may take a few minutes. When this is
      ready it will appear here and also be available from your dashboard.
    </p>
  </v-card>
</template>
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PreparingRequestSpinner",
});
</script>
