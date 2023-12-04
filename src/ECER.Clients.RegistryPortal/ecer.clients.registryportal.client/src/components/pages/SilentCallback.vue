<template><div></div></template>

<script lang="ts">
import { useUserStore } from "@/store/user";

export default {
  setup() {
    const userStore = useUserStore();

    return { userStore };
  },
  mounted() {
    this.handleCallback();
  },
  methods: {
    handleCallback() {
      this.userStore.silentCallback();
    },
  },
};
</script>
