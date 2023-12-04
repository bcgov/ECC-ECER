<template>
  <div>
    <h1>Login</h1>

    <div>
      <button type="button" @click="userStore.login()">Login with BCeID</button>
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
});
</script>
