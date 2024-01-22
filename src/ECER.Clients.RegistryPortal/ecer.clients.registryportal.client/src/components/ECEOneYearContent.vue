<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <p class="small">
          ECE One Year certification requires successful completion of a basic early childhood education training program recognized by the ECE Registry.
          <br />
          <br />
          This certification, allows you to work in the position of a fully certified ECE while working towards your 500 hours of work experience.
        </p>
      </v-col>
      <v-col cols="12">
        <div class="d-flex flex-row align-center">
          <v-icon class="mr-2" size="large" icon="mdi-alert-circle-outline" />
          <p class="small">NOTE: This certificate may only be renewed ONCE.</p>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ECEOneYearContent",
});
</script>
