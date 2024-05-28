<template>
  <Banner type="error" title="Sorry, this link is no longer valid" />
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Banner from "@/components/Banner.vue";

export default defineComponent({
  name: "Invalid",
  components: { Banner },
});
</script>
