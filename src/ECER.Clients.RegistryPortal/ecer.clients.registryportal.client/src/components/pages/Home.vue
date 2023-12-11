<template>
  <main>
    <!-- Only BCeID supports refresh token flow -->
    <button v-if="userStore.getAuthority == 'bceid'" type="button" @click="handleTokenRefresh">Refresh token</button>

    <p>Profile: {{ userStore.getProfile }}</p>

    <p>Authority: {{ userStore.getAuthority }}</p>

    <p>Access Token: {{ userStore.getAccessToken }}</p>

    <Applications />
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Applications from "@/components/Applications.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "Home",
  components: {
    Applications,
  },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    return { userStore, oidcStore };
  },

  methods: {
    handleTokenRefresh: async function () {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        const user = await this.oidcStore.signinSilent(this.userStore.authority);
        if (user) {
          this.userStore.setUser(user);
        }
      }
    },
  },
});
</script>
