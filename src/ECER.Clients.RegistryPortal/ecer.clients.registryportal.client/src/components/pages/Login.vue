<template>
  <div class="weather-component">
    <h1>Login</h1>

    <div>
      <button type="button" @click="login">Login</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "Login",
  setup() {
    const userStore = useUserStore();

    return { userStore };
  },
  methods: {
    login: async function () {
      await this.userStore.login();
    },
  },
});
</script>
@/services/auth
