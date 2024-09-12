<template>
  <v-col cols="12">
    <h1>Apply for ECE One Year certification</h1>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="TODO" />
  </v-col>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";

export default defineComponent({
  name: "ECEOneYearRegistrantRequirements",
  components: { ECEHeader },
});
</script>
