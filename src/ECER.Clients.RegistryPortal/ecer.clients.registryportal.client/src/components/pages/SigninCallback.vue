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
    async handleCallback(): Promise<void> {
      if (this.userStore.authority) {
        const user = await this.oidcStore.signinCallback(this.userStore.authority);
        this.userStore.setUser(user);
      }
    },
  },
};
</script>
