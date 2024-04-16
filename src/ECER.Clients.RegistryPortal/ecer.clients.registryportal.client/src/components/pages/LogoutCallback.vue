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
      await this.oidcStore.completeLogout();
      this.userStore.$reset();
      this.$router.push("/login");
    },
  },
};
</script>
