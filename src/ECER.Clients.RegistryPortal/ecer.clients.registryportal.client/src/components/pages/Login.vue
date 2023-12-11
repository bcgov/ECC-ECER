<template>
  <div>
    <h1>Login</h1>

    <div>
      <button type="button" @click="handleLogin('bceid')">Login with BCeID</button>
    </div>
    <div>
      <button type="button" @click="handleLogin('bcsc')">Login with BCSC</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import type { Authority } from "@/types/authority";

export default defineComponent({
  name: "Login",
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    return { userStore, oidcStore };
  },
  methods: {
    async handleLogin(authority: Authority) {
      this.userStore.setAuthority(authority);
      await this.oidcStore.login(authority);
    },
  },
});
</script>
