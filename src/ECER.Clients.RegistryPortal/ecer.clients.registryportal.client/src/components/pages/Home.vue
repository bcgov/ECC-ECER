<template>
  <main>
    <Applications />
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Applications from "@/components/Applications.vue";

export default defineComponent({
  name: "Home",
  components: {
    Applications,
  },
});
</script>
