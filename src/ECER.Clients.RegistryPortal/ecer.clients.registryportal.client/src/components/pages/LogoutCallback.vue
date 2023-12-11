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
    handleCallback() {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        this.oidcStore.completeLogout(this.userStore.authority);
        this.userStore.$reset();
      }
    },
  },
};
</script>
