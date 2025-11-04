<template>
  <div></div>
</template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default {
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();

    return { userStore, oidcStore, router };
  },
  mounted() {
    this.handleCallback();
  },
  methods: {
    async handleCallback() {
      await this.oidcStore.completeLogout();
      this.userStore.setPspUserProfile(null);
      this.router.push("/login");
    },
  },
};
</script>
