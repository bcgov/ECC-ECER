<template><div></div></template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";

export default {
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();

    return { userStore, oidcStore };
  },
  mounted() {
    this.handleCallback();
  },
  methods: {
    async handleCallback() {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        await this.oidcStore.silentCallback(this.userStore.authority);
      }
    },
  },
};
</script>
