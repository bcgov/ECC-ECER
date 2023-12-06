<template>
  <main>
    <p>
      <router-link to="/">Go to Home</router-link>
    </p>

    <div v-if="userStore.isAuthenticated">
      <button type="button" @click="logout">Logout</button>
    </div>
    <div v-else>
      <router-link to="/login">Login</router-link>
    </div>
    <hr />

    <router-view></router-view>
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "App",
  setup() {
    const userStore = useUserStore();
    return { userStore };
  },
  methods: {
    logout: async function () {
      await this.userStore.logout();
    },
  },
});
</script>
