<template>
  <v-row justify="center">
    <v-col cols="12" class="text-center">
      <v-progress-circular
        indeterminate
        class="mt-10 mb-3"
        color="primary"
        size="64"
      ></v-progress-circular>
      <p>Loading data, please wait...</p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
export default defineComponent({
  name: "Loading",
});
</script>
