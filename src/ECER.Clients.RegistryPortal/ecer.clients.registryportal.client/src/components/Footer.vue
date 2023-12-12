<template>
  <v-footer app height="" color="primary" :style="{ 'min-height': '50px' }">
    <v-row justify="start">
      <v-btn v-for="link in links" :key="link" color="white" variant="text" class="ms-2" rounded="xl" :style="{ 'text-transform': 'unset' }">
        {{ link }}
      </v-btn>
    </v-row> </v-footer
  >>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  setup: () => {
    const links = ["Home", "Disclaimer", "Privacy", "Terms of Use", "Accessibility", "Contact Us"];
    return { links };
  },
});
</script>
