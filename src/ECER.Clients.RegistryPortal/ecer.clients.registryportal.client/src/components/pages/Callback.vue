<!-- src/views/Callback.vue -->
<template>
  <div>
    <p>Loading...</p>
  </div>
</template>

<script lang="ts">
import { useUserStore } from "@/store/user";

export default {
  setup() {
    const userStore = useUserStore();

    return { userStore };
  },
  mounted() {
    // Handle callback logic here
    this.handleCallback();
  },
  methods: {
    handleCallback() {
      // Call the method to complete the login process
      console.log("Callback fired");
      this.userStore.callback();
    },
  },
};
</script>
@/services/auth
