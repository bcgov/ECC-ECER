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
    async handleCallback(): Promise<void> {
      const user = await this.userStore.signinCallback();
      this.userStore.setUser(user);
      this.$router.push("/");
    },
  },
};
</script>
