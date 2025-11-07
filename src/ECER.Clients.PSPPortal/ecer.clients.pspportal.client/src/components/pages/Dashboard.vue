<template>
  <PageContainer :margin-top="false">
    <h1>Hello World</h1>
    <p>Welcome to the dashboard!</p>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";

export default defineComponent({
  name: "Dashboard",
  components: {
    PageContainer,
  },
});
</script>
